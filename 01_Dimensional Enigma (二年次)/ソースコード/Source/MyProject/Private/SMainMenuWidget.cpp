#include "SMainMenuWidget.h"
#include "SlateOptMacros.h"
#include "Kismet/GameplayStatics.h"
#include "Engine/Texture2D.h"
#include "Widgets/SWidget.h"  // �K�v�ȃC���N���[�h
#include "Widgets/Text/STextBlock.h"
#include "Widgets/Input/SButton.h"
#include "Framework/Application/SlateApplication.h"
#include "InputCoreTypes.h"  // EKeys ���g�����߂ɕK�v

#include "SoundManagerSubsystem.h"

BEGIN_SLATE_FUNCTION_BUILD_OPTIMIZATION
void SMainMenuWidget::Construct(const FArguments& InArgs)
{
    // �t�H�[�J�X��ݒ�
    FSlateApplication::Get().SetKeyboardFocus(AsShared());

    // �w�i�摜�̐ݒ�
    BackgroundBrush.SetResourceObject(LoadObject<UTexture2D>(nullptr, TEXT("/Game/Texture/GameTitle.GameTitle")));

    // �t�H���g�ݒ�
    FSlateFontInfo FontInfo = FSlateFontInfo(FPaths::EngineContentDir() / TEXT("Slate/Fonts/Roboto-Bold.ttf"), 32 * 1);

    MenuOptions = {
        SNew(STextBlock)
        .Text(FText::FromString("Tutorial"))
        .Font(FontInfo)
        .ShadowOffset(FVector2D(2.0f, 2.0f))
        .ShadowColorAndOpacity(FLinearColor::Black),

        SNew(STextBlock)
        .Text(FText::FromString("Stage Select"))
        .Font(FontInfo)
        .ShadowOffset(FVector2D(2.0f, 2.0f))
        .ShadowColorAndOpacity(FLinearColor::Black),

        SNew(STextBlock)
        .Text(FText::FromString("End Game"))
        .Font(FontInfo)
        .ShadowOffset(FVector2D(2.0f, 2.0f))
        .ShadowColorAndOpacity(FLinearColor::Black)
    };

    ChildSlot
        [
            SNew(SOverlay)
                + SOverlay::Slot()
                [
                    // �w�i�摜
                    SNew(SImage).Image(&BackgroundBrush)
                ]
                + SOverlay::Slot()
                .HAlign(HAlign_Center)
                .VAlign(VAlign_Top) // ���j���[�̊����񂹂ɕύX
                .Padding(FMargin(0, 650, 0, 0)) // �S�̂�����300�s�N�Z�����炷
                [
                    // ���j���[
                    SNew(SVerticalBox)
                        + SVerticalBox::Slot()
                        .Padding(10)
                        .AutoHeight()
                        [
                            MenuOptions[0].ToSharedRef()
                        ]
                        + SVerticalBox::Slot()
                        .Padding(10)
                        .AutoHeight()
                        [
                            MenuOptions[1].ToSharedRef()
                        ]
                        + SVerticalBox::Slot()
                        .Padding(10)
                        .AutoHeight()
                        [
                            MenuOptions[2].ToSharedRef()
                        ]
                ]
        ];

    HighlightOption(0);

    // FSlateApplication���g����GameInstance���擾����
    UGameInstance* GameInstance = GetGameInstance();
    if (GameInstance)
    {
        UE_LOG(LogTemp, Log, TEXT("GameInstance is valid!"));
    }
    else
    {
        UE_LOG(LogTemp, Warning, TEXT("GameInstance is not valid."));
    }
}
END_SLATE_FUNCTION_BUILD_OPTIMIZATION

FReply SMainMenuWidget::OnKeyDown(const FGeometry& MyGeometry, const FKeyEvent& InKeyEvent)
{
    const FKey Key = InKeyEvent.GetKey();

    if (Key == EKeys::W) // ��ړ�
    {
        HighlightOption((SelectedIndex - 1 + MenuOptions.Num()) % MenuOptions.Num());
        return FReply::Handled();
    }
    else if (Key == EKeys::S) // ���ړ�
    {
        HighlightOption((SelectedIndex + 1) % MenuOptions.Num());
        return FReply::Handled();
    }
    else if (Key == EKeys::Enter) // ����
    {
        HandleSelection();
        return FReply::Handled();
    }

    return SCompoundWidget::OnKeyDown(MyGeometry, InKeyEvent);
}

