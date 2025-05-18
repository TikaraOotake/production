// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Engine/StaticMeshActor.h"
#include "SaveableObject.h"
#include "BlockActor.generated.h"

//class UStaticMeshComponent;

/**
 * 
 */
UCLASS()
class MYPROJECT_API ABlockActor : public AStaticMeshActor, public ISaveableObject
{
	GENERATED_BODY()
	
public:
	ABlockActor();
	
public:
	//String�^�̃Z�[�u�f�[�^���擾
	virtual FString GetSaveDataToString();
	//String�^�̃Z�[�u�f�[�^��ݒ�
	virtual void SetSaveDataToString(const FString _saveData);

private:
	FString m_defaultPath;
};
