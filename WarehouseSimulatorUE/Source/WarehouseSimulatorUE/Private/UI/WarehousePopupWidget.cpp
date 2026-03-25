// Fill out your copyright notice in the Description page of Project Settings.

#include "UI/WarehousePopupWidget.h"
#include "Warehouse/PalletSlot.h"
#include "Network/WarehouseHttpManager.h"
#include "Components/TextBlock.h"
#include "Components/EditableTextBox.h"
#include "Components/Button.h"
#include "Components/Widget.h"
#include "EngineUtils.h"
#include "Internationalization/Regex.h"

void UWarehousePopupWidget::NativeConstruct()
{
	Super::NativeConstruct();

	// HttpManager 찾기
	for (TActorIterator<AActor> It(GetWorld()); It; ++It)
	{
		HttpManager = (*It)->FindComponentByClass<UWarehouseHttpManager>();
		if (HttpManager) break;
	}

	// 버튼 이벤트 바인딩
	if (BtnIncoming) BtnIncoming->OnClicked.AddDynamic(this, &UWarehousePopupWidget::OnIncoming);
	if (BtnMove)     BtnMove->OnClicked.AddDynamic(this,     &UWarehousePopupWidget::OnMove);
	if (BtnOutgoing) BtnOutgoing->OnClicked.AddDynamic(this, &UWarehousePopupWidget::OnOutgoing);
	if (BtnClose)    BtnClose->OnClicked.AddDynamic(this,    &UWarehousePopupWidget::OnClose);

	// 시작 시 팝업 숨김
	if (PopupPanel) PopupPanel->SetVisibility(ESlateVisibility::Hidden);
}

// ─────────────────────────────────────────────────────────
// 클릭 진입점 — 이동 모드 여부에 따라 분기
// ─────────────────────────────────────────────────────────
void UWarehousePopupWidget::HandleSlotClick(UPalletSlot* InSlot)
{
	if (!InSlot) return;

	// 이동 모드: 두 번째 클릭 = 목적지 선택
	if (MoveSourceSlot)
	{
		if (!InSlot->bIsEmpty)
		{
			UE_LOG(LogTemp, Warning, TEXT("[PopupWidget] 목적지 슬롯이 이미 점유 중입니다."));
			return;
		}

		FContainerData Data  = MoveSourceSlot->Container;
		Data.Shelf           = InSlot->Shelf;
		Data.Floor           = InSlot->Floor;
		Data.SlotIndex       = InSlot->SlotIndex;

		MoveSourceSlot->ClearContainer();
		InSlot->LoadContainer(Data);

		if (HttpManager)
			HttpManager->MoveContainer(Data.ContainerId, Data.Shelf, Data.Floor, Data.SlotIndex);

		UE_LOG(LogTemp, Log, TEXT("[PopupWidget] 이동 완료: %s → %s_%d_%d"),
			*Data.ContainerId, *Data.Shelf, Data.Floor, Data.SlotIndex);

		MoveSourceSlot = nullptr;
		ClosePopup();
		return;
	}

	OpenPopup(InSlot);
}

// ─────────────────────────────────────────────────────────
// 팝업 열기
// ─────────────────────────────────────────────────────────
void UWarehousePopupWidget::OpenPopup(UPalletSlot* InSlot)
{
	CurrentSlot = InSlot;

	TitleText->SetText(FText::FromString(
		FString::Printf(TEXT("슬롯: %s-%d-%d"), *InSlot->Shelf, InSlot->Floor, InSlot->SlotIndex)));

	const bool bEmpty = InSlot->bIsEmpty;

	// 패널 전환
	InfoPanel->SetVisibility(bEmpty  ? ESlateVisibility::Collapsed : ESlateVisibility::Visible);
	InputPanel->SetVisibility(bEmpty ? ESlateVisibility::Visible   : ESlateVisibility::Collapsed);

	// 버튼 전환
	BtnIncoming->SetVisibility(bEmpty ? ESlateVisibility::Visible   : ESlateVisibility::Collapsed);
	BtnMove->SetVisibility(bEmpty     ? ESlateVisibility::Collapsed : ESlateVisibility::Visible);
	BtnOutgoing->SetVisibility(bEmpty ? ESlateVisibility::Collapsed : ESlateVisibility::Visible);

	if (!bEmpty)
	{
		InfoIdText->SetText(FText::FromString(TEXT("ID: ") + InSlot->Container.ContainerId));
		InfoNameText->SetText(FText::FromString(TEXT("물건: ") + InSlot->Container.ItemName));
		InfoWeightText->SetText(FText::FromString(
			FString::Printf(TEXT("무게: %.1f kg"), InSlot->Container.Weight)));
		InfoDateText->SetText(FText::FromString(TEXT("입고일: ") + InSlot->Container.ArrivalDate));
		InfoSizeText->SetText(FText::FromString(FString::Printf(
			TEXT("크기: %.1f × %.1f × %.1f m"),
			InSlot->Container.Width, InSlot->Container.Depth, InSlot->Container.Height)));
	}
	else
	{
		InputId->SetText(FText::GetEmpty());
		InputName->SetText(FText::GetEmpty());
		InputWeight->SetText(FText::GetEmpty());
		InputWidth->SetText(FText::GetEmpty());
		InputDepth->SetText(FText::GetEmpty());
		InputHeight->SetText(FText::GetEmpty());
	}

	PopupPanel->SetVisibility(ESlateVisibility::Visible);
}

