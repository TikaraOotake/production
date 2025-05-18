// Fill out your copyright notice in the Description page of Project Settings.

#include "CameraPawn0332.h"
#include "Math/UnrealMathUtility.h"
#include "Kismet/KismetSystemLibrary.h" // �ǉ�
#include "Kismet/GameplayStatics.h" // �ǉ�
#include "Components/InputComponent.h" // �ǉ�
#include "EnhancedInputComponent.h" // �ǉ�
#include "EnhancedInputSubsystems.h" // �ǉ�
#include "SoundManagerSubsystem.h"
#include "StageSubSystem.h"

// Sets default values
ACameraPawn0332::ACameraPawn0332()
{
	// Set this pawn to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	InitCamera();

	// �����ݒ�
	bool bIsOrthographic = false;
	GetCamera()->SetProjectionMode(ECameraProjectionMode::Perspective);
}

// Called when the game starts or when spawned
void ACameraPawn0332::BeginPlay()
{
	Super::BeginPlay();

	//�J�����̏����p�x�ݒ�
	SetCameraAngle(-35.26f, 315.0f, 0.0f);
	//�ʒu��������
	SetActorLocation(FVector(0.0f, 0.0f, 0.0f));

	DefaultCameraPitch = GetActorRotation().Pitch;//�������@��|�ǉ�

	m_lerpRatio = 0.08f;
	//�X�v�����O�A�[���̒����ݒ�
	if (SpringArmLength <= 0.0f)
	{
		SpringArmLength = 1000.0f;
	}
	m_pSpringArm->TargetArmLength = SpringArmLength;

	// GameManager���擾
	world = GetWorld();
	if (world) {
		gameinstance = Cast<UEnigmaGameInstance>(world->GetGameInstance());
		if (gameinstance) {
			gamemanagaer = gameinstance->GetSubsystem<UGameManager>();
		}
		stage = world->GetSubsystem<UStageSubSystem>();
	}

	GetCamera()-> SetProjectionMode(ECameraProjectionMode::Orthographic);
	/*//������
	// Timeline���Z�b�g�A�b�v
	if (TimelineComponent)
	{
		FOnTimelineFloat TimelineCallback;
		TimelineCallback.BindUFunction(this, FName("UpdateProjection"));
		TimelineComponent->AddInterpFloat(nullptr, TimelineCallback);
	}

	// ���͂��o�C���h�iP�L�[�Ő؂�ւ��j
	APlayerController* PlayerController = GetWorld()->GetFirstPlayerController();
	if (PlayerController)
	{
		PlayerController->InputComponent->BindAction("ToggleProjection", IE_Pressed, this, &ACameraPawn0332::ToggleProjection);
	}
	*/
	
}

// Called every frame
void ACameraPawn0332::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	SetCameraRotateInputX();

	UpdateCamera();

	//������
	// Timeline�̍X�V
	/*if (TimelineComponent)
	{
		TimelineComponent->TickComponent(DeltaTime, ELevelTick::LEVELTICK_All, nullptr);
	}
	*/
	
}

// Called to bind functionality to input
void ACameraPawn0332::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);

}

//�Ǘ����Ă���J�������擾
UCameraComponent* ACameraPawn0332::GetCamera() const
{
	return m_pCamera;
}

//�Ǘ����Ă���X�v�����O�A�[�����擾
USpringArmComponent* ACameraPawn0332::GetSpringArm() const
{
	return m_pSpringArm;
}

//�Ǘ�����J������ݒ�
void ACameraPawn0332::SetCamera(UCameraComponent* _pCamera)
{
	m_pCamera = _pCamera;
}

//�Ǘ�����X�v�����O�A�[����ݒ�
void ACameraPawn0332::SetSpringArm(USpringArmComponent* _pSpringArm)
{
	m_pSpringArm = _pSpringArm;
}

