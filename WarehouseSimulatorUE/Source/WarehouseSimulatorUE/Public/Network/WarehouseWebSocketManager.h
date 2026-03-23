// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "Network/WarehouseHttpManager.h"
#include "WarehouseWebSocketManager.generated.h"

// IWebSocket은 .cpp에서 include
class IWebSocket;

// 서버 실시간 이벤트 델리게이트 3종
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOnWSContainerAdded,   FContainerData, Data);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOnWSContainerMoved,   FContainerData, Data);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOnWSContainerRemoved, FString,        ContainerId);

/**
 * 실시간 WebSocket 수신 담당
 * Unity WinForms의 socket.io 수신과 동일한 역할
 * 서버에서 containerAdded / containerMoved / containerRemoved 이벤트 수신
 */
UCLASS(ClassGroup=(Warehouse), meta=(BlueprintSpawnableComponent))
class WAREHOUSESIMULATORUE_API UWarehouseWebSocketManager : public UActorComponent
{
	GENERATED_BODY()

public:
	UWarehouseWebSocketManager();

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Warehouse|Network")
	FString WebSocketUrl = TEXT("ws://localhost:3000");

	UPROPERTY(BlueprintAssignable, Category="Warehouse|Events")
	FOnWSContainerAdded OnContainerAdded;

	UPROPERTY(BlueprintAssignable, Category="Warehouse|Events")
	FOnWSContainerMoved OnContainerMoved;

	UPROPERTY(BlueprintAssignable, Category="Warehouse|Events")
	FOnWSContainerRemoved OnContainerRemoved;

	UFUNCTION(BlueprintCallable, Category="Warehouse|WebSocket")
	void Connect();

	UFUNCTION(BlueprintCallable, Category="Warehouse|WebSocket")
	void Disconnect();

	UFUNCTION(BlueprintPure, Category="Warehouse|WebSocket")
	bool IsConnected() const;

protected:
	virtual void BeginPlay() override;
	virtual void EndPlay(const EEndPlayReason::Type EndPlayReason) override;

private:
	TSharedPtr<IWebSocket> WebSocket;

	void OnMessage(const FString& Message);
};
