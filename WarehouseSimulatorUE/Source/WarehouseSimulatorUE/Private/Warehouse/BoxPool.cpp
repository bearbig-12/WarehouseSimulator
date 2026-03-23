// Fill out your copyright notice in the Description page of Project Settings.

#include "Warehouse/BoxPool.h"
#include "Components/StaticMeshComponent.h"
#include "Engine/StaticMesh.h"

ABoxPool::ABoxPool()
{
	PrimaryActorTick.bCanEverTick = false;
}

void ABoxPool::BeginPlay()
{
	Super::BeginPlay();

	// 시작 시 PoolSize만큼 박스 미리 생성 후 비활성화
	// Unity의 BoxPool.Start()에서 96개 미리 생성하는 것과 동일
	for (int32 i = 0; i < PoolSize; i++)
	{
		AActor* Box = CreateBoxActor();
		if (Box)
		{
			Box->SetActorHiddenInGame(true);
			Box->SetActorEnableCollision(false);
			Pool.Add(Box);
		}
	}

	UE_LOG(LogTemp, Log, TEXT("[BoxPool] 풀 초기화 완료: %d개"), Pool.Num());
}

// ─────────────────────────────────────────────────────────
// 풀에서 박스 꺼내기
// Unity BoxPool.Get()에 대응
// ─────────────────────────────────────────────────────────
AActor* ABoxPool::GetBox()
{
	// 풀에 쉬고 있는 박스 찾기
	for (AActor* Box : Pool)
	{
		if (Box && Box->IsHidden())
		{
			Box->SetActorHiddenInGame(false);
			Box->SetActorEnableCollision(true);
			return Box;
		}
	}

	// 풀 소진 시 새로 생성 (Unity의 자동 확장과 동일)
	UE_LOG(LogTemp, Warning, TEXT("[BoxPool] 풀 소진 — 새 박스 생성"));
	AActor* NewBox = CreateBoxActor();
	if (NewBox) Pool.Add(NewBox);
	return NewBox;
}

// ─────────────────────────────────────────────────────────
// 박스 반환
// Unity BoxPool.Return()에 대응
// ─────────────────────────────────────────────────────────
void ABoxPool::ReturnBox(AActor* Box)
{
	if (!Box) return;

	Box->SetActorHiddenInGame(true);
	Box->SetActorEnableCollision(false);
	Box->SetActorLocation(GetActorLocation()); // BoxPool 위치로 이동
}

// 박스 액터 생성 헬퍼
AActor* ABoxPool::CreateBoxActor()
{
	FActorSpawnParameters Params;
	Params.Owner = this;

	AActor* Box = GetWorld()->SpawnActor<AActor>(AActor::StaticClass(), GetActorLocation(), FRotator::ZeroRotator, Params);
	if (!Box) return nullptr;

	UStaticMeshComponent* MeshComp = NewObject<UStaticMeshComponent>(Box);
	MeshComp->RegisterComponent();
	Box->SetRootComponent(MeshComp);

	if (BoxMesh)
		MeshComp->SetStaticMesh(BoxMesh);

	return Box;
}
