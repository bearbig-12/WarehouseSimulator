// Fill out your copyright notice in the Description page of Project Settings.

#include "Warehouse/PalletSlot.h"
#include "Warehouse/BoxPool.h"
#include "EngineUtils.h"

UPalletSlot::UPalletSlot()
{
	PrimaryComponentTick.bCanEverTick = false;
}

void UPalletSlot::BeginPlay()
{
	Super::BeginPlay();

	// 레벨에서 HttpManager 자동 검색
	for (TActorIterator<AActor> It(GetWorld()); It; ++It)
	{
		HttpManager = (*It)->FindComponentByClass<UWarehouseHttpManager>();
		if (HttpManager) break;
	}

	// 레벨에서 BoxPool 자동 검색
	for (TActorIterator<ABoxPool> It(GetWorld()); It; ++It)
	{
		BoxPoolRef = *It;
		break;
	}

	// SlotWorldLocation이 설정 안 됐으면 오너 액터 위치 사용
	if (SlotWorldLocation.IsZero())
	{
		AActor* Owner = GetOwner();
		if (Owner)
			SlotWorldLocation = Owner->GetActorLocation();
	}
}

// ─────────────────────────────────────────────────────────
// 입고 — DB에 저장 후 슬롯 채우기
// Unity PalletSlot.InsertContainer()에 대응
// ─────────────────────────────────────────────────────────
void UPalletSlot::InsertContainer(const FContainerData& Data)
{
	if (!bIsEmpty)
	{
		UE_LOG(LogTemp, Warning, TEXT("[PalletSlot] %s 슬롯이 이미 점유됨"), *GetSlotKey());
		return;
	}

	Container = Data;
	bIsEmpty  = false;

	// BoxPool에서 박스 꺼내 슬롯 위치에 배치 후 자식으로 부착
	if (BoxPoolRef)
	{
		CurrentBox = BoxPoolRef->GetBox();
		if (CurrentBox)
		{
			CurrentBox->SetActorLocation(SlotWorldLocation);
			CurrentBox->AttachToActor(GetOwner(), FAttachmentTransformRules::KeepWorldTransform);
		}
	}

	if (HttpManager)
		HttpManager->AddContainer(Data);

	UE_LOG(LogTemp, Log, TEXT("[PalletSlot] 입고: %s → %s"), *Data.ContainerId, *GetSlotKey());
}

// ─────────────────────────────────────────────────────────
// 복원 — 게임 시작 시 DB 데이터로 슬롯 채우기 (DB 재저장 없음)
// Unity PalletSlot.LoadContainer()에 대응
// ─────────────────────────────────────────────────────────
void UPalletSlot::LoadContainer(const FContainerData& Data)
{
	// 기존 박스가 있으면 먼저 반환
	if (CurrentBox)
	{
		CurrentBox->DetachFromActor(FDetachmentTransformRules::KeepWorldTransform);
		if (BoxPoolRef) BoxPoolRef->ReturnBox(CurrentBox);
		CurrentBox = nullptr;
	}

	Container = Data;
	bIsEmpty  = false;

	// BoxPool에서 박스 꺼내 슬롯 위치에 배치 후 자식으로 부착
	if (BoxPoolRef)
	{
		CurrentBox = BoxPoolRef->GetBox();
		if (CurrentBox)
		{
			CurrentBox->SetActorLocation(SlotWorldLocation);
			CurrentBox->AttachToActor(GetOwner(), FAttachmentTransformRules::KeepWorldTransform);
		}
	}

	UE_LOG(LogTemp, Log, TEXT("[PalletSlot] 복원: %s → %s"), *Data.ContainerId, *GetSlotKey());
}

// ─────────────────────────────────────────────────────────
// 슬롯 비우기
// Unity PalletSlot.ClearContainer()에 대응
// ─────────────────────────────────────────────────────────
void UPalletSlot::ClearContainer()
{
	// 박스 분리 후 반환
	if (BoxPoolRef && CurrentBox)
	{
		CurrentBox->DetachFromActor(FDetachmentTransformRules::KeepWorldTransform);
		BoxPoolRef->ReturnBox(CurrentBox);
		CurrentBox = nullptr;
	}

	Container = FContainerData();
	bIsEmpty  = true;

	UE_LOG(LogTemp, Log, TEXT("[PalletSlot] 비움: %s"), *GetSlotKey());
}

// 슬롯 키 반환 (예: "A_0_3")
FString UPalletSlot::GetSlotKey() const
{
	return FString::Printf(TEXT("%s_%d_%d"), *Shelf, Floor, Slot);
}
