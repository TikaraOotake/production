#include"GameMng.h"
#include<Fwk/Framework.h>
#include "GameObject.h"
#include "Fruit.h"
#include "Bottle.h"
#include "SquareGround.h"
#include "Dropper.h"
#include "AppDef.h"
#include "TextureFilePathLibrary.h"

#include <iostream>
#include <random>

#include <cstdlib>  // rand() 関数を使うために必要
#include <ctime>    // time() 関数を使うために必要



//クラス唯一のインスタンスのアドレスを保持する
GameMng* GameMng::g_instance = nullptr;

//クラス唯一のインスタンスの生成
void GameMng::GenerateInstance() {
	if (g_instance != nullptr) { return; }
	g_instance = new GameMng();
}
//クラス唯一のインスタンスの破棄
void GameMng::DestroyInstance() {
	APP_SAFE_DELETE(g_instance);
}
//クラス唯一のインスタンスを取得する
GameMng* GameMng::GetInstance() {
	return g_instance;
}

GameMng::GameMng() :
	lastTime(steady_clock::now()),
	m_GameOverLine(64.0f * 5.0f),
	m_GameOverTime(1.5f),
	m_GameOverTimer(0.0f),
	m_IsGameOver(false),
	m_Score(0),
	m_GameMode("")
{
	// 現在の時刻を乱数のシードとして設定
	std::srand(std::time(0));
}
GameMng::~GameMng() {
	Term();
}
void GameMng::Init() {

	//GameObjectが倒された時に呼び返される関数
	function<void(GameObject*)> func = [&](GameObject* pGameObject) {
		m_onDestroy(pGameObject);
		};

	//Start_GamePlay();
	Start_GameTitle();
}
void GameMng::Update() {
	//削除予定のものを削除
	ClearDeleteList();

	if (m_GameMode == "GameTitle")
	{
		SceneGameTitle();
	}
	else
		if (m_GameMode == "GamePlay")
		{
			SceneGamePlay();
		}
		else
			if (m_GameMode == "GameOver")
			{
				SceneGameOver();
			}

	
	

	for (GameObject* obj : m_pGameObjectList) {
		//非アクティブなGameObjectは何も処理しない
		if (!obj->IsActive())
		{
			continue;
		}

		//アクティブなGameObjectだけ更新
		obj->Update();
	}
	
	
}
void GameMng::Render() {
	//ゲームオブジェクトの表示
	Render_GameObjects();
	if (m_GameMode == "GameTitle")
	{
		PrintText("PRESS ENTER", 400.0f, 600.0f);
	}
	if (m_GameMode == "GamePlay")
	{
		string text = "Score : " + to_string(m_Score);
		PrintText(text, 0.0f, 0.0f);
		PrintText("AD    移動", 0.0f, 80.0f);
		PrintText("Space 投下", 0.0f, 120.0f);
		PrintText("R     リセット", 0.0f, 200.0f);
	}
	if (m_GameMode == "GameOver")
	{
		string text = "Score : " + to_string(m_Score);
		PrintText(text, 0.0f, 0.0f);
		PrintText("PRESS ENTER", 400.0f, 600.0f);
	}
}
void GameMng::Term() {
	if (m_pGameObjectList.size() == 0)
	{
		//要素が1つもなかったら終了
		return;
	}
	for (auto it = m_pGameObjectList.begin(); it != m_pGameObjectList.end(); ) {
		GameObject* ptr = *it;
		delete ptr;
		ptr = nullptr;
		it = m_pGameObjectList.erase(it); // リストからポインタを削除
	}
}
void GameMng::Render_GameObjects()
{
	//表示順に並び変える
	std::list<GameObject*> _pGameObjectList_Priority = m_pGameObjectList;
	_pGameObjectList_Priority.sort(CompareByValue);

	if (_pGameObjectList_Priority.size() == 0)
	{
		//要素が1つもなかったら終了
		return;
	}
	for (GameObject* obj : _pGameObjectList_Priority) {
		if (!obj->IsActive())
		{
			//continue;
		}
		//アクティブなGameObjectだけ更新
		obj->Render();
	}
}
float GameMng::GetDeltaTime()
{
	steady_clock::time_point currentTime = steady_clock::now();
	duration<float> deltaTime = duration_cast<duration<float>>(currentTime - lastTime);
	lastTime = currentTime;
	return deltaTime.count();
}
GameObject* GameMng::GenerateGameObject(float _x, float _y) {

	GameObject* pGameObject = new GameObject;//オブジェクトを生成
	pGameObject->SetPosition(_x, _y);
	pGameObject->SetActive(true);
	m_pGameObjectList.push_back(pGameObject);//リストに登録

	// 乱数生成器の準備
	std::random_device rd; // ハードウェア乱数生成器を使う
	std::mt19937 gen(rd()); // メルセンヌ・ツイスタのエンジン
	std::uniform_real_distribution<> dis(10.0, 30.0); //範囲で乱数を生成
	// ランダムな浮動小数点数を生成して返す
	pGameObject->SetMass((float)dis(gen));
	//mp_enemies[i].SetMass(1.0f);


	float Nam = ((float)dis(gen));
	pGameObject->Init();
	pGameObject->SetRadius(Nam);
	Vector2f tempSize(Nam * 2.285714285714286f, Nam * 2.285714285714286f);
	pGameObject->SetSpriteSize(tempSize);
	
	return pGameObject;
}

