#include "MapManager.h"
#include <iostream>

using namespace std;

MapManager::MapManager() :m_MapDataScale({ 15,15 })
{
	//要素数が多いためforループでMapDataを全て0で初期化
	ClearMapData();
}

MapManager::~MapManager()
{
}

void MapManager::GenerateMapData()
{
	//一度全てのマスを空白(0)にする
	ClearMapData();

	//壁を生成
	GenerateWall();

	//出口を生成
	GenerateDoor();

	//未踏破エリアとして通路を隠していきます
	GenerateBlind();

}
void MapManager::PassageRecord(vector2D pos)
{
	//そのマスが未踏破エリアであるかを確認
	if (m_MapData[pos.y][pos.x] == 3)
	{
		m_MapData[pos.y][pos.x] = 0;
	}
}

void MapManager::GenerateWall()
{
	srand((unsigned int)time(NULL)); // 現在時刻の情報で初期化

	//壁を作る方向を指定する
	//方向を記録する配列を作成
	string WallWayData[6][6] = {
		{"","","","","",""},
		{"","","","","",""},
		{"","","","","",""},
		{"","","","","",""},
		{"","","","","",""},
		{"","","","","",""},
	};
	for (int y = 0; y < 6; y++)
	{
		for (int x = 0; x < 6; x++)
		{
			// 1から4までの乱数を作成
			int num = rand() % 4 + 1;

			//0.0fから1.0fまでの乱数を作成
			float RandNum = ((float)(rand() % 1000)) / 1000.0f;
			//要素数の指定
			int FactorNum = 4;
			//序列の指定
			string Hierarchy[4] = {
				"up","left","down","right"
			};


			//二段目移行の場合upを要素から除外
			if (y != 0)
			{
				FactorNum -= 1;
				for (int i = 0; i < 4; ++i)
				{
					//指定されたものを見つけたら列を詰める
					if (Hierarchy[i] == "up")
					{
						for (int u = i; u < 3; ++u)
						{
							Hierarchy[u] = Hierarchy[u + 1];
						}
					}
				}
			}

			//左に壁が既にあった場合leftを要素から除外
			if (x != 0 && WallWayData[y][x - 1] == "right")
			{
				FactorNum -= 1;
				for (int i = 0; i < 4; ++i)
				{
					//指定されたものを見つけたら列を詰める
					if (Hierarchy[i] == "left")
					{
						for (int u = i; u < 3; ++u)
						{
							Hierarchy[u] = Hierarchy[u + 1];
						}
					}
				}
			}

			//確率を元に向きを指定
			if (1.0f / FactorNum >= RandNum)
			{
				WallWayData[y][x] = Hierarchy[0];
			}
			else
				if (2.0f / FactorNum >= RandNum)
				{
					WallWayData[y][x] = Hierarchy[1];
				}
				else
					if (3.0f / FactorNum >= RandNum)
					{
						WallWayData[y][x] = Hierarchy[2];
					}
					else
						if (4.0f / FactorNum >= RandNum || true)
						{
							WallWayData[y][x] = Hierarchy[3];
						}

		}
	}
	//いったん出来た方向を出力してみる 成功しました
	/*for (int y = 0; y < 6; y++)
		{
			for (int x = 0; x < 6; x++)
			{
				if (WallWayData[y][x] == "up")
				{
					cout << "↑";
				}
				if (WallWayData[y][x] == "left")
				{
					cout << "←";
				}
				if (WallWayData[y][x] == "down")
				{
					cout << "↓";
				}
				if (WallWayData[y][x] == "right")
				{
					cout << "→";
				}

			}
			cout << endl;
		}
	*/

	//m_MapDataに外壁を生成していきます
	for (int y = 0; y < m_MapDataScale.y; y++)
	{
		for (int x = 0; x < m_MapDataScale.x; x++)
		{
			//上下の壁
			if (y == 0 || y == m_MapDataScale.y - 1)
			{
				m_MapData[y][x] = 1;
			}
			//左右の壁
			if (x == 0 || x == m_MapDataScale.x - 1)
			{
				m_MapData[y][x] = 1;
			}
		}
	}

	//m_MapDataに柱を生成していきます
	for (int y = 0; y < m_MapDataScale.y; y++)
	{
		for (int x = 0; x < m_MapDataScale.x; x++)
		{
			//xyどちらも偶数であるならば
			if (x % 2 == 0 && y % 2 == 0)
			{
				m_MapData[y][x] = 1;
			}
		}
	}

	//m_MapDataに柱の横に可変的な壁を生成していきます
	for (int y = 0; y < 6; y++)
	{
		for (int x = 0; x < 6; x++)
		{
			if (WallWayData[y][x] == "up")
			{
				m_MapData[y * 2 + 1][x * 2 + 2] = 1;
			}
			else
				if (WallWayData[y][x] == "left")
				{
					m_MapData[y * 2 + 2][x * 2 + 1] = 1;
				}
				else
					if (WallWayData[y][x] == "down")
					{
						m_MapData[y * 2 + 3][x * 2 + 2] = 1;
					}
					else
						if (WallWayData[y][x] == "right")
						{
							m_MapData[y * 2 + 2][x * 2 + 3] = 1;
						}
		}
	}
}


