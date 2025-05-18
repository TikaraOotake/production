// Fill out your copyright notice in the Description page of Project Settings.


#include "ChaseOrderCamera.h"

#include  "Player3D.h"//�v���C���[�̊֐��ɃA�N�Z�X���邽��
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

	//Player�̍��W��Camera�ɑ����������
	if (m_pPlayer && m_pCamera)
	{
		//�ړ����̓J�����̍�����ς������Ȃ����߈ꎞ�L�^���Ă���
		float CameraHeight_temp = m_CameraPos.Z;

		
		//�^�Ԋ҂��Ď擾
		if (m_pPlayer->IsA(APlayer3D::StaticClass()))//�k���`�F�b�N
		{
			APlayer3D* pPlayer3D = Cast<APlayer3D>(m_pPlayer);
			m_CameraPos = pPlayer3D->GetBasePlayerPos();
		}
		else
		{
			//�^�ϊ��Ɏ��s������A�N�^�[�̊֐�������W�����������Ă���
			m_CameraPos = m_pPlayer->GetActorLocation();//�擾
		}

		//�f�����������ɕ␳
		m_CameraPos.Z += m_AddHeight;

		//�ړ�������
		m_pCamera->SetActorLocation(m_CameraPos);
	}
}

// Called to bind functionality to input
void AChaseOrderCamera::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);

}

