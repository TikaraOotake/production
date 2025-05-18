// Fill out your copyright notice in the Description page of Project Settings.

// ����ҁF�y�J�S��
// �O������Ԃł̃L���[�u���Ǘ�����


#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Pawn.h"
#include "WorldSelect.h"
#include "InputMappingContext.h"
#include "InputAction.h"
#include "InputActionValue.h"
#include "EnigmaGameInstance.h" // �ǉ�
#include "GameManager.h"        // �ǉ�
#include "StageSubSystem.h"
#include "Player3D.generated.h"

class UCameraComponent;//��

UCLASS()
class APlayer3D : public APawn
{
	GENERATED_BODY()

public:
	// Sets default values for this pawn's properties
	APlayer3D();

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
	class UInputAction* CameraInput_Y;

	UPROPERTY(EditAnyWhere, Category = "Input")
	class UInputAction* ActionInput;

	UPROPERTY(EditAnyWhere, Category = "Input")
	class UInputAction* BackInput;

	UPROPERTY(EditAnyWhere, Category = "Input")
	class UInputAction* StickRInput_X;
	UPROPERTY(EditAnyWhere, Category = "Input")
	class UInputAction* StickRInput_Y;

private:
	UPROPERTY(EditAnyWhere, Category = "SE_Path")
	FString MoveSE_Path = TEXT("/Game/Sound/HomeGrownSE_BlockMove.HomeGrownSE_BlockMove");//��|�ǉ��@SE�̃p�X��ۑ�����ϐ�

public:
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	// Called to bind functionality to input
	virtual void SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent) override;

	bool GetIsWarpable();

private:
	// Input�̐ݒ�
	void SetupInput();

	void PressedCamera(const FInputActionValue& Value);

	void PressedAction();

	void PressedStart();

	void PressedAxis(const FInputActionValue& Value);

	void PressedBack();

	void PressedStickR_X(const FInputActionValue& Value);
	void PressedStickR_Y(const FInputActionValue& Value);

	// ��]�p�x�̐ݒ�
	void SetMovement(float angleDeg);

	//-------------------------------------------------��|�ǉ�
private:
	float CalculateHeight(float Degrees);
	void UpdateGoalAction(float DeltaTime);

	void FreeCameraControl();
	//void SetupPlayerInputComponent(UInputComponent* PlayerInputComponent);
	void MoveRight(float Value);
	
	bool m_GoalFlag = false;//�S�[�����Ă��邩�̃t���O
	float m_SinkHeight = 10.0f;//�S�[�����ɒn�ʂɒ��ݍ��ލ���
	float m_SinkVel = 0.0f;//���ޑ��x
	float m_SinkAcc = 0.1f;//���މ����x
	FVector m_BasePlayerPos;//����W
	float m_AddjustRotHeight;//��]�ɍ��킹�č������グ���

	AActor* m_DestinationActor;//�ړ���̃A�N�^�[
	FVector m_DestinationPos;//�ړ���̍��W

	bool m_IsMuteSE = true;

	bool m_IsFriezeChecked = false;

	bool m_IsMoveDecision = true;

	UCameraComponent* CameraComponent;
public:
	void SetGoalFlag(bool _Flag);
	float GetFinHeight();
	bool GetIsMoving();
	FVector GetBasePlayerPos();
	//-------------------------------------------------

private:
	void UpdateMove(float DeltaTime);

	void UpdatePlayerCollision();		// �v���C���[�̈ړ��\���菈��

	void UpdateOtherActorCameraInfo(class AActor* CameraActor);		// �ړ������������J���������X�V���鏈��

	UStaticMeshComponent* MeshComponent;

	AWorldSelect* m_WorldSelect;

	bool m_IsMoving;		// �ړ����Ă��邩�ǂ���

	bool m_IsMove;

	bool m_bWarp;

	bool m_bDownWarp;

	bool m_bCameraMove;					// �J�����ړ�����

	FVector m_ForwardDirection;			// �ړ��̑O����

	FVector m_RightDirection;			// �ړ��̉�����

	FVector m_TargetLocation;

	FVector m_DownWarpLocation;

	FVector m_StartLocation;

	float m_Alpha;			// ��Ԃ̐i�s�x

	float ElapsedTime;		// �o�ߎ���

	FVector m_RayVec;

	UPROPERTY(VisibleAnywhere, Category = "Move")
	float m_MoveSpeed;		// �ړ����x

	UPROPERTY(VisibleAnyWhere, Category = "Move")
	FVector m_MoveVector;// �ړ���

	UPROPERTY(VisibleAnyWhere, Category = "Rotation")
	FRotator m_rValue;		// ��]������ʁiFRotator�j

	UPROPERTY(VisibleAnyWhere, Category = "Rotation")
	float RotationAngle; // ��]�����(�x�w��)

	UPROPERTY(VisibleAnyWhere, Category = "Rotation")
	float m_RotationSpeed;	// ��b�ŉ�]���鑬�x

	UPROPERTY(EditAnywhere, Category = "CameraActor")
	AActor* m_CameraActor;				// �ړ������������J����

	UPROPERTY(EditAnywhere, Category = "CameraActor")
	AActor* m_MainCamera;

	float m_CurrentTime;// �o�߂�������

	FVector2D m_MoveInput; // ���͂��ꂽ�p�x

	private:

		UWorld* world;
		UEnigmaGameInstance* gameinstance;
		UStageSubSystem* gamemanager;
};