void ACameraPawn0332::InitCamera()
{
	if (!m_pSpringArm)
	{
		//�X�v�����O�A�[������
		m_pSpringArm = CreateDefaultSubobject<USpringArmComponent>("m_pSpringArm");
		m_pSpringArm->SetupAttachment(RootComponent);

		//�X�v�����O�A�[���̒����ݒ�
		m_pSpringArm->TargetArmLength = SpringArmLength;
		m_pSpringArm->bDoCollisionTest = false;
	}
	
	if (!m_pCamera)
	{
		//�J��������
		m_pCamera = CreateDefaultSubobject<UCameraComponent>("m_pCamera");
		m_pCamera->SetupAttachment(m_pSpringArm);
		m_pCamera->SetProjectionMode(ECameraProjectionMode::Orthographic);
	}
}

void ACameraPawn0332::UpdateCamera()
{

	//�ړ����o�Ȃ��Ȃ�
	if (!m_cameraMoving)
	{
		//���͂���Ă��鎞
		if (m_inputRotationDireX != 0 && !stage->GetGoaling())
		{
			//��]��������͂���Ă�������ɂ���
			m_rotationDireX = m_inputRotationDireX;
			//�ړI�n�̐ݒ�
			m_targetAngle = GetActorRotation() + FRotator(0.0f, 90.0f * m_rotationDireX, 0.0f);
			//Lerp�Ɏg���������ُ�l�Ȃ�␳
			if (m_lerpRatio <= 0 || m_lerpRatio > 1)
				m_lerpRatio = 0.06f;

			//UE_LOG(LogTemp, Log, TEXT("Start Move Target Yaw : %f"), m_targetAngle.Yaw);

			//�ړ����ɕύX
			m_cameraMoving = true;

			if (stage)
			{
				stage->SetCameraMoving(true);
			}
			//UE_LOG(LogTemp, Log, TEXT("m_cameraMoving Is %d"), (int)m_cameraMoving);
		}
		else
		{
			//�t���[�J����

			SetActorRotation(GetActorRotation() - m_FreeCameraRot_temp);//���Z�O�̏�Ԃɖ߂�

			m_BaseCameraRot = GetActorRotation();//��{���W�Ƃ��ċL�^���Ă���

			SetActorRotation(m_BaseCameraRot + m_FreeCameraRot);

			m_FreeCameraRot_temp = m_FreeCameraRot;
		}
	}
	else
	{
		//���݂̉�]��
		FRotator r1 = GetActorRotation();
		//�ړ���̗\�肵�Ă����]��
		FRotator r2 = FMath::Lerp(r1, m_targetAngle, m_lerpRatio);
		//��]������
		SetActorRotation(r2);

		//�ړI�n�ɂƂĂ��߂��Ȃ������͈ړ��ʂ��������Ȃ肷������
		if (FMath::Abs(m_targetAngle.Yaw - r1.Yaw) <= 0.01f || FMath::Abs(r2.Yaw - r1.Yaw) < 0.01f)//��|�@�I�[�Ɉ�a���������邽�ߍX�ɔ͈͂��ׂ���
		{
			//�ړI�n�Ɉړ�
			SetActorRotation(m_targetAngle);

			//�l��������
			m_rotationDireX = 0;
			m_cameraMoving = false;
			m_CameraInput.X = 0.f;

			// �J�����̉�]�񐔂�Manager�ɃJ�E���g������
			if (stage)
			{
				stage->AddCameraMoveCount();
			}

			//UE_LOG(LogTemp, Log, TEXT("m_cameraMoving Is %d"), (int)m_cameraMoving);
		}
	}
}

//�X�v�����O�A�[���̒�����ݒ�
void ACameraPawn0332::SetArmLength(float _length)
{
	if (m_pSpringArm != nullptr)
	{
		m_pSpringArm->TargetArmLength = _length;
	}
}

//�J�����̊p�x��ݒ�
void ACameraPawn0332::SetCameraAngle(float _pitch, float _yaw, float _roll)
{
	SetActorRotation(FRotator(_pitch, _yaw, _roll));
	//UE_LOG(LogTemp, Log, TEXT("Set Camera Rotation"));
	/*FTransform t = GetActorTransform();
	FRotator r = t.Rotator();*/
	/*UE_LOG(LogTemp, Log, TEXT("Set Camera Rotation : p %d y %d r %d"), r.Pitch, r.Yaw, r.Roll);*/
}

