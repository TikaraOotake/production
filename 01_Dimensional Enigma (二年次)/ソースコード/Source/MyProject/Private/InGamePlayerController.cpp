// Fill out your copyright notice in the Description page of Project Settings.


#include "InGamePlayerController.h"
#include "Blueprint/WidgetBlueprintLibrary.h"
#include "InGameHUD.h"
#include "Kismet/GameplayStatics.h"
#include "EnhancedInputComponent.h"
#include "StageSubSystem.h"

AInGamePlayerController::AInGamePlayerController()
{
	// InputAction��ǂݍ���
	PauseAction = LoadObject<UInputAction>(nullptr, TEXT("/Game/EnhancedInput/Input/IA_Start"));

	// Debug�p��Input���擾
	m_DebugEndGame = LoadObject<UInputAction>(nullptr, TEXT("/Game/EnhancedInput/Input/IA_DebugEndGame"));
	m_DebugGameTitle = LoadObject<UInputAction>(nullptr, TEXT("/Game/EnhancedInput/Input/IA_DebugEndGame"));
	m_DebugRestartInGame = LoadObject<UInputAction>(nullptr, TEXT("/Game/EnhancedInput/Input/IA_DebugEndGame"));

	void SetUpInputAction();

}

void AInGamePlayerController::BeginPlay()
{
	// ���͂�Game�݂̂ɂ���
	UWidgetBlueprintLibrary::SetInputMode_GameOnly(this, false);

	// �}�E�X���\���ɂ���
	SetShowMouseCursor(false);
}

void AInGamePlayerController::SetupInputComponent()
{
	Super::SetupInputComponent();

	// �o�C���h�ݒ�
	if (UEnhancedInputComponent* EnhancedInputComponent = CastChecked<UEnhancedInputComponent>(InputComponent)) {
		EnhancedInputComponent->BindAction(PauseAction, ETriggerEvent::Triggered, this, &AInGamePlayerController::DispPause);

		// �f�o�b�O�p�̃L�[
		EnhancedInputComponent->BindAction(m_DebugEndGame, ETriggerEvent::Triggered, this, &AInGamePlayerController::PressedDebugEndGame);
		EnhancedInputComponent->BindAction(m_DebugGameTitle, ETriggerEvent::Triggered, this, &AInGamePlayerController::PressedDebugGameTitle);
		EnhancedInputComponent->BindAction(m_DebugRestartInGame, ETriggerEvent::Triggered, this, &AInGamePlayerController::PressedDebugRestartInGame);

	}
}

// �|�[�Y��ʂ̕\��
void AInGamePlayerController::DispPause(const FInputActionValue& Value)
{
	if (bool V = Value.Get<bool>())
	{
		// GetWorld����StageSubsystem���擾
		if (UWorld* World = GetWorld())
		{
			UStageSubSystem* StageManager = World->GetSubsystem<UStageSubSystem>();

			// �S�[�����Ă�����\���ł��Ȃ��悤��
			if (!StageManager->GetGoaling())
			{
				if (AInGameHUD* HUD = Cast<AInGameHUD>(GetHUD()))
				{
					// Pause��ʂ̕\����\�������s����
					HUD->DispPause(!UGameplayStatics::IsGamePaused(GetWorld()));
				}
			}
		}

	}
}

// �Q�[�����I��������f�o�b�O�L�[�iEsc�L�[�j
void AInGamePlayerController::PressedDebugEndGame()
{
	// GetWorld����R���g���[���[���擾
	UWorld* world = GetWorld();
	if (world)
	{
		APlayerController* PlayerController = world->GetFirstPlayerController();

		// null�`�F�b�N����
		if (PlayerController)
		{
			// �Q�[�����I��
			UKismetSystemLibrary::QuitGame(world, PlayerController, EQuitPreference::Quit, false);
		}
	}
}

// �^�C�g���ɖ߂�f�o�b�O�L�[�iF1�j
void AInGamePlayerController::PressedDebugGameTitle()
{
	UKismetSystemLibrary::PrintString(this, TEXT("DebugGameTitle Pressed"), true, true, FColor::Cyan, 10.0f, TEXT("None"));
	OpenLevel(FName(TEXT("WorldSelect")));
}

// ���݂̃��x�����ăX�^�[�g����f�o�b�O�L�[(F2)
void AInGamePlayerController::PressedDebugRestartInGame()
{
	UKismetSystemLibrary::PrintString(this, TEXT("DebugRestartInGame Pressed"), true, true, FColor::Cyan, 10.0f, TEXT("None"));

	// ���݂̃��x�������擾
	FString CurrentLevelName = UGameplayStatics::GetCurrentLevelName(this, true);
	OpenLevel(FName(CurrentLevelName));
}

// ���x�����J���֐�
void AInGamePlayerController::OpenLevel(FName _LevelName)
{
	UGameplayStatics::OpenLevel(GetWorld(), _LevelName);
}

