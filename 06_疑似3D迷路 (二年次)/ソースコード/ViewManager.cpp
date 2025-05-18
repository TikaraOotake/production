#include "ViewManager.h"
#include <conio.h>
#include <iostream>
#include <fstream>
#include <windows.h>
//#include "CursorControl.h"



ViewManager::ViewManager() :m_SettingDrawNum(0)
{
	SetTextureBoard();
}

ViewManager::~ViewManager()
{
}
void ViewManager::DrawFakeLoading()
{
	cout << "Loading naw";
	Sleep(1000);
	cout << ".";
	Sleep(1000);
	cout << ".";
	Sleep(1000);
	cout << ".";
	Sleep(1000);
	//��ʃN���A
	system("cls");
}
void ViewManager::SettingDraw()
{

	//�܂�����ɂ��Ă���
	for (int y = 0; y < 35; ++y)
	{
		for (int x = 0; x < 35; ++x)
		{
			m_BoardFPV[y][x] = 0;
		}
	}

	for (int i = 0; i < 8; ++i)
	{
		int xFPV;
		int yFPV;
		switch (i) {
		case 0:
			xFPV = 0;
			yFPV = 0;
			break;
		case 1:
			xFPV = 2;
			yFPV = 0;
			break;
		case 2:
			xFPV = 0;
			yFPV = 1;
			break;
		case 3:
			xFPV = 2;
			yFPV = 1;
			break;
		case 4:
			xFPV = 0;
			yFPV = 2;
			break;
		case 5:
			xFPV = 2;
			yFPV = 2;
			break;
		case 6:
			xFPV = 0;
			yFPV = 3;
			break;
		case 7:
			xFPV = 2;
			yFPV = 3;
			break;
		default:
			break;
		}
		for (int y = 0; y < 35; ++y) {
			for (int x = 0; x < 35; ++x) {
				if (yFPV == 0) {
					if (xFPV == 0) {
						if (m_TextureBoard00[y][x] != 0)
						{
							m_BoardFPV[y][x] = m_TextureBoard00[y][x];
						}
					}
					if (xFPV == 2) {
						if (m_TextureBoard20[y][x] != 0)
						{
							m_BoardFPV[y][x] = m_TextureBoard20[y][x];
						}
					}
				}
				if (yFPV == 1) {
					if (xFPV == 0) {
						if (m_TextureBoard01[y][x] != 0)
						{
							m_BoardFPV[y][x] = m_TextureBoard01[y][x];
						}
					}
					if (xFPV == 2) {
						if (m_TextureBoard21[y][x] != 0)
						{
							m_BoardFPV[y][x] = m_TextureBoard21[y][x];
						}
					}
				}
				if (yFPV == 2) {
					if (xFPV == 0) {
						if (m_TextureBoard02[y][x] != 0)
						{
							m_BoardFPV[y][x] = m_TextureBoard02[y][x];
						}
					}
					if (xFPV == 2) {
						if (m_TextureBoard22[y][x] != 0)
						{
							m_BoardFPV[y][x] = m_TextureBoard22[y][x];
						}
					}
				}
				if (yFPV == 3) {
					if (xFPV == 0) {
						if (m_TextureBoard03[y][x] != 0)
						{
							m_BoardFPV[y][x] = m_TextureBoard03[y][x];
						}
					}
					if (xFPV == 2) {
						if (m_TextureBoard23[y][x] != 0)
						{
							m_BoardFPV[y][x] = m_TextureBoard23[y][x];
						}
					}
				}
			}
		}
	}

	int Input = 1;
	if (m_SettingDrawNum == 0) {
		m_SettingDrawNum = 1;
	}

	do {

		//��ʃN���A
		//system("cls");

		moveCursorTo(1, 1);  // ����ɃJ�[�\�����ړ�

		//�`��
		moveCursorTo(1, 1);  // ����ɃJ�[�\�����ړ�
		for (int y = 0; y < 35; ++y) {
			for (int x = 0; x < 35; ++x) {
				if (m_BoardFPV[y][x] == 0 || m_BoardFPV[y][x] == 2)
				{
					DrawGridIndex(0);
				}
				if (m_BoardFPV[y][x] == 1)
				{
					DrawGridIndex(1);
				}
			}
			//�[�ɃS�~���c�邱�Ƃ�����̂œh��Ԃ�
			cout << "                                        ";
			cout << endl;
		}


		cout << "�`��Z�b�g�A�b�v���J�n���܂� ���\������Ă�����͍̂��E�Ώ̂ɂȂ��Ă��܂����H" << endl;
		cout << "1��2�������ĕ`��ɖ�肪�Ȃ����0�������Ă�������" << endl;
		cout << "�������܂̓��͒l�F" << m_SettingDrawNum << endl;

		//�`�悷��
		cout << flush;

		//�L���Ȑ������������܂ŌJ��Ԃ�
		do {
			Input = _getch();
		} while (!(Input == '0' || Input == '1' || Input == '2'));



		if (Input == '1')
		{
			m_SettingDrawNum = 1;
		}
		else
			if (Input == '2')
			{
				m_SettingDrawNum = 2;
			}


		//��ʃN���A
		//system("cls");

	} while (Input != '0');

	cout << "�ݒ肪�������܂���" << endl;
	cout << "�܂��Ȃ��Q�[�����n�܂�܂�" << endl;
	cout << "Press any key to start !" << endl;

	_getch();

	//���o
	DrawFakeLoading();
}
void ViewManager::DrawGridIndex(int _GridID)
{
	if (_GridID == 0)
	{
		if (m_SettingDrawNum == 1)
		{
			cout << "�@";//�S�p
		}
		if (m_SettingDrawNum == 2)
		{
			cout << "  ";//���p
		}
	}
	else
		if (_GridID == 1 || _GridID == 3)
		{
			if (m_SettingDrawNum == 1)
			{
				cout << "��";//�S�p
			}
			if (m_SettingDrawNum == 2)
			{
				cout << "����";//���p
			}
		}
		else
			if (_GridID == 2)
			{
				if (m_SettingDrawNum == 1)
				{
					cout << "��";//�S�p
				}
				if (m_SettingDrawNum == 2)
				{
					cout << "��";//���p
				}
			}
}
void ViewManager::DrawGridIndex(string _Way)
{
	//�����������Ȃ�v���C���[��\��
	if (_Way == "up")
	{
		if (m_SettingDrawNum == 1)
		{
			cout << "��";//�S�p
		}
		if (m_SettingDrawNum == 2)
		{
			cout << "����";//���p
		}
	}
	else
		if (_Way == "left")
		{
			if (m_SettingDrawNum == 1)
			{
				cout << "��";//�S�p
			}
			if (m_SettingDrawNum == 2)
			{
				cout << "����";//���p
			}
		}
		else
			if (_Way == "down")
			{
				if (m_SettingDrawNum == 1)
				{
					cout << "��";//�S�p
				}
				if (m_SettingDrawNum == 2)
				{
					cout << "����";//���p
				}
			}
			else
				if (_Way == "right")
				{
					if (m_SettingDrawNum == 1)
					{
						cout << "��";//�S�p
					}
					if (m_SettingDrawNum == 2)
					{
						cout << "����";//���p
					}
				}

}
void ViewManager::LoadingTextureData(string FileName)
{
	ifstream inputFile;
	inputFile.open(FileName, ios::in);
	if (inputFile.fail())
	{
		cout << FileName << "�t�@�C���I�[�v�����s" << endl;
		_getch();
	}
	else
	{
		//cout << "�t�@�C���I�[�v������" << endl;

		for (int y = 0; y < 35; ++y)
		{
			for (int x = 0; x < 35; ++x)
			{
				inputFile >> m_tempTextureBoard[y][x];
			}
		}
		//�t�@�C�������
		inputFile.close();
	}
}
void ViewManager::SetTextureBoard()
{
	for (int y = 0; y < 35; ++y)
	{
		for (int x = 0; x < 35; ++x)
		{
			m_tempTextureBoard[y][x] = 0;
		}
	}

	//��i��
	LoadingTextureData("Texture/Wall/00.txt");
	for (int y = 0; y < 35; ++y) {
		for (int x = 0; x < 35; ++x) {
			m_TextureBoard00[y][x] = m_tempTextureBoard[y][x];
		}
	}
	LoadingTextureData("Texture/Wall/10.txt");
	for (int y = 0; y < 35; ++y) {
		for (int x = 0; x < 35; ++x) {
			m_TextureBoard10[y][x] = m_tempTextureBoard[y][x];
		}
	}
	LoadingTextureData("Texture/Wall/20.txt");
	for (int y = 0; y < 35; ++y) {
		for (int x = 0; x < 35; ++x) {
			m_TextureBoard20[y][x] = m_tempTextureBoard[y][x];
		}
	}

	//��i��
	LoadingTextureData("Texture/Wall/01.txt");
	for (int y = 0; y < 35; ++y) {
		for (int x = 0; x < 35; ++x) {
			m_TextureBoard01[y][x] = m_tempTextureBoard[y][x];
		}
	}
	LoadingTextureData("Texture/Wall/11.txt");
	for (int y = 0; y < 35; ++y) {
		for (int x = 0; x < 35; ++x) {
			m_TextureBoard11[y][x] = m_tempTextureBoard[y][x];
		}
	}
	LoadingTextureData("Texture/Wall/21.txt");
	for (int y = 0; y < 35; ++y) {
		for (int x = 0; x < 35; ++x) {
			m_TextureBoard21[y][x] = m_tempTextureBoard[y][x];
		}
	}

	//�O�i��
	LoadingTextureData("Texture/Wall/02.txt");
	for (int y = 0; y < 35; ++y) {
		for (int x = 0; x < 35; ++x) {
			m_TextureBoard02[y][x] = m_tempTextureBoard[y][x];
		}
	}
	LoadingTextureData("Texture/Wall/12.txt");
	for (int y = 0; y < 35; ++y) {
		for (int x = 0; x < 35; ++x) {
			m_TextureBoard12[y][x] = m_tempTextureBoard[y][x];
		}
	}
	LoadingTextureData("Texture/Wall/22.txt");
	for (int y = 0; y < 35; ++y) {
		for (int x = 0; x < 35; ++x) {
			m_TextureBoard22[y][x] = m_tempTextureBoard[y][x];
		}
	}

	//�l�i��
	LoadingTextureData("Texture/Wall/03.txt");
	for (int y = 0; y < 35; ++y) {
		for (int x = 0; x < 35; ++x) {
			m_TextureBoard03[y][x] = m_tempTextureBoard[y][x];
		}
	}
	//LoadingTextureData("aaa"); Player�̈ʒu�̂��ߎg��Ȃ�
	//for (int y = 0; y < 35; ++y) {
	//	for (int x = 0; x < 35; ++x) {
	//		m_TextureBoard13[y][x] = m_tempTextureBoard[y][x];
	//	}
	//}
	LoadingTextureData("Texture/Wall/23.txt");
	for (int y = 0; y < 35; ++y) {
		for (int x = 0; x < 35; ++x) {
			m_TextureBoard23[y][x] = m_tempTextureBoard[y][x];
		}
	}
}
void ViewManager::DrawFPV()
{
	//�܂�����ɂ��Ă���
	for (int y = 0; y < 35; ++y)
	{
		for (int x = 0; x < 35; ++x)
		{
			m_BoardFPV[y][x] = 0;
		}
	}

	for (int i = 0; i < 11; ++i)
	{
		int xFPV;
		int yFPV;
		switch (i) {
		case 0:
			xFPV = 0;
			yFPV = 0;
			break;
		case 1:
			xFPV = 2;
			yFPV = 0;
			break;
		case 2:
			xFPV = 1;
			yFPV = 0;
			break;
		case 3:
			xFPV = 0;
			yFPV = 1;
			break;
		case 4:
			xFPV = 2;
			yFPV = 1;
			break;
		case 5:
			xFPV = 1;
			yFPV = 1;
			break;
		case 6:
			xFPV = 0;
			yFPV = 2;
			break;
		case 7:
			xFPV = 2;
			yFPV = 2;
			break;
		case 8:
			xFPV = 1;
			yFPV = 2;
			break;
		case 9:
			xFPV = 0;
			yFPV = 3;
			break;
		case 10:
			xFPV = 2;
			yFPV = 3;
			break;
		default:
			break;
		}
		for (int y = 0; y < 35; ++y) {
			for (int x = 0; x < 35; ++x) {
				if (yFPV == 0) {
					if (xFPV == 0) {
						if (player->GetPlayerFPV(xFPV, yFPV) == 1)
						{
							if (m_TextureBoard00[y][x] != 0)
							{
								m_BoardFPV[y][x] = m_TextureBoard00[y][x];
							}
						}
					}
					if (xFPV == 1) {
						if (player->GetPlayerFPV(xFPV, yFPV) == 1)
						{
							if (m_TextureBoard10[y][x] != 0)
							{
								m_BoardFPV[y][x] = m_TextureBoard10[y][x];
							}

						}
					}
					if (xFPV == 2) {
						if (player->GetPlayerFPV(xFPV, yFPV) == 1)
						{
							if (m_TextureBoard20[y][x] != 0)
							{
								m_BoardFPV[y][x] = m_TextureBoard20[y][x];
							}
						}
					}
				}
				if (yFPV == 1) {
					if (xFPV == 0) {
						if (player->GetPlayerFPV(xFPV, yFPV) == 1)
						{
							if (m_TextureBoard01[y][x] != 0)
							{
								m_BoardFPV[y][x] = m_TextureBoard01[y][x];
							}
						}
					}
					if (xFPV == 1) {
						if (player->GetPlayerFPV(xFPV, yFPV) == 1)
						{
							if (m_TextureBoard11[y][x] != 0)
							{
								m_BoardFPV[y][x] = m_TextureBoard11[y][x];
							}

						}
					}
					if (xFPV == 2) {
						if (player->GetPlayerFPV(xFPV, yFPV) == 1)
						{
							if (m_TextureBoard21[y][x] != 0)
							{
								m_BoardFPV[y][x] = m_TextureBoard21[y][x];
							}

						}
					}
				}
				if (yFPV == 2) {
					if (xFPV == 0) {
						if (player->GetPlayerFPV(xFPV, yFPV) == 1)
						{
							if (m_TextureBoard02[y][x] != 0)
							{
								m_BoardFPV[y][x] = m_TextureBoard02[y][x];
							}
						}
					}
					if (xFPV == 1) {
						if (player->GetPlayerFPV(xFPV, yFPV) == 1)
						{
							if (m_TextureBoard12[y][x] != 0)
							{
								m_BoardFPV[y][x] = m_TextureBoard12[y][x];
							}
						}
					}
					if (xFPV == 2) {
						if (player->GetPlayerFPV(xFPV, yFPV) == 1)
						{
							if (m_TextureBoard22[y][x] != 0)
							{
								m_BoardFPV[y][x] = m_TextureBoard22[y][x];
							}
						}
					}
				}
				if (yFPV == 3) {
					if (xFPV == 0) {
						if (player->GetPlayerFPV(xFPV, yFPV) == 1)
						{
							if (m_TextureBoard03[y][x] != 0)
							{
								m_BoardFPV[y][x] = m_TextureBoard03[y][x];
							}
						}
					}
					//if (xFPV == 1) {
					//	if (player->GetPlayerFPV(xFPV, yFPV) == 1)
					//	{
					//		m_BoardFPV[y][x] = m_TextureBoard13[y][x];
					//	}
					//}
					if (xFPV == 2) {
						if (player->GetPlayerFPV(xFPV, yFPV) == 1)
						{
							if (m_TextureBoard23[y][x] != 0)
							{
								m_BoardFPV[y][x] = m_TextureBoard23[y][x];
							}
						}
					}
				}
			}
		}
	}
	//�`��
	for (int y = 0; y < 35; ++y) {
		for (int x = 0; x < 35; ++x) {
			if (m_BoardFPV[y][x] == 0 || m_BoardFPV[y][x] == 2)
			{
				DrawGridIndex(0);
			}
			if (m_BoardFPV[y][x] == 1)
			{
				DrawGridIndex(1);
			}
		}
		cout << endl;
	}

}
void ViewManager::SetPlayer(Player* _player)
{
	player = _player;
}

