// WarpTile.cpp
// 23cu0228 hanawa

#include "WarpTile.h"
#include "Player3D.h"
#include "MappingCamera3D.h"

// Sets default values
AWarpTile::AWarpTile()
	: m_WarpType(EWarpType::UP)
	, m_WarpDirectionNums()
	, m_pWarpTiles()
	, m_WarpDistination(FVector::ZeroVector)
	, m_pCameraActor(nullptr)
	, m_IsWarpable(false)
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;
}

// Called when the game starts or when spawned
void AWarpTile::BeginPlay()
{
	Super::BeginPlay();
	
	// Tag ��ݒ�
	Tags.Add(FName(TEXT("WarpTile")));

	// GameManager���擾
	world = GetWorld();
	if (world) {
		gameinstance = Cast<UEnigmaGameInstance>(world->GetGameInstance());
		if (gameinstance) {
			gamemanager = world->GetSubsystem<UStageSubSystem>();
		}
	}
}

// Called every frame
void AWarpTile::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
}

// �v���C���[�������ړ�(���[�v)�����鏈��
// �����ɂ� Player3D �Ŏw��
void AWarpTile::WarpPlayer(AActor* Player)
{
	m_IsWarpable = false;
	AMappingCamera3D* pCamera = Cast<AMappingCamera3D>(m_pCameraActor) ;

	if (pCamera)
	{
		for (int j = 0; j < sizeof(m_WarpDirectionNums)/sizeof(m_WarpDirectionNums[0]); ++j)
		{
			m_IsWarpable = false;
			m_IsWarped = false;

			// �J�����̉�p�ԍ��ƃ��[�v�����ԍ�����v����ꍇ�������s��
			if (gamemanager->DirectionCamNumber == m_WarpDirectionNums[j])
			{
				if (m_WarpDirectionNums[j] == j)
				{
					if (!m_pWarpTiles[j]) { return; }

					// �v���C���[���ړ�(���[�v)�\�Ƃ��A�ړ��Ώۂ̃��[�v�^�C���̍��W�𓾂�
					m_IsWarpable = true;
					m_WarpDistination = m_pWarpTiles[j]->GetActorLocation();

					//��|�̏C���R�[�h:02/02
					//���g�ƈړ���̍������ׂĕ��򂳂���
					EWarpType ResultWarpType = EWarpType::UP;
					if (this->GetActorLocation().Z >= m_WarpDistination.Z)
					{
						ResultWarpType = EWarpType::DOWN;
					}
					else
					{
						ResultWarpType= EWarpType::UP;
					}

					//���ʂ����遫
					switch (ResultWarpType)//����switch (m_WarpType)
					{
					case EWarpType::UP:
						// �v���C���[���ړ��Ώۂ̃^�C����Ɉړ�������
						m_IsWarped = true;
						Player->SetActorLocation(FVector(m_WarpDistination.X, m_WarpDistination.Y, m_WarpDistination.Z + 10.f));
						UE_LOG(LogTemp, Warning, TEXT("UP Warpable"));
						break;

					case EWarpType::DOWN:
						// �v���C���[���ړ��Ώۂ̃^�C����Ɉړ�������
						m_IsWarped = true;

						//Player->SetActorLocation(FVector(m_WarpDistination.X, m_WarpDistination.Y, m_WarpDistination.Z + 10.f));
						UE_LOG(LogTemp, Warning, TEXT("DOWN Warpable %s"), *m_WarpDistination.ToString());
						break;
					}
					return;
				}
				UE_LOG(LogTemp, Log, TEXT("Not DirectionNums[%d] == %d"), m_WarpDirectionNums[j], j)
			}
			// ��p�ԍ��ƕ����ԍ�����v���Ȃ��ꍇ�������s��Ȃ�
			else
			{
				// ���[�v�s�\�Ƃ���
				m_IsWarpable = false;
				m_IsWarped = false;

				UE_LOG(LogTemp, Log, TEXT("Not Warpable, GDNum:%d WDNum:%d"), pCamera->GetDirectionNumber(), m_WarpDirectionNums[j]);
			}
			UE_LOG(LogTemp, Log, TEXT("DirectionNumber is %d"), m_WarpDirectionNums[j]);
		}
	}
	
}

// �ړ�(���[�v)�\���ǂ����Ԃ�����
bool AWarpTile::GetIsWarp() const
{
	return m_IsWarpable;
}

bool AWarpTile::GetIsWarped() const
{
	return m_IsWarped;
}

FVector AWarpTile::GetDownWarpLocation()
{
	return FVector(m_WarpDistination.X, m_WarpDistination.Y, m_WarpDistination.Z + 10.f);
}