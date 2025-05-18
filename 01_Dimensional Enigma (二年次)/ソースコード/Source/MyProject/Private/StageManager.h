// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Subsystems/WorldSubsystem.h"
#include "StageManager.generated.h"

/**
 * 
 */
UCLASS()
class UStageManager : public UWorldSubsystem
{
	GENERATED_BODY()
	
private:

	// èâä˙âª
	virtual void Initialize(FSubsystemCollectionBase& Collection);

	virtual void Deinitialize();


};
