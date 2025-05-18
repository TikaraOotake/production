// Fill out your copyright notice in the Description page of Project Settings.


#include "Player_3D.h"
#include "MappingCamera3D.h"
#include "WarpTile.h"
#include "DrawDebugHelpers.h"
#include "Camera/CameraComponent.h"
#include "Kismet/KismetSystemLibrary.h" // �ǉ�
#include "Kismet/GameplayStatics.h" // �ǉ�
#include "Components/InputComponent.h" // �ǉ�
#include "EnhancedInputComponent.h" // �ǉ�
#include "EnhancedInputSubsystems.h" // �ǉ�

// Sets default values
APlayer_3D::APlayer_3D()
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
void APlayer_3D::BeginPlay()
{
	Super::BeginPlay();
	
	// Tag �̐ݒ�
	Tags.Add(FName(TEXT("Player")));
}

// Called every frame
void APlayer_3D::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	UpdateOtherActorCameraInfo(m_CameraActor);

	// �v���C���[�̍X�V����
	UpdateMove(DeltaTime);
}

// Called to bind functionality to input
void APlayer_3D::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);

}

// �v���C���[�̐ڐG����
void APlayer_3D::UpdatePlayerCollision()
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
void APlayer_3D::UpdateOtherActorCameraInfo(class AActor* CameraActor)
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

// m_Moving �� true �̎��̈ړ�����
void APlayer_3D::UpdateMove(float DeltaTime) {

	// �ړ����Ă��Ȃ��Ȃ���s���Ȃ�
	if (m_IsMoving == false) { return; }


	AddActorWorldRotation(m_rValue * DeltaTime, false, 0, ETeleportType::TeleportPhysics);

	AddActorWorldOffset(m_MoveVector * DeltaTime);

	ElapsedTime += DeltaTime;


	// �o�ߎ��Ԃ��w�肵�����Ԃ𒴂�����
	if (ElapsedTime >= m_RotationSpeed) {

		//SetActorRotation(targetRotation, ETeleportType::None);
		//UE_LOG(LogTemp, Warning, TEXT("Moved %s"), *targetRotation.ToString());
		m_IsMoving = false;
		ElapsedTime = 0.0f;
	}

}

// �����̐ݒ�
void APlayer_3D::SetMovement(float angleDeg) {

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

// ���������Enhanced Input�̊֐�
//--------------------------------------------------------------------------

// Input�̏����ݒ�
void APlayer_3D::SetupInput()
{
	// PlayerController���擾
	APlayerController* controller = UGameplayStatics::GetPlayerController(GetWorld(), 0);

	// ���͂�L���ɂ���
	EnableInput(controller);

	if (InputComponent)
	{
		if (UEnhancedInputComponent* EnhancedInputComponent = CastChecked<UEnhancedInputComponent>(InputComponent)) {

			// �J�����̃{�^���������ꂽ�Ƃ��Ɏ��s
			EnhancedInputComponent->BindAction(CameraInput, ETriggerEvent::Triggered, this, &APlayer_3D::PressedCamera);

			// �ړ��̃{�^���������ꂽ�Ƃ��Ɏ��s
			EnhancedInputComponent->BindAction(MoveInput, ETriggerEvent::Triggered, this, &APlayer_3D::PressedAxis);
			// �X�^�[�g�{�^��
			EnhancedInputComponent->BindAction(StartInput, ETriggerEvent::Triggered, this, &APlayer_3D::PressedStart);
			// A�{�^��
			EnhancedInputComponent->BindAction(ActionInput, ETriggerEvent::Triggered, this, &APlayer_3D::PressedAction);
			// B�{�^��
			EnhancedInputComponent->BindAction(BackInput, ETriggerEvent::Triggered, this, &APlayer_3D::PressedBack);

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
void APlayer_3D::PressedCamera(const FInputActionValue& Value) {

	// UKismetSystemLibrary::PrintString(this, TEXT("Camera Pressed"), true, true, FColor::Cyan, 10.0f, TEXT("None"));

	FVector2D v = Value.Get<FVector2D>();


	UKismetSystemLibrary::PrintString(this, FString::Printf(TEXT("Camera: X:%f Y:%f"), v.X, v.Y), true, true, FColor::Cyan, 10.0f, TEXT("None"));



}

// Start�{�^��
void APlayer_3D::PressedStart() {

	UKismetSystemLibrary::PrintString(this, TEXT("Start Pressed"), true, true, FColor::Cyan, 10.0f, TEXT("None"));

}

// ���X�e�B�b�N�ňړ�
void APlayer_3D::PressedAxis(const FInputActionValue& Value) {

	// �ړ����Ă���Ȃ���s���Ȃ�
	if (m_IsMoving == true) { return; }

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
void APlayer_3D::PressedAction()
{

	UKismetSystemLibrary::PrintString(this, TEXT("Action Pressed"), true, true, FColor::Cyan, 10.0f, TEXT("None"));

}

// Backspace�L�[�AB�{�^���i�C�O�ł�X�j�Ŗ߂�
void APlayer_3D::PressedBack()
{

	UKismetSystemLibrary::PrintString(this, TEXT("Back Pressed"), true, true, FColor::Cyan, 10.0f, TEXT("None"));

}
