// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/HUD.h"
#include "InGameHUD.generated.h"

/**
 * 
 */
UCLASS()
class MYPROJECT_API AInGameHUD : public AHUD
{
	GENERATED_BODY()
	

protected:
	virtual void BeginPlay() override;

public:
	// �|�[�Y��ʂ̕\��/��\��
	void DispPause(const bool IsPause);

	// Level���J��
	void OpenLevel(const FName LevelName);

private:
	// �|�[�Y��ʂ̃E�B�W�F�b�g
	UUserWidget* PauseWidget;

};
