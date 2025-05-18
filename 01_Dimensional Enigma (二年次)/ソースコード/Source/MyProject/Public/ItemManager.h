// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Subsystems/WorldSubsystem.h"
#include "ItemBase.h"
#include "ItemManager.generated.h"

/**
 * 
 */
UCLASS()
class UItemManager : public UWorldSubsystem
{
	GENERATED_BODY()
	
public:

	//���������ɌĂяo�����֐�
	virtual void Initialize(FSubsystemCollectionBase& Collection) override;

	//�I�����ɌĂяo�����֐�
	virtual void Deinitialize() override;

	//---------------------------------------------------
	//�I���W�i�����\�b�h
	//---------------------------------------------------

	void RegisterItem(AItemBase* _item);

	void OnGetItem(AItemBase* _item);

	void GenerateItemWidget();

	void RemoveItemWidget();

	TArray<FString> GetItemTexturePath();

	FString GetItemString();

	//---------------------------------------------------
	//�v���p�e�B
	//---------------------------------------------------

private:

	TArray<AItemBase*> m_itemArray;

	UTexture2D* m_itemTexture_on;

	UTexture2D* m_itemTexture_off;

	UTexture2D* m_itemTexture_empty;

	UUserWidget* m_itemWidget;

	enum ItemStatus
	{
		none,
		idle,
		get,
	};
	TArray<ItemStatus> m_itemStatusArray;

	int m_maxItemCount;

	int m_getItemCount;
};
