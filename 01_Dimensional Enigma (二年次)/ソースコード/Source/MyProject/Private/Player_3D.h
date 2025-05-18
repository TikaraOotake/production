// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Pawn.h"
#include "InputMappingContext.h"
#include "InputAction.h"
#include "InputActionValue.h"
#include "Player_3D.generated.h"

UCLASS()
class APlayer_3D : public APawn
{
	GENERATED_BODY()

public:
	// Sets default values for this pawn's properties
	APlayer_3D();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

	UPROPERTY(EditAnyWhere, Category = "Input")
	class UInputMappingContext* DefaultMappingContext;

	UPROPERTY(EditAnyWhere, Category = "Input")
	class UInputAction* MoveInput;

	UPROPERTY(EditAnyWhere, Category = "Input")
	class UInputAction* StartInput;

	UPROPERTY(EditAnyWhere, Category = "Input")
	class UInputAction* CameraInput;

	UPROPERTY(EditAnyWhere, Category = "Input")
	class UInputAction* ActionInput;

	UPROPERTY(EditAnyWhere, Category = "Input")
	class UInputAction* BackInput;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	// Called to bind functionality to input
	virtual void SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent) override;

private:
	// Input�̐ݒ�
	void SetupInput();

	void PressedCamera(const FInputActionValue& Value);

	void PressedAction();

	void PressedStart();

	void PressedAxis(const FInputActionValue& Value);

	void PressedBack();

private:
	// ��]�p�x�̐ݒ�
	void SetMovement(float angleDeg);

	void UpdateMove(float DeltaTime);

	void UpdatePlayerCollision();		// �v���C���[�̈ړ��\���菈��

	void UpdateOtherActorCameraInfo(class AActor* CameraActor);		// �ړ������������J���������X�V���鏈��

	bool m_IsMove;						// �ړ��\���ǂ���

	bool m_bWarp;						// �����ړ��\��

	bool m_bCameraMove;					// �J�����ړ�����

	bool m_IsMoving;					// �ړ����Ă��邩�ǂ���

	float ElapsedTime;					// �o�ߎ���

	FVector m_ForwardDirection;			// �ړ��̑O����

	FVector m_RightDirection;			// �ړ��̉�����

	FVector m_RayVec;					// ���C���΂�����

	FVector2D m_MoveInput;				// �X�e�B�b�N�œ��͂��ꂽ�p�x

	UPROPERTY(EditAnyWhere, Category = "Rotation")
	FVector m_MoveVector;// �ړ���

	UPROPERTY(EditAnyWhere, Category = "Move")
	float m_MoveSpeed;		// �ړ����x

	UPROPERTY(EditAnyWhere, Category = "Move")
	float m_RotationSpeed;	// ��b�ŉ�]���鑬�x

	UPROPERTY(EditAnyWhere, Category = "Rotation")
	FRotator m_rValue;		// ��]������ʁiFRotator�j

	UPROPERTY(EditAnyWhere, Category = "Rotation")
	float RotationAngle; // ��]�����(�x�w��)

	UPROPERTY(EditAnywhere, Category = "CameraActor")
	AActor* m_CameraActor;				// �ړ������������J����
};
