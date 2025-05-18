// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Subsystems/WorldSubsystem.h"
#include "Kismet/KismetSystemLibrary.h" // log�p

#include "LevelManager.generated.h"

/**
 * 
 */
UCLASS()
class MYPROJECT_API ULevelManager : public UWorldSubsystem
{
	GENERATED_BODY()
	
public:

	// ������
	virtual void Initialize(FSubsystemCollectionBase& Collection);

	virtual void Deinitialize();

private:
	void Setting_IA(); // IA�̐ݒ�
	
private:

	// �ݒ肷��InputAction�̖��O
	FString InputActionName;

	class UInputMappingContext* DefaultMappingContext;

	class UInputAction* MoveInput;

	class UInputAction* StartInput;

	class UInputAction* CameraInput;

	class UInputAction* ActionInput;

	class UInputAction* BackInput;

};
