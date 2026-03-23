// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "ShelfRowActor.generated.h"

/**
 * 선반 한 층을 담당 — BeginPlay에서 자식 BP_PalletSlot들에게
 * Shelf/Floor/Slot 값을 자동으로 설정
 */
UCLASS()
class WAREHOUSESIMULATORUE_API AShelfRowActor : public AActor
{
	GENERATED_BODY()

public:
	AShelfRowActor();

	// 선반 이름 (A/B/C/D)
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Warehouse|ShelfRow")
	FString Shelf = TEXT("A");

	// 층 번호 (0/1/2)
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Warehouse|ShelfRow")
	int32 Floor = 0;

protected:
	virtual void BeginPlay() override;
};
