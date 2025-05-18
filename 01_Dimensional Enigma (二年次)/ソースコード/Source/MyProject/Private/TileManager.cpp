// Fill out your copyright notice in the Description page of Project Settings.


#include "TileManager.h"
#include "Engine/StaticMeshActor.h"
#include "UObject/ConstructorHelpers.h"
#include "GameFramework/Actor.h"
//#include "EngineUtils.h" // For TActorIterator
//#include "Block.cpp"
#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "Engine/World.h"        // GetWorld�p
#include "EngineUtils.h"        // TActorIterator�p
#include "TriangleTile.h"
#include "TriangleTile_Up.h"
#include "GameFramework/PlayerController.h"

// �R���X�g���N�^
ATileManager::ATileManager()
{
    PrimaryActorTick.bCanEverTick = true;

    // ���b�V�������[�h
    static ConstructorHelpers::FObjectFinder<UStaticMesh> CubeMeshFinder(TEXT("/Engine/BasicShapes/Cube"));
    if (CubeMeshFinder.Succeeded())
    {
        CubeMesh = CubeMeshFinder.Object;
    }
    else
    {
        UE_LOG(LogTemp, Error, TEXT("Failed to load Cube mesh in constructor!"));
    }
    
    
    /* // ���O�p�`�̃^�C�����b�V�������[�h
    static ConstructorHelpers::FObjectFinder<UStaticMesh> TriangleMeshFinder(TEXT("StaticMesh'/Game/Path/To/Your/TriangleTile.TriangleTile'"));
    if (TriangleMeshFinder.Succeeded())
    {
        CubeMesh = TriangleMeshFinder.Object;
    }
    else
    {
        UE_LOG(LogTemp, Error, TEXT("Failed to load Triangle tile mesh in constructor!"));
    }
    */
   
}

void ATileManager::BeginPlay()
{
    Super::BeginPlay();

    MapData3D.SetNum(100); // �O���̔z���2�v�f�ɐݒ�

    for (int32 i = 0; i < MapData3D.Num(); i++) {
        MapData3D[i].SetNum(100); // ���Ԃ̔z���2�v�f�ɐݒ�
        for (int32 j = 0; j < MapData3D[i].Num(); j++) {
            MapData3D[i][j].SetNum(100); // �����̔z���2�v�f�ɐݒ�
            for (int32 k = 0; k < MapData3D[i][j].Num(); k++) {
                MapData3D[i][j][k] = 0; // �l��0�ŏ�����
            }
        }
    }
}

void ATileManager::Tick(float DeltaTime)
{
    Super::Tick(DeltaTime);

    if (GetWorld()->GetFirstPlayerController()->WasInputKeyJustPressed(EKeys::Q))
    {
        SpawnCubeAtLocation(FVector(0.0f, 0.0f, 0.0f), FRotator(0.0f, 0.0f, 0.0f), FVector(0.1f, 0.1f, 0.1f));
    }
    if (GetWorld()->GetFirstPlayerController()->WasInputKeyJustPressed(EKeys::E))
    {
        //RemoveLastSpawnedActor();
        TestCubeGrid();
    }
   
    if (GetWorld()->GetFirstPlayerController()->WasInputKeyJustPressed(EKeys::M))
    {
    }

    if (GetWorld()->GetFirstPlayerController()->WasInputKeyJustPressed(EKeys::U))
    {
        TileGenerate(1,1);
    }
    if (GetWorld()->GetFirstPlayerController()->WasInputKeyJustPressed(EKeys::I))
    {
        TileGenerate(1,-1);
    }
    if (GetWorld()->GetFirstPlayerController()->WasInputKeyJustPressed(EKeys::O))
    {
        TileGenerate(-1,1);
    }
    if (GetWorld()->GetFirstPlayerController()->WasInputKeyJustPressed(EKeys::P))
    {
        TileGenerate(-1,-1);
    }
}

void ATileManager::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
    Super::SetupPlayerInputComponent(PlayerInputComponent);
    PlayerInputComponent->BindAction("SpawnCube", IE_Pressed, this, &ATileManager::OnSpawnCubeInput);
}

void ATileManager::OnSpawnCubeInput()
{
    SpawnCubeAtLocation(FVector(0.0f, 0.0f, 0.0f), FRotator(0.0f, 0.0f, 0.0f), FVector(0.1f, 0.1f, 0.1f));
}

