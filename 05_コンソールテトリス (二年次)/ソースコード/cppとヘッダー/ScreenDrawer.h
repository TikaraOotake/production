#pragma once

#include "Vector2D.h"

#include <iostream>
using namespace std;

struct GridData {
    string Char;
};

// 前方宣言を追加
class Mino;
class GridField;
class GameStatus;
class UI;

class ScreenDrawer
{
public:
    ScreenDrawer();
    ~ScreenDrawer();

    void DrawField(GridField* _pGridField, Mino* _pMino);
    void DrawScreen();
    void SetScreenData_Initialize();//まっさらにする
    void SetScreenData_Pixel(Vector2D _Pos,string _Char);//1ピクセル毎に書き換えたい場合に
    void SetScreenData_Field(GridField* _pGridField, Mino* _pMino);
    void SetScreenData_NextMino(GameStatus* _pGameStatus);
    void SetScreenData_HoldMino(GameStatus* _pGameStatus);
    void SetScreenData_GameStatus(GameStatus* _pGameStatus);
    void SetScreenData_Fadeout();
    void SetScreenData_GameOver();
    void SetScreenData_GameOverSelect(UI* _pUI);
    void SetScreenData_GameTitle();
    void SetScreenData_GameTitleSelect(UI* _pUI);
    
    void enableAnsiEscape();// Windowsでエスケープコードを有効化
private:
    Vector2D const ScreenSize = {50,22};
    
    //string型をそのまま行列にできないため構造体を噛ます
    GridData m_ScreenCanvas[22][50];
    int m_TitleBackGroundSlide;

    bool IsInArray(Vector2D _pos);
    bool IsInArray(int _x, int _y);

    void moveCursorTo(int x, int y);
    void SetCursorVisibility(bool visible);
    string GetMinoTexture(int x, int y, int _MinoID);

    string AdjustDigitNum(int _Num);//桁を調節して文字列として返す

    
};

