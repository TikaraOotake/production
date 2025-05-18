// Fill out your copyright notice in the Description page of Project Settings.

#include "StageBase.h"
#include "Misc/FileHelper.h"
#include "Misc/Paths.h"
#include "Kismet/GameplayStatics.h"
#include "GameFramework/PlayerController.h"
#include "CameraPawn0332.h"
#include "BlockActor.h"

// Sets default values
AStageBase::AStageBase()
	:m_pStageActors()
	,m_blockSize(10.0f)
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	for (int z = 0; z < STAGE_SIZE; ++z)
	{
		for (int y = 0; y < STAGE_SIZE; ++y)
		{
			for (int x = 0; x < STAGE_SIZE; ++x)
			{
				m_pStageActors[x][y][z] = nullptr;
			}
		}
	}
}

// Called when the game starts or when spawned
void AStageBase::BeginPlay()
{
	Super::BeginPlay();
	
	for (int z = 0; z < STAGE_SIZE; ++z)
	{
		for (int y = 0; y < STAGE_SIZE; ++y)
		{
			for (int x = 0; x < STAGE_SIZE; ++x)
			{
				delete m_pStageActors[x][y][z];
				m_pStageActors[x][y][z] = nullptr;
			}
		}
	}
}

// Called every frame
void AStageBase::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);


	if (GetWorld()->GetFirstPlayerController()->WasInputKeyJustPressed(EKeys::M))
	{
		ReadStageData();
	}

	if (GetWorld()->GetFirstPlayerController()->WasInputKeyJustPressed(EKeys::L))
	{		
		LoadStageData();
	}
}

//���݂̃X�e�[�W�z�u��ݒ�
void AStageBase::ReadStageData()
{
	//���������A�N�^�[�ۑ��p�z��
	TArray<AActor*> FoundStageActors;

	//�X�e�[�W�A�N�^�[������
	for (TObjectIterator<AActor> it; it; ++it)
	{
		AActor* actor = *it;
		if (actor && actor->GetClass()->ImplementsInterface(USaveableObject::StaticClass()))
		{
			FoundStageActors.Add(actor);
		}
	}

	//��l�ۑ��p�ϐ�
	FVector minLocation = FVector(0.0f, 0.0f, 0.0f);		//���W

	//��ʒu��ݒ�
	for (int i = 0; i < FoundStageActors.Num(); ++i)
	{
		//�z��ԍ����ɍ��W����
		FVector target = FoundStageActors[i]->GetActorLocation();

		//���������A�N�^�[����ł����W�����������W��T���o��
		if (minLocation.X > target.X)
		{
			minLocation.X = target.X;
		}
		if (minLocation.Y > target.Y)
		{
			minLocation.Y = target.Y;
		}
		if (minLocation.Z > target.Z)
		{
			minLocation.Z = target.Z;
		}
	}
	//�X�e�[�W�S�̂̃[�����W����̂����ݒ�
	m_stageOffset = minLocation;

	UE_LOG(LogTemp, Log, TEXT("StageOffset is %f, %f, %f"), m_stageOffset.X, m_stageOffset.Y, m_stageOffset.Z)

	//�X�e�[�W�f�[�^�ɕۑ�
	for (int i = 0; i < FoundStageActors.Num(); ++i)
	{
		//��O����
		if (!FoundStageActors[i]) break;

		//�J������ۑ�
		else if (FoundStageActors[i]->GetClass() == ACameraPawn0332::StaticClass())
		{
			m_pCameraPawn = FoundStageActors[i];
			UE_LOG(LogTemp, Log, TEXT("Camera Object %s"), *m_pCameraPawn->GetName())
		}

		//���̑�
		else
		{
			//�ۑ�����A�N�^�[�̈ʒu
			FVector targetPos = FoundStageActors[i]->GetActorLocation();
			//�v�f�ԍ����W
			FVector pos(0.0f, 0.0f, 0.0f);

			//�v�f�ԍ��ɕϊ�
			pos.X = (targetPos.X - m_stageOffset.X) / m_blockSize;
			pos.Y = (targetPos.Y - m_stageOffset.Y) / m_blockSize;
			pos.Z = (targetPos.Z - m_stageOffset.Z) / m_blockSize;

			//�z��ɐݒ�
			m_pStageActors[(int)pos.X][(int)pos.Y][(int)pos.Z] = FoundStageActors[i];
			UE_LOG(LogTemp, Log, TEXT("Set Actor Is %s"), *m_pStageActors[(int)pos.X][(int)pos.Y][(int)pos.Z]->GetName())
			UE_LOG(LogTemp, Log, TEXT("Element Number Is : %d, %d, %d"), (int)pos.X, (int)pos.Y, (int)pos.Z)
		}
		
	}
	
	UE_LOG(LogTemp, Log, TEXT("Finish Reading"));

	SaveStageData();
}