void ATileManager::SpawnCubeAtLocation(FVector Location, FRotator Rotation, FVector Scale) {
    UWorld* World = GetWorld();
    if (!World) {
        UE_LOG(LogTemp, Error, TEXT("World is nullptr!"));
        return;
    }

    FActorSpawnParameters SpawnParams;
    SpawnParams.Owner = this;
    SpawnParams.SpawnCollisionHandlingOverride = ESpawnActorCollisionHandlingMethod::AlwaysSpawn;

    //AStaticMeshActor* Cube = World->SpawnActor<AStaticMeshActor>(Location, FRotator::ZeroRotator, SpawnParams);

    AStaticMeshActor* Cube = Cast<AStaticMeshActor>(GetWorld()->SpawnActor<AActor>(ATriangleTile::StaticClass(), Location, Rotation, SpawnParams));

    //AStaticMeshActor* NewActor = World->SpawnActor<AStaticMeshActor>(AStaticMeshActor::StaticClass(), Location, Rotation, SpawnParams);
    if (!Cube) {
        UE_LOG(LogTemp, Error, TEXT("Failed to spawn AStaticMeshActor!"));
        return;
    }

    UStaticMeshComponent* MeshComp = Cube->GetStaticMeshComponent();
    if (!MeshComp) {
        UE_LOG(LogTemp, Error, TEXT("StaticMeshComponent is nullptr!"));
        return;
    }

    if (CubeMesh) {
        MeshComp->SetMobility(EComponentMobility::Movable); // ������Movable�ɐݒ�
        //MeshComp->SetStaticMesh(CubeMesh);
        MeshComp->SetWorldScale3D(Scale); // �X�P�[����ݒ�
    }

    // ���X�g�ɒǉ�
    SpawnedActors.Add(Cube);
    UE_LOG(LogTemp, Log, TEXT("Spawned Cube at Location: %s"), *Location.ToString());
}

void ATileManager::Spawn2DTileAtLocation(FVector Location, FRotator Rotation, FVector Scale) {
    UWorld* World = GetWorld();
    if (!World) {
        UE_LOG(LogTemp, Error, TEXT("World is nullptr!"));
        return;
    }

    FActorSpawnParameters SpawnParams;
    SpawnParams.Owner = this;
    SpawnParams.SpawnCollisionHandlingOverride = ESpawnActorCollisionHandlingMethod::AlwaysSpawn;

    //AStaticMeshActor* Cube = World->SpawnActor<AStaticMeshActor>(Location, FRotator::ZeroRotator, SpawnParams);

    AStaticMeshActor* Cube = Cast<AStaticMeshActor>(GetWorld()->SpawnActor<AActor>(ATriangleTile::StaticClass(), Location, Rotation, SpawnParams));

    //AStaticMeshActor* NewActor = World->SpawnActor<AStaticMeshActor>(AStaticMeshActor::StaticClass(), Location, Rotation, SpawnParams);
    if (!Cube) {
        UE_LOG(LogTemp, Error, TEXT("Failed to spawn AStaticMeshActor!"));
        return;
    }

    UStaticMeshComponent* MeshComp = Cube->GetStaticMeshComponent();
    if (!MeshComp) {
        UE_LOG(LogTemp, Error, TEXT("StaticMeshComponent is nullptr!"));
        return;
    }

    if (CubeMesh) {
        MeshComp->SetMobility(EComponentMobility::Movable); // ������Movable�ɐݒ�
        //MeshComp->SetStaticMesh(CubeMesh);
        MeshComp->SetWorldScale3D(Scale); // �X�P�[����ݒ�
    }

    // ���X�g�ɒǉ�
    SpawnedActors.Add(Cube);
    UE_LOG(LogTemp, Log, TEXT("Spawned Cube at Location: %s"), *Location.ToString());
}

void ATileManager::RemoveLastSpawnedActor()
{
    if (SpawnedActors.Num() == 0)
    {
        UE_LOG(LogTemp, Warning, TEXT("No actors to remove!"));
        return;
    }

    // �z��̍Ō�̃A�N�^�[���擾���č폜
    AActor* ActorToRemove = SpawnedActors.Pop(); // �z�񂩂�폜���擾
    if (ActorToRemove)
    {
        ActorToRemove->Destroy(); // �A�N�^�[�̍폜
        UE_LOG(LogTemp, Log, TEXT("Removed actor: %s"), *ActorToRemove->GetName());
    }
}
void ATileManager::RemoveAllSpawnedActors()
{
    for (AActor* Actor : SpawnedActors)
    {
        if (Actor)
        {
            Actor->Destroy();
            UE_LOG(LogTemp, Log, TEXT("Removed actor: %s"), *Actor->GetName());
        }
    }

    // �z����N���A
    SpawnedActors.Empty();
    UE_LOG(LogTemp, Log, TEXT("Removed all spawned actors."));
}