void MapManager::GenerateDoor()
{
	srand((unsigned int)time(NULL)); // 現在時刻の情報で初期化

	int PosX = 0;
	int PosY = 0;

	//外壁に扉を設置する位置を決める
	if (rand() % 2 == 1)
	{
		PosX = rand() % 7 + 1;
		PosX = (PosX * 2) - 1;
		if (rand() % 2 == 1)
		{
			//確率で反対側に設置する
			PosY = m_MapDataScale.y - 1;
		}
	}
	else
	{
		PosY = rand() % 7 + 1;
		PosY = (PosY * 2) - 1;
		if (rand() % 2 == 1)
		{
			//確率で反対側に設置する
			PosX = m_MapDataScale.x - 1;
		}
	}

	//設置座標を探し代入
	for (int y = 0; y < m_MapDataScale.y; y++)
	{
		for (int x = 0; x < m_MapDataScale.x; x++)
		{
			if (y == PosY && x == PosX)
			{
				m_MapData[y][x] = 2;
				//cout << "扉に書き換えました" << endl;
			}
		}
	}
	//cout << "扉設置座標X" << PosX << "Y" << PosY << endl;
}
void MapManager::GenerateBlind()
{
	//m_MapData内の全ての空白マスを未踏破マスに塗り替える
	for (int y = 0; y < m_MapDataScale.y; y++)
	{
		for (int x = 0; x < m_MapDataScale.x; x++)
		{
			if (m_MapData[y][x] == 0)
			{
				m_MapData[y][x] = 3;
			}

		}
	}
}
void MapManager::ClearMapData()
{
	//m_MapData内の全てを空白マスに塗り替える
	for (int y = 0; y < m_MapDataScale.y; y++)
	{
		for (int x = 0; x < m_MapDataScale.x; x++)
		{
			m_MapData[y][x] = 0;

		}
	}
}

void MapManager::ShowMapDateTest()
{
	for (int y = 0; y < m_MapDataScale.y; y++)
	{
		for (int x = 0; x < m_MapDataScale.x; x++)
		{
			if (m_MapData[y][x] == 0)
			{
				cout << "  ";
			}
			else
				if (m_MapData[y][x] == 1)
				{
					cout << "壁";
				}
				else
					if (m_MapData[y][x] == 2)
					{
						cout << "口";
					}
					else
						if (m_MapData[y][x] == 3)
						{
							cout << "闇";
						}
						else
						{
							//該当するものがない場合は空白
							cout << "  ";
						}

		}
		cout << endl;
	}
}
int MapManager::GetMapData(vector2D pos)
{
	int id = 1;
	//配列の範囲内かチェック
	if (pos.x >= 0 && pos.x <= 14 && pos.y >= 0 && pos.y <= 14)
	{
		id = m_MapData[pos.y][pos.x];
	}
	return id;
}
int MapManager::GetMapData(int x, int y)
{
	int id = 1;
	//配列の範囲内かチェック
	if (x >= 0 && x <= 14 && y >= 0 && y <= 14)
	{
		id = m_MapData[y][x];
	}
	return id;
}