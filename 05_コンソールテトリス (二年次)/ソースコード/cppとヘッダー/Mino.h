#pragma once

#include <unordered_map>
#include "Vector2D.h"


// 前方宣言を追加
class GridField;
class GameManager;
class Input;

class Mino
{
public:
    Mino();
    ~Mino();

    void Update_Mino();//ミノの更新処理
    bool CheckBlockPos(int x, int y);//ブロックがその座標にあるかチェック
    bool CheckGhostPos(int x, int y);//落下地点がその座標にあるかチェック

    void SetGridField(GridField*  _GridField);//フィールドを設定
    void SetGameManager(GameManager* _pGameManager);//ゲームマネージャーを設定

    int GetShapeID();//形状IDを取得
    int GetScore();//スコアを取得

    void Reset(int _MinoID, int _Level);//リセット

private:

    
    void Drop();//通常落下処理
    void SideMove();//横移動処理
    void Swap();//入れ替え
    bool CheckLand(int _x, int _y);//接地チェック関数
    void Rotate();
    Vector2D RotatePos(Vector2D _pos, int _Angle);
    void SetMinoShape(int _Angle);
    bool CheckCollision(Vector2D pos);
    bool CheckCollision(int _x, int _y);
    void SuperRotation(int _NowAngle, int _NextAngle);
    Vector2D SuperRotationPosIndex(int Step, int _NowAngle, int _NextAngle);


    GridField* m_pGridField;
    GameManager* m_pGameManager;

    Vector2D ShapeIndex(int _BlockNum,int _Angle);//形状辞書

    Vector2D m_CenterPos;
    Vector2D m_BlockPos[4];

    int m_ShapeID;//形状番号
    int m_Angle;//角度

    int m_DropTimer;//落下時間計測
    int m_DropTime;//落下時間

    int m_ScorePoint;//スコアを持つ

    bool m_SwapFlag;//入れ替え可能かのフラグ

    Input* input;//入力受け付け用
};