void ATileManager::TileGenerate(int SignX,int SignY)
{
    //��x�o�������̂��폜
    RemoveAllSpawnedActors();

    //�O�����n�`�����擾
    //UpdateMap3DWithBlocks(Map3D, 61, 61, 61, 30);
    FVector GridSize(100, 100, 100);   // �O���b�h�T�C�Y
    FVector GridOrigin(50, 50, 50);   // ���_
    float GridSpacing = 10.0f;    // �O���b�h�Z���̊Ԋu

    UWorld* World = GetWorld();    // ���݂̃��[���h���擾
    if (!World) {
        UE_LOG(LogTemp, Error, TEXT("NoWorld!"));
        return;
    }

    MapData3D = GetCubeGrid(World, GridSize, GridOrigin, GridSpacing);

    //�O��������␳����������^�C���������S�ʒu���Z�o���^�C���������W�Ƃ��ē񎟌����ɋL�^

    ////�܂�6���^�C����u�����S�_���L�^����
    TArray<TArray<int>> TileSetPoint2D;
    // �O���b�h�̃T�C�Y��������
    TileSetPoint2D.SetNum((GridSize.X * 4) + 4);
    for (int x = 0; x < (GridSize.X * 4) + 4; ++x)
    {
        TileSetPoint2D[x].SetNum((GridSize.Y * 4) + 4);
        for (int y = 0; y < (GridSize.Y * 4) + 4; ++y)
        {
            TileSetPoint2D[x][y] = 0; // �������F���ׂ�0
        }
    }

    //---------------------�����܂ł�OK

    //����̓J�����̌����ɍ��킹�ĕς���

    for (int x = 0; x < GridSize.X; ++x)
    {
        for (int y = 0; y < GridSize.Y; ++y)
        {
            for (int z = 0; z < GridSize.Z; ++z)
            {
                if (MapData3D[x][y][z] == 1)
                {
                    //�z��O�̋��ꂪ����ꍇ�̓X�L�b�v
                    if (x + (z * SignX) +50>= 0 && y + (z * SignY) +50>=0)
                    {
                        //����ȕ␳���|����
                                            //����Z�̒l��XY�������炷
                        TileSetPoint2D[x + (z * SignX)+50][y + (z * SignY)+50] = z + 1;
                    }

                    //�e�X�g�Ƃ��Č��̍��W�Ƀu���b�N��ݒu
                    //SpawnCubeAtLocation(FVector(x * 10, y * 10, z*1.0f), FRotator(0.0f, 0.0f, 0.0f), FVector(1.1f, 1.1f, 1.1f));
                    //�͈͓��͐������擾�ł��Ă��܂�

                    //�e�X�g�Ƃ��Ă��炵�����W�Ƀu���b�N��ݒu
                    SpawnCubeAtLocation(FVector((x + z * SignX) * 10, (y + z * SignY) * 10, -50+z * 10.0f), FRotator(0.0f, 0.0f, 0.0f), FVector(1.1f, 1.1f, 1.1f));

                    //�␳�Ȃ��ŋL�^
                    //TileSetPoint2D[x ][y ] = z + 1;
                }
            }
        }
    }
    //�^�C���ݒu�ʒu�����߂Ă���
    TArray<TArray<int>> TilePoint2D;
    // �O���b�h�̃T�C�Y��������
    TilePoint2D.SetNum((GridSize.X * 10) + 4);
    for (int x = 0; x < (GridSize.X * 10) + 4; ++x)
    {
        TilePoint2D[x].SetNum((GridSize.Y * 5) + 2);
        for (int y = 0; y < (GridSize.Y * 5) + 2; ++y)
        {
            TilePoint2D[x][y] = 0; // �������F���ׂ�0
        }
    }

    //�ʂ̃^�C�������߂Ă���
    
    //�Ⴂ�Ƃ�����D��I�ɖ��߂Ă���
    for (int z = 0; z < GridSize.Z*2; ++z)
    {

        //�s�񃋁[�v
        for (int x = 0; x < (GridSize.X * 2) ; ++x)
        {
            for (int y = 0; y < (GridSize.Y * 2) ; ++y)
            {

                //0�͏Ȃ�
                if (TileSetPoint2D[x][y] == z&& TileSetPoint2D[x][y]!=0)
                {
                    //���҂̕������s��v(+-,-+)�̏ꍇ�����ς��Ȃ���΂Ȃ�Ȃ�

                    TilePoint2D[(x * 2)][y] = 2;
                    TilePoint2D[(x * 2) + 1][y] = 2;
                    TilePoint2D[(x * 2) + 2][y] = 2;
                    TilePoint2D[(x * 2) + 1][y + 1] = 2;
                    TilePoint2D[(x * 2) + 2][y + 1] = 1;    //
                    TilePoint2D[(x * 2) + 3][y + 1] = 1;    //��ʎO�p�`
                    
                    //�m�F���邽�߂ɃA�N�^�[��u���Ă݂�
                    SpawnCubeAtLocation(FVector(x * 10, y * 10, 0.0f), FRotator(0.0f, 0.0f, 0.0f), FVector(1.1f, 1.1f, 1.1f));
                }
            }
        }
    }




    //�^�C�������@
    // //�񎟌�����X��2���Ƃ�X����+��3/2 Y����+0.5������
    // //�񎟌��z���Y��1���Ƃ�X����-��3/2 Y����+0.5������
    // //�^�C�����t�O�p�Ƃ��Ĕz�u������̂�180�x�̉�]��X����+��3/3������(X���&&Y������==�t�O�p�`||X������&&Y���==�t�O�p�`)
    for (int x = 0; x < (GridSize.X * 10) + 2; ++x)
    {
        for (int y = 0; y < (GridSize.Y * 5) + 2; ++y)
        {
            if (TilePoint2D[x][y] == 1|| TilePoint2D[x][y] == 2)
            {
                /*
                 //�ݒu������W���v�Z
                float PosX = 0.0f;
                float PosY = 0.0f;
                int i = x / 2;
                PosX += (std::sqrt(3.0f) / 2.0f) * (float)i;
                PosY += 0.5f * (float)i;
                PosX -= (std::sqrt(3.0f) / 2.0f) * y;
                PosY += 0.5f * y;
                */

                //�ݒu������W���v�Z
                float PosX = 0.0f;
                float PosY = 0.0f;
                int i = x + 1 / 2;

                PosX += (std::sqrt(3.0f) / 2.0f) * (float)i;
                //PosY += (0.5f * x)- (0.25f * x);

                PosX -= (std::sqrt(3.0f) / 2.0f) * y;
                PosY += 1.0f * y;


                //��]������l�𐶐�
                float Rot = 90;
                if ((int)x % 2 == 0)
                {
                    Rot = -90;
                }


                //�ݒu
                Spawn2DTileAtLocation(FVector(PosX * 10, PosY * 10, 0.0f), FRotator(0.0f, Rot, 0.0f), FVector(0.5f, 0.5f, 0.5f));
            }
            if (TilePoint2D[x][y] == 2&&false)
            {
                /*
                 //�ݒu������W���v�Z
                float PosX = 0.0f;
                float PosY = 0.0f;
                int i = x / 2;
                PosX += (std::sqrt(3.0f) / 2.0f) * (float)i;
                PosY += 0.5f * (float)i;
                PosX -= (std::sqrt(3.0f) / 2.0f) * y;
                PosY += 0.5f * y;
                */

                //�ݒu������W���v�Z
                float PosX = 0.0f;
                float PosY = 0.0f;
                int i = x + 1 / 2;

                PosX += (std::sqrt(3.0f) / 2.0f) * (float)i;
                //PosY += (0.5f * x)- (0.25f * x);

                PosX -= (std::sqrt(3.0f) / 2.0f) * y;
                PosY += 1.0f * y;


                //��]������l�𐶐�
                float Rot = 90;
                if ((int)x % 2 == 0)
                {
                    Rot = -90;
                }


                //�ݒu
                Spawn2DTileAtLocation(FVector(PosX * 10, PosY * 10, -1.0f), FRotator(0.0f, Rot, 0.0f), FVector(0.5f, 0.5f, 0.5f));
            }
        }
    }
    
    

}

