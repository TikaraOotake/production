// Player3D_Test.cpp
// 23cu0228 hanawa


#include "Player3D_Test.h"
#include "GameFramework/Character.h"
#include "Camera/CameraComponent.h"
#include "GameFramework/SpringArmComponent.h"
#include "MappingCamera3D.h"
#include "WarpTile.h"
#include "DrawDebugHelpers.h"

#include "Kismet/KismetSystemLibrary.h" // �ǉ�
#include "Kismet/GameplayStatics.h" // �ǉ�
#include "Components/InputComponent.h" // �ǉ�
#include "EnhancedInputComponent.h" // �ǉ�
#include "EnhancedInputSubsystems.h" // �ǉ�


// Sets default values
APlayer3D_Test::APlayer3D_Test()
	: m_IsMove(false)
	, m_bWarp(true)
	, m_bCameraMove(false)
	, m_ForwardDirection(FVector::ZeroVector)
	, m_RightDirection(FVector::ZeroVector)
	, m_RayVec(FVector::ZeroVector)
	, m_MoveInput(FVector2D::ZeroVector)
	, m_MoveVector(FVector::ZeroVector)
	, m_MoveSpeed(0.f)
	, m_RotationSpeed(0.f)
	, m_rValue(FRotator::ZeroRotator)
	, RotationAngle(0.f)
	, m_CameraActor(nullptr)
{
 	// Set this pawn to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	// �v���C���[0�Ƃ��Đݒ�
	AutoPossessPlayer = EAutoReceiveInput::Player0;

}

// Called when the game starts or when spawned
void APlayer3D_Test::BeginPlay()
{
	Super::BeginPlay();

	// Tag �̐ݒ�
	Tags.Add(FName(TEXT("Player")));
}

// Called every frame
void APlayer3D_Test::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	UpdateOtherActorCameraInfo(m_CameraActor);

	// �v���C���[�̍X�V����
	UpdateMove();
	//Move(DeltaTime);
}

// Called to bind functionality to input
void APlayer3D_Test::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);
}

// �v���C���[�̐ڐG����
void APlayer3D_Test::UpdatePlayerCollision()
{
	// ���C�n�_
	FVector Start = GetActorLocation();
	// ���C�I�_
	FVector End = GetActorLocation() + m_RayVec;

	// �f�o�b�O���C���`��
	DrawDebugLine(GetWorld(), Start, End, FColor::Green, false, 1.0f);

	// ���M�����C�L���X�g�̔���Ɋ܂߂Ȃ�
	FCollisionQueryParams CollisionParams;
	CollisionParams.AddIgnoredActor(this);

	FHitResult OutHit;

	// ���C�L���X�g
	bool isHit = GetWorld()->LineTraceSingleByObjectType(OutHit, Start, End, FCollisionObjectQueryParams::AllObjects, CollisionParams);
	if (isHit)
	{
		m_IsMove = true;
		m_bWarp = true;

		AActor* HitActor = OutHit.GetActor();

		// �q�b�g�����I�u�W�F�N�g��"WarpTile"�Ȃ�w�肵�����W�Ɉړ�
		if (HitActor->ActorHasTag(FName("WarpTile")))
		{
			// ���[�v�^�C���̃��[�v�����ƃ��[�v�s����bool�l�̎擾
			AWarpTile* pWarpTile = Cast<AWarpTile>(HitActor);

			pWarpTile->WarpPlayer(this);
			m_bWarp = pWarpTile->GetIsWarp();

			if (!m_bWarp) { UE_LOG(LogTemp, Warning, TEXT("Not Hit WarpTile")); }
		}

		// UE_LOG(LogTemp, Log, TEXT("Hit"));
	}
	else
	{
		m_IsMove = false;
		// UE_LOG(LogTemp, Log, TEXT("No Hit"));
	}
}

// �ړ��p�J�����̕������擾(�X�V����)
void APlayer3D_Test::UpdateOtherActorCameraInfo(class AActor* CameraActor)
{
	// �ړ��p�J�������Z�b�g����Ă����珈�����s��
	if (CameraActor)
	{
		// �J�����̈ړ����̃t���O
		m_bCameraMove = Cast<AMappingCamera3D>(CameraActor)->GetCameraMoving();

		// �ړ��p�J�����̃R���|�[�l���g�̏����擾����
		UCameraComponent* CameraComp = CameraActor->FindComponentByClass<UCameraComponent>();

		if (CameraComp)
		{
			m_ForwardDirection = CameraComp->GetForwardVector();		// �J�����̑O�x�N�^�[
			m_RightDirection = CameraComp->GetRightVector();			// �J�����̍��x�N�^�[
		}
	}
}