void SMainMenuWidget::HighlightOption(int32 NewIndex)
{
    // ���݂̑I�����ڂ̐F�����Z�b�g
    MenuOptions[SelectedIndex]->SetColorAndOpacity(FLinearColor::White);

    // �V�����I�����ڂ������\��
    SelectedIndex = NewIndex;
    MenuOptions[SelectedIndex]->SetColorAndOpacity(FLinearColor::Yellow);
}

void SMainMenuWidget::HandleSelection()
{
    switch (SelectedIndex)
    {
    case 0: // Start Game
        UE_LOG(LogTemp, Log, TEXT("New Game Selected"));
        NewGame();
        break;

    case 1: // Game Setting
        UE_LOG(LogTemp, Log, TEXT("Start Game Selected"));
        StartGame();
        break;

    case 2: // End Game
        UE_LOG(LogTemp, Log, TEXT("End Game Selected"));
        EndGame();
        break;

    default:
        break;
    }
}

void SMainMenuWidget::StartGame()
{
    if (UWorld* World = GetCurrentWorld())
    {

        // ���x����J��
        FString LevelName = TEXT("WorldSelect"); // �K�؂ȃ��x�������w��
        UGameplayStatics::OpenLevel(World, FName(*LevelName));

        // ���x���ڍs��Ƀ^�C�g����ʂ��폜
        if (GEngine && GEngine->GameViewport)
        {
            GEngine->GameViewport->RemoveViewportWidgetContent(AsShared());
        }
    }
    UE_LOG(LogTemp, Warning, TEXT("Enter key is pressed"));
}
void SMainMenuWidget::NewGame()
{
    if (UWorld* World = GetCurrentWorld())
    {
        // ���x����J��
        FString LevelName = TEXT("stage_1_0"); // �K�؂ȃ��x�������w��
        UGameplayStatics::OpenLevel(World, FName(*LevelName));

        // ���x���ڍs��Ƀ^�C�g����ʂ��폜
        if (GEngine && GEngine->GameViewport)
        {
            GEngine->GameViewport->RemoveViewportWidgetContent(AsShared());
        }
    }
    UE_LOG(LogTemp, Warning, TEXT("Enter key is pressed"));
}
void SMainMenuWidget::EndGame()
{
    if (GIsEditor)
    {
        UE_LOG(LogTemp, Warning, TEXT("EndGame called in Editor mode. Skipping request to exit."));

        UWorld* World = GetCurrentWorld();
        if (!World)
        {
            UE_LOG(LogTemp, Error, TEXT("World is null!"));
            return;
        }

        APlayerController* PlayerController = UGameplayStatics::GetPlayerController(World, 0);
        if (PlayerController)
        {
            // �Q�[�����I���iPIE ���[�h�̏ꍇ�̓Z�b�V�����I���j
            UKismetSystemLibrary::QuitGame(World, PlayerController, EQuitPreference::Quit, false);
        }

        return;
    }
    if (!GEngine)
    {
        UE_LOG(LogTemp, Warning, TEXT("GEngine is null!"));
        return;
    }
    if (GEngine && GEngine->GameViewport)
    {
        GEngine->GameViewport->RemoveAllViewportWidgets();
    }

    FGenericPlatformMisc::RequestExit(false);
}

