#pragma once
#include "Vector2D.h"
#include "MapManager.h"
#include <string>
using namespace std;

class Player
{
public:
	Player();
	~Player();

	//メソッド
	vector2D GetPlayerPos();
	string GetPlayerWay();
	int GetPlayerFPV(int x, int y);
	void SetPlayerPos(int _x, int _y);
	void SetPlayerPos(vector2D _pos);
	void SetMapMNG(MapManager* _pMapMNG);

	void RandomTeleportPos();//ランダムな位置へPlayerを移動



	void UpdatePlayer();//Playerの更新

private:
	//メソッド
	void UpdateFPV();//一人称視点の更新
	void PlayerMove();//Playerの移動
	void PlayerGoWay(int _Movement);//Playerの前進後進
	void PlayerRotate(int _Rote);//Playerの回転

	//プロパティ
	vector2D m_PlayerPos;
	string m_PlayerWay;
	MapManager* mp_mapMNG;//MapManagerのポインターを保持

	int m_FirstPersonView[4][3];//一人称視点での地形情報

};