void UWarehousePopupWidget::ClosePopup()
{
	MoveSourceSlot = nullptr;
	PopupPanel->SetVisibility(ESlateVisibility::Hidden);
}

// ─────────────────────────────────────────────────────────
// 버튼 핸들러
// ─────────────────────────────────────────────────────────
void UWarehousePopupWidget::OnIncoming()
{
	if (!CurrentSlot) return;

	const FString Id     = InputId->GetText().ToString().TrimStartAndEnd();
	const FString Name   = InputName->GetText().ToString().TrimStartAndEnd();
	const FString Weight = InputWeight->GetText().ToString();
	const FString Width  = InputWidth->GetText().ToString();
	const FString Depth  = InputDepth->GetText().ToString();
	const FString Height = InputHeight->GetText().ToString();

	if (Id.IsEmpty() || Name.IsEmpty())
	{
		UE_LOG(LogTemp, Warning, TEXT("[PopupWidget] ID와 물건 이름을 입력해주세요."));
		return;
	}

	// ID 형식 체크 (CNT-000 ~ CNT-999)
	const FRegexPattern Pattern(TEXT("^CNT-\\d{3}$"));
	FRegexMatcher Matcher(Pattern, Id);
	if (!Matcher.FindNext())
	{
		UE_LOG(LogTemp, Warning, TEXT("[PopupWidget] ID 형식이 올바르지 않습니다. 예: CNT-001"));
		InputId->SetText(FText::GetEmpty());
		return;
	}

	// 중복 ID 체크
	for (TActorIterator<AActor> It(GetWorld()); It; ++It)
	{
		TArray<UPalletSlot*> Slots;
		(*It)->GetComponents<UPalletSlot>(Slots);
		for (UPalletSlot* S : Slots)
		{
			if (S && !S->bIsEmpty && S->Container.ContainerId == Id)
			{
				UE_LOG(LogTemp, Warning, TEXT("[PopupWidget] 이미 존재하는 컨테이너 ID: %s"), *Id);
				InputId->SetText(FText::GetEmpty());
				return;
			}
		}
	}

	auto Clamp = [](float V) { return FMath::Clamp(V, 0.1f, 5.f); };

	FContainerData Data;
	Data.ContainerId = Id;
	Data.ItemName    = Name;
	Data.Weight      = FCString::Atof(*Weight);
	Data.ArrivalDate = FDateTime::Now().ToString(TEXT("%Y-%m-%d"));
	Data.Shelf       = CurrentSlot->Shelf;
	Data.Floor       = CurrentSlot->Floor;
	Data.SlotIndex   = CurrentSlot->SlotIndex;
	Data.Width       = Width.IsEmpty()  ? 1.f : Clamp(FCString::Atof(*Width));
	Data.Depth       = Depth.IsEmpty()  ? 1.f : Clamp(FCString::Atof(*Depth));
	Data.Height      = Height.IsEmpty() ? 1.f : Clamp(FCString::Atof(*Height));

	CurrentSlot->InsertContainer(Data);
	ClosePopup();
}

void UWarehousePopupWidget::OnMove()
{
	MoveSourceSlot = CurrentSlot;
	TitleText->SetText(FText::FromString(TEXT("이동할 목적지 팔레트를 클릭하세요")));
	BtnMove->SetVisibility(ESlateVisibility::Collapsed);
	BtnOutgoing->SetVisibility(ESlateVisibility::Collapsed);
}

void UWarehousePopupWidget::OnOutgoing()
{
	if (!CurrentSlot || CurrentSlot->bIsEmpty) return;

	if (HttpManager)
		HttpManager->RemoveContainer(CurrentSlot->Container.ContainerId);

	CurrentSlot->ClearContainer();
	ClosePopup();
}

void UWarehousePopupWidget::OnClose()
{
	ClosePopup();
}
