

#pragma once

#include "CoreMinimal.h"
//#include "Subsystems/GameInstanceSubsystem.h"
#include "Blueprint/UserWidget.h"
#include "PopupUISubSystem.generated.h"

UCLASS()
class MYPROJECT_API UPopupUISubSystem : public UGameInstanceSubsystem
{
    GENERATED_BODY()

public:
    // �|�b�v�A�b�vUI��\��
    UFUNCTION(BlueprintCallable, Category = "PopupUI")
    void ShowPopupUI(TSubclassOf<UUserWidget> WidgetClass);
public:
    // �|�b�v�A�b�vUI��\���i�t�@�C���p�X�w��j
    //UFUNCTION(BlueprintCallable, Category = "PopupUI")
    void ShowPopupUI(const FString& WidgetBlueprintPath);
    // �|�b�v�A�b�vUI���\��
    UFUNCTION(BlueprintCallable, Category = "PopupUI")
    void HidePopupUI();

private:
    // �\������ UI �E�B�W�F�b�g
    UPROPERTY()
    UUserWidget* ActiveWidget = nullptr;

    TSubclassOf<UUserWidget> m_PopupWidget;
};