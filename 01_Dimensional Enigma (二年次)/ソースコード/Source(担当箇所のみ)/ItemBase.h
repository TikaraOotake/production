#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "ItemBase.generated.h"

UCLASS()
class MYPROJECT_API AItemBase : public APawn
{
    GENERATED_BODY()

public:
    AItemBase();

protected:
    virtual void BeginPlay() override;

public:
    virtual void Tick(float DeltaTime) override;

    virtual void SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent) override;

    UFUNCTION()
    void OnActorBeginOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor, UPrimitiveComponent* OtherComponent, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult);

    int GetItemID();

private:

    UPROPERTY(EditAnywhere, Category = "Movement")
    float MovementY = 1.2f;//�c�̈ړ����x
    UPROPERTY(EditAnywhere, Category = "Movement")
    float GravityScale = 4.0f;//�d��

    UPROPERTY(EditAnywhere, Category = "Movement")
    float RotateAmount_Idol = 50.0f;//�ҋ@���̉�]��
    UPROPERTY(EditAnywhere, Category = "Movement")
    float RotateAmount_Out = 800.0f;//���Ŏ��̉�]��

    UPROPERTY(EditAnywhere, Category = "ItemID")
    int ItemID = 0;//�A�C�e����ID�@�v�f����10�Ȃ̂�0�`9�̊ԂŐݒ肵�Ă�������

    UPROPERTY(EditAnywhere, Category = "Collision")
    TSubclassOf<AActor> TargetActorClass;

    // �t�@�C���p�X�p�� FString �ϐ�
    UPROPERTY(EditAnywhere,  Category = "SoundEffect")
    FString SE_Path = TEXT("/Game/Sound/GetItemSE.GetItemSE");

    UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Components", meta = (AllowPrivateAccess = "true"))
    class USphereComponent* SphereComponent;

    UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Components", meta = (AllowPrivateAccess = "true"))
    class UStaticMeshComponent* StaticMeshComponent;

    int IsGetting = false;
    float Counter = 0.0f;
    float RotateAmount = 50;//��]��

    float  m_DeleteHeight;//�폜���鍂��

};