// 23cu0221 Tsuchiya


#include "Gamemanager.h"


// ������
void UGameManager::Initialize(FSubsystemCollectionBase& Collection)
{
    UE_LOG(LogTemp, Warning, TEXT("UGameInstanceSubsystem::Initialize()"));

    m_Steps = 0;

    Super::Initialize(Collection);
}

// �j��
void UGameManager::Deinitialize()
{
    UE_LOG(LogTemp, Warning, TEXT("UGameInstanceSubsystem::Deinitialize()"));

    Super::Deinitialize();
}

// Player�̈ʒu��ݒ�
// ���[���h�}�b�v�őI���������[���h�̈ʒu��ۑ����Ă�������
void UGameManager::SetLocation(FVector _location)
{
    m_PlayerLocation = _location;
}

// Player�̈ʒu��Ԃ�
// ���[���h�}�b�v�őI���������[���h�̑O�Ɉړ����邽��
FVector UGameManager::GetLocation()
{
    return m_PlayerLocation;
}

// --------------------------------------------------------
//// Tick���g�p���邽�߂ɕK�{
//TStatId UGameManager::GetStatId() const
//{
//    RETURN_QUICK_DECLARE_CYCLE_STAT(UMyTickableWorldSubsystem, STATGROUP_Tickables);
//}