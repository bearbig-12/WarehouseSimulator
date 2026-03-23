// Fill out your copyright notice in the Description page of Project Settings.

#include "Player/WarehouseCharacter.h"
#include "Camera/CameraComponent.h"
#include "GameFramework/CharacterMovementComponent.h"
#include "EnhancedInputComponent.h"
#include "EnhancedInputSubsystems.h"
#include "InputActionValue.h"

AWarehouseCharacter::AWarehouseCharacter()
{
	PrimaryActorTick.bCanEverTick = false;

	// 1인칭 카메라 — 눈 위치에 부착
	FirstPersonCamera = CreateDefaultSubobject<UCameraComponent>(TEXT("FirstPersonCamera"));
	FirstPersonCamera->SetupAttachment(GetMesh(), FName("head"));
	FirstPersonCamera->bUsePawnControlRotation = true;

	// 캐릭터가 카메라 방향으로 회전
	bUseControllerRotationYaw = true;
	GetCharacterMovement()->MaxWalkSpeed = MoveSpeed;
}

void AWarehouseCharacter::BeginPlay()
{
	Super::BeginPlay();

	// Enhanced Input 매핑 컨텍스트 등록
	if (APlayerController* PC = Cast<APlayerController>(GetController()))
	{
		if (UEnhancedInputLocalPlayerSubsystem* Subsystem =
			ULocalPlayer::GetSubsystem<UEnhancedInputLocalPlayerSubsystem>(PC->GetLocalPlayer()))
		{
			if (InputMappingContext)
				Subsystem->AddMappingContext(InputMappingContext, 0);
		}
	}
}

void AWarehouseCharacter::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);

	if (UEnhancedInputComponent* EIC = Cast<UEnhancedInputComponent>(PlayerInputComponent))
	{
		if (MoveAction) EIC->BindAction(MoveAction, ETriggerEvent::Triggered, this, &AWarehouseCharacter::Move);
		if (LookAction) EIC->BindAction(LookAction, ETriggerEvent::Triggered, this, &AWarehouseCharacter::Look);
	}
}

// WASD 이동 — Unity CameraMove의 Horizontal/Vertical Input과 동일
void AWarehouseCharacter::Move(const FInputActionValue& Value)
{
	FVector2D MoveVec = Value.Get<FVector2D>();
	if (GetController())
	{
		AddMovementInput(GetActorForwardVector(), MoveVec.Y);
		AddMovementInput(GetActorRightVector(),   MoveVec.X);
	}
}

// 마우스 시점 회전 — Unity CameraMove의 MouseX/MouseY와 동일
void AWarehouseCharacter::Look(const FInputActionValue& Value)
{
	FVector2D LookVec = Value.Get<FVector2D>();
	AddControllerYawInput  (LookVec.X * LookSensitivity);
	AddControllerPitchInput(LookVec.Y * LookSensitivity);
}
