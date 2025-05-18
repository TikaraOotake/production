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
    //念のため
    Reset(1,1);
}

Mino::~Mino()
{
    if (input != nullptr) { delete input; input = nullptr; }
}
void Mino::Update_Mino()
{
    //左右移動
    SideMove();

    //回転
    Rotate();

    //保持ミノと入れ替え
    Swap();

    //通常落下
    Drop();
}
void Mino::Reset(int _MinoID,int _Level)
{
    //角度をセット
    m_Angle = 0;
    //中心座標をセット
    m_CenterPos.x = 5;
    m_CenterPos.y = 0;

    //スコアをセット
    m_ScorePoint = 0;

    //周囲のブロックの座標をセット

    //ミノの形を決める
    m_ShapeID = _MinoID;
    //Iミノの場合補正を加える
    if (_MinoID == 6) { 
        ++m_Angle; 
        --m_CenterPos.y;
    }
     SetMinoShape(m_Angle);


    //落下間隔の倍率を計算
    float ratio = 1.0f;
    for (int i = 0; i < _Level + 1; ++i)
    {
        ratio *= 0.85f;//レベル分累乗して減らす
    }

    //落下間隔をセット
    m_DropTime = int(20.0f * ratio);
    //落下計測時間をリセット
    m_DropTimer = 0;

    //地形と重なって居るかを確認してゲームオーバーを判定
    if (CheckCollision(0,0))
    {
        m_pGameManager->Event_Dead();
    }
}
void Mino::SideMove()
{
    //左
    if (input->GetKeyDown('A') || (input->GetKeyDown(VK_NUMPAD4)) ||
        input->GetKeyKeep('A', 3) || input->GetKeyKeep(VK_NUMPAD4, 3))
    {
        //移動可能かチェック
        if (!CheckCollision(-1, 0))
        {
            m_CenterPos.x += -1;
        }


    }
    //右
    if (input->GetKeyDown('D') || (input->GetKeyDown(VK_NUMPAD6)) ||
        input->GetKeyKeep('D', 3) || input->GetKeyKeep(VK_NUMPAD6, 3))
    {
        //移動可能かチェック
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
    //ソフトドロップ
    if (input->GetKey('S') || input->GetKey(VK_NUMPAD5) || input->GetKey(VK_NUMPAD2))
    {
        m_DropTimer += 20;
    }
    else
    {
        ++m_DropTimer;
    }

    //ハードドロップ
    if (input->GetKeyDown('W') || (input->GetKeyDown(VK_NUMPAD8)))
    {
        for (int i = 0; i < 20; ++i)
        {
            //落下可能かチェック
            if (CheckCollision(0, 1))
            {
                //接地
                for (int i = 0; i < 4; ++i)
                {
                    m_pGridField->SetGridData(m_CenterPos.x + m_BlockPos[i].x, m_CenterPos.y + m_BlockPos[i].y, m_ShapeID);
                }

                //マネージャーに着地したことを伝える
                if (m_pGameManager != nullptr) { m_pGameManager->Event_LandMino(); }

                //スワップフラグをリセット
                m_SwapFlag = true;

                //関数を終了
                return;
            }
            else
            {
                //落下
                m_CenterPos.y += 1;
                //スコアを加算
                m_ScorePoint+=2;
            }
            m_DropTimer = 0;
        }
    }
    else
        if (m_DropTimer >= m_DropTime)
        {
            //落下可能かチェック
            if (CheckCollision(0, 1))
            {
                //接地
                for (int i = 0; i < 4; ++i)
                {
                    m_pGridField->SetGridData(m_CenterPos.x + m_BlockPos[i].x, m_CenterPos.y + m_BlockPos[i].y, m_ShapeID);
                }

                //グリッドフィールドへ削除チェックを依頼
                m_pGridField->CheckDeleteLine();

                //マネージャーに着地したことを伝える
                if (m_pGameManager != nullptr) { m_pGameManager->Event_LandMino(); }

                //スワップフラグをリセット
                m_SwapFlag = true;
            }
            else
            {
                //落下
                m_CenterPos.y += 1;

                //ソフトドロップの場合スコアを加算
                if (input->GetKey('S') || input->GetKey(VK_NUMPAD5) || input->GetKey(VK_NUMPAD2))
                {
                    m_ScorePoint+=2;
                }
            }
            m_DropTimer = 0;
        }
}
Vector2D Mino::ShapeIndex(int _BlockNum, int _Angle)//引数　ブロック番号　角度
{
    Vector2D pos;
    pos.x = 0;
    pos.y = 0;

    //　■　 1
    //■■■203
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

    //　■■ 12
    //■■　30
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

    //■■　12
    //　■■ 03
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

    //■　　1
    //■■■203
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

    //　　■
    //■■■
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
    //■■■■1023
    if (m_ShapeID == 6)
    {
        //Iミノだけ回転が特殊なため追加
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

    //■■
    //■■
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
            //座標にブロックがあればtrue
            return true;
        }
    }

    //座標にブロックがなければfalse
    return false;
}
bool Mino::CheckGhostPos(int x, int y)
{
    Vector2D NowPos = m_CenterPos;
    bool result = false;

    //もっとも下まで落下させる
    for (int i = 0; i < 20; ++i)
    {
        //落下可能かチェック
        if (CheckCollision(0, 1))
        {
            break;
        }
        else
        {
            //落下
            m_CenterPos.y += 1;
        }
    }

    //4個のブロックをそれぞれチェック
    for (int i = 0; i < 4; ++i)
    {
        if (m_CenterPos.x + m_BlockPos[i].x == x && m_CenterPos.y + m_BlockPos[i].y == y)
        {
            //座標にブロックがあればtrue
            result= true;
        }
    }
    //座標を元に戻す
    m_CenterPos = NowPos;

    //座標にブロックがなければfalse
    return result;
}
bool Mino::CheckLand(int _x, int _y)//指定した先がブロックかチェック
{
    bool flag = false;
    //有効かチェック
    if (m_pGridField == nullptr)
    {
        //ポインターが有効でない為終了
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
    //時計周り
    if (input->GetKeyDown('C') || input->GetKeyDown('E') || (input->GetKeyDown(VK_NUMPAD3)) || (input->GetKeyDown(VK_NUMPAD9)) || input->GetKeyDown(VK_SPACE))
    {
        --NextAngle;
    }
    //半時計周り
    if (input->GetKeyDown('Z') || input->GetKeyDown('Q') || (input->GetKeyDown(VK_NUMPAD1)) || (input->GetKeyDown(VK_NUMPAD7)) || input->GetKeyDown(VK_LSHIFT))
    {
        ++NextAngle;
    }

    //回転する
    if (NextAngle != m_Angle)
    {
        //角度がオーバーしている場合補正
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
    //指定された座標で地形とぶつかっているか診断
    for (int i = 0; i < 4; ++i)
    {
        if (CheckLand(m_CenterPos.x + m_BlockPos[i].x + _x, m_CenterPos.y + m_BlockPos[i].y + _y))
        {
            return true;
        }
    }

    //衝突が見られないのでfalse
    return false;
}
bool Mino::CheckCollision(Vector2D _pos)
{
    //指定された座標で地形とぶつかっているか診断
    for (int i = 0; i < 4; ++i)
    {
        if (CheckLand(m_CenterPos.x + m_BlockPos[i].x + _pos.x, m_CenterPos.y + m_BlockPos[i].y + _pos.y))
        {
            return true;
        }
    }

    //衝突が見られないのでfalse
    return false;
}
Vector2D Mino::RotatePos(Vector2D _pos, int _Angle)
{
    if (_Angle == 0)
    {
        //何もしない
    }
    if (_Angle == 1)
    {
        //数値を入れ替える
        int SwapNum = _pos.x;
        _pos.x = _pos.y;
        _pos.y = SwapNum;

        //Y軸を反転
        _pos.y *= -1;
    }
    if (_Angle == 2)
    {
        //符号を反転
        _pos.x *= -1;
        _pos.y *= -1;
    }
    if (_Angle == 3)
    {
        //数値を入れ替える
        int SwapNum = _pos.x;
        _pos.x = _pos.y;
        _pos.y = SwapNum;

        //X軸を反転
        _pos.x *= -1;

    }
    return _pos;
}
void Mino::SetMinoShape(int _Angle)
{
    for (int i = 0; i < 4; ++i)
    {
        //形状を決定
        m_BlockPos[i] = ShapeIndex(i, _Angle);
        //回転を加える
        m_BlockPos[i] = RotatePos(m_BlockPos[i], _Angle);
    }
}
Vector2D Mino::SuperRotationPosIndex(int Step, int _NowAngle, int _NextAngle)
{
    //段階と回転方向に合わせて補正値を返します

    Vector2D pos;
    pos.x = 0;
    pos.y = 0;

    //Iミノ用
    if (m_ShapeID == 6)
    {
        if (_NowAngle == 3 && _NextAngle == 0 ||
            _NowAngle == 2 && _NextAngle == 1 ||
            _NowAngle == 0 && _NextAngle == 3 ||
            _NowAngle == 1 && _NextAngle == 2)
        {
            //Iミノ基本SuperRotation座標その1
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
            //Iミノ基本SuperRotation座標その2
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

        //補正
        if (_NowAngle == 0 && _NextAngle == 3 ||
            _NowAngle == 1 && _NextAngle == 2 ||
            _NowAngle == 2 && _NextAngle == 3 ||
            _NowAngle == 1 && _NextAngle == 0)
        {
            //両軸に対して反転
            pos.x *= -1;
            pos.y *= -1;
        }
    }
    else
        //それ以外
    {
        //基本SuperRotation座標
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

        //回転方向に合わせて補正
        if (_NextAngle == 1)
        {
            //何もしない
        }
        else
            if (_NextAngle == 3)
            {
                //X軸に対して反転
                pos.x *= -1;
            }
            else
                if (_NowAngle == 1)
                {
                    //両軸に対して反転
                    pos.x *= -1;
                    pos.y *= -1;
                }
                else
                    if (_NowAngle == 3)
                    {
                        //Y軸に対して反転
                        pos.y *= -1;
                    }
    }

    return pos;
}
void Mino::SuperRotation(int _NowAngle, int _NextAngle)
{
    //Oミノ以外を回転させる
    if (!(m_ShapeID == 7))
    {
        //仮で回転させる
        SetMinoShape(_NextAngle);

        for (int i = 0; i < 5; ++i)
        {
            //補正座標を取得
            Vector2D JustPos = SuperRotationPosIndex(i, _NowAngle, _NextAngle);
            if (CheckCollision(JustPos) == false)
            {
                //問題がなければ代入
                m_Angle = _NextAngle;
                //ミノの座標も補正した座標に合わせる
                m_CenterPos.x += JustPos.x;
                m_CenterPos.y += JustPos.y;

                //関数を終了
                return;
            }
        }

        //回転の際に空いてる座標がなかった為もとに戻しておく
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
