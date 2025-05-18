#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Pawn.h"
#include "Camera/CameraComponent.h"
#include "camera2D.generated.h"

UCLASS()
class Acamera2D
	: public APawn
{
	GENERATED_BODY()

public:
	//�R���X�g���N�^
	Acamera2D();
	//�R���X�g���N�^
	~Acamera2D();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	// Called to bind functionality to input
	virtual void SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent) override;

	//�J�����֌W
	//
	//�Ǘ����Ă���J�����I�u�W�F�N�g���擾
	UCameraComponent* GetCamera() const { return m_pCamera; }

	//�Ǘ�����J�����I�u�W�F�N�g��ݒ�
	void SetCamera(UCameraComponent* _pCamera) { m_pCamera = _pCamera; }

	//�J�����̍��W��ݒ�
	void SetCameraPos(float _x, float _y, float _z);

	//�J�����̊p�x��ݒ�
	void SetCameraRot(float _pitch, float _yaw, float _roll);

	void SetCameraDefaultCondition();

	//�J�����̍��W���擾
	FVector3d GetCameraPos();

	//�J�����̊p�x���擾
	FRotator GetCameraRot();

	//�v���C���[�֌W
	void UpdateCamera();

	void UpdatePawn();

private:
	//�����o�ϐ�
	UCameraComponent* m_pCamera;//�J����

	//���̓o�C���h
	void Cam_RotatePitch(float _axisValue);
	void Cam_RotateYaw(float _axisValue);

	void Pawn_MoveX(float _axisValue);
	void Pawn_MoveZ(float _axisValue);
	//�����o�ϐ�
	//CCameraManager m_cameraMng;
	FVector2D m_cameraRotateInput;
	FVector2D m_pawnMoveInput;

	void InitializeCamera();

	float TestNum;

	float CameraRot_Pitch;
	float CameraRot_Yaw;

	float Rad;
};
