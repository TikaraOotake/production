#include "SoundManagerSubsystem.h"
#include "Components/AudioComponent.h"
#include "Sound/SoundBase.h"
#include "Engine/StreamableManager.h"
#include "Engine/AssetManager.h"
#include "Kismet/GameplayStatics.h"

void USoundManagerSubsystem::Initialize(FSubsystemCollectionBase& Collection)
{
    Super::Initialize(Collection);

    // BGM�p�� AudioComponent ���쐬�i�ď��������Ȃ��j
    /*
    * if (!BGMComponent)
    {
        BGMComponent = NewObject<UAudioComponent>(this, UAudioComponent::StaticClass());
        if (BGMComponent)
        {
            BGMComponent->bAutoActivate = false;
            BGMComponent->bIsUISound = false;
            BGMComponent->bAllowSpatialization = false;
            BGMComponent->RegisterComponentWithWorld(GetWorld()); // �K�� World �ɓo�^
            UE_LOG(LogTemp, Log, TEXT("BGMComponent initialized."));
        }
    }
    */
    
}

void USoundManagerSubsystem::Deinitialize()
{
    if (BGMComponent)
    {
        BGMComponent->Stop();
        BGMComponent->DestroyComponent();
        BGMComponent = nullptr;
    }

    Super::Deinitialize();
}

void USoundManagerSubsystem::PlaySoundEffect(USoundBase* Sound, FVector Location)
{
    if (Sound)
    {
        UGameplayStatics::PlaySoundAtLocation(GetWorld(), Sound, Location);
    }
}

void USoundManagerSubsystem::PlayBGM(const FString& AssetPath)
{
    if (AssetPath.IsEmpty())
    {
        UE_LOG(LogTemp, Warning, TEXT("AssetPath is empty!"));
        return;
    }

    // �A�Z�b�g�}�l�[�W���[�𗘗p���ăT�E���h�����[�h
    FStreamableManager& Streamable = UAssetManager::GetStreamableManager();
    USoundBase* Sound = Cast<USoundBase>(Streamable.LoadSynchronous(FSoftObjectPath(AssetPath)));

    if (Sound)
    {
        // ������ AudioComponent ������ꍇ�͒�~���Ĕj��
        if (BGMComponent && BGMComponent->IsPlaying())
        {
            BGMComponent->Stop();
            UE_LOG(LogTemp, Log, TEXT("Stopped previous BGM"));
        }

        // �V���� AudioComponent ���쐬���ăT�E���h��ݒ�
        if (!BGMComponent)
        {
            BGMComponent = NewObject<UAudioComponent>(this);
            BGMComponent->bAutoActivate = false;
            BGMComponent->bIsUISound = false;
            BGMComponent->bAllowSpatialization = false;
            BGMComponent->RegisterComponentWithWorld(GetWorld()); // �K�� World �ɓo�^
        }

        BGMComponent->SetSound(Sound);
        BGMComponent->Play();
        UE_LOG(LogTemp, Log, TEXT("BGM playback started: %s"), *Sound->GetName());
    }
    else
    {
        UE_LOG(LogTemp, Error, TEXT("Failed to load sound from path: %s"), *AssetPath);
    }
}

void USoundManagerSubsystem::StopBGM()
{
    if (BGMComponent && BGMComponent->IsPlaying())
    {
        BGMComponent->Stop();
        UE_LOG(LogTemp, Log, TEXT("Stopped BGM"));
    }
    else
    {
        UE_LOG(LogTemp, Warning, TEXT("BGM is not currently playing."));
    }
}

void USoundManagerSubsystem::SetBGMVolume(float Volume)
{
    if (BGMComponent)
    {
        BGMComponent->SetVolumeMultiplier(Volume);
    }
}

void USoundManagerSubsystem::PlaySoundFromPath(const FString& AssetPath)
{
    if (AssetPath.IsEmpty())
    {
        UE_LOG(LogTemp, Warning, TEXT("AssetPath is empty!"));
        return;
    }
    if (FPaths::FileExists(AssetPath))
    {
        UE_LOG(LogTemp, Error, TEXT("Invalid sound path: %s"), *AssetPath);
        return;
    }

    // �A�Z�b�g�}�l�[�W���[�𗘗p���ăT�E���h�����[�h
    FStreamableManager& Streamable = UAssetManager::GetStreamableManager();
    USoundBase* Sound = Cast<USoundBase>(Streamable.LoadSynchronous(FSoftObjectPath(AssetPath)));

    if (Sound)
    {
        if (AudioComponent)
        {
            AudioComponent->SetSound(Sound);
            AudioComponent->Play();
        }
        else
        {
            UGameplayStatics::PlaySound2D(GetWorld(), Sound);
        }
    }
    else
    {
        UE_LOG(LogTemp, Error, TEXT("Failed to load sound from path: %s"), *AssetPath);
    }
}
void USoundManagerSubsystem::PlaySoundFromPath(const FString& AssetPath,float _Volume)
{
    if (AssetPath.IsEmpty())
    {
        UE_LOG(LogTemp, Warning, TEXT("AssetPath is empty!"));
        return;
    }
    if (FPaths::FileExists(AssetPath))
    {
        UE_LOG(LogTemp, Error, TEXT("Invalid sound path: %s"), *AssetPath);
        return;
    }

    // �A�Z�b�g�}�l�[�W���[�𗘗p���ăT�E���h�����[�h
    FStreamableManager& Streamable = UAssetManager::GetStreamableManager();
    USoundBase* Sound = Cast<USoundBase>(Streamable.LoadSynchronous(FSoftObjectPath(AssetPath)));

    if (Sound)
    {
        if (AudioComponent)
        {
            AudioComponent->SetSound(Sound);
            AudioComponent->SetVolumeMultiplier(_Volume);  // ���ʒ���
            AudioComponent->Play();
        }
        else
        {
            UGameplayStatics::PlaySound2D(GetWorld(), Sound, _Volume);
        }
    }
    else
    {
        UE_LOG(LogTemp, Error, TEXT("Failed to load sound from path: %s"), *AssetPath);
    }
}