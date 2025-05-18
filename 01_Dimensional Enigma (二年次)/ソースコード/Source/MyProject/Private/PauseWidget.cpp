// Fill out your copyright notice in the Description page of Project Settings.


#include "PauseWidget.h"
#include "Components/Button.h"
#include "Kismet/GameplayStatics.h"
#include "Framework/Application/SlateApplication.h"
#include "InGameHUD.h"

void UPauseWidget::NativeConstruct()
{
	Super::NativeConstruct();

	// ButtonResume��OnClicked�ɁuOnButtonResumeClicked�v���֘A�Â���
	ButtonReStart->OnClicked.AddUniqueDynamic(this, &UPauseWidget::OnButtonReStartClicked);

	// ButtonTitle��OnClicked�ɁuOnButtonTitleClicked�v���֘A�Â���
	ButtonWorldMap->OnClicked.AddUniqueDynamic(this, &UPauseWidget::OnButtonWorldMapClicked);

	// ButtonQuit��OnClicked�ɁuOnButtonQuitClicked�v���֘A�Â���
	ButtonTitle->OnClicked.AddUniqueDynamic(this, &UPauseWidget::OnButtonTitleClicked);

	// ButtonQuit��OnClicked�ɁuOnButtonQuitClicked�v���֘A�Â���
	ButtonCancel->OnClicked.AddUniqueDynamic(this, &UPauseWidget::OnButtonCancelClicked);

	ButtonEndGame->OnClicked.AddUniqueDynamic(this, &UPauseWidget::OnButtonEndGame);

}

// ���X�^�[�g
void UPauseWidget::OnButtonReStartClicked() {

	// PlayerController���擾����
	const APlayerController* PlayerController = UGameplayStatics::GetPlayerController(GetWorld(), 0);

	// InGameHUD�N���X���擾����
	AInGameHUD* HUD = Cast<AInGameHUD>(PlayerController->GetHUD());

	// ���݂̃��x�������擾
	FString CurrentLevelName = UGameplayStatics::GetCurrentLevelName(this, true);

	// ���݂̃��x����ǂݒ���
	HUD->OpenLevel(FName((CurrentLevelName)));
}

// ���[���h�Z���N�g
void UPauseWidget::OnButtonWorldMapClicked() 
{
	// PlayerController���擾����
	const APlayerController* PlayerController = UGameplayStatics::GetPlayerController(GetWorld(), 0);

	// InGameHUD�N���X���擾����
	AInGameHUD* HUD = Cast<AInGameHUD>(PlayerController->GetHUD());

	// ���x�����J��
	HUD->OpenLevel(FName(TEXT("WorldSelect")));
}


// �^�C�g��
void UPauseWidget::OnButtonTitleClicked() {
	// PlayerController���擾����
	const APlayerController* PlayerController = UGameplayStatics::GetPlayerController(GetWorld(), 0);

	// InGameHUD�N���X���擾����
	AInGameHUD* HUD = Cast<AInGameHUD>(PlayerController->GetHUD());

	// Pause����������
	HUD->OpenLevel(FName(TEXT("GameTitle")));

	UKismetSystemLibrary::PrintString(this, TEXT("GameTitle"), true, true, FColor::Cyan, 10.0f, TEXT("None"));

}

// �L�����Z��
void UPauseWidget::OnButtonCancelClicked() {

	UKismetSystemLibrary::PrintString(this, TEXT("Cancel"), true, true, FColor::Cyan, 10.0f, TEXT("None"));

	// PlayerController���擾����
	const APlayerController* PlayerController = UGameplayStatics::GetPlayerController(GetWorld(), 0);

	// InGameHUD�N���X���擾����
	AInGameHUD* HUD = Cast<AInGameHUD>(PlayerController->GetHUD());

	// Pause����������
	HUD->DispPause(false);
}

void UPauseWidget::OnButtonEndGame()
{
	UWorld* World = GetWorld();

	APlayerController* PlayerController = UGameplayStatics::GetPlayerController(World, 0);
	if (PlayerController)
	{
		// �Q�[�����I���iPIE ���[�h�̏ꍇ�̓Z�b�V�����I���j
		UKismetSystemLibrary::QuitGame(World, PlayerController, EQuitPreference::Quit, false);
	}
}


void UPauseWidget::SetFocusToButton(UButton* Button)
{
	TSharedPtr<SWidget> SlateWidget = Button->TakeWidget();
	FSlateApplication::Get().SetKeyboardFocus(SlateWidget, EFocusCause::Navigation);

	// ���݂̃t�H�[�J�X�{�^�����X�V
	CurrentFocusedButton = Button;
}