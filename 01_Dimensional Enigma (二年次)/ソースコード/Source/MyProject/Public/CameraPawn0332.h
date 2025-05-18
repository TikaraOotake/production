// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Camera/CameraComponent.h"
#include "GameFramework/SpringArmComponent.h"
#include "GameFramework/Pawn.h"
#include "SaveableObject.h"
#include "InputMappingContext.h"
#include "InputAction.h"
#include "InputActionValue.h"
#include "EnigmaGameInstance.h"
#include "StageSubSystem.h"
#include "CameraPawn0332.generated.h"

//��
//class UTimelineComponent;

UCLASS()
class MYPROJECT_API ACameraPawn0332 : public APawn, public ISaveableObject
{
	GENERATED_BODY()

public:
	//�R���X�g���N�^
	ACameraPawn0332();

protected:
	//�J�n�t���[������
	virtual void BeginPlay() override;

	UPROPERTY(EditAnyWhere, Category = "Input")
	class UInputMappingContext* DefaultMappingContext;

	UPROPERTY(EditAnyWhere, Category = "Input")
	class UInputAction* CameraInput;

public:
	//���t���[������
	virtual void Tick(float DeltaTime) override;

	// Called to bind functionality to input
	virtual void SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent) override;
public:
	//�Ǘ����Ă���J�������擾
	UCameraComponent* GetCamera() const;

	//�Ǘ����Ă���X�v�����O�A�[�����擾
	USpringArmComponent* GetSpringArm() const;

	//�Ǘ�����J������ݒ�
	void SetCamera(UCameraComponent* _pCamera);

	//�Ǘ�����X�v�����O�A�[����ݒ�
	void SetSpringArm(USpringArmComponent* _pSpringArm);

	//�J�����̏����ݒ�
	void InitCamera();

	//�J�����̍X�V
	void UpdateCamera();

	//�X�v�����O�A�[���̒�����ݒ�
	void SetArmLength(float _length);

	//�J�����̊p�x��ݒ�
	void SetCameraAngle(float _pitch, float _yaw, float _roll);

	//�J�����̊p�x�ɉ��Z
	void AddCameraAngle(float _pitch, float _yaw, float _roll);

	// Input�̐ݒ�
	void SetupInput();

	void PressedCamera(const FInputActionValue& Value);

	void SetCameraInput(FVector2D _Input);

	void SetCameraRotateInputX();

	void SetIsPlayerMoving(bool _IsPlayerMoving);

	void SetCameraAngle(float _Roll);//��|�ǉ�
	void SetTargetCameraRoll();//��|�ǉ�
	void SetCameraMode(bool mode);
	FRotator GetFreeCameraRot();
	void SetFreeCameraRot(FRotator _Rot);

private:
	//���\�b�h

	//���͗p

public:
	//String�^�̃Z�[�u�f�[�^���擾
	virtual FString GetSaveDataToString();
	//String�^�̃Z�[�u�f�[�^��ݒ�
	virtual void SetSaveDataToString(const FString _saveData);

private:
	//�v���p�e�B
	UPROPERTY(EditAnywhere)
	float SpringArmLength;					//�X�v�����O�A�[���̒���

	UPROPERTY(EditAnywhere)
	UCameraComponent* m_pCamera;		//�J����
	UPROPERTY(EditAnywhere)
	USpringArmComponent* m_pSpringArm;  //�X�v�����O�A�[��

private:

	//�J�����̉�]����X�����̓���
	float m_inputRotationDireX;

	//�J��������]���Ă������
	int m_rotationDireX;

	//�J�����ړ���
	bool m_cameraMoving;

	//�J������]�̖ړI�n
	FRotator m_targetAngle;

	//�C�[�W���O���x
	float m_lerpRatio;

	FVector2D m_CameraInput;

	bool m_IsPlayerMoving;

	float DefaultCameraPitch;//��|�ǉ�
	float CameraAngleUpValue = 30.0f;//��|�ǉ��@�J�������グ���
	float TargetCameraRoll = 0.0f;//��|�ǉ�

	FRotator m_FreeCameraRot;
	FRotator m_FreeCameraRot_temp;
	FRotator m_BaseCameraRot;
	
	bool m_CameraMode = true;//false�������e true���s���e

	void ZoomCamera(float TargetFOV, float InterpSpeed);
	//---------------------------------������
	/*void ToggleProjection();
	void UpdateProjection(float Alpha);

	//PrimaryActorTick.bCanEverTick = true;

	// Timeline�̍쐬
	UTimelineComponent* TimelineComponent = CreateDefaultSubobject<UTimelineComponent>(TEXT("TimelineComponent"));

	float TransitionDuration = 2.0f; // �ω��ɂ����鎞�ԁi�b�j
	float TargetOrthoWidth = 1024.0f; // ���s���e���̖ڕWOrthoWidth
	float InitialOrthoWidth = 512.0f; // �����l�i�������e���j
	*/
	
	//---------------------------------

private:

	UWorld* world;
	UEnigmaGameInstance* gameinstance;
	UGameManager* gamemanagaer;
	UStageSubSystem* stage;

};
