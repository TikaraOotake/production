// Fill out your copyright notice in the Description page of Project Settings.


#include "ChaseOrderCamera.h"

#include  "Player3D.h"//プレイヤーの関数にアクセスするため
// Sets default values
AChaseOrderCamera::AChaseOrderCamera()
{
 	// Set this pawn to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

}

// Called when the game starts or when spawned
void AChaseOrderCamera::BeginPlay()
{
	Super::BeginPlay();
	
}

// Called every frame
void AChaseOrderCamera::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	//Playerの座標をCameraに代入し続ける
	if (m_pPlayer && m_pCamera)
	{
		//移動中はカメラの高さを変えたくないため一時記録しておく
		float CameraHeight_temp = m_CameraPos.Z;

		
		//型返還して取得
		if (m_pPlayer->IsA(APlayer3D::StaticClass()))//ヌルチェック
		{
			APlayer3D* pPlayer3D = Cast<APlayer3D>(m_pPlayer);
			m_CameraPos = pPlayer3D->GetBasePlayerPos();
		}
		else
		{
			//型変換に失敗したらアクターの関数から座標を引っ張ってくる
			m_CameraPos = m_pPlayer->GetActorLocation();//取得
		}

		//映したい高さに補正
		m_CameraPos.Z += m_AddHeight;

		//移動させる
		m_pCamera->SetActorLocation(m_CameraPos);
	}
}

// Called to bind functionality to input
void AChaseOrderCamera::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);

}

