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

	//void Update();//ViewManager�̍X�V
	void DrawFPV();//��l�̎��_��`��

	void SetPlayer(Player* _player);
	void DrawGridIndex(int _GridID);
	void DrawGridIndex(string _Way);
	void DrawFakeLoading();

	void SettingDraw();

private:
	//���\�b�h
	void LoadingTextureData(string FileName);
	void SetTextureBoard();

	// �J�[�\���̈ʒu���w�肷�� //�ǂ��ɒu���΂������킩��Ȃ����߂����Ɉꎞ�I�ɒu��
	void moveCursorTo(int x, int y) {
		cout << "\033[" << y << ";" << x << "H" << flush;
	}

	//�v���p�e�B
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
	//int m_TextureBoard13[35][35];//Player���g�̏ꏊ�Ȃ̂Ŏg��Ȃ�
	int m_TextureBoard23[35][35];


	int m_BoardFPV[35][35];//��l�̎��_�p��Board
	int m_tempTextureBoard[35][35];//����TextureBoard

	int m_SettingDrawNum;//

	Player* player;
};
