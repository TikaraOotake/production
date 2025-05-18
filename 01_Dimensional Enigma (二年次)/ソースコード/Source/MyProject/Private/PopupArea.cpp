// Fill out your copyright notice in the Description page of Project Settings.


#include "PopupArea.h"

#include "Components/SphereComponent.h"
#include "Components/StaticMeshComponent.h"
#include "UObject/ConstructorHelpers.h"
#include "Engine/StaticMesh.h"

#include "PopupUISubSystem.h"

APopupArea::APopupArea()
{
    PrimaryActorTick.bCanEverTick = true;

    // ����R���W�����̐ݒ�
    SphereComponent = CreateDefaultSubobject<USphereComponent>(TEXT("SphereComponent"));
    RootComponent = SphereComponent;
    SphereComponent->InitSphereRadius(50.0f);
    SphereComponent->SetCollisionEnabled(ECollisionEnabled::QueryAndPhysics);
    SphereComponent->SetCollisionResponseToAllChannels(ECR_Overlap);
    SphereComponent->OnComponentBeginOverlap.AddDynamic(this, &APopupArea::OnActorBeginOverlap);
    SphereComponent->OnComponentEndOverlap.AddDynamic(this, &APopupArea::OnActorEndOverlap);

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

// Called when the game starts or when spawned
void APopupArea::BeginPlay()
{
    Super::BeginPlay();
}

// Called every frame
void APopupArea::Tick(float DeltaTime)
{
    Super::Tick(DeltaTime);

    if (bIsOverlapping)
    {
        UE_LOG(LogTemp, Warning, TEXT("Overlapping..."));
    }
}

// Called to bind functionality to input
void APopupArea::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
    Super::SetupPlayerInputComponent(PlayerInputComponent);
}

// �G�ꂽ�u�Ԃ̏���
void APopupArea::OnActorBeginOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor, UPrimitiveComponent* OtherComponent, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult)
{
    if (OtherActor && OtherActor->IsA(TargetActorClass))
    {
        UE_LOG(LogTemp, Warning, TEXT("OnCollision (Enter)"));
        bIsOverlapping = true;

        //UI��\��
        UPopupUISubSystem* PopupUI = GetGameInstance()->GetSubsystem<UPopupUISubSystem>();
        if (PopupUI)
        {
            // �p�X��UI��\��
            PopupUI->ShowPopupUI(WidgetClass);

            UE_LOG(LogTemp, Warning, TEXT("ShowPopup!"))
        }
    }
}

// ���ꂽ�u�Ԃ̏���
void APopupArea::OnActorEndOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor, UPrimitiveComponent* OtherComponent, int32 OtherBodyIndex)
{
    if (OtherActor && OtherActor->IsA(TargetActorClass))
    {
        UE_LOG(LogTemp, Warning, TEXT("OnCollision (Exit)"));
        bIsOverlapping = false;

        //UI��\��
        UPopupUISubSystem* PopupUI = GetGameInstance()->GetSubsystem<UPopupUISubSystem>();
        if (PopupUI)
        {
            // �p�X��UI��\��
            PopupUI->HidePopupUI();

            UE_LOG(LogTemp, Warning, TEXT("ClosePopup!"))
        }
    }
}

