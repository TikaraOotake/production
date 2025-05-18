// Fill out your copyright notice in the Description page of Project Settings.


#include "InGameHUD.h"
#include "Kismet/GameplayStatics.h"
#include "Blueprint/WidgetBlueprintLibrary.h"
#include "Kismet/KismetSystemLibrary.h"
#include "Blueprint/UserWidget.h"


void AInGameHUD::BeginPlay()
{
	// WidgetBluePrint�̃N���X���擾
	FString PauseWidgetPath = TEXT("/Game/UI/Pause.Pause_C");
	TSubclassOf<UUserWidget> PauseWidgetClass = TSoftClassPtr<UUserWidget>(FSoftClassPath(*PauseWidgetPath)).LoadSynchronous();
	if (!PauseWidgetClass)
	{
		UE_LOG(LogTemp, Error, TEXT("PauseWidget class failed to load"));
		return;
	}

	// PlayerController���擾
	APlayerController* PlayerController = UGameplayStatics::GetPlayerController(GetWorld(), 0);

	if (PauseWidgetClass && PlayerController)
	{
		PauseWidget = UWidgetBlueprintLibrary::Create(GetWorld(), PauseWidgetClass, PlayerController);
		PauseWidget->SetVisibility(ESlateVisibility::Collapsed);
		PauseWidget->AddToViewport(0);
	}

}

void AInGameHUD::DispPause(const bool IsPause) {

	APlayerController* PlayerController = GetOwningPlayerController();

	if (IsPause)
	{
		// Pause��ʂ�\��
		PauseWidget->SetVisibility(ESlateVisibility::Visible);

		// UI���[�h�ɐݒ�
		UWidgetBlueprintLibrary::SetInputMode_UIOnlyEx(PlayerController, PauseWidget,EMouseLockMode::DoNotLock, false);

		// Game��Pause��Ԃɂ���
		UGameplayStatics::SetGamePaused(GetWorld(), true);
		UE_LOG(LogTemp, Warning, TEXT("Paused"));

		// Mouse�J�[�\����\������
		PlayerController->SetShowMouseCursor(true);
	}
	else
	{
		// Mouse�J�[�\����\������
		PlayerController->SetShowMouseCursor(false);

		UGameplayStatics::SetGamePaused(GetWorld(), false);
		UE_LOG(LogTemp, Warning, TEXT("Restart"));

		UWidgetBlueprintLibrary::SetInputMode_GameOnly(PlayerController, false);

		PauseWidget->SetVisibility(ESlateVisibility::Collapsed);

	}
}

void AInGameHUD::OpenLevel(const FName LevelName)
{
	UGameplayStatics::OpenLevel(GetWorld(), LevelName);
}