// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Blueprint/UserWidget.h"
#include "PauseWidget.generated.h"


class UButton;
/**
 * 
 */
UCLASS()
class MYPROJECT_API UPauseWidget : public UUserWidget
{
	GENERATED_BODY()
	

private:
	UPROPERTY(EditAnywhere, meta = (BindWidget))
	class UButton* ButtonReStart;

	UPROPERTY(EditAnywhere, meta = (BindWidget))
	class UButton* ButtonWorldMap;

	UPROPERTY(EditAnywhere, meta = (BindWidget))
	class UButton* ButtonTitle;

	UPROPERTY(EditAnywhere, meta = (BindWidget))
	class UButton* ButtonCancel;

	UPROPERTY(EditAnywhere, meta = (BindWidget))
	class UButton* ButtonEndGame;


	// ���ݑI�����Ă���{�^��
	UButton* CurrentFocusedButton;

protected:
	// NativeConstruct
	void NativeConstruct() override;

private:
	// ButtonReStart��OnClicked�C�x���g�Ɋ֘A�Â���
	UFUNCTION()
	void OnButtonReStartClicked();

	// ButtonTitle��OnClicked�C�x���g�Ɋ֘A�Â���
	UFUNCTION()
	void OnButtonWorldMapClicked();

	// ButtonTitle��OnClicked�C�x���g�Ɋ֘A�Â���
	UFUNCTION()
	void OnButtonTitleClicked();

	// ButtonQuit��OnClicked�C�x���g�Ɋ֘A�Â���
	UFUNCTION()
	void OnButtonCancelClicked();

	// ButtonQuit��OnClicked�C�x���g�Ɋ֘A�Â���
	UFUNCTION()
	void OnButtonEndGame();


	void SetFocusToButton(UButton* Button);

	void OnNavigateUp();
	void OnNavigateDown();


};
