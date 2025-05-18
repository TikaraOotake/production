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

    // 乱数のシードを現在の時刻で初期化
    std::srand(std::time(0));
}

GameStatus::~GameStatus()
{
}
void GameStatus::Update_GameStats()
{
    //EXPが基準値を超えたらレベル上昇
    if (m_EXP >= 20 * 30)//20fps*30秒
    {
        ++m_Level;
        m_EXP = 0;
    }
    else
    {
        ++m_EXP;
    }
}

int GameStatus::MinoLottery()//使えるミノの中から次に出すミノをランダムに決める
{
    //使えるミノがない場合全てのミノを有効にする
    bool use = false;
    for (int i = 0; i < 7; ++i)//全てのミノをチェック
    {
        if (m_UseMino[i] == true)
        {
            use = true;
        }
    }
    if (use == false)
    {
        for (int i = 0; i < 7; ++i)//全てのミノを有効にする
        {
            m_UseMino[i] = true;
        }
    }

    int MinoRate[7];
    for (int i = 0; i < 7; ++i)
    {
        if (m_UseMino[i] == true)
        {
            //ランダムな数字を与える
            MinoRate[i] = std::rand() % 10000 + 1;
        }
        else
        {
            //使えない為0にする
            MinoRate[i] = 0;
        }
    }

    //最大値を調べる
    int MastRate = 0;
    for (int i = 0; i < 7; ++i)
    {
        if (MastRate < MinoRate[i])
        {
            //最大値を更新
            MastRate = MinoRate[i];
        }
    }

    int NextMino = 0;
    //最大値のものを次に出現させるミノとする
    for (int i = 0; i < 7; ++i)
    {
        if (MastRate == MinoRate[i])
        {
            NextMino = i + 1;
            //ミノを使えなくする
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
    //前に詰める
    for (int i = 0; i < 5; ++i)
    {
        m_NextMino[i] = m_NextMino[i + 1];
    }
    //最後尾を選定
    m_NextMino[5] = MinoLottery();
}
int GameStatus::SwapHoldMino(int _DropMino)
{
    //引数と入れ替えて戻り値として渡す
    int result = m_HoldMino;
    m_HoldMino = _DropMino;

    //保持中ミノがなかった場合次のミノから引っ張ってくる
    if (result == 0)
    {
        result = m_NextMino[0];
        RotateNextMino();//ミノ順を回す
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

    //使えるミノの初期化
    for (int i = 0; i < 7; ++i)
    {
        m_UseMino[i] = true;
    }
    //ネクストミノの初期化
    for (int i = 0; i < 6; ++i)
    {
        m_NextMino[i] = MinoLottery();
    }
}