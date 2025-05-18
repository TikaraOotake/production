
#include "TitleGameMode.h"
#include "Kismet/GameplayStatics.h"
#include "SMainMenuWidget.h"
#include "InGameHUD.h"
#include "InGamePlayerController.h"

ATitleGameMode::ATitleGameMode()
{
    DefaultPawnClass = nullptr; // �����������ꂽPawn�𖳌���
    HUDClass = AInGameHUD::StaticClass();
    PlayerControllerClass = AInGamePlayerController::StaticClass();
}
void ATitleGameMode::BeginPlay()
{
    Super::BeginPlay();

    // ���݂̃��x�������擾
    FString CurrentLevelName = UGameplayStatics::GetCurrentLevelName(this, true);

    // �^�C�g����ʂ̂ݕ\��
    if (CurrentLevelName == "GameTitle")
    {
        if (!MainMenuWidget.IsValid())
        {
            MainMenuWidget = SNew(SMainMenuWidget);
            if (GEngine && GEngine->GameViewport)
            {
                GEngine->GameViewport->AddViewportWidgetContent(MainMenuWidget.ToSharedRef());
            }
        }
    }
    else
    {
        RemoveTitleScreen();
    }
}

void ATitleGameMode::OnLevelChanged(UWorld* LoadedWorld)
{
    if (LoadedWorld && LoadedWorld->GetName() != "GameTitle")
    {
        RemoveTitleScreen();
    }
}

void ATitleGameMode::RemoveTitleScreen()
{
    if (MainMenuWidget.IsValid() && GEngine && GEngine->GameViewport)
    {
        GEngine->GameViewport->RemoveViewportWidgetContent(MainMenuWidget.ToSharedRef());
        MainMenuWidget.Reset(); // ���������
    }
}