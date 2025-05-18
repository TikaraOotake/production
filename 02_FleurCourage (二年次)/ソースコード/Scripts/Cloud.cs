using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public GameObject player;//プレイヤーの情報
    public float[] oldPlayerPositionX = new float[11];//プレイヤーの過去のX座標を入れる変数
    public float[] oldPlayerPositionY = new float[11];//プレイヤーの過去のY座標を入れる変数
    public float TargetPosX;
    public float MaxTargetPosX;
    public float PositionX;
    public float PositionY;

    public float RepeatYPos;
    public bool RepeatFlag;
    public int i;

    public float AttackPosX;
    public float AttackPosY;
    public bool PlayerWay;

    enum Status
    {
        Fine, //通常
        Order,//攻撃命令
        Attack,//攻撃
    }
    Status st;

    public float moveTime;          //何秒間同じ方向に移動し続けたか
    float differencePos;//現在の座標と過去の座標の差

    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.Find("Player");
        oldPlayerPositionX[0] = player.transform.position.x;
        oldPlayerPositionY[0] = player.transform.position.y;
        for (int i = 0; i < 11; ++i)
        {
            oldPlayerPositionX[i] = player.transform.position.x;
        }
        PositionX = player.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        oldPlayerPositionX[0] = player.transform.position.x;
        oldPlayerPositionY[0] = player.transform.position.y;





        oldPlayerPositionX[10] = oldPlayerPositionX[9];
        oldPlayerPositionX[9] = oldPlayerPositionX[8];
        oldPlayerPositionX[8] = oldPlayerPositionX[7];
        oldPlayerPositionX[7] = oldPlayerPositionX[6];
        oldPlayerPositionX[6] = oldPlayerPositionX[5];
        oldPlayerPositionX[5] = oldPlayerPositionX[4];
        oldPlayerPositionX[4] = oldPlayerPositionX[3];
        oldPlayerPositionX[3] = oldPlayerPositionX[2];
        oldPlayerPositionX[2] = oldPlayerPositionX[1];
        oldPlayerPositionX[1] = oldPlayerPositionX[0];

        //プレイヤーが右に進んでいたら
        if (oldPlayerPositionX[10] < oldPlayerPositionX[0])
        {
            moveTime -= Time.deltaTime;
        }
        else
        //プレイヤーが左に進んでいたら
        if (oldPlayerPositionX[10] > oldPlayerPositionX[0])
        {
            moveTime += Time.deltaTime;
        }
        else
        {
            moveTime = 0.0f;
        }

        //現在の座標と過去の座標の差が一定以内なら
        differencePos = oldPlayerPositionX[0] - oldPlayerPositionX[10];
        if (differencePos < 0.1f && differencePos > -0.1f)
        {
            moveTime = 0.0f;
        }

        //プレイヤーが右にN秒間進み続けていたら
        if (moveTime > 0.3f)
        {
            //chaseCamPositionX += 0.01f;
            TargetPosX += moveTime / 10.0f;

        }
        //プレイヤーが左にN秒間進み続けていたら
        if (moveTime < -0.3f)
        {
            //chaseCamPositionX -= 0.01f;
            TargetPosX += moveTime / 10.0f;
        }

        //上限距離を超えたらそれ以上増えないようにする
        if (TargetPosX > MaxTargetPosX)
        {
            TargetPosX = MaxTargetPosX;
        }
        if (TargetPosX < -MaxTargetPosX)
        {
            TargetPosX = -MaxTargetPosX;
        }

        //Y座標の往復運動

        if (i >= 2260)
        {
            i = 0;
        }

        RepeatYPos = Mathf.Sin((float)(i) / 360.0f);

        switch (st)
        {
            //「通常」
            case Status.Fine:
                {
                    i += 10;
                    PositionX += ((player.transform.position.x + TargetPosX) - PositionX) / 10.0f;
                    PositionY += ((player.transform.position.y + 1.0f + RepeatYPos / 10.0f) - PositionY) / 10.0f;
                }
                break;
            //「攻撃命令」
            case Status.Order:
                {
                    AttackPosX = player.transform.position.x;
                    AttackPosY = player.transform.position.y;
                    st = Status.Attack;
                }
                break;
            //「攻撃」
            case Status.Attack:
                {
                    i += 60;

                    PositionY += ((AttackPosY + 2.0f) - PositionY+ RepeatYPos/5.0f) / 3.0f;
                    
                    if (PlayerWay == true)
                    {
                        PositionX += ((AttackPosX - 2.0f) - PositionX) / 3.0f;
                    }
                    if (PlayerWay == false)
                    {
                        PositionX += ((AttackPosX + 2.0f) - PositionX) / 3.0f;
                    }
                }
                break;

                
        }
        transform.position = new Vector2(PositionX, PositionY);
                

    }
    public void UpdateGameObject(GameObject Object)
    {
        player = Object;
    }
    public void UpdateStatus(int StatusID)
    {
        switch (StatusID)
        {
            //「通常」
            case 0:
                {
                    st = Status.Fine;
                }
                break;
            //「攻撃命令」
            case 1:
                {
                    st = Status.Order;
                }
                break;
            //「攻撃」
            case 2:
                {
                    st = Status.Attack;
                }
                break;
        }
    }
    public void UpdateWay(bool Way)
    {
        PlayerWay=Way;
    }
    public void DestroyThisObject()
    {
        Destroy(this.gameObject);
    }
}
