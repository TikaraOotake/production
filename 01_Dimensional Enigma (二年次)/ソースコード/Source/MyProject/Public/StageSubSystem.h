// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Subsystems/WorldSubsystem.h"
#include "Tickable.h"
#include "StageSubSystem.generated.h"

#define STAGE_SIZE 100

UENUM()
enum class EDimensionType : uint8
{
	ThreeD = 0,
	TwoD = 1,
};

/**
 *
 */
UCLASS(Blueprintable)
class MYPROJECT_API UStageSubSystem : public UWorldSubsystem, public FTickableGameObject
{
	GENERATED_BODY()

public:
	//���������ɌĂяo�����֐�
	virtual void Initialize(FSubsystemCollectionBase& Collection) override;

	void OnWorldInitialized(UWorld* World, const UWorld::InitializationValues IVS);

	//�I�����ɌĂяo�����֐�
	virtual void Deinitialize() override;
	
	//--------------------------------------------
	//Tick�֐�
	//--------------------------------------------
	//���t���[������
	virtual void Tick(float DeltaTime) override;

	//Tick�\���ǂ���
	virtual bool IsTickable() const override;

	// Tickable�I�u�W�F�N�g��ID���擾����
	virtual TStatId GetStatId() const override;

	//--------------------------------------------
	//�I���W�i�����\�b�h
	//--------------------------------------------

	//�v���C���[���������񐔂����Z
	void AddPlayerMoveCount();

	//�v���C���[���������񐔂��擾
	UFUNCTION(BlueprintCallable)
	int GetPlayerMoveCount();

	//�v���C���[�������Ă��邩�ݒ�
	void SetPlayerMoving(bool _status);

	//�v���C���[�������Ă��邩�擾
	bool GetPlayerMoving();

	//�J�������������񐔂����Z
	void AddCameraMoveCount();

	//�J�������������񐔂��擾
	UFUNCTION(BlueprintCallable)
	int GetCameraMoveCount();

	//�J�����������Ă��邩�ݒ�
	void SetCameraMoving(bool _status);

	//�J�����������Ă��邩�擾
	bool GetCameraMoving();	

	void SetDimensionType(const EDimensionType _DimensionType);

	EDimensionType GetDimensionType();

	void SetIsDimensionChange(const bool _DimensionChange);

	bool GetIsDimensionChange();

	//�X�e�[�W�̌o�ߎ��Ԃ��擾
	UFUNCTION(BlueprintCallable)
	float GetStageElapsedTime();

	//�N���A�^�C���̎擾
	UFUNCTION(BlueprintCallable)
	float GetClearTime();

	//�����N�̃e�N�X�`�����擾
	FString GetRankTexturePath();

	//�N���A�^�C���̃����L���O�f�[�^���擾
	UFUNCTION(BlueprintCallable)
	TArray<FString> GetClearTimeRanking();

	UFUNCTION(BlueprintCallable)
	TArray<FString> GetPlayerMoveCountRanking();

	UFUNCTION(BlueprintCallable)
	TArray<FString> GetCameraMoveCountRanking();

	//�S�[�����Ă��邩���擾
	bool GetGoaling();

	void SetIsSelect(bool _IsSelect);

	bool GetIsSelect();

	EDimensionType DimensionType;

	int DirectionCamNumber;

	//�S�[���������ɌĂ΂��
	void OnGoal(FName& _nextLevelName);


//--------------------------------------------------------
//�v���C�x�[�g���\�b�h
//--------------------------------------------------------
private:
	//���U���g���ɌĂ΂��
	void OnResult();

	//���U���g�f�[�^��������
	void SetResultSaveData(FString _string);

	//�m���}�f�[�^��ݒ�
	void SetQuotaData(FString _string);

	//���U���g�f�[�^���Z�[�u
	void SaveResultSaveData();

	//�����L���O�f�[�^��ݒ�
	void SetRankingData();

	//�ł��������f�[�^��Ԃ�
	int GetLeastIntNum(TArray<int> _array);

	int GetLeastFloatNum(TArray<float> _array);

	//�E�B�W�F�b�g��؂�ւ���
	void ChangeWidget(int _widgetNum);

	//�t�@�C���̓ǂݍ���
	FString ReadFile(FString _fileDir);

	//�t�@�C���̏�������
	void WriteFile(FString _fileDir, FString _saveString);

	//�E�B�W�F�b�g��ǂݍ���
	void LoadWedget();



private:

	int m_widgetNum;

	TArray<UUserWidget*> m_widgetArray;

	//�X�e�[�W�̏�Ԃ̗񋓌^
	enum StageStatus
	{
		ThreeD,
		TwoD,
	};

	//�X�e�[�W�̏��
	StageStatus m_status;

	//�o�ߎ���
	float m_elapsedTime;

	//�����N�̃e�N�X�`��
	UTexture2D* m_rankTexture;

	//�N���A�^�C��
	float m_clearTime;

	//�v���C���[�̓�������
	int m_playerMoveCount;

	//�J��������������
	int m_cameraMoveCount;

	bool m_playerMoving;

	bool m_cameraMpving;

	bool m_isDimensionChange;

	//�S�[�����Ă��邩
	bool m_goaling;

	bool m_IsSelect;

	//���̃��x���̖��O
	FName m_nextLevelName;


	struct ResultData
	{
		float clearTime;
		int playerMoveCount;
		int cameraMoveCount;

		ResultData(float _clearTime = 0.0f,int _playerMoveCount = 0,int _cameraMoveCount = 0)
			:clearTime(_clearTime)
			,playerMoveCount(_playerMoveCount)
			,cameraMoveCount(_cameraMoveCount)
		{

		}
	};
	TArray<ResultData> m_resultDataArray;

	ResultData m_quotaData;

	TArray<int> m_clearTimeRanking;

	TArray<int> m_playerMoveRanking;

	TArray<int> m_cameraMoveRanking;

	//-------------------------------------------
	//�C���v�b�g�֘A�v���p�e�B
	//-------------------------------------------

	/*UInputMappingContext* m_inputMappingContext;

	UInputAction* m_BAction;
	UInputAction* m_StartAction;
	UInputAction* m_RBAction;
	UInputAction* m_LBAction;*/
};
