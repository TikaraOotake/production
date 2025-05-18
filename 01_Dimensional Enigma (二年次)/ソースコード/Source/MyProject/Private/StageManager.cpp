// Fill out your copyright notice in the Description page of Project Settings.


#include "StageManager.h"

// ‰Šú‰»
void UStageManager::Initialize(FSubsystemCollectionBase& Collection)
{
    UE_LOG(LogTemp, Warning, TEXT("UStageManager::Initialize()"));



    Super::Initialize(Collection);
}

// ”jŠü
void UStageManager::Deinitialize()
{
    UE_LOG(LogTemp, Warning, TEXT("UStageManager::Deinitialize()"));

    Super::Deinitialize();
}

