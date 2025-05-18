using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public GameObject player;//�v���C���[�̏��
    public float[] oldPlayerPositionX = new float[11];//�v���C���[�̉ߋ���X���W������ϐ�
    public float[] oldPlayerPositionY = new float[11];//�v���C���[�̉ߋ���Y���W������ϐ�
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
        Fine, //�ʏ�
        Order,//�U������
        Attack,//�U��
    }
    Status st;

    public float moveTime;          //���b�ԓ��������Ɉړ�����������
    float differencePos;//���݂̍��W�Ɖߋ��̍��W�̍�

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

        //�v���C���[���E�ɐi��ł�����
        if (oldPlayerPositionX[10] < oldPlayerPositionX[0])
        {
            moveTime -= Time.deltaTime;
        }
        else
        //�v���C���[�����ɐi��ł�����
        if (oldPlayerPositionX[10] > oldPlayerPositionX[0])
        {
            moveTime += Time.deltaTime;
        }
        else
        {
            moveTime = 0.0f;
        }

        //���݂̍��W�Ɖߋ��̍��W�̍������ȓ��Ȃ�
        differencePos = oldPlayerPositionX[0] - oldPlayerPositionX[10];
        if (differencePos < 0.1f && differencePos > -0.1f)
        {
            moveTime = 0.0f;
        }

        //�v���C���[���E��N�b�Ԑi�ݑ����Ă�����
        if (moveTime > 0.3f)
        {
            //chaseCamPositionX += 0.01f;
            TargetPosX += moveTime / 10.0f;

        }
        //�v���C���[������N�b�Ԑi�ݑ����Ă�����
        if (moveTime < -0.3f)
        {
            //chaseCamPositionX -= 0.01f;
            TargetPosX += moveTime / 10.0f;
        }

        //��������𒴂����炻��ȏ㑝���Ȃ��悤�ɂ���
        if (TargetPosX > MaxTargetPosX)
        {
            TargetPosX = MaxTargetPosX;
        }
        if (TargetPosX < -MaxTargetPosX)
        {
            TargetPosX = -MaxTargetPosX;
        }

        //Y���W�̉����^��

        if (i >= 2260)
        {
            i = 0;
        }

        RepeatYPos = Mathf.Sin((float)(i) / 360.0f);

        switch (st)
        {
            //�u�ʏ�v
            case Status.Fine:
                {
                    i += 10;
                    PositionX += ((player.transform.position.x + TargetPosX) - PositionX) / 10.0f;
                    PositionY += ((player.transform.position.y + 1.0f + RepeatYPos / 10.0f) - PositionY) / 10.0f;
                }
                break;
            //�u�U�����߁v
            case Status.Order:
                {
                    AttackPosX = player.transform.position.x;
                    AttackPosY = player.transform.position.y;
                    st = Status.Attack;
                }
                break;
            //�u�U���v
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
            //�u�ʏ�v
            case 0:
                {
                    st = Status.Fine;
                }
                break;
            //�u�U�����߁v
            case 1:
                {
                    st = Status.Order;
                }
                break;
            //�u�U���v
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
