// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "Network/WarehouseHttpManager.h"
#include "Network/WarehouseWebSocketManager.h"
#include "WarehouseLoader.generated.h"

class UPalletSlot;

/**
 * Unity WarehouseLoader.cs에 대응
 * 게임 시작 시 DB에서 전체 컨테이너 복원 + 3초 폴링으로 실시간 동기화
 */
UCLASS()
class WAREHOUSESIMULATORUE_API AWarehouseLoader : public AActor
{
	GENERATED_BODY()

public:
	AWarehouseLoader();

	// 폴링 주기 (초) — 기본 3초
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Warehouse|Loader")
	float PollInterval = 3.f;

protected:
	virtual void BeginPlay() override;

private:
	UPROPERTY()
	UWarehouseHttpManager* HttpManager = nullptr;

	UPROPERTY()
	UWarehouseWebSocketManager* WebSocketManager = nullptr;

	// 슬롯 맵: "A_0_3" → UPalletSlot*
	TMap<FString, UPalletSlot*> SlotMap;

	// 한 프레임 뒤 슬롯 수집 + 초기 DB 로드
	void InitSlots();

	// DB에서 전체 컨테이너 로드 → 슬롯에 복원
	void LoadFromDB();

	// HTTP 로드 완료 콜백
	UFUNCTION()
	void OnContainersLoaded(const TArray<FContainerData>& Containers);

	// WebSocket 실시간 이벤트 콜백
	UFUNCTION()
	void OnWSContainerAdded(FContainerData Data);

	UFUNCTION()
	void OnWSContainerMoved(FContainerData Data);

	UFUNCTION()
	void OnWSContainerRemoved(FString ContainerId);

	// 3초 폴링 타이머 핸들
	FTimerHandle PollTimerHandle;
};
