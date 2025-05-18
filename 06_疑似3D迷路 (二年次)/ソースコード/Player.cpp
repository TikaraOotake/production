#include "Player.h"
#include <conio.h>
#include <iostream>

Player::Player() :mp_mapMNG(nullptr)
{
	m_PlayerPos.x = 0;
	m_PlayerPos.y = 0;

	for (int y = 0; y < 4; ++y)
	{
		for (int x = 0; x < 3; ++x)
		{
			m_FirstPersonView[y][x] = 0;
		}
	}

	RandomTeleportPos();
}

Player::~Player()
{
}
vector2D Player::GetPlayerPos()
{
	return m_PlayerPos;
}
string Player::GetPlayerWay()
{
	return m_PlayerWay;
}
int Player::GetPlayerFPV(int x, int y)
{
	return m_FirstPersonView[y][x];
}
void Player::UpdatePlayer()
{
	//テスト用にFPVを出力
	for (int y = 0; y < 4; ++y)
	{
		for (int x = 0; x < 3; ++x)
		{
			if (m_FirstPersonView[y][x] == 1)
			{
				//cout << "1";
			}
			else
			{
				//cout << " ";
			}
		}
		//cout << endl;
	}

	PlayerMove();//Playerの移動
	UpdateFPV();//一人称視点の更新

}
void Player::SetPlayerPos(int _x, int _y)
{
	m_PlayerPos.x = _x;
	m_PlayerPos.y = _y;
}
void Player::SetPlayerPos(vector2D _pos)
{
	m_PlayerPos = _pos;
}
void Player::SetMapMNG(MapManager* _pMapMNG)
{
	mp_mapMNG = _pMapMNG;

	//マップが設定されたら再度一人称視点を更新
	if (mp_mapMNG != nullptr)
	{
		UpdateFPV();
	}
}
void Player::RandomTeleportPos()
{
	// 1から4までの乱数を作成
	int num = rand() % 4 + 1;
	//乱数からPlayerの向きを指定
	if (num == 1)
	{
		m_PlayerWay = "up";
	}
	else
		if (num == 2)
		{
			m_PlayerWay = "left";
		}
		else
			if (num == 3)
			{
				m_PlayerWay = "down";
			}
			else
				if (num == 4)
				{
					m_PlayerWay = "right";
				}
				else
				{

				}
	//移動先を乱数で指定
	vector2D pos;
	pos.x = rand() % 7 + 1;
	pos.x = (pos.x * 2) - 1;
	pos.y = rand() % 7 + 1;
	pos.y = (pos.y * 2) - 1;

	m_PlayerPos = pos;
}
void Player::UpdateFPV()
{
	//nullチェック
	if (mp_mapMNG == nullptr)
	{
		//cout << "UpdateFPV()を実行中...mp_mapMNGにポインターが設定されていません" << endl;
		//_getch();
		return;
	}

	for (int y = 0; y < 4; ++y)
	{
		for (int x = 0; x < 3; ++x)
		{
			vector2D searchPos;

			//基礎値(補正値を加えたもの)を代入
			searchPos.x = x - 1;
			searchPos.y = y - 3;

			//Playerの向きに応じて回転させる
			if (m_PlayerWay == "up")
			{
				//なにもしない
			}
			else
				if (m_PlayerWay == "left")
				{
					//変数を入れ替える
					int tempNum;//仮の変数
					tempNum = searchPos.x;
					searchPos.x = searchPos.y;
					searchPos.y = tempNum;

					//Y軸のみ符号を反転
					searchPos.y *= -1;
				}
				else
					if (m_PlayerWay == "down")
					{
						//符号を反転
						searchPos.x *= -1;
						searchPos.y *= -1;
					}
					else
						if (m_PlayerWay == "right")
						{
							//変数を入れ替える
							int tempNum;//仮の変数
							tempNum = searchPos.x;
							searchPos.x = searchPos.y;
							searchPos.y = tempNum;

							//X軸のみ符号を反転
							searchPos.x *= -1;
						}


			//補正値とプレイヤーの座標を入れてMapData内を検索
			m_FirstPersonView[y][x] = mp_mapMNG->GetMapData(searchPos.x + m_PlayerPos.x, searchPos.y + m_PlayerPos.y);
		}
	}
}
void Player::PlayerMove()
{
	char Input = _getch();
	//前
	if (Input == 'w' || Input == '8')
	{
		//Playerの方向に向けて前進
		PlayerGoWay(1);
	}
	//左
	if (Input == 'a' || Input == '4')
	{
		//Playerの向きを回転
		PlayerRotate(1);
	}
	//下
	if (Input == 's' || Input == '2' || Input == '5')
	{
		//Playerの方向に向けて後進
		PlayerGoWay(-1);
	}
	//右
	if (Input == 'd' || Input == '6')
	{
		//Playerの向きを回転
		PlayerRotate(-1);
	}


}
void Player::PlayerGoWay(int _Movement)
{
	//nullチェック
	if (mp_mapMNG == nullptr)
	{
		cout << "PlayerGoWay()を実行中...mp_mapMNGにポインターが設定されていません" << endl;
		_getch();
		return;
	}
	//移動前に未踏破エリアを更新
	mp_mapMNG->PassageRecord(m_PlayerPos);

	//Playerの方向に合わせて移動
	//もし移動先が壁だったら何もしない
	if (m_PlayerWay == "up")
	{
		if (mp_mapMNG->GetMapData(m_PlayerPos.x, m_PlayerPos.y - _Movement) != 1)
		{
			m_PlayerPos.y -= _Movement;
		}
	}
	if (m_PlayerWay == "left")
	{
		if (mp_mapMNG->GetMapData(m_PlayerPos.x - _Movement, m_PlayerPos.y) != 1)
		{
			m_PlayerPos.x -= _Movement;
		}
	}
	if (m_PlayerWay == "down")
	{
		if (mp_mapMNG->GetMapData(m_PlayerPos.x, m_PlayerPos.y + _Movement) != 1)
		{
			m_PlayerPos.y += _Movement;
		}
	}
	if (m_PlayerWay == "right")
	{
		if (mp_mapMNG->GetMapData(m_PlayerPos.x + _Movement, m_PlayerPos.y) != 1)
		{
			m_PlayerPos.x += _Movement;
		}
	}
	//移動が終わったのでマップの未踏破エリアを更新
	mp_mapMNG->PassageRecord(m_PlayerPos);
}
void Player::PlayerRotate(int _Rote)
{
	//Playerの向きを回転
	 //引数が+なら左回転-なら右回転

	//一度string型からint型に変換して回転させる
	int angle = 0;
	if (m_PlayerWay == "up")
	{
		angle = 0;
	}
	else
		if (m_PlayerWay == "left")
		{
			angle = 1;
		}
		else
			if (m_PlayerWay == "down")
			{
				angle = 2;
			}
			else
				if (m_PlayerWay == "right")
				{
					angle = 3;
				}


	//指定した方向に回転
	angle += _Rote;


	//はみ出した場合補正を掛ける
	if (angle < 0)
	{
		angle += 4;
	}
	if (angle > 3)
	{
		angle -= 4;
	}

	//string型に戻す
	if (angle == 0)
	{
		m_PlayerWay = "up";
	}
	else
		if (angle == 1)
		{
			m_PlayerWay = "left";
		}
		else
			if (angle == 2)
			{
				m_PlayerWay = "down";
			}
			else
				if (angle == 3)
				{
					m_PlayerWay = "right";
				}
}