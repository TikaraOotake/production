// MappingCamera3D
// 23cu0228 hanawa

#pragma once

#include "CoreMinimal.h"
#include "EnigmaGameInstance.h" // �ǉ�
#include "GameManager.h"        // �ǉ�
#include "GameFramework/Pawn.h"
#include "MappingCamera3D.generated.h"

UCLASS()
class AMappingCamera3D : public APawn
{
	GENERATED_BODY()

public:
	// Sets default values for this pawn's properties
	AMappingCamera3D();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	// Called to bind functionality to input
	virtual void SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent) override;

	//void SetIsPlayerMoving(bool _IsPlayerMoving);

	int GetDirectionNumber();

	bool GetCameraMoving();

	void SetCameraInput(FVector2D _Input);

	void SetIsPlayerMoving(bool _IsPlayerMoving);

private:
	void UpdateMappingCamera(float DeltaTime);

	FRotator m_CameraRotation;

	UPROPERTY(EditAnywhere, category = "Direction")
	int m_DirectionCamNumber;

	//�C�[�W���O�p
	
	//�J�����̍X�V
	void UpdateCamera();

	//���͗p
	void SetCameraRotateInputX();

	//�J�����̉�]����X�����̓���
	float m_inputRotationDireX;

	//�J��������]���Ă������
	int m_rotationDireX;

	//�J�����ړ���
	bool m_cameraMoving;

	//// �v���C���[�ړ���
	//bool m_IsPlayerMoving;

	//�J������]�̖ړI�n
	FRotator m_targetAngle;

	//�C�[�W���O���x
	float m_lerpRatio;

	FVector2D m_CameraInput;

	bool m_IsPlayerMoving;

private:

	UWorld* world;
	UEnigmaGameInstance* gameinstance;
	UStageSubSystem* gamemanager;
};
