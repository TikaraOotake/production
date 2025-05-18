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
    //�z������`�F�b�N
    if (0 <= _x && 9 >= _x && 0 <= _y && 19 >= _y)
    {
        Num = m_GridData[_y][_x];
    }
    else
    {
        //�z��O���������(-1)�ɂ��Ă���
        Num = -1;

        //�V�䂾�����ꍇ�͓V��(-2)�ɂ��Ă���
        if (0 <= _x && 9 >= _x && 0 > _y)
        {
            Num = -2;
        }
    }
    return Num;
}
void GridField::SetGridData(int _x, int _y, int _GridID)
{
    //�z������`�F�b�N
    if (0 <= _x && 9 >= _x && 0 <= _y && 19 >= _y)
    {
        m_GridData[_y][_x] = _GridID;
    }
}
bool GridField::CheckDeleteLine()
{
    bool Result = false;
    for (int y = 19; y > 0; --y)//��񂩂�20�i�S�ă`�F�b�N
    {

        //�Y�����C�����u���b�N�Ŗ������Ă��邩�`�F�b�N
        if (CheckFillLine(y))
        {
            //�Y�����C���������ď�̐���(�O�̐���)�Ŗ��߂Ă���
            FillLine(y);

            //�Q�[���}�l�[�W���[�Ƀ��C���������s��ꂽ���Ƃ�`����
            if (m_pGameManager != nullptr)
            {
                m_pGameManager->Event_DeleteLine();
            }
            //true��Ԃ�
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
        //�ŏ��̍s��0�ɂ���
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
    //�̈���܂������
    for (int y = 0; y < 20; ++y)
    {
        for (int x = 0; x < 10; ++x)
        {
            m_GridData[y][x] = 0;
        }
    }
}
