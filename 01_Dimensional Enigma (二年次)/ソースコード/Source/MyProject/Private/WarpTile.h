// WarpTile.h
// 23cu0228 hanawa

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "WarpTile.generated.h"

UENUM()
enum class EWarpType : uint8
{
	UP = 0,
	DOWN = 1,
};

UCLASS()
class AWarpTile : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	AWarpTile();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	void WarpPlayer(AActor* Player);	// �v���C���[�������ړ�(���[�v)�����鏈��

	bool GetIsWarp() const;					// �ړ�(���[�v)�\���ǂ����Ԃ�����

	bool GetIsWarped() const;

	FVector GetDownWarpLocation();

private:
	UPROPERTY(EditAnywhere, Category = "Warp")
	EWarpType m_WarpType;

	UPROPERTY(EditAnywhere, Category = "Warp")
	int m_WarpDirectionNums[5];		// �ړ�(���[�v)�\�ȉ�p�ԍ�

	UPROPERTY(EditANywhere, Category = "Warp")
	AActor* m_pWarpTiles[5];		// �ړ�(���[�v)��̃^�C���̏��

	FVector m_WarpDistination;		// �ړ�(���[�v)��̃^�C���̍��W

	UPROPERTY(EditAnywhere, Category = "CameraDirection")
	AActor* m_pCameraActor;			// ��p�ԍ��𓾂邽�߂̃J����

	bool m_IsWarpable;				// �ړ��\���ǂ���

	bool m_IsWarped;				// ����̃��[�v���ǂ���

private:
	UWorld* world;
	UEnigmaGameInstance* gameinstance;
	UStageSubSystem* gamemanager;
};
