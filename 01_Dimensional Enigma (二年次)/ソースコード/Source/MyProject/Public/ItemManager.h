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

	//初期化時に呼び出される関数
	virtual void Initialize(FSubsystemCollectionBase& Collection) override;

	//終了時に呼び出される関数
	virtual void Deinitialize() override;

	//---------------------------------------------------
	//オリジナルメソッド
	//---------------------------------------------------

	void RegisterItem(AItemBase* _item);

	void OnGetItem(AItemBase* _item);

	void GenerateItemWidget();

	void RemoveItemWidget();

	TArray<FString> GetItemTexturePath();

	FString GetItemString();

	//---------------------------------------------------
	//プロパティ
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
