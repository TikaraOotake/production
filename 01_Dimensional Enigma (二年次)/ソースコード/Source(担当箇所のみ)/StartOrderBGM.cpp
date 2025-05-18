// Fill out your copyright notice in the Description page of Project Settings.


#include "StartOrderBGM.h"
#include "SoundManagerSubsystem.h"//BGM�𗬂����߂ɃC���N���[�h

// Sets default values
AStartOrderBGM::AStartOrderBGM()
{
 	// Set this pawn to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

}

// Called when the game starts or when spawned
void AStartOrderBGM::BeginPlay()
{
	Super::BeginPlay();
	
    //BGM���Đ�
    USoundManagerSubsystem* SoundManager = GetGameInstance()->GetSubsystem<USoundManagerSubsystem>();
    if (SoundManager)
    {
        // �p�X�ŉ������Đ�
        SoundManager->PlayBGM(BGM_Path);

        UE_LOG(LogTemp, Warning, TEXT("PlayBGM"))
    }
    //�������I�������ߍ폜���Ă���
    this->Destroy();
}

// Called every frame
void AStartOrderBGM::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

// Called to bind functionality to input
void AStartOrderBGM::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);

}

