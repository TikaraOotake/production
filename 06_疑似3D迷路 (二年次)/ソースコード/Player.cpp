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
	//�e�X�g�p��FPV���o��
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

	PlayerMove();//Player�̈ړ�
	UpdateFPV();//��l�̎��_�̍X�V

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

	//�}�b�v���ݒ肳�ꂽ��ēx��l�̎��_���X�V
	if (mp_mapMNG != nullptr)
	{
		UpdateFPV();
	}
}
void Player::RandomTeleportPos()
{
	// 1����4�܂ł̗������쐬
	int num = rand() % 4 + 1;
	//��������Player�̌������w��
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
	//�ړ���𗐐��Ŏw��
	vector2D pos;
	pos.x = rand() % 7 + 1;
	pos.x = (pos.x * 2) - 1;
	pos.y = rand() % 7 + 1;
	pos.y = (pos.y * 2) - 1;

	m_PlayerPos = pos;
}
void Player::UpdateFPV()
{
	//null�`�F�b�N
	if (mp_mapMNG == nullptr)
	{
		//cout << "UpdateFPV()�����s��...mp_mapMNG�Ƀ|�C���^�[���ݒ肳��Ă��܂���" << endl;
		//_getch();
		return;
	}

	for (int y = 0; y < 4; ++y)
	{
		for (int x = 0; x < 3; ++x)
		{
			vector2D searchPos;

			//��b�l(�␳�l������������)����
			searchPos.x = x - 1;
			searchPos.y = y - 3;

			//Player�̌����ɉ����ĉ�]������
			if (m_PlayerWay == "up")
			{
				//�Ȃɂ����Ȃ�
			}
			else
				if (m_PlayerWay == "left")
				{
					//�ϐ������ւ���
					int tempNum;//���̕ϐ�
					tempNum = searchPos.x;
					searchPos.x = searchPos.y;
					searchPos.y = tempNum;

					//Y���̂ݕ����𔽓]
					searchPos.y *= -1;
				}
				else
					if (m_PlayerWay == "down")
					{
						//�����𔽓]
						searchPos.x *= -1;
						searchPos.y *= -1;
					}
					else
						if (m_PlayerWay == "right")
						{
							//�ϐ������ւ���
							int tempNum;//���̕ϐ�
							tempNum = searchPos.x;
							searchPos.x = searchPos.y;
							searchPos.y = tempNum;

							//X���̂ݕ����𔽓]
							searchPos.x *= -1;
						}


			//�␳�l�ƃv���C���[�̍��W������MapData��������
			m_FirstPersonView[y][x] = mp_mapMNG->GetMapData(searchPos.x + m_PlayerPos.x, searchPos.y + m_PlayerPos.y);
		}
	}
}
void Player::PlayerMove()
{
	char Input = _getch();
	//�O
	if (Input == 'w' || Input == '8')
	{
		//Player�̕����Ɍ����đO�i
		PlayerGoWay(1);
	}
	//��
	if (Input == 'a' || Input == '4')
	{
		//Player�̌�������]
		PlayerRotate(1);
	}
	//��
	if (Input == 's' || Input == '2' || Input == '5')
	{
		//Player�̕����Ɍ����Č�i
		PlayerGoWay(-1);
	}
	//�E
	if (Input == 'd' || Input == '6')
	{
		//Player�̌�������]
		PlayerRotate(-1);
	}


}
void Player::PlayerGoWay(int _Movement)
{
	//null�`�F�b�N
	if (mp_mapMNG == nullptr)
	{
		cout << "PlayerGoWay()�����s��...mp_mapMNG�Ƀ|�C���^�[���ݒ肳��Ă��܂���" << endl;
		_getch();
		return;
	}
	//�ړ��O�ɖ����j�G���A���X�V
	mp_mapMNG->PassageRecord(m_PlayerPos);

	//Player�̕����ɍ��킹�Ĉړ�
	//�����ړ��悪�ǂ������牽�����Ȃ�
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
	//�ړ����I������̂Ń}�b�v�̖����j�G���A���X�V
	mp_mapMNG->PassageRecord(m_PlayerPos);
}
void Player::PlayerRotate(int _Rote)
{
	//Player�̌�������]
	 //������+�Ȃ獶��]-�Ȃ�E��]

	//��xstring�^����int�^�ɕϊ����ĉ�]������
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


	//�w�肵�������ɉ�]
	angle += _Rote;


	//�͂ݏo�����ꍇ�␳���|����
	if (angle < 0)
	{
		angle += 4;
	}
	if (angle > 3)
	{
		angle -= 4;
	}

	//string�^�ɖ߂�
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