#include "PopupUISubSystem.h"

#include "Kismet/GameplayStatics.h"
#include "Blueprint/UserWidget.h"

void UPopupUISubSystem::ShowPopupUI(TSubclassOf<UUserWidget> WidgetClass)
{
    if (!WidgetClass) return;

    // ���ł� UI ���\������Ă�����폜
    if (ActiveWidget)
    {
        ActiveWidget->RemoveFromParent();
        ActiveWidget = nullptr;
    }

    // ���[���h���擾
    UWorld* World = GetWorld();
    if (!World) return;

    // �v���C���[�� HUD ���擾
    APlayerController* PlayerController = UGameplayStatics::GetPlayerController(World, 0);
    if (!PlayerController) return;

    // �E�B�W�F�b�g���쐬
    ActiveWidget = CreateWidget<UUserWidget>(PlayerController, WidgetClass);
    if (ActiveWidget)
    {
        ActiveWidget->AddToViewport();
    }
}
void UPopupUISubSystem::ShowPopupUI(const FString& WidgetBlueprintPath)
{
	// ���łɕ\������Ă���ꍇ�͍폜
	if (ActiveWidget)
	{
		ActiveWidget->RemoveFromParent();
		ActiveWidget = nullptr;
	}

	// ���O�o�͂Ńf�o�b�O
	UE_LOG(LogTemp, Log, TEXT("ShowPopupUI called with path: %s"), *WidgetBlueprintPath);

	// �������^�� TSoftClassPtr �𐶐�
	TSoftClassPtr<UUserWidget> SoftWidgetClass = TSoftClassPtr<UUserWidget>(FSoftObjectPath(WidgetBlueprintPath));

	// LoadSynchronous() �𐳂����Ăяo��
	TSubclassOf<UUserWidget> WidgetClass = SoftWidgetClass.LoadSynchronous();

	// ���[�h�`�F�b�N
	if (!WidgetClass)
	{
		UE_LOG(LogTemp, Error, TEXT("Failed to load Widget: %s"), *WidgetBlueprintPath);
		return;
	}

	// GetWorld() �̈��S�Ȏ擾
	UWorld* World = GetGameInstance()->GetWorld();
	if (!World)
	{
		UE_LOG(LogTemp, Error, TEXT("Failed to get World"));
		return;
	}

	// �v���C���[�R���g���[���[�擾
	APlayerController* PlayerController = UGameplayStatics::GetPlayerController(World, 0);
	if (!PlayerController)
	{
		UE_LOG(LogTemp, Error, TEXT("Failed to get PlayerController"));
		return;
	}

	// �E�B�W�F�b�g����
	ActiveWidget = CreateWidget<UUserWidget>(PlayerController, WidgetClass);
	if (ActiveWidget)
	{
		ActiveWidget->AddToViewport();
		UE_LOG(LogTemp, Log, TEXT("Widget added to viewport successfully"));
	}
	else
	{
		UE_LOG(LogTemp, Error, TEXT("Failed to create widget"));
	}
}

void UPopupUISubSystem::HidePopupUI()
{
    if (ActiveWidget)
    {
        ActiveWidget->RemoveFromParent();
        ActiveWidget = nullptr;
    }
}

