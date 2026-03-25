// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Blueprint/UserWidget.h"
#include "Network/WarehouseHttpManager.h"
#include "WarehousePopupWidget.generated.h"

class UPalletSlot;
class UTextBlock;
class UEditableTextBox;
class UButton;
class UWidget;

UCLASS()
class WAREHOUSESIMULATORUE_API UWarehousePopupWidget : public UUserWidget
{
	GENERATED_BODY()

public:
	// PlayerController에서 클릭 시 호출
	void HandleSlotClick(UPalletSlot* InSlot);

protected:
	virtual void NativeConstruct() override;

	// ── 패널 ──────────────────────────────────────
	UPROPERTY(meta=(BindWidget))
	UWidget* PopupPanel;

	UPROPERTY(meta=(BindWidget))
	UWidget* InfoPanel;

	UPROPERTY(meta=(BindWidget))
	UWidget* InputPanel;

	// ── 공통 텍스트 ───────────────────────────────
	UPROPERTY(meta=(BindWidget))
	UTextBlock* TitleText;

	// ── 컨테이너 정보 (점유 슬롯) ─────────────────
	UPROPERTY(meta=(BindWidget))
	UTextBlock* InfoIdText;

	UPROPERTY(meta=(BindWidget))
	UTextBlock* InfoNameText;

	UPROPERTY(meta=(BindWidget))
	UTextBlock* InfoWeightText;

	UPROPERTY(meta=(BindWidget))
	UTextBlock* InfoDateText;

	UPROPERTY(meta=(BindWidget))
	UTextBlock* InfoSizeText;

	// ── 입고 입력 필드 (빈 슬롯) ──────────────────
	UPROPERTY(meta=(BindWidget))
	UEditableTextBox* InputId;

	UPROPERTY(meta=(BindWidget))
	UEditableTextBox* InputName;

	UPROPERTY(meta=(BindWidget))
	UEditableTextBox* InputWeight;

	UPROPERTY(meta=(BindWidget))
	UEditableTextBox* InputWidth;

	UPROPERTY(meta=(BindWidget))
	UEditableTextBox* InputDepth;

	UPROPERTY(meta=(BindWidget))
	UEditableTextBox* InputHeight;

	// ── 버튼 ──────────────────────────────────────
	UPROPERTY(meta=(BindWidget))
	UButton* BtnIncoming;

	UPROPERTY(meta=(BindWidget))
	UButton* BtnMove;

	UPROPERTY(meta=(BindWidget))
	UButton* BtnOutgoing;

	UPROPERTY(meta=(BindWidget))
	UButton* BtnClose;

private:
	UPROPERTY()
	UPalletSlot* CurrentSlot = nullptr;

	UPROPERTY()
	UPalletSlot* MoveSourceSlot = nullptr;

	UPROPERTY()
	UWarehouseHttpManager* HttpManager = nullptr;

	void OpenPopup(UPalletSlot* InSlot);
	void ClosePopup();

	UFUNCTION()
	void OnIncoming();

	UFUNCTION()
	void OnMove();

	UFUNCTION()
	void OnOutgoing();

	UFUNCTION()
	void OnClose();
};
