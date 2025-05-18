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

	//���\�b�h
	vector2D GetPlayerPos();
	string GetPlayerWay();
	int GetPlayerFPV(int x, int y);
	void SetPlayerPos(int _x, int _y);
	void SetPlayerPos(vector2D _pos);
	void SetMapMNG(MapManager* _pMapMNG);

	void RandomTeleportPos();//�����_���Ȉʒu��Player���ړ�



	void UpdatePlayer();//Player�̍X�V

private:
	//���\�b�h
	void UpdateFPV();//��l�̎��_�̍X�V
	void PlayerMove();//Player�̈ړ�
	void PlayerGoWay(int _Movement);//Player�̑O�i��i
	void PlayerRotate(int _Rote);//Player�̉�]

	//�v���p�e�B
	vector2D m_PlayerPos;
	string m_PlayerWay;
	MapManager* mp_mapMNG;//MapManager�̃|�C���^�[��ێ�

	int m_FirstPersonView[4][3];//��l�̎��_�ł̒n�`���

};
