#include "MapManager.h"
#include <iostream>

using namespace std;

MapManager::MapManager() :m_MapDataScale({ 15,15 })
{
	//�v�f������������for���[�v��MapData��S��0�ŏ�����
	ClearMapData();
}

MapManager::~MapManager()
{
}

void MapManager::GenerateMapData()
{
	//��x�S�Ẵ}�X����(0)�ɂ���
	ClearMapData();

	//�ǂ𐶐�
	GenerateWall();

	//�o���𐶐�
	GenerateDoor();

	//�����j�G���A�Ƃ��ĒʘH���B���Ă����܂�
	GenerateBlind();

}
void MapManager::PassageRecord(vector2D pos)
{
	//���̃}�X�������j�G���A�ł��邩���m�F
	if (m_MapData[pos.y][pos.x] == 3)
	{
		m_MapData[pos.y][pos.x] = 0;
	}
}

void MapManager::GenerateWall()
{
	srand((unsigned int)time(NULL)); // ���ݎ����̏��ŏ�����

	//�ǂ����������w�肷��
	//�������L�^����z����쐬
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
			// 1����4�܂ł̗������쐬
			int num = rand() % 4 + 1;

			//0.0f����1.0f�܂ł̗������쐬
			float RandNum = ((float)(rand() % 1000)) / 1000.0f;
			//�v�f���̎w��
			int FactorNum = 4;
			//����̎w��
			string Hierarchy[4] = {
				"up","left","down","right"
			};


			//��i�ڈڍs�̏ꍇup��v�f���珜�O
			if (y != 0)
			{
				FactorNum -= 1;
				for (int i = 0; i < 4; ++i)
				{
					//�w�肳�ꂽ���̂������������l�߂�
					if (Hierarchy[i] == "up")
					{
						for (int u = i; u < 3; ++u)
						{
							Hierarchy[u] = Hierarchy[u + 1];
						}
					}
				}
			}

			//���ɕǂ����ɂ������ꍇleft��v�f���珜�O
			if (x != 0 && WallWayData[y][x - 1] == "right")
			{
				FactorNum -= 1;
				for (int i = 0; i < 4; ++i)
				{
					//�w�肳�ꂽ���̂������������l�߂�
					if (Hierarchy[i] == "left")
					{
						for (int u = i; u < 3; ++u)
						{
							Hierarchy[u] = Hierarchy[u + 1];
						}
					}
				}
			}

			//�m�������Ɍ������w��
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
	//��������o�����������o�͂��Ă݂� �������܂���
	/*for (int y = 0; y < 6; y++)
		{
			for (int x = 0; x < 6; x++)
			{
				if (WallWayData[y][x] == "up")
				{
					cout << "��";
				}
				if (WallWayData[y][x] == "left")
				{
					cout << "��";
				}
				if (WallWayData[y][x] == "down")
				{
					cout << "��";
				}
				if (WallWayData[y][x] == "right")
				{
					cout << "��";
				}

			}
			cout << endl;
		}
	*/

	//m_MapData�ɊO�ǂ𐶐����Ă����܂�
	for (int y = 0; y < m_MapDataScale.y; y++)
	{
		for (int x = 0; x < m_MapDataScale.x; x++)
		{
			//�㉺�̕�
			if (y == 0 || y == m_MapDataScale.y - 1)
			{
				m_MapData[y][x] = 1;
			}
			//���E�̕�
			if (x == 0 || x == m_MapDataScale.x - 1)
			{
				m_MapData[y][x] = 1;
			}
		}
	}

	//m_MapData�ɒ��𐶐����Ă����܂�
	for (int y = 0; y < m_MapDataScale.y; y++)
	{
		for (int x = 0; x < m_MapDataScale.x; x++)
		{
			//xy�ǂ���������ł���Ȃ��
			if (x % 2 == 0 && y % 2 == 0)
			{
				m_MapData[y][x] = 1;
			}
		}
	}

	//m_MapData�ɒ��̉��ɉϓI�ȕǂ𐶐����Ă����܂�
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
	srand((unsigned int)time(NULL)); // ���ݎ����̏��ŏ�����

	int PosX = 0;
	int PosY = 0;

	//�O�ǂɔ���ݒu����ʒu�����߂�
	if (rand() % 2 == 1)
	{
		PosX = rand() % 7 + 1;
		PosX = (PosX * 2) - 1;
		if (rand() % 2 == 1)
		{
			//�m���Ŕ��Α��ɐݒu����
			PosY = m_MapDataScale.y - 1;
		}
	}
	else
	{
		PosY = rand() % 7 + 1;
		PosY = (PosY * 2) - 1;
		if (rand() % 2 == 1)
		{
			//�m���Ŕ��Α��ɐݒu����
			PosX = m_MapDataScale.x - 1;
		}
	}

	//�ݒu���W��T�����
	for (int y = 0; y < m_MapDataScale.y; y++)
	{
		for (int x = 0; x < m_MapDataScale.x; x++)
		{
			if (y == PosY && x == PosX)
			{
				m_MapData[y][x] = 2;
				//cout << "���ɏ��������܂���" << endl;
			}
		}
	}
	//cout << "���ݒu���WX" << PosX << "Y" << PosY << endl;
}
void MapManager::GenerateBlind()
{
	//m_MapData���̑S�Ă̋󔒃}�X�𖢓��j�}�X�ɓh��ւ���
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
	//m_MapData���̑S�Ă��󔒃}�X�ɓh��ւ���
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
					cout << "��";
				}
				else
					if (m_MapData[y][x] == 2)
					{
						cout << "��";
					}
					else
						if (m_MapData[y][x] == 3)
						{
							cout << "��";
						}
						else
						{
							//�Y��������̂��Ȃ��ꍇ�͋�
							cout << "  ";
						}

		}
		cout << endl;
	}
}
int MapManager::GetMapData(vector2D pos)
{
	int id = 1;
	//�z��͈͓̔����`�F�b�N
	if (pos.x >= 0 && pos.x <= 14 && pos.y >= 0 && pos.y <= 14)
	{
		id = m_MapData[pos.y][pos.x];
	}
	return id;
}
int MapManager::GetMapData(int x, int y)
{
	int id = 1;
	//�z��͈͓̔����`�F�b�N
	if (x >= 0 && x <= 14 && y >= 0 && y <= 14)
	{
		id = m_MapData[y][x];
	}
	return id;
}