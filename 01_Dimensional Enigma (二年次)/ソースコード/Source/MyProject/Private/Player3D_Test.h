// Player3D_Test.h
// 23cu0228 hanawa
// 23cu0221 tsuchiya

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Pawn.h"
#include "GameFramework/PlayerController.h"
#include "Components/InputComponent.h"
#include "Engine/World.h"
#include "GameFramework/Character.h"
#include "InputMappingContext.h"
#include "InputAction.h"
#include "InputActionValue.h"
#include "Player3D_Test.generated.h"

UCLASS()
class APlayer3D_Test : public APawn
{
	GENERATED_BODY()

public:
	// Sets default values for this pawn's properties
	APlayer3D_Test();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	// Called to bind functionality to input
	virtual void SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent) override;

private:
	void UpdateMove();					// �v���C���[�̈ړ�����

	void UpdatePlayerCollision();		// �v���C���[�̈ړ��\���菈��

	void UpdateOtherActorCameraInfo(class AActor* CameraActor);		// �ړ������������J���������X�V���鏈��

	//void MoveForward(float Value);		// ���̓o�C���h(�ړ��FX��)

	//void MoveRight(float Value);		// ���̓o�C���h(�ړ��FY��)

	// Input�̐ݒ�
	void SetupInput();					// Input�̏����ݒ�

	void PressedCamera(const FInputActionValue& Value); // �J�����̉�]

	void PressedAction();				// �Z�{�^���ł̃A�N�V����

	void PressedStart();				// Start�{�^��

	void PressedAxis(const FInputActionValue& Value);	// �X�e�B�b�N�ł̈ړ�

	void PressedBack();					// �~�{�^���Ŗ߂�

	void SetMovement(float angleDeg);	// ��]�p�x�̐ݒ�

	void Move(float _DeltaTime);		// ���̉�]����

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

	UPROPERTY(EditAnyWhere, Category = "Input")
	class UInputMappingContext* DefaultMappingContext;

	UPROPERTY(EditAnyWhere, Category = "Input")
	class UInputAction* MoveInput;		// �ړ�

	UPROPERTY(EditAnyWhere, Category = "Input")
	class UInputAction* StartInput;		// Start�{�^��

	UPROPERTY(EditAnyWhere, Category = "Input")
	class UInputAction* CameraInput;	// �J����

	UPROPERTY(EditAnyWhere, Category = "Input")
	class UInputAction* ActionInput;	// �Z�{�^��

	UPROPERTY(EditAnyWhere, Category = "Input")
	class UInputAction* BackInput;		// �~�{�^��

};
