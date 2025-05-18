#include "RollingCubeGrid.h"
#include "Components/StaticMeshComponent.h"
#include "GameFramework/PlayerController.h"

ARollingCubeGrid::ARollingCubeGrid()
{
    PrimaryActorTick.bCanEverTick = true;

    // ���b�V���̍쐬
    CubeMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("CubeMesh"));
    RootComponent = CubeMesh;

    // �����l
    GridSize = 100.0f; // �}�X�ڂ̃T�C�Y
    bIsMoving = false;

    TargetPosition = GetActorLocation();
    CurrentRotation = GetActorQuat();
    TargetRotation = CurrentRotation;
}

void ARollingCubeGrid::BeginPlay()
{
    Super::BeginPlay();
}

void ARollingCubeGrid::Tick(float DeltaTime)
{
    Super::Tick(DeltaTime);

    // �ړ����łȂ��ꍇ�A���͂��`�F�b�N���ē�����J�n
    if (!bIsMoving)
    {
        APlayerController* PlayerController = GetWorld()->GetFirstPlayerController();
        if (PlayerController)
        {
            if (PlayerController->IsInputKeyDown(EKeys::W))
            {
                MoveForward();
            }
            else if (PlayerController->IsInputKeyDown(EKeys::D))
            {
                MoveRight();
            }
        }
    }

    // �ړ����̕��
    if (bIsMoving)
    {
        InterpolateMovement(DeltaTime);
    }
}

void ARollingCubeGrid::MoveForward()
{
    if (!bIsMoving)
    {
        FVector Direction = FVector(GridSize, 0, 0); // �O����
        RotationAxis = CalculateRotationAxis(Direction);

        TargetPosition = GetActorLocation() + Direction;
        TargetRotation = FQuat(RotationAxis, FMath::DegreesToRadians(90)) * CurrentRotation;

        bIsMoving = true;
    }
}

void ARollingCubeGrid::MoveRight()
{
    if (!bIsMoving)
    {
        FVector Direction = FVector(0, GridSize, 0); // �E����
        RotationAxis = CalculateRotationAxis(Direction);

        TargetPosition = GetActorLocation() + Direction;
        TargetRotation = FQuat(RotationAxis, FMath::DegreesToRadians(90)) * CurrentRotation;

        bIsMoving = true;
    }
}

void ARollingCubeGrid::InterpolateMovement(float DeltaTime)
{
    FVector CurrentPosition = GetActorLocation();
    FQuat CurrentQuat = GetActorQuat();

    // �ʒu�̕��
    FVector NewPosition = FMath::VInterpTo(CurrentPosition, TargetPosition, DeltaTime, 5.0f);

    // ��]�̕��
    FQuat NewRotation = FMath::QInterpTo(CurrentQuat, TargetRotation, DeltaTime, 5.0f);

    // �A�N�^�[�̍X�V
    SetActorLocation(NewPosition);
    SetActorRotation(NewRotation);

    // �ړ������̊m�F
    if (NewPosition.Equals(TargetPosition, 1.0f) && NewRotation.Equals(TargetRotation, 1.0f))
    {
        bIsMoving = false;
        CurrentRotation = TargetRotation;
    }
}

FVector ARollingCubeGrid::CalculateRotationAxis(FVector Direction)
{
    // �ړ������Ɋ�Â��ĉ�]���S���v�Z
    FVector Offset = Direction * 0.5f; // �}�X�ڂ̔��������I�t�Z�b�g
    return (GetActorLocation() + FVector(0, 0, -GridSize * 0.5f) + Offset);
}