//�X�e�[�W�z�u�z�u���Z�[�u
void AStageBase::SaveStageData()
{
	//�ۑ����镶����
	TArray<FString> saveArray;

	for (int z = 0; z < STAGE_SIZE; ++z)
	{
		for (int y = 0; y < STAGE_SIZE; ++y)
		{
			for (int x = 0; x < STAGE_SIZE; ++x)
			{
				if (m_pStageActors[x][y][z])
				{
					//�p�X
					FString classPath = *m_pStageActors[x][y][z]->GetClass()->GetPathName();
					FRotator rotator = m_pStageActors[x][y][z]->GetActorRotation();
					//�Z�[�u�\�I�u�W�F�N�g�̃Z�[�u�f�[�^
					FString objectSave = Cast<ISaveableObject>(m_pStageActors[x][y][z])->GetSaveDataToString();

					//�Z�[�u�f�[�^���܂Ƃ߂�
					//�N���X�̃p�X
					//�v�f�ԍ��~�R
					//Rotator(float �~ 3)
					//�Z�[�u�\�I�u�W�F�N�g�̃Z�[�u�f�[�^
					FString saveString = FString::Printf(TEXT("%s,%d,%d,%d,%f,%f,%f,%s"), 
						*classPath,
						x, y, z,
						rotator.Pitch,rotator.Yaw,rotator.Roll,
						*objectSave);

					UE_LOG(LogTemp, Log, TEXT("%s"), *saveString)

					//�Z�[�u�f�[�^�Ƀ}�[�W
					saveArray.Add(saveString);
				}
			}
		}
	}


	//�}�[�W���ꂽ�Z�[�u�f�[�^�ۑ��p
	FString saveData;

	//�f�[�^���I�u�W�F�N�g���Ƃɉ��s����
	for (int i = 0; i < saveArray.Num(); ++i)
	{
		saveData.Append(saveArray[i]);

		if (i < saveArray.Num() - 1)
		{
			saveData.Append(TEXT("\n"));
		}
	}

	UE_LOG(LogTemp, Log, TEXT("Save Text Is %s"), *saveData);
	WriteCSVFile(saveData, SaveFileName);
}

//�Z�[�u�f�[�^����z�u
void AStageBase::LoadStageData()
{
	//�I�u�W�F�N�g���Ƃ̔z��
	TArray<FString> saveArray;
	//�X�e�[�W�f�[�^�̎擾
	FString saveText = ReadCSVFile(LoadFileName);

	//�X�e�[�W�f�[�^���I�u�W�F�N�g���Ƃɕ�����
	saveText.ParseIntoArrayLines(saveArray);
	
	//�X�e�[�W�̃Z�[�u�f�[�^��ǂݍ���
	//saveArray[0]

	//�I�u�W�F�N�g�̔z�u
	PlaceStageActor(saveArray);
	//�J�����̔z�u
	PlaceCamera();
}

