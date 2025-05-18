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


	//現在のステージ配置を設定
	void ReadStageData();

	//ステージ配置配置をセーブ
	void SaveStageData();

	//セーブデータから配置
	void LoadStageData();

	//ステージアクターを設置
	void PlaceStageActor(TArray<FString> _stageDataArray);

	//カメラを設置
	void PlaceCamera();

	//CSVファイルに書き出す
	void WriteCSVFile(const FString _string, const FString _fileName);

	//CSVファイルを読み取る
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
	//ステージオブジェクトのインスタンス
	AActor* m_pStageActors[STAGE_SIZE][STAGE_SIZE][STAGE_SIZE];

	//ブロックの一辺の長さ
	float m_blockSize;

	//ステージの原点からのズレ
	FVector m_stageOffset;
};
