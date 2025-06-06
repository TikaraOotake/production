// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Subsystems/WorldSubsystem.h"
#include "Kismet/KismetSystemLibrary.h" // log用

#include "LevelManager.generated.h"

/**
 * 
 */
UCLASS()
class MYPROJECT_API ULevelManager : public UWorldSubsystem
{
	GENERATED_BODY()
	
public:

	// 初期化
	virtual void Initialize(FSubsystemCollectionBase& Collection);

	virtual void Deinitialize();

private:
	void Setting_IA(); // IAの設定
	
private:

	// 設定するInputActionの名前
	FString InputActionName;

	class UInputMappingContext* DefaultMappingContext;

	class UInputAction* MoveInput;

	class UInputAction* StartInput;

	class UInputAction* CameraInput;

	class UInputAction* ActionInput;

	class UInputAction* BackInput;

};
