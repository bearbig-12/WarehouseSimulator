// Fill out your copyright notice in the Description page of Project Settings.

#include "Warehouse/ShelfRowActor.h"
#include "Warehouse/PalletSlot.h"

AShelfRowActor::AShelfRowActor()
{
	PrimaryActorTick.bCanEverTick = false;
}

void AShelfRowActor::BeginPlay()
{
	Super::BeginPlay();

	// 자식 액터 목록 가져오기
	TArray<AActor*> SlotActors;
	GetAttachedActors(SlotActors);

	// 이름 오름차순 정렬 (PalletSlot1 → index 0, PalletSlot8 → index 7)
	SlotActors.Sort([](const AActor& A, const AActor& B)
	{
		return A.GetActorLabel() < B.GetActorLabel();
	});

	// 각 자식에게 Shelf/Floor/Slot 설정
	for (int32 i = 0; i < SlotActors.Num(); i++)
	{
		UPalletSlot* SlotComp = SlotActors[i]->FindComponentByClass<UPalletSlot>();
		if (SlotComp)
		{
			SlotComp->Shelf = Shelf;
			SlotComp->Floor = Floor;
			SlotComp->Slot  = i;
		}
	}

	UE_LOG(LogTemp, Log, TEXT("[ShelfRowActor] %s층%d — 슬롯 %d개 설정"), *Shelf, Floor, SlotActors.Num());
}
