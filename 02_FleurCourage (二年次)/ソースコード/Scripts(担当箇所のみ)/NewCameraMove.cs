using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCameraMove : MonoBehaviour
{
    public GameObject player;//プレイヤーの情報
    public Vector2 PlayerPos;//プレイヤーの座標を入れる
    public float oldPlayerPositionX;//プレイヤーの過去の座標を入れる変数
    public float moveTime;          //何秒間同じ方向に移動し続けたか
    public float chaseCamPositionX; //カメラの追跡する座標
    public float CamPositionX; //カメラの視点座標
    public float CamPositionY; //カメラの視点座標
    public float MaxDistanceCamPositionX; //カメラの最大視点距離
    float differencePos;//現在の座標と過去の座標の差


    public float Angle;//現在の角度
    public float Radius;//半径
    public float VibrationTime;//振動時間
    public float VibrationInterval;//振動間隔
    public float RotateSpeed;//回転速度
    public float RotatePosX;//回転X座標
    public float RotatePosY;//回転Y座標
    public bool VibrationFlag;//振動しているか

    public bool CameraLockFlag;//カメラを定位置に固定するかのフラグ
    public Vector2 CameraLockPos;//カメラの固定する座標

    // Start is called before the first frame update
    void Start()
    {
        if (player != null)
        {
            oldPlayerPositionX = player.transform.position.x;
        }

        CameraLockFlag = false;

        //未設定なら代入
        if (RotateSpeed == 0)
        {
            RotateSpeed = 400;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            PlayerPos = player.transform.position;
        }

        //テスト
        if (Input.GetKeyDown(KeyCode.T))
        {
            VibrationTime = 2.0f;
            Radius = 0.2f;
            VibrationFlag = true;
        }
        RotateVibration();

        //プレイヤーが右に進んでいたら
        if (oldPlayerPositionX < PlayerPos.x)
        {
            if (moveTime < 0.0f)
            {
                moveTime = 0.0f;
            }
            moveTime += Time.deltaTime;
        }
        else
        //プレイヤーが左に進んでいたら
        if (oldPlayerPositionX > PlayerPos.x)
        {
            if (moveTime > 0.0f)
            {
                moveTime = 0.0f;
            }
            moveTime -= Time.deltaTime;
        }
        else
        {
            moveTime = 0.0f;
        }

        //現在の座標と過去の座標の差が一定以内なら
        differencePos = PlayerPos.x - oldPlayerPositionX;
        if (differencePos < 0.08f && differencePos > -0.08f)
        {
            moveTime = 0.0f;
        }

        //プレイヤーが右にN秒間進み続けていたら
        if (moveTime > 0.1f)
        {
            //chaseCamPositionX += 0.01f;
            chaseCamPositionX += moveTime / 20.0f;

        }
        //プレイヤーが左にN秒間進み続けていたら
        if (moveTime < -0.1f)
        {
            //chaseCamPositionX -= 0.01f;
            chaseCamPositionX += moveTime / 30.0f;
        }

        //上限距離を超えたらそれ以上増えないようにする
        if (chaseCamPositionX > MaxDistanceCamPositionX)
        {
            chaseCamPositionX = MaxDistanceCamPositionX;
        }
        if (chaseCamPositionX < -MaxDistanceCamPositionX)
        {
            chaseCamPositionX = -MaxDistanceCamPositionX;
        }

        //ロックフラグがtrueだった場合カメラを固定
        if (CameraLockFlag)
        {

            CamPositionX += (CameraLockPos.x - (CameraLockPos.x - PlayerPos.x) * 0.05f - CamPositionX) / 30.0f;
            CamPositionY += (CameraLockPos.y - (CameraLockPos.y - PlayerPos.y) * 0.05f - CamPositionY) / 30.0f;
        }
        else//通常時の処理
        {
            //現在座標　　+=　　　(目標座標　+　補正値)　-　現在座標　　/　　係数
            CamPositionX += ((PlayerPos.x + chaseCamPositionX) - CamPositionX) / 4.0f;
            CamPositionY += ((PlayerPos.y + 1.5f) - CamPositionY) / 20.0f;
        }




        transform.position = new Vector3(CamPositionX + RotatePosX, CamPositionY - RotatePosY, -10.0f);


        oldPlayerPositionX = PlayerPos.x;
    }
    void RotateVibration()
    {
        if (VibrationFlag)
        {
            //角度をループさせる
            if (Angle > 2260)
            {
                Angle = 0;
            }
            if (Angle < 0)
            {
                Angle = 2260;
            }

            //角度と半径から座標を算出
            RotatePosY = Mathf.Sin((float)(Angle) / 360.0f) * Radius;
            RotatePosX = Mathf.Cos((float)(Angle) / 360.0f) * Radius;

            //角度を増加させる
            Angle += RotateSpeed;
            //振動時間を経過させる
            VibrationTime -= Time.deltaTime;
            if (VibrationTime <= 0.0f)
            {
                VibrationTime = 0.0f;
                RotatePosY = 0.0f;
                RotatePosX = 0.0f;
                VibrationFlag = false;
            }
            //半径を小さくしていく
            Radius = Radius / 1.08f;
        }
    }

    /// <summary>
    ///関数名：VibrationCamera()<br />
    ///引数名：振動時間　振動半径	<br />
    ///戻り値：なし <br />
    ///用　途：カメラを揺らす
    /// </summary>
    public void VibrationCamera(float SetVibrationTime, float SetRadius)
    {
        //振動時間と振動半径を設定
        VibrationTime = SetVibrationTime;
        Radius = SetRadius;

        //フラグをtrueに
        VibrationFlag = true;
    }

    /// <summary>
    ///関数名：SetPlayerObject()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：カメラを設定
    /// </summary>
    public void SetPlayerObject(GameObject SetPlayer)
    {
        player = SetPlayer;
    }
    /// <summary>
    ///関数名：GetPlayerObject()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：カメラを取得
    /// </summary>
    public GameObject GetPlayerObject()
    {
        return player;
    }

    /// <summary>
    ///関数名：CameraLock()<br />
    ///引数名：座標ｘ、ｙ	<br />
    ///戻り値：なし <br />
    ///用　途：カメラを特定の位置に固定し続ける
    /// </summary>
    public void CameraLock(float x, float y)
    {
        CameraLockFlag = true;
        CameraLockPos = new Vector2(x, y);
    }

    /// <summary>
    ///関数名：CameraUnlock()<br />
    ///引数名：なし<br />
    ///戻り値：なし <br />
    ///用　途：なし
    /// </summary>
    public void CameraUnlock()
    {
        CameraLockFlag = false;
    }

    public Vector2 GetPlayerPos()
    {
        return player.transform.position;
    }
}