//�J�����̊p�x�ɉ��Z
void ACameraPawn0332::AddCameraAngle(float _pitch, float _yaw, float _roll)
{
	AddActorWorldRotation(FRotator(_pitch, _yaw, _roll));
}

//���͒l����
void ACameraPawn0332::SetCameraRotateInputX()
{
	if (m_CameraInput.X == 1.f)
	{
		m_inputRotationDireX = -1;
		//UE_LOG(LogTemp, Log, TEXT("Input Right"));
	}
	else if (m_CameraInput.X == -1.f)
	{
		m_inputRotationDireX =  1;
		//UE_LOG(LogTemp, Log, TEXT("Input Left"));
	}
	else
	{
		m_inputRotationDireX = 0;
		//UE_LOG(LogTemp, Log, TEXT("Input None"));
	}
}

//String�^�̃Z�[�u�f�[�^���擾
FString ACameraPawn0332::GetSaveDataToString()
{
	return FString("");
}
//String�^�̃Z�[�u�f�[�^��ݒ�
void ACameraPawn0332::SetSaveDataToString(const FString _saveData)
{

}

void ACameraPawn0332::SetupInput()
{

}

void ACameraPawn0332::PressedCamera(const FInputActionValue& Value)
{

}

void ACameraPawn0332::SetCameraInput(FVector2D _Input)
{
	m_CameraInput = _Input;
}

void ACameraPawn0332::SetIsPlayerMoving(bool _IsPlayerMoving)
{
	m_IsPlayerMoving = _IsPlayerMoving;
}

//-------------------------------��|���ǉ������֐�
void ACameraPawn0332::SetCameraAngle(float _Roll)
{
	FRotator CameraRot = GetActorRotation();
	CameraRot.Pitch = -_Roll+ DefaultCameraPitch;
	SetActorRotation(CameraRot);
}
void ACameraPawn0332::SetTargetCameraRoll()
{

}

//������
void ACameraPawn0332::SetCameraMode(bool mode)
{
	if (mode)
	{
		GetCamera()->SetProjectionMode(ECameraProjectionMode::Perspective);
		m_CameraMode = true;

		//�X�v�����O�A�[���̒����ݒ�
		SpringArmLength = 300.0f;
		m_pSpringArm->TargetArmLength = SpringArmLength;
	}
	else
	{
		GetCamera()->SetProjectionMode(ECameraProjectionMode::Orthographic);
		m_CameraMode = false;

		//�X�v�����O�A�[���̒����ݒ�
		SpringArmLength = 1000.0f;
		m_pSpringArm->TargetArmLength = SpringArmLength;
	}


}


/*void ACameraPawn0332::ToggleProjection()
{
	if (bIsOrthographic)
	{
		// �������e�ɖ߂�
		CameraComponent->SetProjectionMode(ECameraProjectionMode::Perspective);
	}
	else
	{
		// ���s���e�ɕύX
		CameraComponent->SetProjectionMode(ECameraProjectionMode::Orthographic);
	}

	// Timeline�̊J�n
	TimelineComponent->PlayFromStart();

	// �ڕW�l�̐ݒ�
	InitialOrthoWidth = CameraComponent->GetOrthoWidth();
	TargetOrthoWidth = (bIsOrthographic) ? 1024.0f : 512.0f; // �����ƕ��s�̕���ݒ�

	bIsOrthographic = !bIsOrthographic;
}

void ACameraPawn0332::UpdateProjection(float Alpha)
{
	// ��Ԃ�OrthoWidth��ύX
	CameraComponent->SetOrthoWidth(FMath::Lerp(InitialOrthoWidth, TargetOrthoWidth, Alpha));
}

*/
void ACameraPawn0332::SetFreeCameraRot(FRotator _Rot)
{
	m_FreeCameraRot = _Rot;
}
FRotator ACameraPawn0332::GetFreeCameraRot()
{
	return m_FreeCameraRot;
}

//-------------------------------