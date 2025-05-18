#pragma once

#include <unordered_map>
#include "Vector2D.h"


// �O���錾��ǉ�
class GridField;
class GameManager;
class Input;

class Mino
{
public:
    Mino();
    ~Mino();

    void Update_Mino();//�~�m�̍X�V����
    bool CheckBlockPos(int x, int y);//�u���b�N�����̍��W�ɂ��邩�`�F�b�N
    bool CheckGhostPos(int x, int y);//�����n�_�����̍��W�ɂ��邩�`�F�b�N

    void SetGridField(GridField*  _GridField);//�t�B�[���h��ݒ�
    void SetGameManager(GameManager* _pGameManager);//�Q�[���}�l�[�W���[��ݒ�

    int GetShapeID();//�`��ID���擾
    int GetScore();//�X�R�A���擾

    void Reset(int _MinoID, int _Level);//���Z�b�g

private:

    
    void Drop();//�ʏ헎������
    void SideMove();//���ړ�����
    void Swap();//����ւ�
    bool CheckLand(int _x, int _y);//�ڒn�`�F�b�N�֐�
    void Rotate();
    Vector2D RotatePos(Vector2D _pos, int _Angle);
    void SetMinoShape(int _Angle);
    bool CheckCollision(Vector2D pos);
    bool CheckCollision(int _x, int _y);
    void SuperRotation(int _NowAngle, int _NextAngle);
    Vector2D SuperRotationPosIndex(int Step, int _NowAngle, int _NextAngle);


    GridField* m_pGridField;
    GameManager* m_pGameManager;

    Vector2D ShapeIndex(int _BlockNum,int _Angle);//�`�󎫏�

    Vector2D m_CenterPos;
    Vector2D m_BlockPos[4];

    int m_ShapeID;//�`��ԍ�
    int m_Angle;//�p�x

    int m_DropTimer;//�������Ԍv��
    int m_DropTime;//��������

    int m_ScorePoint;//�X�R�A������

    bool m_SwapFlag;//����ւ��\���̃t���O

    Input* input;//���͎󂯕t���p
};
