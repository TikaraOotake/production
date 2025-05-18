
// 23cu0221 Tsuchiya
// ���x�����J�ڂ��Ă��l���ێ����邽�߂�Manager

#pragma once

#include "CoreMinimal.h"
#include "Kismet/KismetSystemLibrary.h" // Log�p

#include "Subsystems/GameInstanceSubsystem.h"
#include "GameManager.generated.h"

/**
 * 
 */
UCLASS()
class UGameManager : public UGameInstanceSubsystem
{
	GENERATED_BODY()
	

public:

	// ������
	virtual void Initialize(FSubsystemCollectionBase& Collection);

	virtual void Deinitialize();

	//// �G���[���
	//TStatId GetStatId() const;

	// Player�̈ʒu��ݒ�
	void SetLocation(FVector _location);

	// Player�̈ʒu��Ԃ�
	FVector GetLocation();


private:

	FVector m_PlayerLocation;	// ���[���h�Z���N�g���̈ʒu

	int m_Steps;				// �ړ�������

	int m_CameraCount;			// �J�����̉�]��

};
