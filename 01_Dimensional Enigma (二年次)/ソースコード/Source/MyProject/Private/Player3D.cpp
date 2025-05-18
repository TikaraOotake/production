// ����ҁF�y�J�S��
// �O������Ԃł̃L���[�u���Ǘ�����

#include "Player3D.h"
#include "Components/StaticMeshComponent.h"
#include "WarpTile.h"
#include "MappingCamera3D.h"
#include "CameraPawn0332.h"
#include "Camera/CameraComponent.h"
#include "Kismet/KismetSystemLibrary.h" // �ǉ�
#include "Kismet/GameplayStatics.h" // �ǉ�
#include "Components/InputComponent.h" // �ǉ�
#include "EnhancedInputComponent.h" // �ǉ�
#include "EnhancedInputSubsystems.h" // �ǉ�

#include "SoundManagerSubsystem.h"
#include "Math/UnrealMathUtility.h"

#include "EnigmaGameInstance.h"
#include "GameManager.h"
#include "StageSubSystem.h"

// Sets default values
APlayer3D::APlayer3D()
	: m_IsMoving(false)
	, m_MoveSpeed(20.f)
	, m_MoveVector(.0f, .0f, .0f)
	, m_rValue(FRotator::ZeroRotator)
	, RotationAngle(180.f)
	, m_RotationSpeed(2.0f)
{
	// Set this pawn to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	// �A�N�^�[��StaticMeshComponent���擾
	MeshComponent = this->FindComponentByClass<UStaticMeshComponent>();

	// �v���C���[0�Ƃ��Đݒ�
	AutoPossessPlayer = EAutoReceiveInput::Player0;

	if (StickRInput_X == nullptr)
	{
		StickRInput_X = LoadObject<UInputAction>(nullptr, TEXT("/Game/EnhancedInput/Input/IA_StickR_X.IA_StickR_X"));
	}
	if (StickRInput_Y == nullptr)
	{
		StickRInput_Y = LoadObject<UInputAction>(nullptr, TEXT("/Game/EnhancedInput/Input/IA_StickR_Y.IA_StickR_Y"));
	}

}

// Called when the game starts or when spawned
void APlayer3D::BeginPlay()
{
	Super::BeginPlay();

	SetupInput();

	// GameManager���擾
	world = GetWorld();
	if (world) {
		gameinstance = Cast<UEnigmaGameInstance>(world->GetGameInstance());
		if (gameinstance) {
			gamemanager = world->GetSubsystem<UStageSubSystem>();
		}
	}


	Tags.Add(FName("Player"));

	m_StartLocation = GetActorLocation();

	//--------��|�ǉ�
	//��{���W��������
	m_BasePlayerPos = GetActorLocation();
	//--------

	//SE���Đ�(����)�󌂂�������
	USoundManagerSubsystem* SoundManager = GetGameInstance()->GetSubsystem<USoundManagerSubsystem>();
	if (SoundManager)
	{
		// �p�X�ŉ������Đ�
		SoundManager->PlaySoundFromPath(MoveSE_Path, 0.001f);

		UE_LOG(LogTemp, Warning, TEXT("PlaySE"))
	}
}

// Called every frame
void APlayer3D::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	UpdateOtherActorCameraInfo(m_CameraActor);

	UpdateMove(DeltaTime);

	UpdateGoalAction(DeltaTime);//��|�ǉ�

	FreeCameraControl();
}

// Called to bind functionality to input
void APlayer3D::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);

	PlayerInputComponent->BindAxis("Gamepad_LeftX", this, &APlayer3D::MoveRight);
}

