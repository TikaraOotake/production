#include "ScreenDrawer.h"

#include "GridField.h"
#include "Mino.h"
#include "GameStatus.h"
#include "UI.h"

#include <sstream>  // std::stringstream を使うために必要

ScreenDrawer::ScreenDrawer() :
    m_TitleBackGroundSlide(0)
{
    //カーソルを非表示に
    SetCursorVisibility(false);

    SetScreenData_Initialize();//キャンバスをまっさらな状態に
}

ScreenDrawer::~ScreenDrawer()
{
}
void ScreenDrawer::DrawField(GridField* _pGridField, Mino* _pMino)
{
    //moveCursorTo(1, 1);  // 左上にカーソルを移動

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
    moveCursorTo(1, 1);  // 左上にカーソルを移動

    for (int y = 0; y < 22; ++y)
    {
        for (int x = 0; x < 50; ++x)
        {
            std::cout << m_ScreenCanvas[y][x].Char ; // 1 文字ずつ出力
        }
        std:: cout << endl;
    }
    //一気に描画?
    cout << flush;
}
void ScreenDrawer::SetScreenData_Field(GridField* _pGridField, Mino* _pMino)
{
    //補正値
    Vector2D AdjustPos;
    AdjustPos.x = 6;
    AdjustPos.y = 0;

    for (int y = 0; y < 22; ++y)
    {
        for (int x = 0; x < 50; ++x)
        {
            //配列内かチェック
            if (x + AdjustPos.x >= 0 && x + AdjustPos.x < ScreenSize.x &&
                y + AdjustPos.y >= 0 && y + AdjustPos.y < ScreenSize.y)
            {
               // m_ScreenCanvas[y + AdjustPos.y][x + AdjustPos.x].Char = "口";
            }

        }
    }

    for (int y = 0; y < 22; ++y)
    {
        for (int x = 0; x < 12; ++x)
        {
            //補正値を足した値が配列内かチェック
            if (x + AdjustPos.x >= 0 && x + AdjustPos.x < ScreenSize.x &&
                y + AdjustPos.y >= 0 && y + AdjustPos.y < ScreenSize.y)
            {
                string Char = "　";

                int GridData = _pGridField->GetGridData(x - 1, y - 1);
                if (GridData == -2 || GridData == -1)//壁か天井
                {
                    Char = "口";
                }
                else if (GridData == 1) { Char = "国"; }
                else if (GridData == 2) { Char = "因"; }
                else if (GridData == 3) { Char = "固"; }
                else if (GridData == 4) { Char = "回"; }
                else if (GridData == 5) { Char = "囮"; }
                else if (GridData == 6) { Char = "図"; }
                else if (GridData == 7) { Char = "囲"; }
                else if (GridData == 0) {
                    //ミノが居るかチェック
                    if (_pMino->CheckBlockPos(x - 1, y - 1))
                    {
                        //形状IDを取得
                        int ShapeID = _pMino->GetShapeID();
                        if (ShapeID == 0) { Char = "口"; }
                        else if (ShapeID  == 1) { Char = "国"; }
                        else if (ShapeID  == 2) { Char = "因"; }
                        else if (ShapeID  == 3) { Char = "固"; }
                        else if (ShapeID  == 4) { Char = "回"; }
                        else if (ShapeID  == 5) { Char = "囮"; }
                        else if (ShapeID  == 6) { Char = "図"; }
                        else if (ShapeID  == 7) { Char = "囲"; }
                        else { Char = "迷"; }
                    }
                    else //ゴーストが居るかチェック
                        if (_pMino->CheckGhostPos(x - 1, y - 1))
                        {
                            Char = "Ｘ"; 
                        }
                    else//居ない場合は空白にしておく
                    {
                        Char = "　";
                    }
                }
                else { Char = "迷"; }

                //代入
                m_ScreenCanvas[y + AdjustPos.y][x + AdjustPos.x].Char = Char;
            }

        }
    }

}
void ScreenDrawer::SetScreenData_NextMino(GameStatus* _pGameStatus)
{
    //補正値
    Vector2D AdjustPos;
    AdjustPos.x = 18;
    AdjustPos.y = 0;

    //外枠を作る
    for (int y = 0; y < 30; ++y)
    {
        for (int x = 0; x < 6; ++x)
        {
            //配列内かチェック
            if (x + AdjustPos.x >= 0 && x + AdjustPos.x < ScreenSize.x &&
                y + AdjustPos.y >= 0 && y + AdjustPos.y < ScreenSize.y)
            {
                //代入
                m_ScreenCanvas[y + AdjustPos.y][x + AdjustPos.x].Char = "圏";
            }
        }
    }

    //次のミノ
    for (int y = 0; y < 3; ++y)
    {
        for (int x = 0; x < 4; ++x)
        {
            int MinoID = _pGameStatus->GetNextMino(0);
            string Char = "　";
            if (y == 0)
            {
                if (x == 0) { Char = "Ｎ"; }
                if (x == 1) { Char = "Ｅ"; }
                if (x == 2) { Char = "Ｘ"; }
                if (x == 3) { Char = "Ｔ"; }
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
                //代入
                m_ScreenCanvas[y + AdjustPos.y + 1][x + AdjustPos.x + 1].Char = Char;
            }

        }
    }

    //更に後ろのミノ
    //処理がほぼ同一なため繰り返す
    for (int i = 0; i < 5; ++i)
    {
        for (int y = 0; y < 4; ++y)
        {
            for (int x = 0; x < 4; ++x)
            {
                int MinoID = _pGameStatus->GetNextMino(i + 1);
                string Char = "　";
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
                    //代入
                    m_ScreenCanvas[y + AdjustPos.y + 5 + (i * 3)][x + AdjustPos.x + 1].Char = Char;
                }
            }
        }
    }
}
void ScreenDrawer::SetScreenData_HoldMino(GameStatus* _pGameStatus)
{
    //補正値
    Vector2D AdjustPos;
    AdjustPos.x = 0;
    AdjustPos.y = 0;

    //外枠を作る
    for (int y = 0; y < 5; ++y)
    {
        for (int x = 0; x < 6; ++x)
        {
            //配列内かチェック
            if (x + AdjustPos.x >= 0 && x + AdjustPos.x < ScreenSize.x &&
                y + AdjustPos.y >= 0 && y + AdjustPos.y < ScreenSize.y)
            {
                //代入
                m_ScreenCanvas[y + AdjustPos.y][x + AdjustPos.x].Char = "圏";
            }
        }
    }
    //次のミノ
    for (int y = 0; y < 3; ++y)
    {
        for (int x = 0; x < 4; ++x)
        {
            int MinoID = _pGameStatus->GetHoldMino();
            string Char = "　";
            if (y == 0)
            {
                if (x == 0) { Char = "Ｈ"; }
                if (x == 1) { Char = "Ｏ"; }
                if (x == 2) { Char = "Ｌ"; }
                if (x == 3) { Char = "Ｄ"; }
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
                //代入
                m_ScreenCanvas[y + AdjustPos.y + 1][x + AdjustPos.x + 1].Char = Char;
            }

        }
    }
}
string ScreenDrawer::GetMinoTexture(int x, int y, int _MinoID)
{
    string Char = "　";
    if (0 == _MinoID)//設定なし
    {
        if (y == 0) { if (x == 0) { Char = "　"; }if (x == 1) { Char = "　"; }if (x == 2) { Char = "　"; }if (x == 3) { Char = "　"; } }
        if (y == 1) { if (x == 0) { Char = "　"; }if (x == 1) { Char = "　"; }if (x == 2) { Char = "　"; }if (x == 3) { Char = "　"; } }
    }
    if (1 == _MinoID)//Tミノ
    {
        if (y == 0) { if (x == 0) { Char = "　"; }if (x == 1) { Char = "　"; }if (x == 2) { Char = "口"; }if (x == 3) { Char = "　"; } }
        if (y == 1) { if (x == 0) { Char = "　"; }if (x == 1) { Char = "口"; }if (x == 2) { Char = "口"; }if (x == 3) { Char = "口"; } }
    }
    if (2 == _MinoID)//Sミノ
    {
        if (y == 0) { if (x == 0) { Char = "　"; }if (x == 1) { Char = "　"; }if (x == 2) { Char = "口"; }if (x == 3) { Char = "口"; } }
        if (y == 1) { if (x == 0) { Char = "　"; }if (x == 1) { Char = "口"; }if (x == 2) { Char = "口"; }if (x == 3) { Char = "　"; } }
    }
    if (3 == _MinoID)//Zミノ
    {
        if (y == 0) { if (x == 0) { Char = "　"; }if (x == 1) { Char = "口"; }if (x == 2) { Char = "口"; }if (x == 3) { Char = "　"; } }
        if (y == 1) { if (x == 0) { Char = "　"; }if (x == 1) { Char = "　"; }if (x == 2) { Char = "口"; }if (x == 3) { Char = "口"; } }
    }
    if (4 == _MinoID)//Jミノ
    {
        if (y == 0) { if (x == 0) { Char = "　"; }if (x == 1) { Char = "口"; }if (x == 2) { Char = "　"; }if (x == 3) { Char = "　"; } }
        if (y == 1) { if (x == 0) { Char = "　"; }if (x == 1) { Char = "口"; }if (x == 2) { Char = "口"; }if (x == 3) { Char = "口"; } }
    }
    if (5 == _MinoID)//Lミノ
    {
        if (y == 0) { if (x == 0) { Char = "　"; }if (x == 1) { Char = "　"; }if (x == 2) { Char = "　"; }if (x == 3) { Char = "口"; } }
        if (y == 1) { if (x == 0) { Char = "　"; }if (x == 1) { Char = "口"; }if (x == 2) { Char = "口"; }if (x == 3) { Char = "口"; } }
    }
    if (6 == _MinoID)//Iミノ
    {
        if (y == 0) { if (x == 0) { Char = "　"; }if (x == 1) { Char = "　"; }if (x == 2) { Char = "　"; }if (x == 3) { Char = "　"; } }
        if (y == 1) { if (x == 0) { Char = "口"; }if (x == 1) { Char = "口"; }if (x == 2) { Char = "口"; }if (x == 3) { Char = "口"; } }
    }
    if (7 == _MinoID)//Oミノ
    {
        if (y == 0) { if (x == 0) { Char = "　"; }if (x == 1) { Char = "口"; }if (x == 2) { Char = "口"; }if (x == 3) { Char = "　"; } }
        if (y == 1) { if (x == 0) { Char = "　"; }if (x == 1) { Char = "口"; }if (x == 2) { Char = "口"; }if (x == 3) { Char = "　"; } }
    }
    return Char;
}
void ScreenDrawer::SetScreenData_GameStatus(GameStatus* _pGameStatus)
{
    //補正値
    Vector2D AdjustPos;
    AdjustPos.x = 0;
    AdjustPos.y = 6;

    //外枠を作る
    for (int y = 0; y < 8; ++y)
    {
        for (int x = 0; x < 6; ++x)
        {
            //配列内かチェック
            if (IsInArray(x + AdjustPos.x,y + AdjustPos.y))
            {
                //代入
                m_ScreenCanvas[y + AdjustPos.y][x + AdjustPos.x].Char = "圏";
            }
        }
    }

    //必要な変数を取得・作成
    int Level = _pGameStatus->GetLevel();
    int Score = _pGameStatus->GetScore();
    int Lines = _pGameStatus->GetLines();

    for (int i = 0; i < 4; ++i)
    {
        for (int x = 0; x < 4; ++x)
        {
            string Char = "無";
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
                if (x == 3) { Char = "　"; }//ここでは数字は表示しない
            }
            if (i == 3)
            {
                if (x == 0) { Char = AdjustDigitNum(Score / 1000000); }
                if (x == 1) { Char = AdjustDigitNum(Score / 10000); }
                if (x == 2) { Char = AdjustDigitNum(Score / 100); }
                if (x == 3) { Char = AdjustDigitNum(Score / 1); }
            }
            //配列内かチェック
            Vector2D GridPos;
            GridPos.x = AdjustPos.x + x + 1;
            GridPos.y = AdjustPos.y + i * 2 + 1;
            if (i == 3) { --GridPos.y; }//3のときだけ少しずらす
            if (IsInArray(GridPos))
            {
                //代入
                m_ScreenCanvas[GridPos.y][GridPos.x].Char = Char;
            }
        }
    }
}
void ScreenDrawer::SetScreenData_GameOver()//ゲームオーバーをキャンバスに描く
{
    //補正値
    Vector2D AdjustPos;
    AdjustPos.x = 6;
    AdjustPos.y = 0;
    for (int y = 0; y < 20; ++y)//後列から
    {
        for (int x = 0; x < 10; ++x)
        {
            //配列内かチェック
            if (IsInArray(x + 1 + AdjustPos.x, y + 1 + AdjustPos.y))
            {
                m_ScreenCanvas[y + 1 + AdjustPos.y][x + 1 + AdjustPos.x].Char = "　";
            }
        }
    }
    //ゲームオーバーを描画
    for (int x = 0; x < 8; ++x)
    {
        //配列内かチェック
        Vector2D GridPoint;
        GridPoint.x = AdjustPos.x + x + 2;
        GridPoint.y = AdjustPos.y + 5;
        if (IsInArray(GridPoint.x, GridPoint.y))
        {
            string Char = "　";
            if (x == 0) { Char = "Ｇ"; }
            if (x == 1) { Char = "Ａ"; }
            if (x == 2) { Char = "Ｍ"; }
            if (x == 3) { Char = "Ｅ"; }
            if (x == 4) { Char = "Ｏ"; }
            if (x == 5) { Char = "Ｖ"; }
            if (x == 6) { Char = "Ｅ"; }
            if (x == 7) { Char = "Ｒ"; }
            m_ScreenCanvas[GridPoint.y][GridPoint.x].Char = Char;
        }
    }
}
void ScreenDrawer::SetScreenData_Fadeout()
{
    //補正値
    Vector2D AdjustPos;
    AdjustPos.x = 6;
    AdjustPos.y = 0;

    for (int y = 21; y >= 0; --y)//後列から
    {
        for (int x = 0; x < 12; ++x)
        {
            //配列内かチェック
            if (IsInArray(x + AdjustPos.x, y + AdjustPos.y))
            {
                m_ScreenCanvas[y + AdjustPos.y][x + AdjustPos.x].Char = "口";
            }
        }
        DrawScreen();//アニメーションのためここで表示
        Sleep(20);
    }
}
void ScreenDrawer::SetScreenData_GameOverSelect(UI* _pUI)
{
    //補正値
    Vector2D AdjustPos;
    AdjustPos.x = 7;
    AdjustPos.y = 4;
    //選択項目を表示
    //Continue
    //Title
    //GameEnd
    for (int i = 0; i < 3; ++i)//選択項目が3つのため3回繰り返し
    {
        for (int x = 0; x < 5; ++x)
        {
            //配列内かチェック
            Vector2D GridPoint;
            GridPoint.x = AdjustPos.x + x + 2;
            GridPoint.y = AdjustPos.y + i * 2 + 5;
            if (IsInArray(GridPoint.x, GridPoint.y))
            {
                string Char = "　";
                if (x == 0) {
                    if (_pUI != nullptr) {
                        int SelectNum = _pUI->GetSelectNum();
                        if (SelectNum == i ) { Char = "->"; }//選択中の項目に矢印
                    }
                }
                if (x == 1) { if (i == 0) { Char = "Co"; }if (i == 1) { Char = "Ti"; }  if (i == 2) { Char = "Ga"; } }//縦読み
                if (x == 2) { if (i == 0) { Char = "nt"; }if (i == 1) { Char = "tl"; }  if (i == 2) { Char = "me"; } }
                if (x == 3) { if (i == 0) { Char = "in"; }if (i == 1) { Char = "e "; }  if (i == 2) { Char = " E"; } }
                if (x == 4) { if (i == 0) { Char = "ue"; }if (i == 1) { Char = "　"; }  if (i == 2) { Char = "nd"; } }
                m_ScreenCanvas[GridPoint.y][GridPoint.x].Char = Char;
            }
        }
    }
}
void ScreenDrawer::SetScreenData_GameTitle()
{
    ++m_TitleBackGroundSlide;//ずらす量を加算
    if (m_TitleBackGroundSlide >= 120) 
    { m_TitleBackGroundSlide = 0; }//増えすぎたら戻す

    //背景を描画(グリッド背景)
    for (int y = 0; y < 22; ++y)
    {
        for (int x = 0; x < 22; ++x)
        {
            //配列内でループを回すためチェックは省く

            string Char = "　";
            if ((x + m_TitleBackGroundSlide/20) % 6 == 0 || (y + m_TitleBackGroundSlide/20) % 6 == 0)
            {
                Char = "口";
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

    //タイトルロゴを描画
    for (int y = 0; y < 7; ++y)
    {
        for (int x = 0; x < 20; ++x)
        {
            //配列内かチェック
            Vector2D GridPoint;
            GridPoint.x = x + 1;//ずらしたい場合は補正値を与える
            GridPoint.y = y + 1;
            if (IsInArray(GridPoint.x, GridPoint.y))
            {
                string Char = "　";
                int TexturePixelNum = TitleLogoTexture[y][x];
                if (TexturePixelNum == 1) { Char = "国"; }
                if (TexturePixelNum == 2) { Char = "口"; }
                if (TexturePixelNum == 9) { Char = "--"; }
                m_ScreenCanvas[GridPoint.y][GridPoint.x].Char = Char;
            }
        }
    }
}
void ScreenDrawer::SetScreenData_GameTitleSelect(UI* _pUI)
{
    //補正値
    Vector2D AdjustPos;
    AdjustPos.x = 6;
    AdjustPos.y = 7;

    //選択項目を表示
    //GameStart
    //GameEnd
    for (int i = 0; i < 2; ++i)//選択項目が二つのため二回繰り返し
    {
        for (int x = 0; x < 6; ++x)
        {
            //配列内かチェック
            Vector2D GridPoint;
            GridPoint.x = AdjustPos.x + x + 2;
            GridPoint.y = AdjustPos.y + i * 2 + 5;
            if (IsInArray(GridPoint.x, GridPoint.y))
            {
                string Char = "　";
                if (x == 0) {
                    if (_pUI != nullptr) {
                        int SelectNum = _pUI->GetSelectNum();
                        if (SelectNum == i) { Char = "->"; }//選択中の項目に矢印
                    }
                }
                if (x == 1) { if (i == 0) { Char = "Ga"; }if (i == 1) { Char = "Ga"; } }//縦読み
                if (x == 2) { if (i == 0) { Char = "me"; }if (i == 1) { Char = "me"; } }
                if (x == 3) { if (i == 0) { Char = " S"; }if (i == 1) { Char = " E"; } }
                if (x == 4) { if (i == 0) { Char = "ta"; }if (i == 1) { Char = "nd"; } }
                if (x == 5) { if (i == 0) { Char = "rt"; }if (i == 1) { Char = "　"; } }
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
    string Result = "　";
    //三桁以上を削除
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
        //二桁目に0を表示
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
            m_ScreenCanvas[y][x].Char = "　";
        }
    }
}
void ScreenDrawer::SetScreenData_Pixel(Vector2D _Pos, string _Char)//1ピクセル毎に書き換えたい場合に
{
    //配列内かチェック
    if (IsInArray(_Pos.x, _Pos.y))
    {
        m_ScreenCanvas[_Pos.y][_Pos.x].Char = _Char;
    }
}

//Windowsで正しく動作させるための関数
void ScreenDrawer::enableAnsiEscape() {
#ifdef _WIN32
    HANDLE hOut = GetStdHandle(STD_OUTPUT_HANDLE);
    DWORD dwMode = 0;
    GetConsoleMode(hOut, &dwMode);
    SetConsoleMode(hOut, dwMode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
#endif
}