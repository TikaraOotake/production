#include "ItemBase.h"
#include "Components/SphereComponent.h"
#include "Components/StaticMeshComponent.h"
#include "UObject/ConstructorHelpers.h"
#include "Engine/StaticMesh.h"

#include "ItemGameInstance.h"
#include "ItemSubsystem.h"
#include "ItemManager.h"
#include "SoundManagerSubsystem.h"

AItemBase::AItemBase()
{
    PrimaryActorTick.bCanEverTick = true;

    // ����R���W�����̐ݒ�
    SphereComponent = CreateDefaultSubobject<USphereComponent>(TEXT("SphereComponent"));
    RootComponent = SphereComponent;
    SphereComponent->InitSphereRadius(50.0f);  // ���a��50�ɐݒ�
    SphereComponent->SetCollisionEnabled(ECollisionEnabled::QueryAndPhysics);
    SphereComponent->SetCollisionResponseToAllChannels(ECR_Overlap);
    SphereComponent->OnComponentBeginOverlap.AddDynamic(this, &AItemBase::OnActorBeginOverlap);

    // ���b�V���R���|�[�l���g�̐ݒ�
    StaticMeshComponent = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("StaticMeshComponent"));
    StaticMeshComponent->SetupAttachment(RootComponent);

    // �f�t�H���g���b�V���̓K�p
    static ConstructorHelpers::FObjectFinder<UStaticMesh> DefaultMesh(TEXT("/Engine/BasicShapes/Cube.Cube"));
    if (DefaultMesh.Succeeded())
    {
        StaticMeshComponent->SetStaticMesh(DefaultMesh.Object);
    }
}

void AItemBase::BeginPlay()
{
    Super::BeginPlay();

    //��]�ʂ��Z�b�g
    RotateAmount = RotateAmount_Idol;

    if (!StaticMeshComponent->GetStaticMesh())
    {
        static ConstructorHelpers::FObjectFinder<UStaticMesh> DefaultMesh(TEXT("/Engine/BasicShapes/Cube.Cube"));
        if (DefaultMesh.Succeeded())
        {
            StaticMeshComponent->SetStaticMesh(DefaultMesh.Object);
        }
    }

    UWorld* world = GetWorld();
    if (world)
    {
        UItemManager* itemMng =  world->GetSubsystem<UItemManager>();
        if (itemMng)
        {
            itemMng->RegisterItem(this);
        }
    }

    
}

void AItemBase::Tick(float DeltaTime)
{
    Super::Tick(DeltaTime);

    // ���݂̉�]���擾
    FRotator CurrentRotation = GetActorRotation();

    // ��]���`
    FRotator NewRotation = CurrentRotation + FRotator(0.0f, RotateAmount * DeltaTime, 0.0f);

    // �A�N�^�[�̉�]���X�V
    SetActorRotation(NewRotation);


    //�擾��̏���
    if (IsGetting)
    {
        //���݂̍��W���擾
        FVector Pos = GetActorLocation();
        FVector NewPos = Pos + FVector(0.0f, 0.0f, MovementY);

        //�A�N�^�[�̍��W���X�V
        SetActorLocation(NewPos);

        //�d�͂�^����
        MovementY -= GravityScale * DeltaTime;


        Counter += DeltaTime;
    }
    else
    {
        //�폜���鍂�����Z�b�g(�������ς�邱�Ƃ����邽�ߏ�ɊĎ�)
        m_DeleteHeight = GetActorLocation().Z;
    }
    //���ɒB������폜
    if ( m_DeleteHeight - 5.0f > GetActorLocation().Z)
    {
        Destroy();
    }
}

void AItemBase::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
    Super::SetupPlayerInputComponent(PlayerInputComponent);
}

void AItemBase::OnActorBeginOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor, UPrimitiveComponent* OtherComponent, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult)
{
    if (OtherActor && OtherActor->IsA(TargetActorClass))
    {
        if (!IsGetting)
        {
            UE_LOG(LogTemp, Warning, TEXT("OnCollision"));
            //�����ŏ��𑗂�
            UItemSubsystem* ItemSubsystem = GetGameInstance()->GetSubsystem<UItemSubsystem>();
            if (ItemSubsystem)
            {
                ItemSubsystem->SetItemFlag(ItemID, true); // �擾��Ԃ�
                ItemSubsystem->GetCount += 1;//�擾�������Z
                GetWorld()->GetSubsystem<UItemManager>()->OnGetItem(this);

                UE_LOG(LogTemp, Warning, TEXT("GetCount: %d"), ItemSubsystem->GetCount)
            }

            //SE���Đ�
            USoundManagerSubsystem* SoundManager = GetGameInstance()->GetSubsystem<USoundManagerSubsystem>();
            if (SoundManager)
            {
                // �p�X�ŉ������Đ�
                SoundManager->PlaySoundFromPath(SE_Path);

                UE_LOG(LogTemp, Warning, TEXT("PlaySE"))
            }

            IsGetting = true;
            //��]�ʂ��Z�b�g
            RotateAmount = RotateAmount_Out;
        }
    }
}

int AItemBase::GetItemID()
{
    return ItemID;
}