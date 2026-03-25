// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/PlayerController.h"
#include "WarehousePlayerController.generated.h"

class UWarehousePopupWidget;

UCLASS()
class WAREHOUSESIMULATORUE_API AWarehousePlayerController : public APlayerController
{
	GENERATED_BODY()

public:
	// 에디터에서 WBP_SlotPopup 지정
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="UI")
	TSubclassOf<UWarehousePopupWidget> PopupWidgetClass;

protected:
	virtual void BeginPlay() override;
	virtual void SetupInputComponent() override;

private:
	UPROPERTY()
	UWarehousePopupWidget* PopupWidget = nullptr;

	UFUNCTION(BlueprintCallable, Category="UI")
	void OnLeftClick();
};
