#include "GridField.h"
#include "GameManager.h"

GridField::GridField() :
    m_pGameManager(nullptr)
{
}

GridField::~GridField()
{
}
int GridField::GetGridData(int _x, int _y)
{
    int Num = 1;
    //配列内かチェック
    if (0 <= _x && 9 >= _x && 0 <= _y && 19 >= _y)
    {
        Num = m_GridData[_y][_x];
    }
    else
    {
        //配列外だったら壁(-1)にしておく
        Num = -1;

        //天井だった場合は天井(-2)にしておく
        if (0 <= _x && 9 >= _x && 0 > _y)
        {
            Num = -2;
        }
    }
    return Num;
}
void GridField::SetGridData(int _x, int _y, int _GridID)
{
    //配列内かチェック
    if (0 <= _x && 9 >= _x && 0 <= _y && 19 >= _y)
    {
        m_GridData[_y][_x] = _GridID;
    }
}
bool GridField::CheckDeleteLine()
{
    bool Result = false;
    for (int y = 19; y > 0; --y)//後列から20段全てチェック
    {

        //該当ラインがブロックで満たしているかチェック
        if (CheckFillLine(y))
        {
            //該当ラインを消して上の数字(前の数字)で埋めていく
            FillLine(y);

            //ゲームマネージャーにライン消去が行われたことを伝える
            if (m_pGameManager != nullptr)
            {
                m_pGameManager->Event_DeleteLine();
            }
            //trueを返す
            Result = true;
        }
    }
    return Result;
}
bool GridField::CheckFillLine(int y)
{
    bool flag = true;
    for (int x = 0; x < 10; ++x)
    {
        if (m_GridData[y][x] == 0)
        {
            flag = false;
        }
    }
    return flag;
}
void GridField::FillLine(int y)
{
    if (20 > y)
    {
        for (int i = y; i > 0; --i)
        {
            for (int x = 0; x < 10; ++x)
            {
                m_GridData[i][x] = m_GridData[i - 1][x];
            }
        }
        //最初の行を0にする
        for (int x = 0; x < 10; ++x)
        {
            m_GridData[0][x] = 0;
        }
    }
}
void GridField:: SetGameManager(GameManager* _pGameManager)
{
    m_pGameManager = _pGameManager;
}
void GridField::ResetField()
{
    //領域をまっさらに
    for (int y = 0; y < 20; ++y)
    {
        for (int x = 0; x < 10; ++x)
        {
            m_GridData[y][x] = 0;
        }
    }
}
