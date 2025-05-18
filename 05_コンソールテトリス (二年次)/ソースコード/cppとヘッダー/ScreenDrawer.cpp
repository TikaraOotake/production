#include "ScreenDrawer.h"

#include "GridField.h"
#include "Mino.h"
#include "GameStatus.h"
#include "UI.h"

#include <sstream>  // std::stringstream ���g�����߂ɕK�v

ScreenDrawer::ScreenDrawer() :
    m_TitleBackGroundSlide(0)
{
    //�J�[�\�����\����
    SetCursorVisibility(false);

    SetScreenData_Initialize();//�L�����o�X���܂�����ȏ�Ԃ�
}

ScreenDrawer::~ScreenDrawer()
{
}
void ScreenDrawer::DrawField(GridField* _pGridField, Mino* _pMino)
{
    //moveCursorTo(1, 1);  // ����ɃJ�[�\�����ړ�

    //SetScreenData_Field(_pGridField, _pMino);
    //DrawScreen();
}
void ScreenDrawer::moveCursorTo(int x, int y) {
    cout << "\033[" << y << ";" << x << "H" << flush;
}
void ScreenDrawer::SetCursorVisibility(bool visible)
{
    HANDLE consoleHandle = GetStdHandle(STD_OUTPUT_HANDLE);
    CONSOLE_CURSOR_INFO cursorInfo;
    GetConsoleCursorInfo(consoleHandle, &cursorInfo);
    cursorInfo.bVisible = visible ? TRUE : FALSE;
    SetConsoleCursorInfo(consoleHandle, &cursorInfo);
}
void ScreenDrawer::DrawScreen()
{
    moveCursorTo(1, 1);  // ����ɃJ�[�\�����ړ�

    for (int y = 0; y < 22; ++y)
    {
        for (int x = 0; x < 50; ++x)
        {
            std::cout << m_ScreenCanvas[y][x].Char ; // 1 �������o��
        }
        std:: cout << endl;
    }
    //��C�ɕ`��?
    cout << flush;
}
void ScreenDrawer::SetScreenData_Field(GridField* _pGridField, Mino* _pMino)
{
    //�␳�l
    Vector2D AdjustPos;
    AdjustPos.x = 6;
    AdjustPos.y = 0;

    for (int y = 0; y < 22; ++y)
    {
        for (int x = 0; x < 50; ++x)
        {
            //�z������`�F�b�N
            if (x + AdjustPos.x >= 0 && x + AdjustPos.x < ScreenSize.x &&
                y + AdjustPos.y >= 0 && y + AdjustPos.y < ScreenSize.y)
            {
               // m_ScreenCanvas[y + AdjustPos.y][x + AdjustPos.x].Char = "��";
            }

        }
    }

    for (int y = 0; y < 22; ++y)
    {
        for (int x = 0; x < 12; ++x)
        {
            //�␳�l�𑫂����l���z������`�F�b�N
            if (x + AdjustPos.x >= 0 && x + AdjustPos.x < ScreenSize.x &&
                y + AdjustPos.y >= 0 && y + AdjustPos.y < ScreenSize.y)
            {
                string Char = "�@";

                int GridData = _pGridField->GetGridData(x - 1, y - 1);
                if (GridData == -2 || GridData == -1)//�ǂ��V��
                {
                    Char = "��";
                }
                else if (GridData == 1) { Char = "��"; }
                else if (GridData == 2) { Char = "��"; }
                else if (GridData == 3) { Char = "��"; }
                else if (GridData == 4) { Char = "��"; }
                else if (GridData == 5) { Char = "��"; }
                else if (GridData == 6) { Char = "�}"; }
                else if (GridData == 7) { Char = "��"; }
                else if (GridData == 0) {
                    //�~�m�����邩�`�F�b�N
                    if (_pMino->CheckBlockPos(x - 1, y - 1))
                    {
                        //�`��ID���擾
                        int ShapeID = _pMino->GetShapeID();
                        if (ShapeID == 0) { Char = "��"; }
                        else if (ShapeID  == 1) { Char = "��"; }
                        else if (ShapeID  == 2) { Char = "��"; }
                        else if (ShapeID  == 3) { Char = "��"; }
                        else if (ShapeID  == 4) { Char = "��"; }
                        else if (ShapeID  == 5) { Char = "��"; }
                        else if (ShapeID  == 6) { Char = "�}"; }
                        else if (ShapeID  == 7) { Char = "��"; }
                        else { Char = "��"; }
                    }
                    else //�S�[�X�g�����邩�`�F�b�N
                        if (_pMino->CheckGhostPos(x - 1, y - 1))
                        {
                            Char = "�w"; 
                        }
                    else//���Ȃ��ꍇ�͋󔒂ɂ��Ă���
                    {
                        Char = "�@";
                    }
                }
                else { Char = "��"; }

                //���
                m_ScreenCanvas[y + AdjustPos.y][x + AdjustPos.x].Char = Char;
            }

        }
    }

}
void ScreenDrawer::SetScreenData_NextMino(GameStatus* _pGameStatus)
{
    //�␳�l
    Vector2D AdjustPos;
    AdjustPos.x = 18;
    AdjustPos.y = 0;

    //�O�g�����
    for (int y = 0; y < 30; ++y)
    {
        for (int x = 0; x < 6; ++x)
        {
            //�z������`�F�b�N
            if (x + AdjustPos.x >= 0 && x + AdjustPos.x < ScreenSize.x &&
                y + AdjustPos.y >= 0 && y + AdjustPos.y < ScreenSize.y)
            {
                //���
                m_ScreenCanvas[y + AdjustPos.y][x + AdjustPos.x].Char = "��";
            }
        }
    }

    //���̃~�m
    for (int y = 0; y < 3; ++y)
    {
        for (int x = 0; x < 4; ++x)
        {
            int MinoID = _pGameStatus->GetNextMino(0);
            string Char = "�@";
            if (y == 0)
            {
                if (x == 0) { Char = "�m"; }
                if (x == 1) { Char = "�d"; }
                if (x == 2) { Char = "�w"; }
                if (x == 3) { Char = "�s"; }
            }
            if (y == 1)
            {
                Char = GetMinoTexture(x, y - 1, MinoID);
            }
            if (y == 2)
            {
                Char = GetMinoTexture(x, y - 1, MinoID);
            }
            if (x + AdjustPos.x + 1 >= 0 && x + AdjustPos.x + 1 < ScreenSize.x &&
                y + AdjustPos.y + 1 >= 0 && y + AdjustPos.y + 1 < ScreenSize.y)
            {
                //���
                m_ScreenCanvas[y + AdjustPos.y + 1][x + AdjustPos.x + 1].Char = Char;
            }

        }
    }

    //�X�Ɍ��̃~�m
    //�������قړ���Ȃ��ߌJ��Ԃ�
    for (int i = 0; i < 5; ++i)
    {
        for (int y = 0; y < 4; ++y)
        {
            for (int x = 0; x < 4; ++x)
            {
                int MinoID = _pGameStatus->GetNextMino(i + 1);
                string Char = "�@";
                if (y == 0 || y == 3)
                {
                    Char = "--";
                }
                if (y == 1)
                {
                    Char = GetMinoTexture(x, y - 1, MinoID);
                }
                if (y == 2)
                {
                    Char = GetMinoTexture(x, y - 1, MinoID);
                }

                if (x + AdjustPos.x + 1 >= 0 && x + AdjustPos.x + 1 < ScreenSize.x &&
                    y + AdjustPos.y + 5 + (i * 3) >= 0 && y + AdjustPos.y + 5 + (i * 3) < ScreenSize.y)
                {
                    //���
                    m_ScreenCanvas[y + AdjustPos.y + 5 + (i * 3)][x + AdjustPos.x + 1].Char = Char;
                }
            }
        }
    }
}
void ScreenDrawer::SetScreenData_HoldMino(GameStatus* _pGameStatus)
{
    //�␳�l
    Vector2D AdjustPos;
    AdjustPos.x = 0;
    AdjustPos.y = 0;

    //�O�g�����
    for (int y = 0; y < 5; ++y)
    {
        for (int x = 0; x < 6; ++x)
        {
            //�z������`�F�b�N
            if (x + AdjustPos.x >= 0 && x + AdjustPos.x < ScreenSize.x &&
                y + AdjustPos.y >= 0 && y + AdjustPos.y < ScreenSize.y)
            {
                //���
                m_ScreenCanvas[y + AdjustPos.y][x + AdjustPos.x].Char = "��";
            }
        }
    }
    //���̃~�m
    for (int y = 0; y < 3; ++y)
    {
        for (int x = 0; x < 4; ++x)
        {
            int MinoID = _pGameStatus->GetHoldMino();
            string Char = "�@";
            if (y == 0)
            {
                if (x == 0) { Char = "�g"; }
                if (x == 1) { Char = "�n"; }
                if (x == 2) { Char = "�k"; }
                if (x == 3) { Char = "�c"; }
            }
            if (y == 1)
            {
                Char = GetMinoTexture(x, y - 1, MinoID);
            }
            if (y == 2)
            {
                Char = GetMinoTexture(x, y - 1, MinoID);
            }
            if (x + AdjustPos.x + 1 >= 0 && x + AdjustPos.x + 1 < ScreenSize.x &&
                y + AdjustPos.y + 1 >= 0 && y + AdjustPos.y + 1 < ScreenSize.y)
            {
                //���
                m_ScreenCanvas[y + AdjustPos.y + 1][x + AdjustPos.x + 1].Char = Char;
            }

        }
    }
}
string ScreenDrawer::GetMinoTexture(int x, int y, int _MinoID)
{
    string Char = "�@";
    if (0 == _MinoID)//�ݒ�Ȃ�
    {
        if (y == 0) { if (x == 0) { Char = "�@"; }if (x == 1) { Char = "�@"; }if (x == 2) { Char = "�@"; }if (x == 3) { Char = "�@"; } }
        if (y == 1) { if (x == 0) { Char = "�@"; }if (x == 1) { Char = "�@"; }if (x == 2) { Char = "�@"; }if (x == 3) { Char = "�@"; } }
    }
    if (1 == _MinoID)//T�~�m
    {
        if (y == 0) { if (x == 0) { Char = "�@"; }if (x == 1) { Char = "�@"; }if (x == 2) { Char = "��"; }if (x == 3) { Char = "�@"; } }
        if (y == 1) { if (x == 0) { Char = "�@"; }if (x == 1) { Char = "��"; }if (x == 2) { Char = "��"; }if (x == 3) { Char = "��"; } }
    }
    if (2 == _MinoID)//S�~�m
    {
        if (y == 0) { if (x == 0) { Char = "�@"; }if (x == 1) { Char = "�@"; }if (x == 2) { Char = "��"; }if (x == 3) { Char = "��"; } }
        if (y == 1) { if (x == 0) { Char = "�@"; }if (x == 1) { Char = "��"; }if (x == 2) { Char = "��"; }if (x == 3) { Char = "�@"; } }
    }
    if (3 == _MinoID)//Z�~�m
    {
        if (y == 0) { if (x == 0) { Char = "�@"; }if (x == 1) { Char = "��"; }if (x == 2) { Char = "��"; }if (x == 3) { Char = "�@"; } }
        if (y == 1) { if (x == 0) { Char = "�@"; }if (x == 1) { Char = "�@"; }if (x == 2) { Char = "��"; }if (x == 3) { Char = "��"; } }
    }
    if (4 == _MinoID)//J�~�m
    {
        if (y == 0) { if (x == 0) { Char = "�@"; }if (x == 1) { Char = "��"; }if (x == 2) { Char = "�@"; }if (x == 3) { Char = "�@"; } }
        if (y == 1) { if (x == 0) { Char = "�@"; }if (x == 1) { Char = "��"; }if (x == 2) { Char = "��"; }if (x == 3) { Char = "��"; } }
    }
    if (5 == _MinoID)//L�~�m
    {
        if (y == 0) { if (x == 0) { Char = "�@"; }if (x == 1) { Char = "�@"; }if (x == 2) { Char = "�@"; }if (x == 3) { Char = "��"; } }
        if (y == 1) { if (x == 0) { Char = "�@"; }if (x == 1) { Char = "��"; }if (x == 2) { Char = "��"; }if (x == 3) { Char = "��"; } }
    }
    if (6 == _MinoID)//I�~�m
    {
        if (y == 0) { if (x == 0) { Char = "�@"; }if (x == 1) { Char = "�@"; }if (x == 2) { Char = "�@"; }if (x == 3) { Char = "�@"; } }
        if (y == 1) { if (x == 0) { Char = "��"; }if (x == 1) { Char = "��"; }if (x == 2) { Char = "��"; }if (x == 3) { Char = "��"; } }
    }
    if (7 == _MinoID)//O�~�m
    {
        if (y == 0) { if (x == 0) { Char = "�@"; }if (x == 1) { Char = "��"; }if (x == 2) { Char = "��"; }if (x == 3) { Char = "�@"; } }
        if (y == 1) { if (x == 0) { Char = "�@"; }if (x == 1) { Char = "��"; }if (x == 2) { Char = "��"; }if (x == 3) { Char = "�@"; } }
    }
    return Char;
}
void ScreenDrawer::SetScreenData_GameStatus(GameStatus* _pGameStatus)
{
    //�␳�l
    Vector2D AdjustPos;
    AdjustPos.x = 0;
    AdjustPos.y = 6;

    //�O�g�����
    for (int y = 0; y < 8; ++y)
    {
        for (int x = 0; x < 6; ++x)
        {
            //�z������`�F�b�N
            if (IsInArray(x + AdjustPos.x,y + AdjustPos.y))
            {
                //���
                m_ScreenCanvas[y + AdjustPos.y][x + AdjustPos.x].Char = "��";
            }
        }
    }

    //�K�v�ȕϐ����擾�E�쐬
    int Level = _pGameStatus->GetLevel();
    int Score = _pGameStatus->GetScore();
    int Lines = _pGameStatus->GetLines();

    for (int i = 0; i < 4; ++i)
    {
        for (int x = 0; x < 4; ++x)
        {
            string Char = "��";
            if (i == 0)
            {
                if (x == 0) { Char = "Le"; }
                if (x == 1) { Char = "ve"; }
                if (x == 2) { Char = "l:"; }
                if (x == 3) { Char = AdjustDigitNum(Level); }
            }
            if (i == 1)
            {
                if (x == 0) { Char = "Li"; }
                if (x == 1) { Char = "ne"; }
                if (x == 2) { Char = "s:"; }
                if (x == 3) { Char = AdjustDigitNum(Lines); }
            }
            if (i == 2)
            {
                if (x == 0) { Char = "Sc"; }
                if (x == 1) { Char = "or"; }
                if (x == 2) { Char = "e:"; }
                if (x == 3) { Char = "�@"; }//�����ł͐����͕\�����Ȃ�
            }
            if (i == 3)
            {
                if (x == 0) { Char = AdjustDigitNum(Score / 1000000); }
                if (x == 1) { Char = AdjustDigitNum(Score / 10000); }
                if (x == 2) { Char = AdjustDigitNum(Score / 100); }
                if (x == 3) { Char = AdjustDigitNum(Score / 1); }
            }
            //�z������`�F�b�N
            Vector2D GridPos;
            GridPos.x = AdjustPos.x + x + 1;
            GridPos.y = AdjustPos.y + i * 2 + 1;
            if (i == 3) { --GridPos.y; }//3�̂Ƃ������������炷
            if (IsInArray(GridPos))
            {
                //���
                m_ScreenCanvas[GridPos.y][GridPos.x].Char = Char;
            }
        }
    }
}
void ScreenDrawer::SetScreenData_GameOver()//�Q�[���I�[�o�[���L�����o�X�ɕ`��
{
    //�␳�l
    Vector2D AdjustPos;
    AdjustPos.x = 6;
    AdjustPos.y = 0;
    for (int y = 0; y < 20; ++y)//��񂩂�
    {
        for (int x = 0; x < 10; ++x)
        {
            //�z������`�F�b�N
            if (IsInArray(x + 1 + AdjustPos.x, y + 1 + AdjustPos.y))
            {
                m_ScreenCanvas[y + 1 + AdjustPos.y][x + 1 + AdjustPos.x].Char = "�@";
            }
        }
    }
    //�Q�[���I�[�o�[��`��
    for (int x = 0; x < 8; ++x)
    {
        //�z������`�F�b�N
        Vector2D GridPoint;
        GridPoint.x = AdjustPos.x + x + 2;
        GridPoint.y = AdjustPos.y + 5;
        if (IsInArray(GridPoint.x, GridPoint.y))
        {
            string Char = "�@";
            if (x == 0) { Char = "�f"; }
            if (x == 1) { Char = "�`"; }
            if (x == 2) { Char = "�l"; }
            if (x == 3) { Char = "�d"; }
            if (x == 4) { Char = "�n"; }
            if (x == 5) { Char = "�u"; }
            if (x == 6) { Char = "�d"; }
            if (x == 7) { Char = "�q"; }
            m_ScreenCanvas[GridPoint.y][GridPoint.x].Char = Char;
        }
    }
}
void ScreenDrawer::SetScreenData_Fadeout()
{
    //�␳�l
    Vector2D AdjustPos;
    AdjustPos.x = 6;
    AdjustPos.y = 0;

    for (int y = 21; y >= 0; --y)//��񂩂�
    {
        for (int x = 0; x < 12; ++x)
        {
            //�z������`�F�b�N
            if (IsInArray(x + AdjustPos.x, y + AdjustPos.y))
            {
                m_ScreenCanvas[y + AdjustPos.y][x + AdjustPos.x].Char = "��";
            }
        }
        DrawScreen();//�A�j���[�V�����̂��߂����ŕ\��
        Sleep(20);
    }
}
void ScreenDrawer::SetScreenData_GameOverSelect(UI* _pUI)
{
    //�␳�l
    Vector2D AdjustPos;
    AdjustPos.x = 7;
    AdjustPos.y = 4;
    //�I�����ڂ�\��
    //Continue
    //Title
    //GameEnd
    for (int i = 0; i < 3; ++i)//�I�����ڂ�3�̂���3��J��Ԃ�
    {
        for (int x = 0; x < 5; ++x)
        {
            //�z������`�F�b�N
            Vector2D GridPoint;
            GridPoint.x = AdjustPos.x + x + 2;
            GridPoint.y = AdjustPos.y + i * 2 + 5;
            if (IsInArray(GridPoint.x, GridPoint.y))
            {
                string Char = "�@";
                if (x == 0) {
                    if (_pUI != nullptr) {
                        int SelectNum = _pUI->GetSelectNum();
                        if (SelectNum == i ) { Char = "->"; }//�I�𒆂̍��ڂɖ��
                    }
                }
                if (x == 1) { if (i == 0) { Char = "Co"; }if (i == 1) { Char = "Ti"; }  if (i == 2) { Char = "Ga"; } }//�c�ǂ�
                if (x == 2) { if (i == 0) { Char = "nt"; }if (i == 1) { Char = "tl"; }  if (i == 2) { Char = "me"; } }
                if (x == 3) { if (i == 0) { Char = "in"; }if (i == 1) { Char = "e "; }  if (i == 2) { Char = " E"; } }
                if (x == 4) { if (i == 0) { Char = "ue"; }if (i == 1) { Char = "�@"; }  if (i == 2) { Char = "nd"; } }
                m_ScreenCanvas[GridPoint.y][GridPoint.x].Char = Char;
            }
        }
    }
}
void ScreenDrawer::SetScreenData_GameTitle()
{
    ++m_TitleBackGroundSlide;//���炷�ʂ����Z
    if (m_TitleBackGroundSlide >= 120) 
    { m_TitleBackGroundSlide = 0; }//������������߂�

    //�w�i��`��(�O���b�h�w�i)
    for (int y = 0; y < 22; ++y)
    {
        for (int x = 0; x < 22; ++x)
        {
            //�z����Ń��[�v���񂷂��߃`�F�b�N�͏Ȃ�

            string Char = "�@";
            if ((x + m_TitleBackGroundSlide/20) % 6 == 0 || (y + m_TitleBackGroundSlide/20) % 6 == 0)
            {
                Char = "��";
            }
            m_ScreenCanvas[y][x].Char = Char;
        }
    }
    int TitleLogoTexture[7][20] = {
        {9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9},
        {9,1,1,1,2,2,1,1,1,0,2,0,1,1,0,2,0,1,1,9},
        {9,0,1,0,2,0,0,1,0,2,0,2,1,0,1,0,1,0,0,9},
        {9,0,1,0,2,2,0,1,0,2,0,2,1,1,0,2,0,1,0,9},
        {9,0,1,0,2,0,0,1,0,2,0,2,1,0,1,2,0,0,1,9},
        {9,0,1,0,2,2,0,1,0,0,2,0,1,0,1,2,1,1,0,9},
        {9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9},
    };

    //�^�C�g�����S��`��
    for (int y = 0; y < 7; ++y)
    {
        for (int x = 0; x < 20; ++x)
        {
            //�z������`�F�b�N
            Vector2D GridPoint;
            GridPoint.x = x + 1;//���炵�����ꍇ�͕␳�l��^����
            GridPoint.y = y + 1;
            if (IsInArray(GridPoint.x, GridPoint.y))
            {
                string Char = "�@";
                int TexturePixelNum = TitleLogoTexture[y][x];
                if (TexturePixelNum == 1) { Char = "��"; }
                if (TexturePixelNum == 2) { Char = "��"; }
                if (TexturePixelNum == 9) { Char = "--"; }
                m_ScreenCanvas[GridPoint.y][GridPoint.x].Char = Char;
            }
        }
    }
}
void ScreenDrawer::SetScreenData_GameTitleSelect(UI* _pUI)
{
    //�␳�l
    Vector2D AdjustPos;
    AdjustPos.x = 6;
    AdjustPos.y = 7;

    //�I�����ڂ�\��
    //GameStart
    //GameEnd
    for (int i = 0; i < 2; ++i)//�I�����ڂ���̂��ߓ��J��Ԃ�
    {
        for (int x = 0; x < 6; ++x)
        {
            //�z������`�F�b�N
            Vector2D GridPoint;
            GridPoint.x = AdjustPos.x + x + 2;
            GridPoint.y = AdjustPos.y + i * 2 + 5;
            if (IsInArray(GridPoint.x, GridPoint.y))
            {
                string Char = "�@";
                if (x == 0) {
                    if (_pUI != nullptr) {
                        int SelectNum = _pUI->GetSelectNum();
                        if (SelectNum == i) { Char = "->"; }//�I�𒆂̍��ڂɖ��
                    }
                }
                if (x == 1) { if (i == 0) { Char = "Ga"; }if (i == 1) { Char = "Ga"; } }//�c�ǂ�
                if (x == 2) { if (i == 0) { Char = "me"; }if (i == 1) { Char = "me"; } }
                if (x == 3) { if (i == 0) { Char = " S"; }if (i == 1) { Char = " E"; } }
                if (x == 4) { if (i == 0) { Char = "ta"; }if (i == 1) { Char = "nd"; } }
                if (x == 5) { if (i == 0) { Char = "rt"; }if (i == 1) { Char = "�@"; } }
                m_ScreenCanvas[GridPoint.y][GridPoint.x].Char = Char;
            }
        }
    }
}

