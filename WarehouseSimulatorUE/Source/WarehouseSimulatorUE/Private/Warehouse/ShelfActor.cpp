// Fill out your copyright notice in the Description page of Project Settings.

#include "Warehouse/ShelfActor.h"
#include "Warehouse/PalletSlot.h"

AShelfActor::AShelfActor()
{
	PrimaryActorTick.bCanEverTick = false;
}

void AShelfActor::BeginPlay()
{
	Super::BeginPlay();

	for (int32 f = 0; f < NumFloors; f++)
	{
		for (int32 s = 0; s < NumSlotsPerFloor; s++)
		{
			UPalletSlot* Slot = NewObject<UPalletSlot>(this);
			Slot->Shelf = ShelfLabel;
			Slot->Floor = f;
			Slot->Slot  = s;

			// 슬롯 월드 위치 계산 (X: 슬롯 방향, Z: 층 높이)
			FVector LocalOffset(s * SlotSpacingX, 0.f, f * FloorSpacingZ);
			Slot->SlotWorldLocation = GetActorLocation() + GetActorRotation().RotateVector(LocalOffset);

			Slot->RegisterComponent();
			PalletSlots.Add(Slot);
		}
	}

	UE_LOG(LogTemp, Log, TEXT("[ShelfActor] %s 슬롯 %d개 생성 완료"), *ShelfLabel, PalletSlots.Num());
}
