#pragma once
#include "Vector2D.h"
#include "Player.h"
#include "MapManager.h"
#include <string>

#include <conio.h>
#include <iostream>

using namespace std;


class ViewManager
{
public:
	ViewManager();
	~ViewManager();

	//void Update();//ViewManagerの更新
	void DrawFPV();//一人称視点を描画

	void SetPlayer(Player* _player);
	void DrawGridIndex(int _GridID);
	void DrawGridIndex(string _Way);
	void DrawFakeLoading();

	void SettingDraw();

private:
	//メソッド
	void LoadingTextureData(string FileName);
	void SetTextureBoard();

	// カーソルの位置を指定する //どこに置けばいいかわからないためここに一時的に置く
	void moveCursorTo(int x, int y) {
		cout << "\033[" << y << ";" << x << "H" << flush;
	}

	//プロパティ
	int m_TextureBoard00[35][35];
	int m_TextureBoard10[35][35];
	int m_TextureBoard20[35][35];

	int m_TextureBoard01[35][35];
	int m_TextureBoard11[35][35];
	int m_TextureBoard21[35][35];

	int m_TextureBoard02[35][35];
	int m_TextureBoard12[35][35];
	int m_TextureBoard22[35][35];

	int m_TextureBoard03[35][35];
	//int m_TextureBoard13[35][35];//Player自身の場所なので使わない
	int m_TextureBoard23[35][35];


	int m_BoardFPV[35][35];//一人称視点用のBoard
	int m_tempTextureBoard[35][35];//仮のTextureBoard

	int m_SettingDrawNum;//

	Player* player;
};