// �v���C���[�̍X�V����
void APlayer3D_Test::UpdateMove()
{
	if (!m_bCameraMove)
	{
		// "W"�L�[����x�����ꂽ��X���v���X�����ɐi�ޏ���
		if (GetWorld()->GetFirstPlayerController()->WasInputKeyJustPressed(EKeys::W))
		{
			// ���C���΂��Ĕ�����s��
			m_RayVec = FVector(m_ForwardDirection.X * 13.f, m_ForwardDirection.Y * 13.f, -10.f);
			UpdatePlayerCollision();

			// �ړ��\�����[�v�\�ȍ� X���v���X�����ɐi��
			if (m_IsMove)
			{
				m_IsMoving = true;
				if (!m_bWarp) { return; }

				AddActorLocalOffset(m_ForwardDirection * 10.f);
			}
		}
		// "S"�L�[����x�����ꂽ��X���}�C�i�X�����ɐi�ޏ���
		if (GetWorld()->GetFirstPlayerController()->WasInputKeyJustPressed(EKeys::S))
		{
			// ���C���΂��Ĕ�����s��
			m_RayVec = FVector(m_ForwardDirection.X * -13.f, m_ForwardDirection.Y * -13.f, -10.f);
			UpdatePlayerCollision();

			// �ړ��\�����[�v�\�ȍ� X���}�C�i�X�����ɐi��
			if (m_IsMove)
			{
				if (!m_bWarp) { return; }

				AddActorLocalOffset(m_ForwardDirection * -10.f);
			}
		}
		// "D"�L�[����x�����ꂽ��Y���v���X�����ɐi�ޏ���
		if (GetWorld()->GetFirstPlayerController()->WasInputKeyJustPressed(EKeys::D))
		{
			// ���C���΂��Ĕ�����s��
			m_RayVec = FVector(m_RightDirection.X * 13.f, m_RightDirection.Y * 13.f, -10.f);
			UpdatePlayerCollision();

			// �ړ��\�����[�v�\�ȍ� Y���v���X�����ɐi��
			if (m_IsMove)
			{
				if (!m_bWarp) { return; }

				AddActorLocalOffset(m_RightDirection * 10);
			}
		}
		// "A"�L�[����x�����ꂽ��Y���}�C�i�X�����ɐi�ޏ���
		if (GetWorld()->GetFirstPlayerController()->WasInputKeyJustPressed(EKeys::A))
		{
			// ���C���΂��Ĕ�����s��
			m_RayVec = FVector(m_RightDirection.X * -13.f, m_RightDirection.Y * -13.f, -10.f);
			UpdatePlayerCollision();

			// �ړ��\�����[�v�\�ȍ� Y���}�C�i�X�����ɐi��
			if (m_IsMove)
			{
				if (!m_bWarp) { return; }

				AddActorLocalOffset(m_RightDirection * -10);
			}
		}
	}
}


// �����̐ݒ�
void APlayer3D_Test::SetMovement(float angleDeg) {

	// �E��
	if (angleDeg >= 0 && angleDeg < 90) {

		m_rValue = (FRotator(-RotationAngle, .0f, .0f));
		m_MoveVector = FVector(m_MoveSpeed, 0.0f, 0.0f);

		return;
	}

	// �E��
	if (angleDeg >= 90 && angleDeg < 180) {

		m_rValue = (FRotator(.0f, .0f, RotationAngle));
		m_MoveVector = FVector(0.0f, m_MoveSpeed, 0.0f);

	}

	// ����
	if (angleDeg >= 180 && angleDeg < 270) {

		m_rValue = (FRotator(RotationAngle, .0f, .0f));
		m_MoveVector = FVector(-m_MoveSpeed, 0.0f, 0.0f);
	}

	// ����
	if (angleDeg >= 270 && angleDeg <= 360) {

		m_rValue = (FRotator(.0f, .0f, -RotationAngle));
		m_MoveVector = FVector(.0f, -m_MoveSpeed, .0f);
	}

}

void APlayer3D_Test::Move(float _DeltaTime) 
{
	// �ړ����Ă��Ȃ��Ȃ���s���Ȃ�
	if (m_IsMoving == false) { return; }
	UE_LOG(LogTemp, Log, TEXT("Move"));

	AddActorWorldRotation(m_rValue * _DeltaTime, false, 0, ETeleportType::TeleportPhysics);

	AddActorWorldOffset(m_MoveVector * _DeltaTime);

	ElapsedTime += _DeltaTime;


	// �o�ߎ��Ԃ��w�肵�����Ԃ𒴂�����
	if (ElapsedTime >= m_RotationSpeed) {

		//SetActorRotation(targetRotation, ETeleportType::None);
		//UE_LOG(LogTemp, Warning, TEXT("Moved %s"), *targetRotation.ToString());
		m_IsMoving = false;
		ElapsedTime = 0.0f;
	}
}

