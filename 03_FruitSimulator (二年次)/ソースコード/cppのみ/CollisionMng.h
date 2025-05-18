#pragma once

#include<list>
#include<vector>

//前方宣言
class ICollision;

//コリジョン管理クラス
class CollisionMng
{
private:
	CollisionMng();
	static CollisionMng* g_instance;

public:
	static void GenerateInstance();
	static void DestroyInstance();
	static CollisionMng* GetInstance();

public:
	//publicが先
	~CollisionMng();

	void Init();
	void Process();
	void Term();//←終了処理の関数名

	//登録関数
	void Register(ICollision* pCollision);
	//登録解除
	void Unregister(ICollision* pCollision);

	//privateが後
private:

	std::vector<ICollision*> m_pCollisionsVector;
};