TArray<TArray<TArray<int>>> ATileManager::GetCubeGrid(UWorld* World, FVector GridSize, FVector GridCenter, float GridSpacing)
{
    // ���ʂ��i�[����3�����z��
    TArray<TArray<TArray<int>>> CubeGrid;

    // �O���b�h�̃T�C�Y��������
    CubeGrid.SetNum(GridSize.X);
    for (int x = 0; x < GridSize.X; ++x)
    {
        CubeGrid[x].SetNum(GridSize.Y);
        for (int y = 0; y < GridSize.Y; ++y)
        {
            CubeGrid[x][y].SetNum(GridSize.Z);
            for (int z = 0; z < GridSize.Z; ++z)
            {
                CubeGrid[x][y][z] = 0; // �������F���ׂ�0
            }
        }
    }

    // ���[���h�������ȏꍇ�͏I��
    if (!World) return CubeGrid;

    // �O���b�h�̃I�t�Z�b�g���v�Z�i���S�����_�Ƃ��č��W�𒲐��j
    FVector GridOrigin = GridCenter - FVector(GridSize.X / 2.0f, GridSize.Y / 2.0f, GridSize.Z / 2.0f) * GridSpacing;

    // ���x�����̂��ׂẴA�N�^�[���擾
    for (TActorIterator<AActor> ActorItr(World); ActorItr; ++ActorItr)
    {
        AActor* Actor = *ActorItr;
        if (//(Actor && Actor->GetClass()->GetName().Contains(TEXT("Cube"))) ||
            //(Actor && Actor->GetClass()->GetName().Contains(TEXT("Block"))) ||
            (Actor && Actor->GetClass()->GetName().Contains(TEXT("StaticMeshActor")))) // ���O�Ŕ���
        {
            // �L���[�u�̃��[���h���W���擾
            FVector ActorLocation = Actor->GetActorLocation();

            // �O���b�h���W�ɕϊ�
            int32 GridX = FMath::FloorToInt((ActorLocation.X - GridOrigin.X) / GridSpacing);
            int32 GridY = FMath::FloorToInt((ActorLocation.Y - GridOrigin.Y) / GridSpacing);
            int32 GridZ = FMath::FloorToInt((ActorLocation.Z - GridOrigin.Z) / GridSpacing);

            // �O���b�h���W���͈͓��ł���ꍇ�ɋL�^
            if (GridX >= 0 && GridX < GridSize.X &&
                GridY >= 0 && GridY < GridSize.Y &&
                GridZ >= 0 && GridZ < GridSize.Z)
            {
                CubeGrid[GridX][GridY][GridZ] = 1; // �L���[�u�����݂���ꏊ��1��ݒ�
            }
        }
    }

    return CubeGrid;
}
void ATileManager::TestCubeGrid()
{
    FVector GridSize(100, 100, 100);   // �O���b�h�T�C�Y
    FVector GridOrigin(50, 50, 50);   // ���_
    float GridSpacing = 10.0f;    // �O���b�h�Z���̊Ԋu

    UWorld* World = GetWorld();    // ���݂̃��[���h���擾
    if (!World) {
        UE_LOG(LogTemp, Error, TEXT("NoWorld!"));
        return;
    }

    TArray<TArray<TArray<int>>> CubeGrid = GetCubeGrid(World, GridSize, GridOrigin, GridSpacing);

    // ���ʂ����O�ɏo��
    for (int x = 0; x < GridSize.X; ++x)
    {
        for (int y = 0; y < GridSize.Y; ++y)
        {
            for (int z = 0; z < GridSize.Z; ++z)
            {
                if (CubeGrid[x][y][z] == 1)
                {
                    UE_LOG(LogTemp, Log, TEXT("Cube found at (%d, %d, %d)"), x, y, z);
                    SpawnCubeAtLocation(FVector(x * 10.0f - 450.0f, y * 10.0f - 450.0f, z * 10.0f - 450.0f), FRotator(0.0f, 0.0f, 0.0f), FVector(0.11f, 0.11f, 0.11f));
                    //SpawnCubeAtLocation(FVector(x * 10.0f , y * 10.0f , z * 10.0f), FRotator(0.0f, 0.0f, 0.0f), FVector(0.11f, 0.11f, 0.11f));
                }
                else
                    if (CubeGrid[x][y][z] == 0)
                    {
                        //UE_LOG(LogTemp, Log, TEXT("NoCube found at (%d, %d, %d)"), x, y, z);
                    }
            }
        }
    }
}

// �A�N�^�[���\���ɂ���֐�
void ATileManager::HideActor(AActor* TargetActor)
{
    if (TargetActor)
    {
        // �A�N�^�[�������ׂẴ��b�V���R���|�[�l���g���\���ɂ���
        TArray<UStaticMeshComponent*> MeshComponents;
        TargetActor->GetComponents<UStaticMeshComponent>(MeshComponents);

        for (UStaticMeshComponent* MeshComponent : MeshComponents)
        {
            MeshComponent->SetVisibility(false, true);  // ���b�V�����\���ɂ���
        }
    }
}

// �A�N�^�[��\������֐�
void ATileManager::ShowActor(AActor* TargetActor)
{
    if (TargetActor)
    {
        // �A�N�^�[�������ׂẴ��b�V���R���|�[�l���g��\������
        TArray<UStaticMeshComponent*> MeshComponents;
        TargetActor->GetComponents<UStaticMeshComponent>(MeshComponents);

        for (UStaticMeshComponent* MeshComponent : MeshComponents)
        {
            MeshComponent->SetVisibility(true, true);  // ���b�V����\������
        }
    }
}