bool ScreenDrawer::IsInArray(Vector2D _pos)
{
    if (_pos.x >= 0 && _pos.x < ScreenSize.x &&
        _pos.y >= 0 && _pos.y < ScreenSize.y)
    {
        return true;
    }
    else
    {
        return false;
    }
}
bool ScreenDrawer::IsInArray(int _x, int _y)
{
    if (_x >= 0 && _x < ScreenSize.x &&
        _y >= 0 && _y < ScreenSize.y)
    {
        return true;
    }
    else
    {
        return false;
    }
}
string ScreenDrawer::AdjustDigitNum(int _Num)
{
    string Result = "�@";
    //�O���ȏ���폜
    while (_Num >= 100)
    {
        _Num -= 100;
    }
    if (_Num >= 10)
    {

        Result = std::to_string(_Num);
    }
    else
    {
        //�񌅖ڂ�0��\��
        Result = "0" + std::to_string(_Num);
    }
    return Result;
}

void ScreenDrawer::SetScreenData_Initialize()
{
    for (int y = 0; y < 22; ++y)
    {
        for (int x = 0; x < 50; ++x)
        {
            m_ScreenCanvas[y][x].Char = "�@";
        }
    }
}
void ScreenDrawer::SetScreenData_Pixel(Vector2D _Pos, string _Char)//1�s�N�Z�����ɏ������������ꍇ��
{
    //�z������`�F�b�N
    if (IsInArray(_Pos.x, _Pos.y))
    {
        m_ScreenCanvas[_Pos.y][_Pos.x].Char = _Char;
    }
}

//Windows�Ő��������삳���邽�߂̊֐�
void ScreenDrawer::enableAnsiEscape() {
#ifdef _WIN32
    HANDLE hOut = GetStdHandle(STD_OUTPUT_HANDLE);
    DWORD dwMode = 0;
    GetConsoleMode(hOut, &dwMode);
    SetConsoleMode(hOut, dwMode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
#endif
}