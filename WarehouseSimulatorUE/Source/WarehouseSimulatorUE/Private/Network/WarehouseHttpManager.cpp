// Fill out your copyright notice in the Description page of Project Settings.

#include "Network/WarehouseHttpManager.h"
#include "HttpModule.h"
#include "Interfaces/IHttpRequest.h"
#include "Interfaces/IHttpResponse.h"
#include "Dom/JsonObject.h"
#include "Dom/JsonValue.h"
#include "Serialization/JsonReader.h"
#include "Serialization/JsonSerializer.h"
#include "Serialization/JsonWriter.h"

UWarehouseHttpManager::UWarehouseHttpManager()
{
	PrimaryComponentTick.bCanEverTick = false;
}

// ─────────────────────────────────────────────────────────
// GET /containers
// ─────────────────────────────────────────────────────────
void UWarehouseHttpManager::LoadAllContainers()
{
	TSharedRef<IHttpRequest, ESPMode::ThreadSafe> Req = FHttpModule::Get().CreateRequest();
	Req->SetURL(ServerUrl + TEXT("/containers"));
	Req->SetVerb(TEXT("GET"));
	Req->SetHeader(TEXT("Content-Type"), TEXT("application/json"));
	Req->OnProcessRequestComplete().BindUObject(this, &UWarehouseHttpManager::OnLoadAllResponse);
	Req->ProcessRequest();
}

void UWarehouseHttpManager::OnLoadAllResponse(TSharedPtr<IHttpRequest, ESPMode::ThreadSafe> Req, TSharedPtr<IHttpResponse> Res, bool bConnected)
{
	if (!bConnected || !Res.IsValid() || Res->GetResponseCode() != 200)
	{
		UE_LOG(LogTemp, Error, TEXT("[HttpManager] LoadAllContainers 실패"));
		return;
	}
	TArray<FContainerData> Result = ParseContainerArray(Res->GetContentAsString());
	OnContainersLoaded.Broadcast(Result);
}

// ─────────────────────────────────────────────────────────
// POST /containers
// ─────────────────────────────────────────────────────────
void UWarehouseHttpManager::AddContainer(const FContainerData& Data)
{
	TSharedPtr<FJsonObject> Json = MakeShared<FJsonObject>();
	Json->SetStringField(TEXT("container_id"),  Data.ContainerId);
	Json->SetStringField(TEXT("item_name"),     Data.ItemName);
	Json->SetNumberField(TEXT("weight"),        Data.Weight);
	Json->SetStringField(TEXT("arrival_date"),  Data.ArrivalDate);
	Json->SetStringField(TEXT("shelf"),         Data.Shelf);
	Json->SetNumberField(TEXT("floor"),         Data.Floor);
	Json->SetNumberField(TEXT("slot"),          Data.Slot);
	Json->SetNumberField(TEXT("width"),         Data.Width);
	Json->SetNumberField(TEXT("depth"),         Data.Depth);
	Json->SetNumberField(TEXT("height"),        Data.Height);

	FString Body;
	TSharedRef<TJsonWriter<>> Writer = TJsonWriterFactory<>::Create(&Body);
	FJsonSerializer::Serialize(Json.ToSharedRef(), Writer);

	TSharedRef<IHttpRequest, ESPMode::ThreadSafe> Req = FHttpModule::Get().CreateRequest();
	Req->SetURL(ServerUrl + TEXT("/containers"));
	Req->SetVerb(TEXT("POST"));
	Req->SetHeader(TEXT("Content-Type"), TEXT("application/json"));
	Req->SetContentAsString(Body);
	Req->OnProcessRequestComplete().BindUObject(this, &UWarehouseHttpManager::OnAddResponse);
	Req->ProcessRequest();
}

void UWarehouseHttpManager::OnAddResponse(TSharedPtr<IHttpRequest, ESPMode::ThreadSafe> Req, TSharedPtr<IHttpResponse> Res, bool bConnected)
{
	bool bOk = bConnected && Res.IsValid() && Res->GetResponseCode() == 200;
	if (!bOk) UE_LOG(LogTemp, Error, TEXT("[HttpManager] AddContainer 실패"));
	OnContainerAdded.Broadcast(bOk);
}

