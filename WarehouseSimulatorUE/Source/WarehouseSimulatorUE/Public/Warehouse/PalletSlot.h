// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "Network/WarehouseHttpManager.h"
#include "PalletSlot.generated.h"

class ABoxPool;

/**
 * Unity PalletSlot.cs에 대응
 * 팔레트 슬롯 하나를 담당 — 컨테이너 입고/이동/출고/복원 관리
 */
UCLASS(ClassGroup=(Warehouse), meta=(BlueprintSpawnableComponent))
class WAREHOUSESIMULATORUE_API UPalletSlot : public UActorComponent
{
	GENERATED_BODY()

public:
	UPalletSlot();

	// 슬롯 위치 정보 (에디터에서 설정)
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Warehouse|Slot")
	FString Shelf;   // A/B/C/D

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Warehouse|Slot")
	int32 Floor = 0; // 0/1/2

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Warehouse|Slot")
	int32 Slot = 0;  // 0~7

	// 현재 슬롯에 있는 컨테이너 데이터
	UPROPERTY(BlueprintReadOnly, Category="Warehouse|Slot")
	FContainerData Container;

	UPROPERTY(BlueprintReadOnly, Category="Warehouse|Slot")
	bool bIsEmpty = true;

	// ShelfActor가 BeginPlay에서 설정 — 슬롯의 월드 위치
	UPROPERTY(BlueprintReadOnly, Category="Warehouse|Slot")
	FVector SlotWorldLocation = FVector::ZeroVector;

	// ── 함수 ────────────────────────────────────────────────────

	// 입고 (DB에 저장 + 박스 스폰)
	UFUNCTION(BlueprintCallable, Category="Warehouse|Slot")
	void InsertContainer(const FContainerData& Data);

	// DB 복원용 (게임 시작 시 — DB 재저장 없음)
	UFUNCTION(BlueprintCallable, Category="Warehouse|Slot")
	void LoadContainer(const FContainerData& Data);

	// 출고/이동 시 슬롯 비우기
	UFUNCTION(BlueprintCallable, Category="Warehouse|Slot")
	void ClearContainer();

	// 슬롯 키 반환 (예: "A_0_3")
	UFUNCTION(BlueprintPure, Category="Warehouse|Slot")
	FString GetSlotKey() const;

	// Http 매니저 참조 (BeginPlay에서 자동 검색)
	UPROPERTY(BlueprintReadWrite, Category="Warehouse|Slot")
	UWarehouseHttpManager* HttpManager = nullptr;

	// BoxPool 참조 (BeginPlay에서 자동 검색)
	UPROPERTY()
	ABoxPool* BoxPoolRef = nullptr;

	// 현재 슬롯에 스폰된 박스 액터
	UPROPERTY()
	AActor* CurrentBox = nullptr;

protected:
	virtual void BeginPlay() override;
};
