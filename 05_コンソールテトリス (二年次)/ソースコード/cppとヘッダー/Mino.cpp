#include "Mino.h"
#include "GameManager.h"
#include "GridField.h"
#include "Input.h"

Mino::Mino() :
    m_DropTime(20),
    m_DropTimer(0),
    m_pGridField(nullptr),
    m_pGameManager(nullptr),
    input(new Input),
    m_SwapFlag(true)
{
    //�O�̂���
    Reset(1,1);
}

Mino::~Mino()
{
    if (input != nullptr) { delete input; input = nullptr; }
}
void Mino::Update_Mino()
{
    //���E�ړ�
    SideMove();

    //��]
    Rotate();

    //�ێ��~�m�Ɠ���ւ�
    Swap();

    //�ʏ헎��
    Drop();
}
void Mino::Reset(int _MinoID,int _Level)
{
    //�p�x���Z�b�g
    m_Angle = 0;
    //���S���W���Z�b�g
    m_CenterPos.x = 5;
    m_CenterPos.y = 0;

    //�X�R�A���Z�b�g
    m_ScorePoint = 0;

    //���͂̃u���b�N�̍��W���Z�b�g

    //�~�m�̌`�����߂�
    m_ShapeID = _MinoID;
    //I�~�m�̏ꍇ�␳��������
    if (_MinoID == 6) { 
        ++m_Angle; 
        --m_CenterPos.y;
    }
     SetMinoShape(m_Angle);


    //�����Ԋu�̔{�����v�Z
    float ratio = 1.0f;
    for (int i = 0; i < _Level + 1; ++i)
    {
        ratio *= 0.85f;//���x�����ݏ悵�Č��炷
    }

    //�����Ԋu���Z�b�g
    m_DropTime = int(20.0f * ratio);
    //�����v�����Ԃ����Z�b�g
    m_DropTimer = 0;

    //�n�`�Əd�Ȃ��ċ��邩���m�F���ăQ�[���I�[�o�[�𔻒�
    if (CheckCollision(0,0))
    {
        m_pGameManager->Event_Dead();
    }
}
void Mino::SideMove()
{
    //��
    if (input->GetKeyDown('A') || (input->GetKeyDown(VK_NUMPAD4)) ||
        input->GetKeyKeep('A', 3) || input->GetKeyKeep(VK_NUMPAD4, 3))
    {
        //�ړ��\���`�F�b�N
        if (!CheckCollision(-1, 0))
        {
            m_CenterPos.x += -1;
        }


    }
    //�E
    if (input->GetKeyDown('D') || (input->GetKeyDown(VK_NUMPAD6)) ||
        input->GetKeyKeep('D', 3) || input->GetKeyKeep(VK_NUMPAD6, 3))
    {
        //�ړ��\���`�F�b�N
        if (!CheckCollision(1, 0))
        {
            m_CenterPos.x += +1;
        }
    }
}
void Mino::Swap()
{
    if (input->GetKeyDown(VK_LCONTROL) || (input->GetKeyDown(VK_NUMPAD0)))
    {
        if (m_SwapFlag == true)
        {
            m_pGameManager->Event_SwapMino();
            m_SwapFlag = false;
        }
    }
}
void Mino::Drop()
{
    //�\�t�g�h���b�v
    if (input->GetKey('S') || input->GetKey(VK_NUMPAD5) || input->GetKey(VK_NUMPAD2))
    {
        m_DropTimer += 20;
    }
    else
    {
        ++m_DropTimer;
    }

    //�n�[�h�h���b�v
    if (input->GetKeyDown('W') || (input->GetKeyDown(VK_NUMPAD8)))
    {
        for (int i = 0; i < 20; ++i)
        {
            //�����\���`�F�b�N
            if (CheckCollision(0, 1))
            {
                //�ڒn
                for (int i = 0; i < 4; ++i)
                {
                    m_pGridField->SetGridData(m_CenterPos.x + m_BlockPos[i].x, m_CenterPos.y + m_BlockPos[i].y, m_ShapeID);
                }

                //�}�l�[�W���[�ɒ��n�������Ƃ�`����
                if (m_pGameManager != nullptr) { m_pGameManager->Event_LandMino(); }

                //�X���b�v�t���O�����Z�b�g
                m_SwapFlag = true;

                //�֐����I��
                return;
            }
            else
            {
                //����
                m_CenterPos.y += 1;
                //�X�R�A�����Z
                m_ScorePoint+=2;
            }
            m_DropTimer = 0;
        }
    }
    else
        if (m_DropTimer >= m_DropTime)
        {
            //�����\���`�F�b�N
            if (CheckCollision(0, 1))
            {
                //�ڒn
                for (int i = 0; i < 4; ++i)
                {
                    m_pGridField->SetGridData(m_CenterPos.x + m_BlockPos[i].x, m_CenterPos.y + m_BlockPos[i].y, m_ShapeID);
                }

                //�O���b�h�t�B�[���h�֍폜�`�F�b�N���˗�
                m_pGridField->CheckDeleteLine();

                //�}�l�[�W���[�ɒ��n�������Ƃ�`����
                if (m_pGameManager != nullptr) { m_pGameManager->Event_LandMino(); }

                //�X���b�v�t���O�����Z�b�g
                m_SwapFlag = true;
            }
            else
            {
                //����
                m_CenterPos.y += 1;

                //�\�t�g�h���b�v�̏ꍇ�X�R�A�����Z
                if (input->GetKey('S') || input->GetKey(VK_NUMPAD5) || input->GetKey(VK_NUMPAD2))
                {
                    m_ScorePoint+=2;
                }
            }
            m_DropTimer = 0;
        }
}
Vector2D Mino::ShapeIndex(int _BlockNum, int _Angle)//�����@�u���b�N�ԍ��@�p�x
{
    Vector2D pos;
    pos.x = 0;
    pos.y = 0;

    //�@���@ 1
    //������203
    if (m_ShapeID == 1)
    {
        if (_BlockNum == 1)
        {
            pos.x = 0;
            pos.y = -1;
        }
        if (_BlockNum == 2)
        {
            pos.x = -1;
            pos.y = 0;
        }
        if (_BlockNum == 3)
        {
            pos.x = 1;
            pos.y = 0;
        }
    }

    //�@���� 12
    //�����@30
    if (m_ShapeID == 2)
    {
        if (_BlockNum == 1)
        {
            pos.x = 0;
            pos.y = -1;
        }
        if (_BlockNum == 2)
        {
            pos.x = 1;
            pos.y = -1;
        }
        if (_BlockNum == 3)
        {
            pos.x = -1;
            pos.y = 0;
        }
    }

    //�����@12
    //�@���� 03
    if (m_ShapeID == 3)
    {
        if (_BlockNum == 1)
        {
            pos.x = -1;
            pos.y = -1;
        }
        if (_BlockNum == 2)
        {
            pos.x = 0;
            pos.y = -1;
        }
        if (_BlockNum == 3)
        {
            pos.x = 1;
            pos.y = 0;
        }
    }

    //���@�@1
    //������203
    if (m_ShapeID == 4)
    {
        if (_BlockNum == 1)
        {
            pos.x = -1;
            pos.y = -1;
        }
        if (_BlockNum == 2)
        {
            pos.x = -1;
            pos.y = 0;
        }
        if (_BlockNum == 3)
        {
            pos.x = 1;
            pos.y = 0;
        }
    }

    //�@�@��
    //������
    if (m_ShapeID == 5)
    {
        if (_BlockNum == 1)
        {
            pos.x = 1;
            pos.y = -1;
        }
        if (_BlockNum == 2)
        {
            pos.x = -1;
            pos.y = 0;
        }
        if (_BlockNum == 3)
        {
            pos.x = 1;
            pos.y = 0;
        }
    }

    //
    //��������1023
    if (m_ShapeID == 6)
    {
        //I�~�m������]������Ȃ��ߒǉ�
        if (_BlockNum == 0)
        {
            pos.x = 0;
            pos.y = 0;
        }

        if (_BlockNum == 1)
        {
            pos.x = 0;
            pos.y = -1;
        }
        if (_BlockNum == 2)
        {
            pos.x = 0;
            pos.y = 1;
        }
        if (_BlockNum == 3)
        {
            pos.x = 0;
            pos.y = 2;
        }

        if (_Angle == 1)
        {
            pos.x += -1;
            pos.y += 0;
        }
        if (_Angle == 2)
        {
            pos.x += -1;
            pos.y += -1;
        }
        if (_Angle == 3)
        {
            pos.x += 0;
            pos.y += -1;
        }
    }

    //����
    //����
    if (m_ShapeID == 7)
    {
        if (_BlockNum == 1)
        {
            pos.x = 0;
            pos.y = -1;
        }
        if (_BlockNum == 2)
        {
            pos.x = 1;
            pos.y = -1;
        }
        if (_BlockNum == 3)
        {
            pos.x = 1;
            pos.y = 0;
        }
    }

    return pos;
}
bool Mino::CheckBlockPos(int x, int y)
{
    for (int i = 0; i < 4; ++i)
    {
        if (m_CenterPos.x + m_BlockPos[i].x == x && m_CenterPos.y + m_BlockPos[i].y == y)
        {
            //���W�Ƀu���b�N�������true
            return true;
        }
    }

    //���W�Ƀu���b�N���Ȃ����false
    return false;
}
bool Mino::CheckGhostPos(int x, int y)
{
    Vector2D NowPos = m_CenterPos;
    bool result = false;

    //�����Ƃ����܂ŗ���������
    for (int i = 0; i < 20; ++i)
    {
        //�����\���`�F�b�N
        if (CheckCollision(0, 1))
        {
            break;
        }
        else
        {
            //����
            m_CenterPos.y += 1;
        }
    }

    //4�̃u���b�N�����ꂼ��`�F�b�N
    for (int i = 0; i < 4; ++i)
    {
        if (m_CenterPos.x + m_BlockPos[i].x == x && m_CenterPos.y + m_BlockPos[i].y == y)
        {
            //���W�Ƀu���b�N�������true
            result= true;
        }
    }
    //���W�����ɖ߂�
    m_CenterPos = NowPos;

    //���W�Ƀu���b�N���Ȃ����false
    return result;
}
bool Mino::CheckLand(int _x, int _y)//�w�肵���悪�u���b�N���`�F�b�N
{
    bool flag = false;
    //�L�����`�F�b�N
    if (m_pGridField == nullptr)
    {
        //�|�C���^�[���L���łȂ��׏I��
        return false;
    }
    if (m_pGridField->GetGridData(_x, _y) != 0 && m_pGridField->GetGridData(_x, _y) != -2)
    {
        flag = true;
    }
    return flag;
}
void Mino::SetGridField(GridField* _pGridField)
{
    m_pGridField = _pGridField;
}
void Mino::SetGameManager(GameManager* _pGameManager)
{
    m_pGameManager = _pGameManager;
}
void Mino::Rotate()
{
    int NextAngle = m_Angle;
    //���v����
    if (input->GetKeyDown('C') || input->GetKeyDown('E') || (input->GetKeyDown(VK_NUMPAD3)) || (input->GetKeyDown(VK_NUMPAD9)) || input->GetKeyDown(VK_SPACE))
    {
        --NextAngle;
    }
    //�����v����
    if (input->GetKeyDown('Z') || input->GetKeyDown('Q') || (input->GetKeyDown(VK_NUMPAD1)) || (input->GetKeyDown(VK_NUMPAD7)) || input->GetKeyDown(VK_LSHIFT))
    {
        ++NextAngle;
    }

    //��]����
    if (NextAngle != m_Angle)
    {
        //�p�x���I�[�o�[���Ă���ꍇ�␳
        while (NextAngle < 0)
        {
            NextAngle += 4;
        }
        while (NextAngle > 3)
        {
            NextAngle -= 4;
        }

        SuperRotation(m_Angle, NextAngle);
    }
}
bool Mino::CheckCollision(int _x, int _y)
{
    //�w�肳�ꂽ���W�Œn�`�ƂԂ����Ă��邩�f�f
    for (int i = 0; i < 4; ++i)
    {
        if (CheckLand(m_CenterPos.x + m_BlockPos[i].x + _x, m_CenterPos.y + m_BlockPos[i].y + _y))
        {
            return true;
        }
    }

    //�Փ˂������Ȃ��̂�false
    return false;
}
bool Mino::CheckCollision(Vector2D _pos)
{
    //�w�肳�ꂽ���W�Œn�`�ƂԂ����Ă��邩�f�f
    for (int i = 0; i < 4; ++i)
    {
        if (CheckLand(m_CenterPos.x + m_BlockPos[i].x + _pos.x, m_CenterPos.y + m_BlockPos[i].y + _pos.y))
        {
            return true;
        }
    }

    //�Փ˂������Ȃ��̂�false
    return false;
}
Vector2D Mino::RotatePos(Vector2D _pos, int _Angle)
{
    if (_Angle == 0)
    {
        //�������Ȃ�
    }
    if (_Angle == 1)
    {
        //���l�����ւ���
        int SwapNum = _pos.x;
        _pos.x = _pos.y;
        _pos.y = SwapNum;

        //Y���𔽓]
        _pos.y *= -1;
    }
    if (_Angle == 2)
    {
        //�����𔽓]
        _pos.x *= -1;
        _pos.y *= -1;
    }
    if (_Angle == 3)
    {
        //���l�����ւ���
        int SwapNum = _pos.x;
        _pos.x = _pos.y;
        _pos.y = SwapNum;

        //X���𔽓]
        _pos.x *= -1;

    }
    return _pos;
}
void Mino::SetMinoShape(int _Angle)
{
    for (int i = 0; i < 4; ++i)
    {
        //�`�������
        m_BlockPos[i] = ShapeIndex(i, _Angle);
        //��]��������
        m_BlockPos[i] = RotatePos(m_BlockPos[i], _Angle);
    }
}
Vector2D Mino::SuperRotationPosIndex(int Step, int _NowAngle, int _NextAngle)
{
    //�i�K�Ɖ�]�����ɍ��킹�ĕ␳�l��Ԃ��܂�

    Vector2D pos;
    pos.x = 0;
    pos.y = 0;

    //I�~�m�p
    if (m_ShapeID == 6)
    {
        if (_NowAngle == 3 && _NextAngle == 0 ||
            _NowAngle == 2 && _NextAngle == 1 ||
            _NowAngle == 0 && _NextAngle == 3 ||
            _NowAngle == 1 && _NextAngle == 2)
        {
            //I�~�m��{SuperRotation���W����1
            if (Step == 1)
            {
                pos.x = -1;
                pos.y = 0;
            }
            else
                if (Step == 2)
                {
                    pos.x = 2;
                    pos.y = 0;
                }
                else
                    if (Step == 3)
                    {
                        pos.x = -1;
                        pos.y = -2;
                    }
                    else
                        if (Step == 4)
                        {
                            pos.x = 2;
                            pos.y = 1;
                        }
        }
        if (_NowAngle == 3 && _NextAngle == 2 ||
            _NowAngle == 0 && _NextAngle == 1 ||
            _NowAngle == 2 && _NextAngle == 3 ||
            _NowAngle == 1 && _NextAngle == 0)
        {
            //I�~�m��{SuperRotation���W����2
            if (Step == 1)
            {
                pos.x = -2;
                pos.y = 0;
            }
            else
                if (Step == 2)
                {
                    pos.x = 1;
                    pos.y = 0;
                }
                else
                    if (Step == 3)
                    {
                        pos.x = -2;
                        pos.y = 1;
                    }
                    else
                        if (Step == 4)
                        {
                            pos.x = 1;
                            pos.y = -2;
                        }
        }

        //�␳
        if (_NowAngle == 0 && _NextAngle == 3 ||
            _NowAngle == 1 && _NextAngle == 2 ||
            _NowAngle == 2 && _NextAngle == 3 ||
            _NowAngle == 1 && _NextAngle == 0)
        {
            //�����ɑ΂��Ĕ��]
            pos.x *= -1;
            pos.y *= -1;
        }
    }
    else
        //����ȊO
    {
        //��{SuperRotation���W
        if (Step == 1)
        {
            pos.x = 1;
            pos.y = 0;
        }
        else
            if (Step == 2)
            {
                pos.x = 1;
                pos.y = -1;
            }
            else
                if (Step == 3)
                {
                    pos.x = 0;
                    pos.y = 2;
                }
                else
                    if (Step == 4)
                    {
                        pos.x = 1;
                        pos.y = 2;
                    }

        //��]�����ɍ��킹�ĕ␳
        if (_NextAngle == 1)
        {
            //�������Ȃ�
        }
        else
            if (_NextAngle == 3)
            {
                //X���ɑ΂��Ĕ��]
                pos.x *= -1;
            }
            else
                if (_NowAngle == 1)
                {
                    //�����ɑ΂��Ĕ��]
                    pos.x *= -1;
                    pos.y *= -1;
                }
                else
                    if (_NowAngle == 3)
                    {
                        //Y���ɑ΂��Ĕ��]
                        pos.y *= -1;
                    }
    }

    return pos;
}
void Mino::SuperRotation(int _NowAngle, int _NextAngle)
{
    //O�~�m�ȊO����]������
    if (!(m_ShapeID == 7))
    {
        //���ŉ�]������
        SetMinoShape(_NextAngle);

        for (int i = 0; i < 5; ++i)
        {
            //�␳���W���擾
            Vector2D JustPos = SuperRotationPosIndex(i, _NowAngle, _NextAngle);
            if (CheckCollision(JustPos) == false)
            {
                //��肪�Ȃ���Α��
                m_Angle = _NextAngle;
                //�~�m�̍��W���␳�������W�ɍ��킹��
                m_CenterPos.x += JustPos.x;
                m_CenterPos.y += JustPos.y;

                //�֐����I��
                return;
            }
        }

        //��]�̍ۂɋ󂢂Ă���W���Ȃ������ׂ��Ƃɖ߂��Ă���
        SetMinoShape(_NowAngle);
    }
}
int Mino::GetShapeID()
{
    return m_ShapeID;
}
int Mino::GetScore()
{
    return m_ScorePoint;
}
