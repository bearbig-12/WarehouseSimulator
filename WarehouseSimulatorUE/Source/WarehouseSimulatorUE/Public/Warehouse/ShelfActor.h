// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "ShelfActor.generated.h"

class UPalletSlot;

/**
 * 선반 1개를 담당 — BeginPlay에서 PalletSlot 컴포넌트를 자동 생성
 * 레벨에 4개 배치 후 ShelfLabel만 A/B/C/D로 설정
 */
UCLASS()
class WAREHOUSESIMULATORUE_API AShelfActor : public AActor
{
	GENERATED_BODY()

public:
	AShelfActor();

	// 선반 이름 (A/B/C/D)
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Warehouse|Shelf")
	FString ShelfLabel = TEXT("A");

	// 층 수 (기본 3)
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Warehouse|Shelf")
	int32 NumFloors = 3;

	// 층당 슬롯 수 (기본 8)
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Warehouse|Shelf")
	int32 NumSlotsPerFloor = 8;

	// 슬롯 간격 X (cm)
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Warehouse|Shelf")
	float SlotSpacingX = 120.f;

	// 층 높이 간격 Z (cm)
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Warehouse|Shelf")
	float FloorSpacingZ = 150.f;

protected:
	virtual void BeginPlay() override;

private:
	UPROPERTY()
	TArray<UPalletSlot*> PalletSlots;
};