void SMainMenuWidget::GameSetting()
{
    // �ݒ��ʂ̃��W�b�N
}
void SMainMenuWidget::Tick(const FGeometry& AllottedGeometry, const double InCurrentTime, const float InDeltaTime)
{
    CheckForStickMovement();//���X�e�B�b�N�̓��͂��Ď�

    //�X�V����O�̓��͒l���L�^����
    bool IsLStickUpSignal_old = IsLStickUpSignal;
    bool IsLStickDownSignal_old = IsLStickDownSignal;
    // �v���C���[�R���g���[���[���擾
    APlayerController* PlayerController = Cast<APlayerController>(GetCurrentWorld()->GetFirstPlayerController());
    if (PlayerController)
    {
        float StickX, StickY;
        // ���X�e�B�b�N�̏�Ԃ��擾
        PlayerController->GetInputAnalogStickState(EControllerAnalogStick::CAS_LeftStick, StickX, StickY);

        // Y���i�㉺�j�̒l�����O�ɏo��
        if (StickY != 0.0f)
        {
            UE_LOG(LogTemp, Log, TEXT("Left Stick Y: %f"), StickY);
        }
        if (StickY >= 0.5f)
        {
            IsLStickUpSignal = true;
        }
        else
        {
            IsLStickUpSignal = false;
        }
        if (StickY <= -0.5f)
        {
            IsLStickDownSignal = true;
        }
        else
        {
            IsLStickDownSignal = false;
        }
    }
    else
    {
        IsLStickUpSignal = false;
        IsLStickDownSignal = false;
    }

    if (GetCurrentWorld()->GetFirstPlayerController()->WasInputKeyJustPressed(EKeys::W) ||
        IsStickMoveRecently == 1 ||
        bIsStickMovedUpRecently == true ||
        (IsLStickUpSignal == true && IsLStickUpSignal_old == false)||
        PlayerController->WasInputKeyJustPressed(EKeys::Gamepad_DPad_Up))
    {
        HighlightOption((SelectedIndex - 1 + MenuOptions.Num()) % MenuOptions.Num());
    }
    if (GetCurrentWorld()->GetFirstPlayerController()->WasInputKeyJustPressed(EKeys::S) ||
        IsStickMoveRecently == -1 ||
        (IsLStickDownSignal == true && IsLStickDownSignal_old == false)||
        PlayerController->WasInputKeyJustPressed(EKeys::Gamepad_DPad_Down))
    {
        HighlightOption((SelectedIndex + 1 + MenuOptions.Num()) % MenuOptions.Num());
    }
    if (GetCurrentWorld()->GetFirstPlayerController()->WasInputKeyJustPressed(EKeys::Enter) ||
        GetCurrentWorld()->GetFirstPlayerController()->WasInputKeyJustPressed(EKeys::Gamepad_FaceButton_Right))
    {
        HandleSelection();
    }

    if (GetCurrentWorld()->GetFirstPlayerController()->WasInputKeyJustPressed(EKeys::Q))
    {
        //BGM���Đ�
        USoundManagerSubsystem* SoundManager = GetGameInstance()->GetSubsystem<USoundManagerSubsystem>();
        if (SoundManager)
        {
            //����BGM�Đ�
            //SoundManager->PlayBGM(TEXT("/Game/Sound/TestBGM.TestBGM"));

            UE_LOG(LogTemp, Warning, TEXT("PlayBGM"))
        }
    }
    if (GetCurrentWorld()->GetFirstPlayerController()->WasInputKeyJustPressed(EKeys::E))
    {
        //BGM���Đ�
        USoundManagerSubsystem* SoundManager = GetGameInstance()->GetSubsystem<USoundManagerSubsystem>();
        if (SoundManager)
        {
            //����BGM�Đ�
            //SoundManager->StopBGM();

            UE_LOG(LogTemp, Warning, TEXT("StopBGM"))
        }
    }
}
UWorld* SMainMenuWidget::GetCurrentWorld() const
{
    if (GEngine && GEngine->GameViewport)
    {
        return GEngine->GameViewport->GetWorld();
    }
    return nullptr;
}

/*+// A�{�^��(�C�O�ł́Z)�ŃA�N�V�������N����
UWorld* SMainMenuWidget::PressedAction()
{

    //UKismetSystemLibrary::PrintString(this, TEXT("Action Pressed"), true, true, FColor::Cyan, 10.0f, TEXT("None"));

}
*/


void  SMainMenuWidget::CheckForStickMovement()
{
    // �Q�[���p�b�h�̍��X�e�B�b�NY���̓��͒l���擾
    float CurrentStickValue = GetCurrentWorld()->GetFirstPlayerController()->GetInputAxisValue("Gamepad_LeftY");

    // ���X�e�B�b�N��������i���̕����j�ɓ|���ꂽ�u�Ԃ����o
    if (CurrentStickValue > Threshold && LastStickValue <= Threshold)
    {
        bIsStickMovedUpRecently = true;
        IsStickMoveRecently = 1;
    }
    else
        if (CurrentStickValue < -Threshold && LastStickValue >= -Threshold)
        {
            bIsStickMovedUpRecently = false;
            IsStickMoveRecently = -1;
        }
    else
    {
        bIsStickMovedUpRecently = false;
        IsStickMoveRecently = 0;
    }

    // �O��̃X�e�B�b�N�̏�Ԃ�ۑ�
    LastStickValue = CurrentStickValue;
}