#pragma once

#include "CoreMinimal.h"
#include "Subsystems/GameInstanceSubsystem.h"
#include "SoundManagerSubsystem.generated.h"

class USoundBase;
class UAudioComponent;

UCLASS()
class MYPROJECT_API USoundManagerSubsystem : public UGameInstanceSubsystem
{
    GENERATED_BODY()

public:
    // ���ʉ��̍Đ�
    UFUNCTION(BlueprintCallable, Category = "Sound")
    void PlaySoundEffect(USoundBase* Sound, FVector Location);

    // BGM�̍Đ�
    //UFUNCTION(BlueprintCallable, Category = "Sound")
    //void PlayBGM(USoundBase* BGM);
    UFUNCTION(BlueprintCallable, Category = "Sound")
    void PlayBGM(const FString& AssetPath);

    // BGM�̒�~
    UFUNCTION(BlueprintCallable, Category = "Sound")
    void StopBGM();

    // �{�����[���̐ݒ�
    UFUNCTION(BlueprintCallable, Category = "Sound")
    void SetBGMVolume(float Volume);

    void PlaySoundFromPath(const FString& AssetPath);
    void PlaySoundFromPath(const FString& AssetPath, float _Volume);

protected:
    virtual void Initialize(FSubsystemCollectionBase& Collection) override;
    virtual void Deinitialize() override;

private:
    // BGM�p�̃I�[�f�B�I�R���|�[�l���g
    UPROPERTY()
    UAudioComponent* BGMComponent;

    UPROPERTY()
    UAudioComponent* AudioComponent;
};
