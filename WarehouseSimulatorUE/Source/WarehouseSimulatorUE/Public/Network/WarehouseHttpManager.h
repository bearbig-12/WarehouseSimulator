// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "WarehouseHttpManager.generated.h"

// Http.h는 .cpp에서 include — 헤더엔 forward declaration만
class IHttpRequest;
class IHttpResponse;

// Unity의 ContainerData struct와 동일
USTRUCT(BlueprintType)
struct FContainerData
{
	GENERATED_BODY()

	UPROPERTY(BlueprintReadWrite) FString ContainerId;
	UPROPERTY(BlueprintReadWrite) FString ItemName;
	UPROPERTY(BlueprintReadWrite) float   Weight      = 0.f;
	UPROPERTY(BlueprintReadWrite) FString ArrivalDate;
	UPROPERTY(BlueprintReadWrite) FString Shelf;
	UPROPERTY(BlueprintReadWrite) int32   Floor       = 0;
	UPROPERTY(BlueprintReadWrite) int32   SlotIndex   = 0;
	UPROPERTY(BlueprintReadWrite) float   Width       = 1.f;
	UPROPERTY(BlueprintReadWrite) float   Depth       = 1.f;
	UPROPERTY(BlueprintReadWrite) float   Height      = 1.f;
};

DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOnContainersLoaded, const TArray<FContainerData>&, Containers);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOnRequestResult,    bool,                          bSuccess);

/**
 * Unity DatabaseManager.cs에 대응
 * HTTP REST API 호출 담당 (GET / POST / PATCH / DELETE)
 */
UCLASS(ClassGroup=(Warehouse), meta=(BlueprintSpawnableComponent))
class WAREHOUSESIMULATORUE_API UWarehouseHttpManager : public UActorComponent
{
	GENERATED_BODY()

public:
	UWarehouseHttpManager();

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Warehouse|Network")
	FString ServerUrl = TEXT("http://localhost:3000");

	UPROPERTY(BlueprintAssignable, Category="Warehouse|Events")
	FOnContainersLoaded OnContainersLoaded;

	UPROPERTY(BlueprintAssignable, Category="Warehouse|Events")
	FOnRequestResult OnContainerAdded;

	UPROPERTY(BlueprintAssignable, Category="Warehouse|Events")
	FOnRequestResult OnContainerMoved;

	UPROPERTY(BlueprintAssignable, Category="Warehouse|Events")
	FOnRequestResult OnContainerRemoved;

	UFUNCTION(BlueprintCallable, Category="Warehouse|HTTP")
	void LoadAllContainers();

	UFUNCTION(BlueprintCallable, Category="Warehouse|HTTP")
	void AddContainer(const FContainerData& Data);

	UFUNCTION(BlueprintCallable, Category="Warehouse|HTTP")
	void MoveContainer(const FString& ContainerId, const FString& Shelf, int32 Floor, int32 Slot);

	UFUNCTION(BlueprintCallable, Category="Warehouse|HTTP")
	void RemoveContainer(const FString& ContainerId);

private:
	TArray<FContainerData> ParseContainerArray(const FString& JsonString);

	// FHttpRequestPtr  = TSharedPtr<IHttpRequest, ESPMode::ThreadSafe>
	// FHttpResponsePtr = TSharedPtr<IHttpResponse>
	void OnLoadAllResponse(TSharedPtr<IHttpRequest, ESPMode::ThreadSafe> Req, TSharedPtr<IHttpResponse> Res, bool bConnected);
	void OnAddResponse    (TSharedPtr<IHttpRequest, ESPMode::ThreadSafe> Req, TSharedPtr<IHttpResponse> Res, bool bConnected);
	void OnMoveResponse   (TSharedPtr<IHttpRequest, ESPMode::ThreadSafe> Req, TSharedPtr<IHttpResponse> Res, bool bConnected);
	void OnRemoveResponse (TSharedPtr<IHttpRequest, ESPMode::ThreadSafe> Req, TSharedPtr<IHttpResponse> Res, bool bConnected);
};
