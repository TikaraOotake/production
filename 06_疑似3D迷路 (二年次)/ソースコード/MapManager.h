#pragma once
#include "Vector2D.h"

class MapManager
{
public:
	MapManager();
	~MapManager();

	//���\�b�h
	void GenerateMapData();//Map�ɒn�`�����쐬
	void ShowMapDateTest();//Map���������I�ɏo��
	void PassageRecord(vector2D pos);//�ʍs�ςƂ���m_MapData�ɋL�^
	int GetMapData(vector2D pos);//�n�`�����擾
	int GetMapData(int x, int y);

private:
	//���\�b�h
	void ClearMapData();//Map�������Z�b�g
	void GenerateWall();//�ǂ𐶐�
	void GenerateDoor();//���𐶐�
	void GenerateBlind();//�����j�G���A�Ƃ��ĒʘH��h��Ԃ�

	//�v���p�e�B
	const vector2D   m_MapDataScale;//m_MapData�̑傫����ۑ�
	int m_MapData[15][15];//�n�`���
};
