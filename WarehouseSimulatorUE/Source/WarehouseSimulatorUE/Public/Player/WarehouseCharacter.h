// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Character.h"
#include "WarehouseCharacter.generated.h"

class UCameraComponent;
class UInputMappingContext;
class UInputAction;
struct FInputActionValue;

/**
 * Unity CameraMove.cs에 대응
 * WASD 이동 + 마우스 시점 회전 (1인칭)
 * Enhanced Input 사용
 */
UCLASS()
class WAREHOUSESIMULATORUE_API AWarehouseCharacter : public ACharacter
{
	GENERATED_BODY()

public:
	AWarehouseCharacter();

	// 1인칭 카메라
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category="Warehouse|Camera")
	UCameraComponent* FirstPersonCamera;

	// Enhanced Input — Blueprint에서 연결
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Warehouse|Input")
	UInputMappingContext* InputMappingContext;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Warehouse|Input")
	UInputAction* MoveAction;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Warehouse|Input")
	UInputAction* LookAction;

	// 이동 속도
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Warehouse|Movement")
	float MoveSpeed = 600.f;

	// 마우스 감도
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Warehouse|Movement")
	float LookSensitivity = 0.5f;

protected:
	virtual void BeginPlay() override;
	virtual void SetupPlayerInputComponent(UInputComponent* PlayerInputComponent) override;

private:
	void Move(const FInputActionValue& Value);
	void Look(const FInputActionValue& Value);
};
