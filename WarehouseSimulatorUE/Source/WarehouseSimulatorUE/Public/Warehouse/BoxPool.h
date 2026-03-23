// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "BoxPool.generated.h"

/**
 * Unity BoxPool.cs에 대응
 * PlasticBox 스태틱메시 액터를 미리 96개 생성해두고 재사용
 * Shelf 4 × Floor 3 × Slot 8 = 96개
 */
UCLASS()
class WAREHOUSESIMULATORUE_API ABoxPool : public AActor
{
	GENERATED_BODY()

public:
	ABoxPool();

	// Inspector에서 박스 스태틱메시 연결
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Warehouse|Pool")
	UStaticMesh* BoxMesh = nullptr;

	// 풀 초기 크기 (기본 96)
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Warehouse|Pool")
	int32 PoolSize = 96;

	// 풀에서 박스 꺼내기 (Unity BoxPool.Get()에 대응)
	UFUNCTION(BlueprintCallable, Category="Warehouse|Pool")
	AActor* GetBox();

	// 박스 반환 (Unity BoxPool.Return()에 대응)
	UFUNCTION(BlueprintCallable, Category="Warehouse|Pool")
	void ReturnBox(AActor* Box);

protected:
	virtual void BeginPlay() override;

private:
	TArray<AActor*> Pool;

	AActor* CreateBoxActor();
};
