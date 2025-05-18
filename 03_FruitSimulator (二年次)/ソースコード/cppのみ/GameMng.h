#pragma once
#include<functional>
#include <list>

#include <iostream>

#include "GameObject.h"

#include <chrono>//デルタタイムを扱う為
using namespace std;
using namespace std::chrono;
/*//前方宣言
class GameObject;
*/


//Game管理クラス
class GameMng
{
private:
	GameMng();

	static GameMng* g_instance;

	

public:
	static void GenerateInstance();
	static void DestroyInstance();
	static GameMng* GetInstance();

	
public:
	//publicが先
	~GameMng();

	void Init();
	void Update();
	void Render();
	void Term();//←終了処理の関数名

	void Render_GameObjects();
	
	float GetDeltaTime();

	//GameObjectを生成する
	GameObject* GenerateGameObject(float _x, float _y);
	bool DestroyGameObject(GameObject* pGameObject);//指定されたGameObjectを削除
	void DestroyAllGameObject_Tag(string _GameTag);//指定されたタグを持つオブジェクトを全て削除
	template <typename T>
	void DestroyAllGameObject_Class();//指定されたクラスを持つオブジェクトを全て削除

	//Fruitを生成する
	GameObject* GenerateFruit(float _x, float _y, int _Level);
	int GetGameObjectCount();

	//リスト内に指定されたオブジェクトが存在するかチェック
	bool CheckExistObjectList(GameObject* _pGameObejct);

	void SetOnDestroyFunction(std::function<void(GameObject*)>& _func);

	void CheckGameOver();

	
	void Start_GameTitle();
	void Start_GamePlay();
	void Start_GameOver();
	

	//アクセサ
	void SetScore(int _Score);
	void AddScore(int _FruitLevel);

	void SetGameObjectList(GameObject* _pGameObject);

	//privateが後
private:

	std::list<GameObject*> m_pGameObjectList;//ゲームオブジェクトのポインタ管理
	std::list<GameObject*> m_pDeleteList;//削除予定のポインタ管理

	float m_GameOverLine;//ゲームオーバーになる高さ
	float m_GameOverTimer;
	float m_GameOverTime;
	bool m_IsGameOver;

	int m_Score;//得点

	string m_GameMode;

	void ClearDeleteList();
	void CheckHoldingPointer(GameObject* _pDeleteObject);

	static bool CompareByValue(GameObject* _pGameObject1, GameObject* _pGameObject2);

	void SceneGameTitle();
	void SceneGamePlay();
	void SceneGameOver();

	std::function<void(GameObject*)> m_onDestroy;

	steady_clock::time_point lastTime;
};

