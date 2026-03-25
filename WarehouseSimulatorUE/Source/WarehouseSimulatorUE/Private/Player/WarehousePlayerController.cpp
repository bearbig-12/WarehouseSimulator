// Fill out your copyright notice in the Description page of Project Settings.

#include "Player/WarehousePlayerController.h"
#include "UI/WarehousePopupWidget.h"
#include "Warehouse/PalletSlot.h"
#include "Blueprint/UserWidget.h"
#include "Engine/EngineTypes.h"

void AWarehousePlayerController::BeginPlay()
{
	Super::BeginPlay();

	// 팝업 위젯 생성 후 뷰포트에 추가
	if (PopupWidgetClass)
	{
		PopupWidget = CreateWidget<UWarehousePopupWidget>(this, PopupWidgetClass);
		if (PopupWidget)
			PopupWidget->AddToViewport();
	}

	// 마우스 커서 표시 + 게임&UI 입력 동시 허용
	bShowMouseCursor = true;
	SetInputMode(FInputModeGameAndUI());
}

void AWarehousePlayerController::SetupInputComponent()
{
	Super::SetupInputComponent();

	// Enhanced Input과 별개로 마우스 좌클릭 직접 바인딩
	InputComponent->BindKey(EKeys::LeftMouseButton, IE_Pressed, this, &AWarehousePlayerController::OnLeftClick);
}

void AWarehousePlayerController::OnLeftClick()
{
	if (!PopupWidget) { UE_LOG(LogTemp, Warning, TEXT("[Click] PopupWidget is null")); return; }

	FHitResult Hit;
	if (!GetHitResultUnderCursorByChannel(UEngineTypes::ConvertToTraceType(ECC_Visibility), false, Hit))
	{
		UE_LOG(LogTemp, Warning, TEXT("[Click] No hit result"));
		return;
	}

	AActor* HitActor = Hit.GetActor();
	UE_LOG(LogTemp, Log, TEXT("[Click] Hit actor: %s"), HitActor ? *HitActor->GetName() : TEXT("null"));

	UPalletSlot* Slot = nullptr;

	// 히트된 컴포넌트의 오너에서 직접 찾기
	if (UPrimitiveComponent* HitComp = Hit.GetComponent())
	{
		AActor* CompOwner = HitComp->GetOwner();
		while (CompOwner)
		{
			Slot = CompOwner->FindComponentByClass<UPalletSlot>();
			if (Slot && !Slot->Shelf.IsEmpty()) break;
			Slot = nullptr;
			CompOwner = CompOwner->GetAttachParentActor();
		}
	}

	if (!Slot) { UE_LOG(LogTemp, Warning, TEXT("[Click] No PalletSlot found")); return; }

	PopupWidget->HandleSlotClick(Slot);
}
