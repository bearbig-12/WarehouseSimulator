// Fill out your copyright notice in the Description page of Project Settings.

#include "Network/WarehouseWebSocketManager.h"
#include "WebSocketsModule.h"
#include "IWebSocket.h"
#include "Dom/JsonObject.h"
#include "Serialization/JsonReader.h"
#include "Serialization/JsonSerializer.h"

UWarehouseWebSocketManager::UWarehouseWebSocketManager()
{
	PrimaryComponentTick.bCanEverTick = false;
}

void UWarehouseWebSocketManager::BeginPlay()
{
	Super::BeginPlay();
	Connect(); // 게임 시작 시 자동 연결
}

void UWarehouseWebSocketManager::EndPlay(const EEndPlayReason::Type EndPlayReason)
{
	Disconnect();
	Super::EndPlay(EndPlayReason);
}

// ─────────────────────────────────────────────────────────
// WebSocket 연결
// ─────────────────────────────────────────────────────────
void UWarehouseWebSocketManager::Connect()
{
	if (WebSocket.IsValid() && WebSocket->IsConnected()) return;

	if (!FModuleManager::Get().IsModuleLoaded("WebSockets"))
		FModuleManager::LoadModuleChecked<FWebSocketsModule>("WebSockets");

	WebSocket = FWebSocketsModule::Get().CreateWebSocket(WebSocketUrl, TEXT("ws"));

	WebSocket->OnConnected().AddLambda([this]()
	{
		UE_LOG(LogTemp, Log, TEXT("[WebSocket] 연결됨: %s"), *WebSocketUrl);
	});

	WebSocket->OnConnectionError().AddLambda([this](const FString& Error)
	{
		UE_LOG(LogTemp, Error, TEXT("[WebSocket] 연결 실패: %s"), *Error);
	});

	WebSocket->OnClosed().AddLambda([](int32 Code, const FString& Reason, bool bWasClean)
	{
		UE_LOG(LogTemp, Warning, TEXT("[WebSocket] 종료 (Code=%d)"), Code);
	});

	WebSocket->OnMessage().AddUObject(this, &UWarehouseWebSocketManager::OnMessage);

	WebSocket->Connect();
}

void UWarehouseWebSocketManager::Disconnect()
{
	if (WebSocket.IsValid() && WebSocket->IsConnected())
		WebSocket->Close();
}

bool UWarehouseWebSocketManager::IsConnected() const
{
	return WebSocket.IsValid() && WebSocket->IsConnected();
}

// ─────────────────────────────────────────────────────────
// 메시지 수신
// 서버 JSON 형태: { "event": "containerAdded", "data": { ... } }
// ─────────────────────────────────────────────────────────
void UWarehouseWebSocketManager::OnMessage(const FString& Message)
{
	TSharedPtr<FJsonObject> Root;
	TSharedRef<TJsonReader<>> Reader = TJsonReaderFactory<>::Create(Message);
	if (!FJsonSerializer::Deserialize(Reader, Root) || !Root.IsValid()) return;

	FString Event;
	if (!Root->TryGetStringField(TEXT("event"), Event)) return;

	const TSharedPtr<FJsonObject>* DataObj;

	if (Event == TEXT("containerAdded") && Root->TryGetObjectField(TEXT("data"), DataObj))
	{
		FContainerData D;
		(*DataObj)->TryGetStringField(TEXT("container_id"), D.ContainerId);
		(*DataObj)->TryGetStringField(TEXT("item_name"),    D.ItemName);
		(*DataObj)->TryGetStringField(TEXT("arrival_date"), D.ArrivalDate);
		(*DataObj)->TryGetStringField(TEXT("shelf"),        D.Shelf);
		double Tmp = 0;
		if ((*DataObj)->TryGetNumberField(TEXT("weight"), Tmp)) D.Weight = (float)Tmp;
		if ((*DataObj)->TryGetNumberField(TEXT("floor"),  Tmp)) D.Floor  = (int32)Tmp;
		if ((*DataObj)->TryGetNumberField(TEXT("slot"),   Tmp)) D.Slot   = (int32)Tmp;
		if ((*DataObj)->TryGetNumberField(TEXT("width"),  Tmp)) D.Width  = (float)Tmp;
		if ((*DataObj)->TryGetNumberField(TEXT("depth"),  Tmp)) D.Depth  = (float)Tmp;
		if ((*DataObj)->TryGetNumberField(TEXT("height"), Tmp)) D.Height = (float)Tmp;
		OnContainerAdded.Broadcast(D);
	}
	else if (Event == TEXT("containerMoved") && Root->TryGetObjectField(TEXT("data"), DataObj))
	{
		FContainerData D;
		(*DataObj)->TryGetStringField(TEXT("container_id"), D.ContainerId);
		(*DataObj)->TryGetStringField(TEXT("shelf"),        D.Shelf);
		double Tmp = 0;
		if ((*DataObj)->TryGetNumberField(TEXT("floor"), Tmp)) D.Floor = (int32)Tmp;
		if ((*DataObj)->TryGetNumberField(TEXT("slot"),  Tmp)) D.Slot  = (int32)Tmp;
		OnContainerMoved.Broadcast(D);
	}
	else if (Event == TEXT("containerRemoved") && Root->TryGetObjectField(TEXT("data"), DataObj))
	{
		FString ContainerId;
		(*DataObj)->TryGetStringField(TEXT("container_id"), ContainerId);
		OnContainerRemoved.Broadcast(ContainerId);
	}
}