// ─────────────────────────────────────────────────────────
// PATCH /containers/:id/move
// ─────────────────────────────────────────────────────────
void UWarehouseHttpManager::MoveContainer(const FString& ContainerId, const FString& Shelf, int32 Floor, int32 Slot)
{
	TSharedPtr<FJsonObject> Json = MakeShared<FJsonObject>();
	Json->SetStringField(TEXT("shelf"), Shelf);
	Json->SetNumberField(TEXT("floor"), Floor);
	Json->SetNumberField(TEXT("slot"),  Slot);

	FString Body;
	TSharedRef<TJsonWriter<>> Writer = TJsonWriterFactory<>::Create(&Body);
	FJsonSerializer::Serialize(Json.ToSharedRef(), Writer);

	TSharedRef<IHttpRequest, ESPMode::ThreadSafe> Req = FHttpModule::Get().CreateRequest();
	Req->SetURL(ServerUrl + TEXT("/containers/") + ContainerId + TEXT("/move"));
	Req->SetVerb(TEXT("PATCH"));
	Req->SetHeader(TEXT("Content-Type"), TEXT("application/json"));
	Req->SetContentAsString(Body);
	Req->OnProcessRequestComplete().BindUObject(this, &UWarehouseHttpManager::OnMoveResponse);
	Req->ProcessRequest();
}

void UWarehouseHttpManager::OnMoveResponse(TSharedPtr<IHttpRequest, ESPMode::ThreadSafe> Req, TSharedPtr<IHttpResponse> Res, bool bConnected)
{
	bool bOk = bConnected && Res.IsValid() && Res->GetResponseCode() == 200;
	if (!bOk) UE_LOG(LogTemp, Error, TEXT("[HttpManager] MoveContainer 실패"));
	OnContainerMoved.Broadcast(bOk);
}

// ─────────────────────────────────────────────────────────
// DELETE /containers/:id
// ─────────────────────────────────────────────────────────
void UWarehouseHttpManager::RemoveContainer(const FString& ContainerId)
{
	TSharedRef<IHttpRequest, ESPMode::ThreadSafe> Req = FHttpModule::Get().CreateRequest();
	Req->SetURL(ServerUrl + TEXT("/containers/") + ContainerId);
	Req->SetVerb(TEXT("DELETE"));
	Req->SetHeader(TEXT("Content-Type"), TEXT("application/json"));
	Req->OnProcessRequestComplete().BindUObject(this, &UWarehouseHttpManager::OnRemoveResponse);
	Req->ProcessRequest();
}

void UWarehouseHttpManager::OnRemoveResponse(TSharedPtr<IHttpRequest, ESPMode::ThreadSafe> Req, TSharedPtr<IHttpResponse> Res, bool bConnected)
{
	bool bOk = bConnected && Res.IsValid() && Res->GetResponseCode() == 200;
	if (!bOk) UE_LOG(LogTemp, Error, TEXT("[HttpManager] RemoveContainer 실패"));
	OnContainerRemoved.Broadcast(bOk);
}

// ─────────────────────────────────────────────────────────
// JSON 배열 파싱
// ─────────────────────────────────────────────────────────
TArray<FContainerData> UWarehouseHttpManager::ParseContainerArray(const FString& JsonString)
{
	TArray<FContainerData> Result;

	TArray<TSharedPtr<FJsonValue>> JsonArray;
	TSharedRef<TJsonReader<>> Reader = TJsonReaderFactory<>::Create(JsonString);
	if (!FJsonSerializer::Deserialize(Reader, JsonArray)) return Result;

	for (auto& Val : JsonArray)
	{
		TSharedPtr<FJsonObject> Obj = Val->AsObject();
		if (!Obj.IsValid()) continue;

		FContainerData D;
		D.ContainerId = Obj->GetStringField(TEXT("container_id"));
		D.ItemName    = Obj->GetStringField(TEXT("item_name"));
		D.Weight      = (float)Obj->GetNumberField(TEXT("weight"));
		D.ArrivalDate = Obj->GetStringField(TEXT("arrival_date"));
		D.Shelf       = Obj->GetStringField(TEXT("shelf"));
		D.Floor       = (int32)Obj->GetNumberField(TEXT("floor"));
		D.Slot        = (int32)Obj->GetNumberField(TEXT("slot"));
		D.Width       = Obj->HasField(TEXT("width"))  ? (float)Obj->GetNumberField(TEXT("width"))  : 1.f;
		D.Depth       = Obj->HasField(TEXT("depth"))  ? (float)Obj->GetNumberField(TEXT("depth"))  : 1.f;
		D.Height      = Obj->HasField(TEXT("height")) ? (float)Obj->GetNumberField(TEXT("height")) : 1.f;
		Result.Add(D);
	}

	return Result;
}