GameObject* GameMng::GenerateFruit(float _x, float _y ,int _Level)
{
	Fruit* pGameObject = new Fruit;//オブジェクトを生成
	pGameObject->SetPosition(_x, _y);
	pGameObject->SetActive(true);
	pGameObject->SetLevel(_Level);
	pGameObject->Init();

	float Diameter = 64.0f + 64 * _Level * 0.25f;

	Vector2f Size(Diameter, Diameter);
	pGameObject->SetSpriteSize(Size);
	pGameObject->SetRadius(Diameter / 2.0f);
	pGameObject->SetMass(Diameter * Diameter * 3.1415295f);
	pGameObject->SetSpritePos(_x, _y);


	m_pGameObjectList.push_back(pGameObject);//リストに登録

	return pGameObject;
}

int GameMng::GetGameObjectCount() {
	int cnt = 0;
	for (GameObject* obj : m_pGameObjectList)
	{
		if (obj->IsActive())
		{
			++cnt;
		}
	}
	return cnt;
}
bool GameMng::CheckExistObjectList(GameObject* _pGameObejct)
{
	if (m_pGameObjectList.size() == 0 || _pGameObejct == nullptr)
	{
		// 要素が1つもないか引数がnullだったら終了
		return false;
	}

	for (auto it = m_pGameObjectList.begin(); it != m_pGameObjectList.end(); ) {
		GameObject* pGameObject = *it;
		if (pGameObject == _pGameObejct)
		{
			// 一致したため有効性を確認
			return true;
		}
		++it; //次の要素に進む
	}

	DebugLog("指定されたオブジェクトがリスト内に見つかりませんでした");
	return false;
}

