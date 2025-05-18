#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "RollingCubeGrid.generated.h"

UCLASS()
class MYPROJECT_API ARollingCubeGrid : public AActor
{
    GENERATED_BODY()

public:
    ARollingCubeGrid();

protected:
    virtual void BeginPlay() override;

public:
    virtual void Tick(float DeltaTime) override;

private:
    // �����̃��b�V��
    UPROPERTY(VisibleAnywhere)
    UStaticMeshComponent* CubeMesh;

    // �}�X�ڂ̃T�C�Y
    float GridSize;

    // �ړ����̃t���O
    bool bIsMoving;

    // ���݂̊p�x
    FQuat CurrentRotation;

    // ��]���S�i���j�̈ʒu
    FVector RotationAxis;

    // �ڕW�ʒu�Ɖ�]
    FVector TargetPosition;
    FQuat TargetRotation;

    // �ړ��֐�
    void MoveForward();
    void MoveRight();

    // �ړ����
    void InterpolateMovement(float DeltaTime);

    // ��]���S���v�Z
    FVector CalculateRotationAxis(FVector Direction);
};
