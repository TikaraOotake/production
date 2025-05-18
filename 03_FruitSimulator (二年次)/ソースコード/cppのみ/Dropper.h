#pragma once//インクルードガード（多重読み込みを防ぐ）

#include "GameObject.h"



//前方宣言
class GameMng;

class Dropper :public GameObject
{
public:
	Dropper();
	~Dropper();

	void Init()override;
	void Update()override;
	void Render()override;
	void Term()override;

	//アクセサ
	int GetNextFruitLevel();

	void SetFallingFruit(GameObject* _pGameObject);
	GameObject* GetFallingFruit();
private:
	void UpdateSpritePosition()override;

	Sprite m_FruitSprite;
	Texture m_FruitTexture;
	int m_HoldFruitLevel;//保持中のフルーツレベル
	int m_NextFruitLevel;//次に補填されるフルーツレベル
	float m_ReloadTimer;//補填までの時間を計る
	float m_ReloadTime;//補填までの時間
	
	GameObject* m_pNextFruitSpriteBoard;//Nextを表示するためのオブジェクト
	GameObject* m_pNextFrameSpriteBoard;//Nextを表示する枠

	GameMng* m_pGameMng;
	GameObject* m_pFallingFruit;//落下中のフルーツ
};