bool GameMng::DestroyGameObject(GameObject* pGameObject)
{
	if (m_pGameObjectList.size() == 0)
	{
		//要素が1つもなかったら終了
		return false;
	}
	for (auto it = m_pGameObjectList.begin(); it != m_pGameObjectList.end(); ) {
		if (*it == pGameObject) {
			//誰かが削除するクラスのポインターを取得したままでないかチェック
			CheckHoldingPointer(pGameObject);

			// 一致するポインタを削除
			pGameObject->Term();
			//削除予定の物としてリストに登録(呼び出し元のクラスが自身を削除する場合に対応するため)
			m_pDeleteList.push_back(*it);

			it = m_pGameObjectList.erase(it); // リストから削除

			std::cout << "対象のクラスを削除しました\n";
			return true; // 一致するものを削除したら終了
		}
		else {
			++it; // 次の要素へ
		}
	}
	std::cout << "一致するクラスが見つかりませんでした\n";

	return false;
}
void GameMng::DestroyAllGameObject_Tag(string _GameTag)
{
	if (m_pGameObjectList.size() == 0)
	{
		// 要素が1つもなかったら終了
		return;
	}

	for (auto it = m_pGameObjectList.begin(); it != m_pGameObjectList.end(); ) {
		GameObject* pGameObject = *it;
		if (pGameObject->GetGameTag() == _GameTag)
		{
			// 一致するポインタを削除
			pGameObject->Term();

			// 削除予定の物としてリストに登録
			m_pDeleteList.push_back(*it);

			// リストから削除し、次の要素を指すイテレータを取得
			it = m_pGameObjectList.erase(it);  // erase は次の要素を指すイテレータを返す
			std::cout << "対象のクラスを削除しました\n";
		}
		else {
			++it; // 一致しない場合のみ次の要素に進む
		}
	}
}
template <typename T>
void GameMng::DestroyAllGameObject_Class() {
	if (m_pGameObjectList.empty()) {
		return;
	}

	for (auto it = m_pGameObjectList.begin(); it != m_pGameObjectList.end(); ) {
		GameObject* pGameObject = *it;
		if (dynamic_cast<T*>(pGameObject)) {  // `T` の型にキャストできるか判定
			pGameObject->Term();
			m_pDeleteList.push_back(*it);
			it = m_pGameObjectList.erase(it);
			std::cout << "対象のクラスを削除しました\n";
		}
		else {
			++it;
		}
	}
}

void GameMng::ClearDeleteList()
{
	if (m_pDeleteList.size() == 0)
	{
		//要素が1つもなかったら終了
		return;
	}
	for (auto it = m_pDeleteList.begin(); it != m_pDeleteList.end(); ) {
		GameObject* ptr = *it;

		//誰かが削除するクラスのポインターを取得したままでないかチェック
		CheckHoldingPointer(ptr);

		delete ptr;
		ptr = nullptr;
		it = m_pDeleteList.erase(it); // リストからポインタを削除
		DebugLog("領域を解放しました");
	}
}
void GameMng::SetOnDestroyFunction(std::function<void(GameObject*) >& _func)
{
	m_onDestroy = _func;
}
void GameMng::CheckGameOver()
{
	bool Result = false;
	for (GameObject* obj : m_pGameObjectList)
	{
		Fruit* pFruit = dynamic_cast<Fruit*>(obj);
		if (pFruit)//フルーツだったら
		{
			//ゲームオーバーラインを超えているか？
			if (pFruit->GetPosition().y + pFruit->GetRadius() >= m_GameOverLine)
			{
				Result = true;
				break;
			}
		}
	}

	//フルーツがラインを超えていたら
	if (Result)
	{
		float DeltaTime = GetDeltaTime();
		//デルタタイムが異常に大きい場合は補正する
		if (DeltaTime > 0.03f)
		{
			DeltaTime = 0.03f;
		}
		m_GameOverTimer += DeltaTime;

		if (m_GameOverTimer >= m_GameOverTime)
		{
			m_IsGameOver = true;
		}
	}
	else
	{
		m_GameOverTimer = 0.0f;
	}
}
void GameMng::CheckHoldingPointer(GameObject* _pDeleteObject)
{
	//削除予定のクラスを誰かが保持したままでないか調べる
	if (m_pGameObjectList.size() == 0)
	{
		// 要素が1つもなかったら終了
		return;
	}

	for (auto it = m_pGameObjectList.begin(); it != m_pGameObjectList.end(); ) {
		GameObject* pGameObject = *it;
		Dropper* pDropper = dynamic_cast<Dropper*>(pGameObject);
		if (pDropper)
		{
			if (_pDeleteObject == pDropper->GetFallingFruit())
			{
				DebugLog("保持していたのでnullを入れます");
				pDropper->SetFallingFruit(nullptr);
			}
		}
		++it; // 一致しない場合のみ次の要素に進む
	}
}