//�X�e�[�W�A�N�^�[��ݒu
void AStageBase::PlaceStageActor(TArray<FString> _stageDataArray)
{
	TArray<UClass*> actorCache;

	for (int i = 1; i < _stageDataArray.Num(); ++i)
	{
		//�I�u�W�F�N�g�f�[�^��1���ɕ�����
		TArray<FString> saveData;
		_stageDataArray[i].ParseIntoArray(saveData, TEXT(","), false);

		//�X�|�[��������A�N�^�[
		UClass* actorClass(nullptr);

		//�L���b�V�����ɓ����N���X������΂�����g��
		for (int chaceIt = 0; chaceIt < actorCache.Num(); ++chaceIt)
		{
			if (actorCache[chaceIt]->GetPathName() == *saveData[0])
			{
				actorClass = actorCache[chaceIt];
				break;
			}
		}
		//�L���b�V�����ɂȂ�������p�X����T��
		if (!actorClass)
		{
			actorClass = StaticLoadClass(AActor::StaticClass(), nullptr, *saveData[0]);

			//��O����
			if(!actorClass) UE_LOG(LogTemp, Error, TEXT("Spawn Class Is Get Lost : %s"), *saveData[0])
		}

		//�z��ԍ����擾
		int pos[3](FCString::Atoi(*saveData[1]), FCString::Atoi(*saveData[2]), FCString::Atoi(*saveData[3]));
		//���W��ݒ�
		FVector v((float)pos[0] * m_blockSize, (float)pos[1] * m_blockSize, (float)pos[2] * m_blockSize);
		//��]��ݒ�
		FRotator r(FCString::Atof(*saveData[4]), FCString::Atof(*saveData[5]), FCString::Atof(*saveData[5]));
		//�g�����X�t�H�[���̐ݒ�
		FTransform t(r , v + m_stageOffset, FVector(1.0f, 1.0f, 1.0f));

		//�A�N�^�[���X�|�[��
		m_pStageActors[pos[0]][pos[1]][pos[2]] = GetWorld()->SpawnActor<AActor>(actorClass, t);
	}
	
}

//�J������ݒu
void AStageBase::PlaceCamera()
{
	//�g�����X�t�H�[����ݒ�
	FVector  l(0.0f, 0.0f, 0.0f);
	FRotator r(-35.0f, -45.0f, 0.0f);
	FVector  s(1.0f, 1.0f, 1.0f);
	FTransform cameraTransform(r, l, s);

	//�J�������X�|�[��
	m_pCameraPawn = GetWorld()->SpawnActor<ACameraPawn0332>(ACameraPawn0332::StaticClass(), cameraTransform);
	//�r���[�^�[�Q�b�g�ɐݒ�
	UGameplayStatics::GetPlayerController(GetWorld(), 0)->SetViewTargetWithBlend(m_pCameraPawn);

	UE_LOG(LogTemp, Log, TEXT("Pawn Camera"));
}

//CSV�t�@�C���ɏ����o��
void AStageBase::WriteCSVFile(FString _string, FString _fileName)
{
	//�p�X
	FString filePath = FPaths::ProjectDir() + TEXT("StageFile/") + _fileName + TEXT(".csv");

	if (FFileHelper::SaveStringToFile(_string, *filePath))
	{
		UE_LOG(LogTemp, Log, TEXT("Saved csvFile path : %s"), *filePath);
	}
	else
	{
		UE_LOG(LogTemp, Error, TEXT("Failed Save csvFile"));
	}
}

//CSV�t�@�C����ǂݎ��
FString AStageBase::ReadCSVFile(const FString _fileName)
{
	//�p�X
	FString filePath = FPaths::ProjectDir() + TEXT("StageFile/") + _fileName + TEXT(".csv");
	//�ǂݎ��p�ϐ�
	FString fileString("");

	//�ǂݎ��
	if (FFileHelper::LoadFileToString(fileString, *filePath))
	{
		UE_LOG(LogTemp, Log, TEXT("Read csvFile"))
	}
	else
	{
		UE_LOG(LogTemp, Error, TEXT("Failed Read csvFile path : %s"), *filePath)
	}

	return fileString;
}