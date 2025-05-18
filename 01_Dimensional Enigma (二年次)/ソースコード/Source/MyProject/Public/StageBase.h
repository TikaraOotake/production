// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "StageBase.generated.h"

#define STAGE_SIZE 100

class ABlockActor;
class ACameraPawn0332;

UCLASS()
class MYPROJECT_API AStageBase : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	AStageBase();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;


	//���݂̃X�e�[�W�z�u��ݒ�
	void ReadStageData();

	//�X�e�[�W�z�u�z�u���Z�[�u
	void SaveStageData();

	//�Z�[�u�f�[�^����z�u
	void LoadStageData();

	//�X�e�[�W�A�N�^�[��ݒu
	void PlaceStageActor(TArray<FString> _stageDataArray);

	//�J������ݒu
	void PlaceCamera();

	//CSV�t�@�C���ɏ����o��
	void WriteCSVFile(const FString _string, const FString _fileName);

	//CSV�t�@�C����ǂݎ��
	FString ReadCSVFile(const FString _fileName);

private:

	UPROPERTY(EditAnywhere)
	FString SaveFileName;

	UPROPERTY(EditAnywhere)
	FString LoadFileName;

	//
	UPROPERTY(EditAnywhere)
	FVector m_playerStartPosition;

	UPROPERTY(EditAnywhere)
	AActor* m_pCameraPawn;

private:
	//�X�e�[�W�I�u�W�F�N�g�̃C���X�^���X
	AActor* m_pStageActors[STAGE_SIZE][STAGE_SIZE][STAGE_SIZE];

	//�u���b�N�̈�ӂ̒���
	float m_blockSize;

	//�X�e�[�W�̌��_����̃Y��
	FVector m_stageOffset;
};
