#pragma once//インクルードガード（多重読み込みを防ぐ）


#include "GameObject.h"

class Fruit :public GameObject
{
public:
	Fruit();
	~Fruit();

	void Init() override;
	void Update() override;

	void OnOverlapped(ICollision* _pCollision) override;

	//アクセサ
	void SetLevel(int _Level);
	int GetLevel();

	bool GetIsLanding();

private:
	int m_Level;

	bool m_StaticFlag;
	bool m_IsLanding;//接地フラグ
};