// �v���C���[�̐ڐG����
void APlayer3D::UpdatePlayerCollision()
{
	// ���C�n�_
	FVector Start = GetActorLocation();
	// ���C�I�_
	FVector End = GetActorLocation() + m_RayVec;

	//// �f�o�b�O���C���`��
	//DrawDebugLine(GetWorld(), Start, End, FColor::Green, false, 1.0f);

	// ���M�����C�L���X�g�̔���Ɋ܂߂Ȃ�
	FCollisionQueryParams CollisionParams;
	CollisionParams.AddIgnoredActor(this);

	FHitResult OutHit;

	// ���C�L���X�g
	bool isHit = GetWorld()->LineTraceSingleByObjectType(OutHit, Start, End, FCollisionObjectQueryParams::AllObjects, CollisionParams);
	if (isHit && !gamemanager->GetCameraMoving() && !gamemanager->GetGoaling())
	{
		m_IsMove = true;
		m_bWarp = true;

		AActor* HitActor = OutHit.GetActor();
		m_DestinationActor = HitActor;
		m_DestinationPos = m_DestinationActor->GetActorLocation();

		// �q�b�g�����I�u�W�F�N�g��"WarpTile"�Ȃ�w�肵�����W�Ɉړ�
		if (HitActor->ActorHasTag(FName("WarpTile")))
		{
			// ���[�v�^�C���̃��[�v�����ƃ��[�v�s����bool�l�̎擾
			AWarpTile* pWarpTile = Cast<AWarpTile>(HitActor);

			pWarpTile->WarpPlayer(this);
			m_bWarp = pWarpTile->GetIsWarp();
			m_bDownWarp = pWarpTile->GetIsWarped();
			m_DownWarpLocation = pWarpTile->GetDownWarpLocation();

			if (!m_bWarp) { UE_LOG(LogTemp, Warning, TEXT("Not Hit WarpTile")); }
		}
		else
		{
			m_TargetLocation = HitActor->GetActorLocation();
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
void APlayer3D::UpdateOtherActorCameraInfo(class AActor* CameraActor)
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
void APlayer3D::UpdateMove(float DeltaTime)
{
	if (!m_IsMoving) { return; }

	if (!m_IsFriezeChecked)
	{
		if (m_DestinationPos != m_DestinationActor->GetActorLocation())
		{
			gamemanager->SetPlayerMoving(false);

			SetActorLocation(m_StartLocation);

			m_IsMoving = false;
			Cast<ACameraPawn0332>(m_MainCamera)->SetIsPlayerMoving(false);
			Cast<AMappingCamera3D>(m_CameraActor)->SetIsPlayerMoving(false);
			UE_LOG(LogTemp, Warning, TEXT("false"));
			ElapsedTime = 0.0f;

			return;
		}
	}
	//���Ȃ�������true��
	m_IsFriezeChecked = true;
	

	
	if (gamemanager->GetIsSelect()) { gamemanager->SetIsSelect(false); }

	if (gamemanager)
	{
		gamemanager->SetPlayerMoving(true);
	}

	Cast<ACameraPawn0332>(m_MainCamera)->SetIsPlayerMoving(true);
	Cast<AMappingCamera3D>(m_CameraActor)->SetIsPlayerMoving(false);

	AddActorWorldRotation(m_rValue * DeltaTime * m_RotationSpeed);

	//-----------------------------------------------------��|�ǉ�
	//�p�x�ɍ��킹�ăv���C���[�̍����������グ��
	FVector PlayerPos = GetActorLocation();//�擾
	FRotator PlayerRot = GetActorRotation();

	//1F�O�̏󋵂ɖ߂�
	PlayerPos.Z -= m_AddjustRotHeight;

	//��{���W�Ƃ��ċL�^
	m_BasePlayerPos = PlayerPos;

	m_AddjustRotHeight = CalculateHeight(PlayerRot.Pitch + PlayerRot.Yaw + PlayerRot.Roll) * 10.0f;//���炷�������v�Z
	PlayerPos.Z += m_AddjustRotHeight;//�␳
	SetActorLocation(PlayerPos);//���
	//-----------------------------------------------------

	AddActorWorldOffset(m_MoveVector * DeltaTime * m_RotationSpeed);

	ElapsedTime += DeltaTime * m_RotationSpeed;

	// �o�ߎ��Ԃ��w�肵�����Ԃ𒴂�����
	if (ElapsedTime >= 0.47f)//��|���X�ύX 0.5f����0.47f
	{
		gamemanager->SetPlayerMoving(false);

		UE_LOG(LogTemp, Log, TEXT("TargetLocation %s"), *m_TargetLocation.ToString());

		SetActorRotation(m_rValue * 0.5f * m_RotationSpeed);
		//SetActorLocation(FVector(FMath::Floor(TargetLocation.X), FMath::Floor(TargetLocation.Y), FMath::Floor(TargetLocation.Z)));


		if (m_bDownWarp)
		{
			m_StartLocation = m_DownWarpLocation + FVector(m_MoveVector * 0.25f * m_RotationSpeed);

			m_bDownWarp = false;
			UE_LOG(LogTemp, Warning, TEXT("DownWarp"))
		}
		else
		{
			m_StartLocation += FVector(m_MoveVector * 0.25f * m_RotationSpeed);
			UE_LOG(LogTemp, Warning, TEXT("location %s"), *FVector(m_MoveVector * 0.25f * m_RotationSpeed).ToString());
		}

		SetActorLocation(m_StartLocation);

		if (gamemanager)
		{
			gamemanager->AddPlayerMoveCount();
		}

		//-------------------------------------��|�ǉ�
		//SE���Đ�
		USoundManagerSubsystem* SoundManager = GetGameInstance()->GetSubsystem<USoundManagerSubsystem>();
		if (SoundManager)
		{
			// �p�X�ŉ������Đ�
			SoundManager->PlaySoundFromPath(MoveSE_Path);

			UE_LOG(LogTemp, Warning, TEXT("PlaySE"))
		}


		//�Î~�`�F�b�N�����Ƃɖ߂�
		m_IsFriezeChecked = false;

		//�����̕␳�ʂ�߂��Ă���
		m_AddjustRotHeight = 0.0f;

		//��{���W�Ƃ��ċL�^
		m_BasePlayerPos = GetActorLocation();
		//-------------------------------------

		m_IsMoving = false;
		Cast<ACameraPawn0332>(m_MainCamera)->SetIsPlayerMoving(false);
		Cast<AMappingCamera3D>(m_CameraActor)->SetIsPlayerMoving(false);
		UE_LOG(LogTemp, Warning, TEXT("false"));
		ElapsedTime = 0.0f;
	}
}

// �����̐ݒ�
void APlayer3D::SetMovement(float angleDeg)
{
	if (m_MoveInput.X == 0.f)
	{
		m_rValue = (FRotator(m_ForwardDirection.X * m_MoveInput.Y * -RotationAngle, 0.f, m_ForwardDirection.Y * m_MoveInput.Y * RotationAngle));
		m_MoveVector = FVector(m_ForwardDirection.X * m_MoveInput.Y * m_MoveSpeed, m_ForwardDirection.Y * m_MoveInput.Y * m_MoveSpeed, 0.f);
	}

	if (m_MoveInput.Y == 0.f)
	{
		m_rValue = (FRotator(m_RightDirection.X * m_MoveInput.X * -RotationAngle, 0.f, m_RightDirection.Y * m_MoveInput.X * RotationAngle));
		m_MoveVector = FVector(m_RightDirection.X * m_MoveInput.X * m_MoveSpeed, m_RightDirection.Y * m_MoveInput.X * m_MoveSpeed, 0.f);
	}
}

bool APlayer3D::GetIsWarpable()
{
	return m_IsMoving;
}

// ���������Enhanced Input�̊֐�
//--------------------------------------------------------------------------

// Input�̏����ݒ�
void APlayer3D::SetupInput()
{
	// PlayerController���擾
	APlayerController* controller = UGameplayStatics::GetPlayerController(GetWorld(), 0);

	// ���͂�L���ɂ���
	EnableInput(controller);

	if (InputComponent)
	{
		if (UEnhancedInputComponent* EnhancedInputComponent = CastChecked<UEnhancedInputComponent>(InputComponent)) {

			// �J�����̃{�^���������ꂽ�Ƃ��Ɏ��s
			EnhancedInputComponent->BindAction(CameraInput, ETriggerEvent::Triggered, this, &APlayer3D::PressedCamera);

			// �ړ��̃{�^���������ꂽ�Ƃ��Ɏ��s
			EnhancedInputComponent->BindAction(MoveInput, ETriggerEvent::Triggered, this, &APlayer3D::PressedAxis);
			// �X�^�[�g�{�^��
			EnhancedInputComponent->BindAction(StartInput, ETriggerEvent::Triggered, this, &APlayer3D::PressedStart);
			// A�{�^��
			EnhancedInputComponent->BindAction(ActionInput, ETriggerEvent::Triggered, this, &APlayer3D::PressedAction);
			// B�{�^��
			EnhancedInputComponent->BindAction(BackInput, ETriggerEvent::Triggered, this, &APlayer3D::PressedBack);

			// �E�X�e�B�b�N
			EnhancedInputComponent->BindAction(StickRInput_X, ETriggerEvent::Triggered, this, &APlayer3D::PressedStickR_X);
			EnhancedInputComponent->BindAction(StickRInput_Y, ETriggerEvent::Triggered, this, &APlayer3D::PressedStickR_Y);
		}

		// �}�b�s���O�R���e�L�X�g��K�p
		if (APlayerController* PlayerController = Cast<APlayerController>(controller))
		{
			if (UEnhancedInputLocalPlayerSubsystem* subsystem = ULocalPlayer::GetSubsystem<UEnhancedInputLocalPlayerSubsystem>(PlayerController->GetLocalPlayer()))
			{
				subsystem->AddMappingContext(DefaultMappingContext, 0);
			}
		}
	}
}

// L�AR�{�^���ŃJ�����̉�]
void APlayer3D::PressedCamera(const FInputActionValue& Value)
{
	if (gamemanager->GetDimensionType() == EDimensionType::TwoD || gamemanager->GetIsDimensionChange()) { return; }

	//�J�����ƃv���C���[�ǂ���������Ă��Ȃ����Ƃ��m�F����
	if (!gamemanager->GetPlayerMoving() && !gamemanager->GetCameraMoving())
	{
		FVector2D v = Value.Get<FVector2D>();

		if (m_MainCamera)
		{
			AMappingCamera3D* pMappingCamera = Cast<AMappingCamera3D>(m_CameraActor);
			ACameraPawn0332* pMainCamera = Cast<ACameraPawn0332>(m_MainCamera);
			pMainCamera->SetCameraInput(v);
			pMainCamera->SetCameraRotateInputX();

			pMappingCamera->SetCameraInput(v);
		}
	}
	else
	{
		UE_LOG(LogTemp, Log, TEXT("Not Cam Moving"));
	}
}

// Start�{�^��
void APlayer3D::PressedStart()
{
	//UKismetSystemLibrary::PrintString(this, TEXT("Start Pressed"), true, true, FColor::Cyan, 10.0f, TEXT("None"));
}

// ���X�e�B�b�N�ňړ�
void APlayer3D::PressedAxis(const FInputActionValue& Value)
{
	// �ړ����Ă���Ƃ��A���͎����ω����Ȃ���s���Ȃ�
	if (m_IsMoving || gamemanager->GetIsDimensionChange()) { return; }

	//�X�e�B�b�N�J�������Ȃ���s���Ȃ�
	ACameraPawn0332* pMainCamera = Cast<ACameraPawn0332>(m_MainCamera);
	if (pMainCamera->GetFreeCameraRot() != FRotator(0.0f, 0.0f, 0.0f)) { return; }

	if (!m_bCameraMove)
	{

		m_MoveInput = Value.Get<FVector2D>();

		// MoveInput �� XY ���t
		if (m_MoveInput.X == 0.f)
		{
			m_RayVec = FVector(m_ForwardDirection.X * m_MoveInput.Y * 13.f, m_ForwardDirection.Y * m_MoveInput.Y * 13.f, -10.f);
			UpdatePlayerCollision();

			if (m_IsMove)
			{
				if (!m_bWarp) { return; }

				// ���͂��ꂽ������360���\�L(Radian)�ɕϊ�
				float angle = atan2(m_ForwardDirection.Y * m_MoveInput.Y, m_ForwardDirection.X * m_MoveInput.Y);
				float angleDeg = FMath::RadiansToDegrees(angle);

				UE_LOG(LogTemp, Warning, TEXT("X ForwardD %s RightD %s"), *m_ForwardDirection.ToString(), *m_RightDirection.ToString());

				// �}�C�i�X�̏ꍇ�������ɂ���
				if (angleDeg < 0)
				{
					angleDeg += 360;
				}

				SetMovement(angleDeg);

				m_IsMoving = true;

				// UKismetSystemLibrary::PrintString(this, FString::Printf(TEXT("Moved: X:%f Y:%f"), m_MoveInput.X, m_MoveInput.Y), true, true, FColor::Cyan, 10.0f, TEXT("None"));
				//UKismetSystemLibrary::PrintString(this, FString::Printf(TEXT("Degree: %f"), angleDeg), true, true, FColor::Cyan, 10.0f, TEXT("None"));
			}
		}
		else if (m_MoveInput.Y == 0.f)
		{
			m_RayVec = FVector(m_RightDirection.X * m_MoveInput.X * 13.f, m_RightDirection.Y * m_MoveInput.X * 13.f, -10.f);
			UpdatePlayerCollision();

			if (m_IsMove)
			{
				if (!m_bWarp) { return; }

				// ���͂��ꂽ������360���\�L(Radian)�ɕϊ�
				float angle = atan2(m_RightDirection.Y * m_MoveInput.X, m_RightDirection.X * m_MoveInput.X);
				float angleDeg = FMath::RadiansToDegrees(angle);

				UE_LOG(LogTemp, Warning, TEXT("X ForwardD %s RightD %s"), *m_ForwardDirection.ToString(), *m_RightDirection.ToString());

				// �}�C�i�X�̏ꍇ�������ɂ���
				if (angleDeg < 0)
				{
					angleDeg += 360;
				}

				SetMovement(angleDeg);

				m_IsMoving = true;

				// UKismetSystemLibrary::PrintString(this, FString::Printf(TEXT("Moved: X:%f Y:%f"), m_MoveInput.X, m_MoveInput.Y), true, true, FColor::Cyan, 10.0f, TEXT("None"));
				//UKismetSystemLibrary::PrintString(this, FString::Printf(TEXT("Degree: %f"), angleDeg), true, true, FColor::Cyan, 10.0f, TEXT("None"));
			}
		}
	}
}

// A�{�^��(�C�O�ł́Z)�ŃA�N�V�������N����
void APlayer3D::PressedAction()
{
	gamemanager->SetIsSelect(true);
	//UKismetSystemLibrary::PrintString(this, TEXT("Action Pressed"), true, true, FColor::Cyan, 10.0f, TEXT("None"));
}

// Backspace�L�[�AB�{�^���i�C�O�ł�X�j�Ŗ߂�
void APlayer3D::PressedBack()
{
	//UKismetSystemLibrary::PrintString(this, TEXT("Back Pressed"), true, true, FColor::Cyan, 10.0f, TEXT("None"));
}

//�EStick
void APlayer3D::PressedStickR_X(const FInputActionValue& Value)
{
	//�^�㎋�_�̏ꍇ�͏������Ȃ�
	if (EDimensionType::TwoD == gamemanager->GetDimensionType() || gamemanager->GetIsDimensionChange()) { return; }

	FVector2D _StickR_Input = Value.Get<FVector2D>();

	ACameraPawn0332* pMainCamera = Cast<ACameraPawn0332>(m_MainCamera);

	//�v���C���[�������Ă���t���[�J���������ɖ߂��đ��I��
	if (m_IsMoving)
	{

		pMainCamera->SetFreeCameraRot(FRotator(0.0f, 0.0f, 0.0f));
		return;
	}

	pMainCamera->SetFreeCameraRot(FRotator(0.0f, _StickR_Input.X, 0.0f) + pMainCamera->GetFreeCameraRot());
}
//�EStick
void APlayer3D::PressedStickR_Y(const FInputActionValue& Value)
{
	//�^�㎋�_�̏ꍇ�͏������Ȃ�
	if (EDimensionType::TwoD == gamemanager->GetDimensionType() || gamemanager->GetIsDimensionChange()) { return; }

	FVector2D _StickR_Input = Value.Get<FVector2D>();

	ACameraPawn0332* pMainCamera = Cast<ACameraPawn0332>(m_MainCamera);

	//�v���C���[�������Ă���t���[�J���������ɖ߂��đ��I��
	if (m_IsMoving)
	{

		pMainCamera->SetFreeCameraRot(FRotator(0.0f, 0.0f, 0.0f));
		return;
	}
	
	if (_StickR_Input.X > 0.0f)//�v���X�̎�
	{
		if (pMainCamera->GetFreeCameraRot().Pitch > 30.0f)//30�𒴂������H
		{
			_StickR_Input.X = 0.0f;
		}
	}
	else
		if (_StickR_Input.X < 0.0f)//�}�C�i�X�̎�
		{
			if (pMainCamera->GetFreeCameraRot().Pitch < -30.0f)//-30�𒴂������H
			{
				_StickR_Input.X = 0.0f;
			}
		}

	pMainCamera->SetFreeCameraRot(FRotator( _StickR_Input.X,0.0f, 0.0f) + pMainCamera->GetFreeCameraRot());
}


//--------��|���ǉ������֐�

float APlayer3D::CalculateHeight(float Degrees)//�ړ����p�x�ɉ����ăv���C���[�̍����������ς���
{
	// �p�x��0�`90�x�͈̔͂ɐ��K��
	while (Degrees < 0.0f)
	{
		Degrees += 90.0f;
	}
	while (Degrees > 90.0f)
	{
		Degrees -= 90.0f;
	}

	Degrees += 45.0f;//45�x���炷

	// �p�x�����W�A���ɕϊ�
	float AngleRadians = FMath::DegreesToRadians(Degrees);

	// �����isin�l�j���v�Z
	float Height = FMath::Sin(AngleRadians) - FMath::DegreesToRadians(45.0f);


	if (Height < 0)
	{
		Height = 0;
	}
	return Height;
}
void APlayer3D::UpdateGoalAction(float DeltaTime)
{
	if (!m_GoalFlag || m_IsMoving)
	{
		return;
	}

	FVector PlayerPos = GetActorLocation();
	if (PlayerPos.Z <= m_BasePlayerPos.Z - m_SinkHeight)//���ݍ��ލ����𒴂�����I��
	{
		PlayerPos.Z = m_BasePlayerPos.Z - m_SinkHeight;//�����𑵂���
		SetActorLocation(PlayerPos);
		return;
	}

	//������������
	m_SinkVel -= m_SinkAcc;//���x������������
	PlayerPos.Z += m_SinkVel * DeltaTime;
	SetActorLocation(PlayerPos);
}
void APlayer3D::SetGoalFlag(bool _Flag)
{
	m_GoalFlag = _Flag;
}
float APlayer3D::GetFinHeight()//�I�_�̍������擾
{
	return m_StartLocation.Z;
}
bool APlayer3D::GetIsMoving()
{
	return m_IsMoving;
}
FVector APlayer3D::GetBasePlayerPos()
{
	return m_BasePlayerPos;
}

void APlayer3D::FreeCameraControl()
{
	ACameraPawn0332* pMainCamera = Cast<ACameraPawn0332>(m_MainCamera);

	//�v���C���[�������Ă���t���[�J���������ɖ߂��đ��I��
	if (m_IsMoving)
	{

		pMainCamera->SetFreeCameraRot(FRotator(0.0f, 0.0f, 0.0f));
		return;
	}

	FRotator FreeRot = pMainCamera->GetFreeCameraRot();
	pMainCamera->SetFreeCameraRot(FreeRot * 0.94f);
	if (FreeRot.Pitch < 0.05f && FreeRot.Pitch > -0.05f &&
		FreeRot.Roll < 0.05f && FreeRot.Roll > -0.05f &&
		FreeRot.Yaw < 0.05f && FreeRot.Yaw > -0.05f)
	{
		pMainCamera->SetFreeCameraRot(FRotator(0.0f, 0.0f, 0.0f));
	}


	// �Q�[���p�b�h�̍��X�e�B�b�NY���̓��͒l���擾
	//float CurrentStickValueX = GetWorld()->GetFirstPlayerController()->GetInputAxisValue("Gamepad_LeftX");
	// �Q�[���p�b�h�̍��X�e�B�b�NY���̓��͒l���擾
	//float CurrentStickValueY = GetWorld()->GetFirstPlayerController()->GetInputAxisValue("Gamepad_LeftY");
	//if (CurrentStickValueX != 0.0f || CurrentStickValueY != 0)
	{
	//	UE_LOG(LogTemp, Warning, TEXT("InputStick!!"));
	}
	//if (GetWorld()->GetFirstPlayerController()->GetInputAxisValue("MoveRight"))
	{
		//UE_LOG(LogTemp, Warning, TEXT("InputStick!!"));
	}

	//pMainCamera->SetFreeCameraRot(FRotator(0.0f, CurrentStickValueY*10.0f, CurrentStickValueX*10.0f));
}

void APlayer3D::MoveRight(float Value)
{
	UE_LOG(LogTemp, Warning, TEXT("LeftX: %f"), Value);
}