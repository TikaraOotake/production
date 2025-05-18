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

//現在のステージ配置を設定
void AStageBase::ReadStageData()
{
	//検索したアクター保存用配列
	TArray<AActor*> FoundStageActors;

	//ステージアクターを検索
	for (TObjectIterator<AActor> it; it; ++it)
	{
		AActor* actor = *it;
		if (actor && actor->GetClass()->ImplementsInterface(USaveableObject::StaticClass()))
		{
			FoundStageActors.Add(actor);
		}
	}

	//基準値保存用変数
	FVector minLocation = FVector(0.0f, 0.0f, 0.0f);		//座標

	//基準位置を設定
	for (int i = 0; i < FoundStageActors.Num(); ++i)
	{
		//配列番号順に座標を代入
		FVector target = FoundStageActors[i]->GetActorLocation();

		//検索したアクターから最も座標が小さい座標を探し出す
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
	//ステージ全体のゼロ座標からのずれを設定
	m_stageOffset = minLocation;

	UE_LOG(LogTemp, Log, TEXT("StageOffset is %f, %f, %f"), m_stageOffset.X, m_stageOffset.Y, m_stageOffset.Z)

	//ステージデータに保存
	for (int i = 0; i < FoundStageActors.Num(); ++i)
	{
		//例外処理
		if (!FoundStageActors[i]) break;

		//カメラを保存
		else if (FoundStageActors[i]->GetClass() == ACameraPawn0332::StaticClass())
		{
			m_pCameraPawn = FoundStageActors[i];
			UE_LOG(LogTemp, Log, TEXT("Camera Object %s"), *m_pCameraPawn->GetName())
		}

		//その他
		else
		{
			//保存するアクターの位置
			FVector targetPos = FoundStageActors[i]->GetActorLocation();
			//要素番号座標
			FVector pos(0.0f, 0.0f, 0.0f);

			//要素番号に変換
			pos.X = (targetPos.X - m_stageOffset.X) / m_blockSize;
			pos.Y = (targetPos.Y - m_stageOffset.Y) / m_blockSize;
			pos.Z = (targetPos.Z - m_stageOffset.Z) / m_blockSize;

			//配列に設定
			m_pStageActors[(int)pos.X][(int)pos.Y][(int)pos.Z] = FoundStageActors[i];
			UE_LOG(LogTemp, Log, TEXT("Set Actor Is %s"), *m_pStageActors[(int)pos.X][(int)pos.Y][(int)pos.Z]->GetName())
			UE_LOG(LogTemp, Log, TEXT("Element Number Is : %d, %d, %d"), (int)pos.X, (int)pos.Y, (int)pos.Z)
		}
		
	}
	
	UE_LOG(LogTemp, Log, TEXT("Finish Reading"));

	SaveStageData();
}

//ステージ配置配置をセーブ
void AStageBase::SaveStageData()
{
	//保存する文字列
	TArray<FString> saveArray;

	for (int z = 0; z < STAGE_SIZE; ++z)
	{
		for (int y = 0; y < STAGE_SIZE; ++y)
		{
			for (int x = 0; x < STAGE_SIZE; ++x)
			{
				if (m_pStageActors[x][y][z])
				{
					//パス
					FString classPath = *m_pStageActors[x][y][z]->GetClass()->GetPathName();
					FRotator rotator = m_pStageActors[x][y][z]->GetActorRotation();
					//セーブ可能オブジェクトのセーブデータ
					FString objectSave = Cast<ISaveableObject>(m_pStageActors[x][y][z])->GetSaveDataToString();

					//セーブデータをまとめる
					//クラスのパス
					//要素番号×３
					//Rotator(float × 3)
					//セーブ可能オブジェクトのセーブデータ
					FString saveString = FString::Printf(TEXT("%s,%d,%d,%d,%f,%f,%f,%s"), 
						*classPath,
						x, y, z,
						rotator.Pitch,rotator.Yaw,rotator.Roll,
						*objectSave);

					UE_LOG(LogTemp, Log, TEXT("%s"), *saveString)

					//セーブデータにマージ
					saveArray.Add(saveString);
				}
			}
		}
	}


	//マージされたセーブデータ保存用
	FString saveData;

	//データをオブジェクトごとに改行する
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

//セーブデータから配置
void AStageBase::LoadStageData()
{
	//オブジェクトごとの配列
	TArray<FString> saveArray;
	//ステージデータの取得
	FString saveText = ReadCSVFile(LoadFileName);

	//ステージデータをオブジェクトごとに分ける
	saveText.ParseIntoArrayLines(saveArray);
	
	//ステージのセーブデータを読み込む
	//saveArray[0]

	//オブジェクトの配置
	PlaceStageActor(saveArray);
	//カメラの配置
	PlaceCamera();
}

//ステージアクターを設置
void AStageBase::PlaceStageActor(TArray<FString> _stageDataArray)
{
	TArray<UClass*> actorCache;

	for (int i = 1; i < _stageDataArray.Num(); ++i)
	{
		//オブジェクトデータを1つずつに分ける
		TArray<FString> saveData;
		_stageDataArray[i].ParseIntoArray(saveData, TEXT(","), false);

		//スポーンさせるアクター
		UClass* actorClass(nullptr);

		//キャッシュ内に同じクラスがあればそれを使う
		for (int chaceIt = 0; chaceIt < actorCache.Num(); ++chaceIt)
		{
			if (actorCache[chaceIt]->GetPathName() == *saveData[0])
			{
				actorClass = actorCache[chaceIt];
				break;
			}
		}
		//キャッシュ内になかったらパスから探す
		if (!actorClass)
		{
			actorClass = StaticLoadClass(AActor::StaticClass(), nullptr, *saveData[0]);

			//例外処理
			if(!actorClass) UE_LOG(LogTemp, Error, TEXT("Spawn Class Is Get Lost : %s"), *saveData[0])
		}

		//配列番号を取得
		int pos[3](FCString::Atoi(*saveData[1]), FCString::Atoi(*saveData[2]), FCString::Atoi(*saveData[3]));
		//座標を設定
		FVector v((float)pos[0] * m_blockSize, (float)pos[1] * m_blockSize, (float)pos[2] * m_blockSize);
		//回転を設定
		FRotator r(FCString::Atof(*saveData[4]), FCString::Atof(*saveData[5]), FCString::Atof(*saveData[5]));
		//トランスフォームの設定
		FTransform t(r , v + m_stageOffset, FVector(1.0f, 1.0f, 1.0f));

		//アクターをスポーン
		m_pStageActors[pos[0]][pos[1]][pos[2]] = GetWorld()->SpawnActor<AActor>(actorClass, t);
	}
	
}

//カメラを設置
void AStageBase::PlaceCamera()
{
	//トランスフォームを設定
	FVector  l(0.0f, 0.0f, 0.0f);
	FRotator r(-35.0f, -45.0f, 0.0f);
	FVector  s(1.0f, 1.0f, 1.0f);
	FTransform cameraTransform(r, l, s);

	//カメラをスポーン
	m_pCameraPawn = GetWorld()->SpawnActor<ACameraPawn0332>(ACameraPawn0332::StaticClass(), cameraTransform);
	//ビューターゲットに設定
	UGameplayStatics::GetPlayerController(GetWorld(), 0)->SetViewTargetWithBlend(m_pCameraPawn);

	UE_LOG(LogTemp, Log, TEXT("Pawn Camera"));
}

//CSVファイルに書き出す
void AStageBase::WriteCSVFile(FString _string, FString _fileName)
{
	//パス
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

//CSVファイルを読み取る
FString AStageBase::ReadCSVFile(const FString _fileName)
{
	//パス
	FString filePath = FPaths::ProjectDir() + TEXT("StageFile/") + _fileName + TEXT(".csv");
	//読み取り用変数
	FString fileString("");

	//読み取り
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