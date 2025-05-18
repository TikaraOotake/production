#include "App.h"

#include "CollisionMng.h"
#include "GameMng.h"
#include "GameObject.h"


//---------------------------------------------------------
//ゲームの初期化処理を行う関数
//---------------------------------------------------------
void App::Init() {
	//コリジョンマネージャインスタンスの生成
	CollisionMng::GenerateInstance();
	//コリジョンマネージャの初期化
	CollisionMng::GetInstance()->Init();

	//ゲームマネージャーインスタンスの生成
	GameMng::GenerateInstance();
	//コリジョンマネージャの初期化
	GameMng::GetInstance()->Init();


}

//---------------------------------------------------------
//ゲームの更新処理を行う関数
//---------------------------------------------------------
void App::Update() {
//衝突処理実行
	CollisionMng::GetInstance()->Process();
	CollisionMng::GetInstance()->Process();
	CollisionMng::GetInstance()->Process();
	CollisionMng::GetInstance()->Process();
	//ゲームマネージャーの更新
	GameMng::GetInstance()->Update();

	
}

//---------------------------------------------------------
//ゲームの描画処理を行う関数
//---------------------------------------------------------
void App::Render() {

	//オブジェクトの描画
	GameMng::GetInstance()->Render();
}
//---------------------------------------------------------
// ゲームの後片付け処理を行う関数
//---------------------------------------------------------
void App::Term() {

	//コリジョンマネージャインスタンスの破棄
	CollisionMng::DestroyInstance();

	//ゲームマネージャインスタンスの破棄
	GameMng::DestroyInstance();
}