bool GameMng::CompareByValue(GameObject* _pGameObject1, GameObject* _pGameObject2)
{
	return _pGameObject1->GetDisplayPriority() < _pGameObject2->GetDisplayPriority();
}
void GameMng::SceneGameTitle()
{
	if (Input_I->IsKeyDown(VK_RETURN))
	{
		Start_GamePlay();
	}
}
void GameMng::SceneGamePlay()
{
	if (Input_I->IsKeyDown(VK_SPACE) && false)
	{
		DebugLog("ぬ？");

		GenerateGameObject(0.0f, 300.0f);
	}
	if (Input_I->IsKeyDown('F'))
	{
		// 0から5の範囲でランダムな値を生成
		int randomValue = std::rand() % 6;

		GenerateFruit(0.0f, 300.0f, randomValue);
	}
	if (Input_I->IsKeyDown('R'))
	{
		DestroyAllGameObject_Tag("Fruit");
		m_Score = 0;
	}
	if (Input_I->IsKeyDown('G'))
	{
		DebugLog("リスト内を開示");
		for (GameObject* obj : m_pGameObjectList)
		{
			cout << obj << ":";
			if (obj->IsActive())
			{
				cout << "true";
			}
			else
			{
				cout << "false";
			}
			cout << endl;
		}
	}

	CheckGameOver();
	if (m_IsGameOver == true)
	{
		//全てのFruitを削除
		//DestroyAllGameObject_Tag("Fruit");
		//m_IsGameOver = false;
		//m_Score = 0;

		//Sceneをゲームオーバーに
		Start_GameOver();
	}
}
void GameMng::SceneGameOver()
{
	if (Input_I->IsKeyDown(VK_RETURN))
	{
		Start_GameTitle();
	}
}

