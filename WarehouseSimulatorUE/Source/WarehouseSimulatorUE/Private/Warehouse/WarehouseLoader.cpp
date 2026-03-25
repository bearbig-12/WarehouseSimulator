// Fill out your copyright notice in the Description page of Project Settings.

#include "Warehouse/WarehouseLoader.h"
#include "Warehouse/PalletSlot.h"
#include "EngineUtils.h"

AWarehouseLoader::AWarehouseLoader()
{
	PrimaryActorTick.bCanEverTick = false;
}

void AWarehouseLoader::BeginPlay()
{
	Super::BeginPlay();

	// 레벨에서 HttpManager 찾기
	for (TActorIterator<AActor> It(GetWorld()); It; ++It)
	{
		HttpManager = (*It)->FindComponentByClass<UWarehouseHttpManager>();
		if (HttpManager) break;
	}

	if (!HttpManager)
	{
		UE_LOG(LogTemp, Error, TEXT("[WarehouseLoader] HttpManager를 찾을 수 없음"));
		return;
	}

	// HTTP 로드 완료 콜백
	HttpManager->OnContainersLoaded.AddDynamic(this, &AWarehouseLoader::OnContainersLoaded);

	// WebSocketManager 찾기 + 실시간 이벤트 구독
	for (TActorIterator<AActor> It(GetWorld()); It; ++It)
	{
		WebSocketManager = (*It)->FindComponentByClass<UWarehouseWebSocketManager>();
		if (WebSocketManager) break;
	}

	if (WebSocketManager)
	{
		WebSocketManager->OnContainerAdded.AddDynamic(this,   &AWarehouseLoader::OnWSContainerAdded);
		WebSocketManager->OnContainerMoved.AddDynamic(this,   &AWarehouseLoader::OnWSContainerMoved);
		WebSocketManager->OnContainerRemoved.AddDynamic(this, &AWarehouseLoader::OnWSContainerRemoved);
		UE_LOG(LogTemp, Log, TEXT("[WarehouseLoader] WebSocket 이벤트 구독 완료"));
	}
	else
	{
		UE_LOG(LogTemp, Warning, TEXT("[WarehouseLoader] WebSocketManager를 찾을 수 없음"));
	}

	// ShelfRowActor BeginPlay가 먼저 끝나도록 한 프레임 뒤에 초기화
	GetWorldTimerManager().SetTimerForNextTick(this, &AWarehouseLoader::InitSlots);
}

void AWarehouseLoader::InitSlots()
{
	// 레벨의 모든 PalletSlot 수집 → SlotMap 구성
	for (TActorIterator<AActor> It(GetWorld()); It; ++It)
	{
		TArray<UPalletSlot*> Slots;
		(*It)->GetComponents<UPalletSlot>(Slots);
		for (UPalletSlot* Slot : Slots)
			SlotMap.Add(Slot->GetSlotKey(), Slot);
	}

	UE_LOG(LogTemp, Log, TEXT("[WarehouseLoader] 슬롯 %d개 수집됨"), SlotMap.Num());

	// DB 초기 로드
	LoadFromDB();

	// 3초 폴링 타이머 시작
	GetWorldTimerManager().SetTimer(
		PollTimerHandle,
		this,
		&AWarehouseLoader::LoadFromDB,
		PollInterval,
		true
	);
}

void AWarehouseLoader::LoadFromDB()
{
	if (HttpManager)
		HttpManager->LoadAllContainers();
}

// ─────────────────────────────────────────────────────────
// DB 로드 완료 콜백
// Unity WarehouseLoader.OnRefresh()에 대응
// ─────────────────────────────────────────────────────────
void AWarehouseLoader::OnContainersLoaded(const TArray<FContainerData>& Containers)
{
	// DB에서 점유 중인 슬롯 키 목록
	TSet<FString> DBSlotKeys;
	for (const FContainerData& D : Containers)
		DBSlotKeys.Add(FString::Printf(TEXT("%s_%d_%d"), *D.Shelf, D.Floor, D.SlotIndex));

	// DB에 없는 슬롯은 비우기
	for (auto& Pair : SlotMap)
	{
		UPalletSlot* S = Pair.Value;
		if (S && !S->bIsEmpty && !DBSlotKeys.Contains(Pair.Key))
			S->ClearContainer();
	}

	// DB에 있는 컨테이너 슬롯에 복원
	for (const FContainerData& D : Containers)
	{
		FString Key = FString::Printf(TEXT("%s_%d_%d"), *D.Shelf, D.Floor, D.SlotIndex);
		UPalletSlot** SlotPtr = SlotMap.Find(Key);
		if (!SlotPtr) continue;

		UPalletSlot* S = *SlotPtr;
		// 이미 같은 컨테이너면 스킵
		if (!S->bIsEmpty && S->Container.ContainerId == D.ContainerId) continue;

		S->LoadContainer(D);
	}
}

// ─────────────────────────────────────────────────────────
// WebSocket 실시간 이벤트 핸들러
// ─────────────────────────────────────────────────────────
void AWarehouseLoader::OnWSContainerAdded(FContainerData Data)
{
	FString Key = FString::Printf(TEXT("%s_%d_%d"), *Data.Shelf, Data.Floor, Data.SlotIndex);
	UPalletSlot** SlotPtr = SlotMap.Find(Key);
	if (!SlotPtr) return;

	UPalletSlot* S = *SlotPtr;
	if (!S->bIsEmpty && S->Container.ContainerId == Data.ContainerId) return;

	S->LoadContainer(Data);
	UE_LOG(LogTemp, Log, TEXT("[WarehouseLoader] WS 입고: %s → %s"), *Data.ContainerId, *Key);
}

void AWarehouseLoader::OnWSContainerMoved(FContainerData Data)
{
	// 기존 슬롯 찾아서 데이터 보존 후 비우기
	FContainerData FullData;
	for (auto& Pair : SlotMap)
	{
		UPalletSlot* S = Pair.Value;
		if (S && !S->bIsEmpty && S->Container.ContainerId == Data.ContainerId)
		{
			FullData = S->Container;
			S->ClearContainer();
			break;
		}
	}

	// 새 위치로 업데이트 후 로드
	FullData.Shelf = Data.Shelf;
	FullData.Floor = Data.Floor;
	FullData.SlotIndex = Data.SlotIndex;

	FString Key = FString::Printf(TEXT("%s_%d_%d"), *FullData.Shelf, FullData.Floor, FullData.SlotIndex);
	UPalletSlot** SlotPtr = SlotMap.Find(Key);
	if (SlotPtr)
		(*SlotPtr)->LoadContainer(FullData);

	UE_LOG(LogTemp, Log, TEXT("[WarehouseLoader] WS 이동: %s → %s"), *Data.ContainerId, *Key);
}

void AWarehouseLoader::OnWSContainerRemoved(FString ContainerId)
{
	for (auto& Pair : SlotMap)
	{
		UPalletSlot* S = Pair.Value;
		if (S && !S->bIsEmpty && S->Container.ContainerId == ContainerId)
		{
			S->ClearContainer();
			UE_LOG(LogTemp, Log, TEXT("[WarehouseLoader] WS 출고: %s"), *ContainerId);
			break;
		}
	}
}
