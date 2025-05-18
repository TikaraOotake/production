#include "GameStatus.h"
#include <iostream>

GameStatus::GameStatus() :
    m_Level(1),
    m_EXP(0),
    m_Score(0),
    m_HoldMino(0),
    m_Lines(0),
    m_DeadFlag(false)
{
    for (int i = 0; i < 7; ++i)
    {
        m_UseMino[i] = true;
    }
    for (int i = 0; i < 6; ++i)
    {
        m_NextMino[i] = MinoLottery();
    }

    // �����̃V�[�h�����݂̎����ŏ�����
    std::srand(std::time(0));
}

GameStatus::~GameStatus()
{
}
void GameStatus::Update_GameStats()
{
    //EXP����l�𒴂����烌�x���㏸
    if (m_EXP >= 20 * 30)//20fps*30�b
    {
        ++m_Level;
        m_EXP = 0;
    }
    else
    {
        ++m_EXP;
    }
}

int GameStatus::MinoLottery()//�g����~�m�̒����玟�ɏo���~�m�������_���Ɍ��߂�
{
    //�g����~�m���Ȃ��ꍇ�S�Ẵ~�m��L���ɂ���
    bool use = false;
    for (int i = 0; i < 7; ++i)//�S�Ẵ~�m���`�F�b�N
    {
        if (m_UseMino[i] == true)
        {
            use = true;
        }
    }
    if (use == false)
    {
        for (int i = 0; i < 7; ++i)//�S�Ẵ~�m��L���ɂ���
        {
            m_UseMino[i] = true;
        }
    }

    int MinoRate[7];
    for (int i = 0; i < 7; ++i)
    {
        if (m_UseMino[i] == true)
        {
            //�����_���Ȑ�����^����
            MinoRate[i] = std::rand() % 10000 + 1;
        }
        else
        {
            //�g���Ȃ���0�ɂ���
            MinoRate[i] = 0;
        }
    }

    //�ő�l�𒲂ׂ�
    int MastRate = 0;
    for (int i = 0; i < 7; ++i)
    {
        if (MastRate < MinoRate[i])
        {
            //�ő�l���X�V
            MastRate = MinoRate[i];
        }
    }

    int NextMino = 0;
    //�ő�l�̂��̂����ɏo��������~�m�Ƃ���
    for (int i = 0; i < 7; ++i)
    {
        if (MastRate == MinoRate[i])
        {
            NextMino = i + 1;
            //�~�m���g���Ȃ�����
            m_UseMino[i] = false;
        }
    }
    return NextMino;
}

int GameStatus::GetNextMino(int _Num)
{
    if (_Num >= 0 && _Num <= 5)
    {
        return m_NextMino[_Num];
    }
    else
    {
        return 0;
    }
}
int GameStatus::GetHoldMino()
{
    return m_HoldMino;
}
void GameStatus::RotateNextMino()
{
    //�O�ɋl�߂�
    for (int i = 0; i < 5; ++i)
    {
        m_NextMino[i] = m_NextMino[i + 1];
    }
    //�Ō����I��
    m_NextMino[5] = MinoLottery();
}
int GameStatus::SwapHoldMino(int _DropMino)
{
    //�����Ɠ���ւ��Ė߂�l�Ƃ��ēn��
    int result = m_HoldMino;
    m_HoldMino = _DropMino;

    //�ێ����~�m���Ȃ������ꍇ���̃~�m������������Ă���
    if (result == 0)
    {
        result = m_NextMino[0];
        RotateNextMino();//�~�m������
    }

    return result;
}
void GameStatus::SetDeadFlag(bool _Flag)
{
    m_DeadFlag = _Flag;
}
bool GameStatus::GetDeadFlag()
{
    return m_DeadFlag;
}
void GameStatus::SetLevel(int _Level)
{
    m_Level = _Level;
}
int GameStatus::GetLevel()
{
    return m_Level;
}
void GameStatus::SetLines(int _Lines)
{
    m_Lines = _Lines;
}
int GameStatus::GetLines()
{
    return m_Lines;
}
void GameStatus::SetScore(int _Score)
{
    m_Score = _Score;
}
int GameStatus::GetScore()
{
    return m_Score;
}

void GameStatus::ResetStatus()
{
    m_Level = 1;
    m_EXP = 0;
    m_Score = 0;
    m_HoldMino = 0;
    m_Lines = 0;
    m_DeadFlag = false;

    //�g����~�m�̏�����
    for (int i = 0; i < 7; ++i)
    {
        m_UseMino[i] = true;
    }
    //�l�N�X�g�~�m�̏�����
    for (int i = 0; i < 6; ++i)
    {
        m_NextMino[i] = MinoLottery();
    }
}