void GameMng::Start_GameTitle()
{
	m_GameMode = "GameTitle";

	//全てのGameObjectを削除
	DestroyAllGameObject_Class<GameObject>();

	m_IsGameOver = false;
	m_Score = 0;

	TextureFilePathLibrary Library;

	//背景画像生成
	GameObject* pBackGround = new GameObject;
	pBackGround->SetGameTag("BackGround");
	pBackGround->SetDisplayPriority(-10.0f);
	pBackGround->SetActive(false);
	pBackGround->SetTextureFilePath(Library.GetTextureFilePath(3, "Background"));
	pBackGround->SetSpriteSize(64.0f * 18.0f, 64.0f * 15.0f);
	m_pGameObjectList.push_back(pBackGround);//リストに登録

	//タイトル画像生成
	GameObject* pTitleLogo = new GameObject;
	pTitleLogo->SetGameTag("TitleLogo");
	pTitleLogo->SetDisplayPriority(0.0f);
	pTitleLogo->SetActive(false);
	pTitleLogo->SetTextureFilePath(Library.GetTextureFilePath(5, "UI"));
	pTitleLogo->SetSpriteSize(64.0f * 10.0f, 64.0f * 10.0f);
	m_pGameObjectList.push_back(pTitleLogo);//リストに登録
}
void GameMng::Start_GamePlay()
{
	m_GameMode = "GamePlay";
	//全てのGameObjectを削除
	DestroyAllGameObject_Class<GameObject>();

	TextureFilePathLibrary Library;

	//背景画像生成
	GameObject* pSpriteBoard = new GameObject;
	pSpriteBoard->SetGameTag("BackGround");
	pSpriteBoard->SetDisplayPriority(-10.0f);
	pSpriteBoard->SetActive(false);
	pSpriteBoard->SetTextureFilePath(Library.GetTextureFilePath(3, "Background"));
	pSpriteBoard->SetSpriteSize(64.0f * 18.0f, 64.0f * 15.0f);
	m_pGameObjectList.push_back(pSpriteBoard);//リストに登録

	//背景画像生成
	GameObject* pBottle2 = new GameObject;
	pBottle2->SetGameTag("BackGround");
	pBottle2->SetDisplayPriority(-1.0f);
	pBottle2->SetActive(false);
	pBottle2->SetTextureFilePath(Library.GetTextureFilePath(9, "Background"));
	pBottle2->SetPosition(0.0f , 64.0f);
	pBottle2->SetSpriteSize(64.0f * 10.0f, 64.0f * 15.0f);
	m_pGameObjectList.push_back(pBottle2);//リストに登録

	GameObject* pDeadLine = new GameObject;
	pDeadLine->SetGameTag("BackGround");
	pDeadLine->SetDisplayPriority(-1.0f);
	pDeadLine->SetActive(false);
	pDeadLine->SetTextureFilePath(Library.GetTextureFilePath(1, "Temp"));
	pDeadLine->SetPosition(0.0f, 64.0f*5.0f);
	pDeadLine->SetSpriteSize(64.0f * 8.0f, 1.0f);
	m_pGameObjectList.push_back(pDeadLine);//リストに登録


	
	SquareGround* pSquareGround = new SquareGround;//オブジェクトを生成
	pSquareGround->SetPosition(0.0f, 64.0f * -5.0f - 32.0f);
	pSquareGround->SetSpritePos(pSquareGround->GetPosition());
	pSquareGround->SetActive(true);
	pSquareGround->SetSize(64.0f * 9.0f, 64.0f);
	pSquareGround->SetSpriteSize(0.0f, 0.0f);
	m_pGameObjectList.push_back(pSquareGround);//リストに登録

	SquareGround* pSquareGround2 = new SquareGround;//オブジェクトを生成
	pSquareGround2->SetPosition(64.0f * -4.0f, 0.0f);
	pSquareGround2->SetSpritePos(pSquareGround2->GetPosition());
	pSquareGround2->SetActive(true);
	pSquareGround2->SetSize(64.0f, 64.0f * 100.0f);
	pSquareGround2->SetSpriteSize(0.0f,0.0f);
	m_pGameObjectList.push_back(pSquareGround2);//リストに登録

	SquareGround* pSquareGround3 = new SquareGround;//オブジェクトを生成
	pSquareGround3->SetPosition(64.0f * 4.0f, 0.0f);
	pSquareGround3->SetSpritePos(pSquareGround3->GetPosition());
	pSquareGround3->SetActive(true);
	pSquareGround3->SetSize(64.0f, 64.0f * 100.0f);
	pSquareGround3->SetSpriteSize(0.0f, 0.0f);
	m_pGameObjectList.push_back(pSquareGround3);//リストに登録

	Dropper* pDropper = new Dropper;
	m_pGameObjectList.push_back(pDropper);//リストに登録
}
void GameMng::Start_GameOver()
{
	m_GameMode = "GameOver";

	//プレイヤーを削除
	DestroyAllGameObject_Tag("Dropper");

	//テストで足場を削除
	//DestroyAllGameObject_Class<SquareGround>();

	TextureFilePathLibrary Library;//ファイルパスを取得するため

	//ゲームオーバー画像を生成
	GameObject* pSpriteBoard = new GameObject;
	pSpriteBoard->SetGameTag("SpriteBoard");
	pSpriteBoard->SetDisplayPriority(10.0f);
	pSpriteBoard->SetActive(false);
	pSpriteBoard->SetPosition(0.0f, 0.0f);
	pSpriteBoard->SetTextureFilePath(Library.GetTextureFilePath(4, "UI"));
	pSpriteBoard->SetSpriteSize(64.0f * 8.0f, 64.0f * 5.0f);
	m_pGameObjectList.push_back(pSpriteBoard);//リストに登録
}
void GameMng::SetScore(int _Score)
{
	m_Score = _Score;
}
void GameMng::AddScore(int _FruitLevel)
{
	m_Score += (_FruitLevel * (_FruitLevel + 1)) / 2;
}
void GameMng::SetGameObjectList(GameObject* _pGameObject)
{
	if (_pGameObject == nullptr) {
		DebugLog("おいおいヌルを入れようとするな！！");
		return;
	}
	m_pGameObjectList.push_back(_pGameObject);
}