#pragma once
#include "Vector2D.h"

class MapManager
{
public:
	MapManager();
	~MapManager();

	//メソッド
	void GenerateMapData();//Mapに地形情報を作成
	void ShowMapDateTest();//Map情報を試験的に出力
	void PassageRecord(vector2D pos);//通行済としてm_MapDataに記録
	int GetMapData(vector2D pos);//地形情報を取得
	int GetMapData(int x, int y);

private:
	//メソッド
	void ClearMapData();//Map情報をリセット
	void GenerateWall();//壁を生成
	void GenerateDoor();//扉を生成
	void GenerateBlind();//未踏破エリアとして通路を塗りつぶし

	//プロパティ
	const vector2D   m_MapDataScale;//m_MapDataの大きさを保存
	int m_MapData[15][15];//地形情報
};
