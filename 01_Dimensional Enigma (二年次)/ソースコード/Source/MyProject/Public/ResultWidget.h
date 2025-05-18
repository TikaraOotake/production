// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Blueprint/UserWidget.h"
#include "Components/Image.h"
#include "ResultWidget.generated.h"

/**
 * 
 */
UCLASS()
class MYPROJECT_API UResultWidget : public UUserWidget
{
	GENERATED_BODY()
	
public:

	virtual void NativeConstruct() override;

	virtual void NativeDestruct() override;

	UFUNCTION(BlueprintCallable, Category = "UI")
	void SetRank(UTexture2D* _texture);

	UFUNCTION(BlueprintCallable, Category = "UI")
	void SetClearTime(float _clearTime);

	UFUNCTION(BlueprintCallable, Category = "UI")
	void SetPlayerStep(int _playerStep);

	UFUNCTION(BlueprintCallable, Category = "UI")
	void SetCameraMove(int _cameraMove);

	UFUNCTION(BlueprintCallable, Category = "UI")
	void SetItemTexture(UTexture2D* _newTexture);

public:
	UPROPERTY(BlueprintReadOnly)
	UTexture2D* m_rankTexture;

	UPROPERTY(BlueprintReadOnly)
	TArray<FString> m_itemTexturePathArray;

	UPROPERTY(BlueprintReadOnly)
	TArray<FSlateBrush> m_itemBrush;

	UPROPERTY(BlueprintReadOnly)
	FSlateBrush m_rankBrush;

	UPROPERTY(BlueprintReadOnly)
	float m_clearTime;

	UPROPERTY(BlueprintReadOnly)
	int m_playerStep;

	UPROPERTY(BlueprintReadOnly)
	int m_cameraMove;

	UPROPERTY(BlueprintReadOnly)
	FString m_itemString;
};