// ���������Enhanced Input�̊֐�
//--------------------------------------------------------------------------


void APlayer3D_Test::SetupInput()
{
	// PlayerController���擾����
	APlayerController* controller = UGameplayStatics::GetPlayerController(GetWorld(), 0);

	// ���͂�L���ɂ���
	EnableInput(controller);

	if (InputComponent)
	{
		if (UEnhancedInputComponent* EnhancedInputComponent = CastChecked<UEnhancedInputComponent>(InputComponent)) {

			// �J�����̃{�^���������ꂽ�Ƃ��Ɏ��s
			EnhancedInputComponent->BindAction(CameraInput, ETriggerEvent::Triggered, this, &APlayer3D_Test::PressedCamera);

			// �ړ��̃{�^���������ꂽ�Ƃ��Ɏ��s
			EnhancedInputComponent->BindAction(MoveInput, ETriggerEvent::Triggered, this, &APlayer3D_Test::PressedAxis);
			// �X�^�[�g�{�^��
			EnhancedInputComponent->BindAction(StartInput, ETriggerEvent::Triggered, this, &APlayer3D_Test::PressedStart);
			// A�{�^��
			EnhancedInputComponent->BindAction(ActionInput, ETriggerEvent::Triggered, this, &APlayer3D_Test::PressedAction);
			// B�{�^��
			EnhancedInputComponent->BindAction(BackInput, ETriggerEvent::Triggered, this, &APlayer3D_Test::PressedBack);

		}

		// �}�b�s���O�R���e�L�X�g��K�p
		if (APlayerController* PlayerController = Cast<APlayerController>(controller))
		{
			if (UEnhancedInputLocalPlayerSubsystem* subsystem = ULocalPlayer::GetSubsystem<UEnhancedInputLocalPlayerSubsystem>(PlayerController->GetLocalPlayer())) {

				subsystem->AddMappingContext(DefaultMappingContext, 0);
			}
		}

	}

}

// L�AR�{�^���ŃJ�����̉�]
void APlayer3D_Test::PressedCamera(const FInputActionValue& Value) {

	// UKismetSystemLibrary::PrintString(this, TEXT("Camera Pressed"), true, true, FColor::Cyan, 10.0f, TEXT("None"));



	FVector2D v = Value.Get<FVector2D>();

	UKismetSystemLibrary::PrintString(this, FString::Printf(TEXT("Camera: X:%f Y:%f"), v.X, v.Y), true, true, FColor::Cyan, 10.0f, TEXT("None"));



}

// Start�{�^��
void APlayer3D_Test::PressedStart() {

	UKismetSystemLibrary::PrintString(this, TEXT("Start Pressed"), true, true, FColor::Cyan, 10.0f, TEXT("None"));

}

// ���X�e�B�b�N�ňړ�
void APlayer3D_Test::PressedAxis(const FInputActionValue& Value) {

	// �ړ����Ă���Ȃ���s���Ȃ�
	if (m_IsMoving) { return; }
	UE_LOG(LogTemp, Warning, TEXT("Pressed"))

	m_MoveInput = Value.Get<FVector2D>();


	// ���͂��ꂽ������360���\�L(Radian)�ɕϊ�
	float angle = atan2(m_MoveInput.X, m_MoveInput.Y);
	float angleDeg = FMath::RadiansToDegrees(angle);

	// �}�C�i�X�̏ꍇ�������ɂ���
	if (angleDeg < 0) {
		angleDeg += 360;
	}

	SetMovement(angleDeg);
	m_IsMoving = true;


	// UKismetSystemLibrary::PrintString(this, FString::Printf(TEXT("Moved: X:%f Y:%f"), m_MoveInput.X, m_MoveInput.Y), true, true, FColor::Cyan, 10.0f, TEXT("None"));
	UKismetSystemLibrary::PrintString(this, FString::Printf(TEXT("Degree: %f"), angleDeg), true, true, FColor::Cyan, 10.0f, TEXT("None"));

}

// A�{�^��(�C�O�ł́Z)�ŃA�N�V�������N����
void APlayer3D_Test::PressedAction()
{

	UKismetSystemLibrary::PrintString(this, TEXT("Action Pressed"), true, true, FColor::Cyan, 10.0f, TEXT("None"));

}

// Backspace�L�[�AB�{�^���i�C�O�ł�X�j�Ŗ߂�
void APlayer3D_Test::PressedBack()
{

	UKismetSystemLibrary::PrintString(this, TEXT("Back Pressed"), true, true, FColor::Cyan, 10.0f, TEXT("None"));

}




//void APlayer3D_Test::MoveForward(float Value)
//{
//}
//
//void APlayer3D_Test::MoveRight(float Value)
//{
//}