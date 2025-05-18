using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCameraMove : MonoBehaviour
{
    public GameObject player;//�v���C���[�̏��
    public Vector2 PlayerPos;//�v���C���[�̍��W������
    public float oldPlayerPositionX;//�v���C���[�̉ߋ��̍��W������ϐ�
    public float moveTime;          //���b�ԓ��������Ɉړ�����������
    public float chaseCamPositionX; //�J�����̒ǐՂ�����W
    public float CamPositionX; //�J�����̎��_���W
    public float CamPositionY; //�J�����̎��_���W
    public float MaxDistanceCamPositionX; //�J�����̍ő压�_����
    float differencePos;//���݂̍��W�Ɖߋ��̍��W�̍�


    public float Angle;//���݂̊p�x
    public float Radius;//���a
    public float VibrationTime;//�U������
    public float VibrationInterval;//�U���Ԋu
    public float RotateSpeed;//��]���x
    public float RotatePosX;//��]X���W
    public float RotatePosY;//��]Y���W
    public bool VibrationFlag;//�U�����Ă��邩

    public bool CameraLockFlag;//�J�������ʒu�ɌŒ肷�邩�̃t���O
    public Vector2 CameraLockPos;//�J�����̌Œ肷����W

    // Start is called before the first frame update
    void Start()
    {
        if (player != null)
        {
            oldPlayerPositionX = player.transform.position.x;
        }

        CameraLockFlag = false;

        //���ݒ�Ȃ���
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

        //�e�X�g
        if (Input.GetKeyDown(KeyCode.T))
        {
            VibrationTime = 2.0f;
            Radius = 0.2f;
            VibrationFlag = true;
        }
        RotateVibration();

        //�v���C���[���E�ɐi��ł�����
        if (oldPlayerPositionX < PlayerPos.x)
        {
            if (moveTime < 0.0f)
            {
                moveTime = 0.0f;
            }
            moveTime += Time.deltaTime;
        }
        else
        //�v���C���[�����ɐi��ł�����
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

        //���݂̍��W�Ɖߋ��̍��W�̍������ȓ��Ȃ�
        differencePos = PlayerPos.x - oldPlayerPositionX;
        if (differencePos < 0.08f && differencePos > -0.08f)
        {
            moveTime = 0.0f;
        }

        //�v���C���[���E��N�b�Ԑi�ݑ����Ă�����
        if (moveTime > 0.1f)
        {
            //chaseCamPositionX += 0.01f;
            chaseCamPositionX += moveTime / 20.0f;

        }
        //�v���C���[������N�b�Ԑi�ݑ����Ă�����
        if (moveTime < -0.1f)
        {
            //chaseCamPositionX -= 0.01f;
            chaseCamPositionX += moveTime / 30.0f;
        }

        //��������𒴂����炻��ȏ㑝���Ȃ��悤�ɂ���
        if (chaseCamPositionX > MaxDistanceCamPositionX)
        {
            chaseCamPositionX = MaxDistanceCamPositionX;
        }
        if (chaseCamPositionX < -MaxDistanceCamPositionX)
        {
            chaseCamPositionX = -MaxDistanceCamPositionX;
        }

        //���b�N�t���O��true�������ꍇ�J�������Œ�
        if (CameraLockFlag)
        {

            CamPositionX += (CameraLockPos.x - (CameraLockPos.x - PlayerPos.x) * 0.05f - CamPositionX) / 30.0f;
            CamPositionY += (CameraLockPos.y - (CameraLockPos.y - PlayerPos.y) * 0.05f - CamPositionY) / 30.0f;
        }
        else//�ʏ펞�̏���
        {
            //���ݍ��W�@�@+=�@�@�@(�ڕW���W�@+�@�␳�l)�@-�@���ݍ��W�@�@/�@�@�W��
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
            //�p�x�����[�v������
            if (Angle > 2260)
            {
                Angle = 0;
            }
            if (Angle < 0)
            {
                Angle = 2260;
            }

            //�p�x�Ɣ��a������W���Z�o
            RotatePosY = Mathf.Sin((float)(Angle) / 360.0f) * Radius;
            RotatePosX = Mathf.Cos((float)(Angle) / 360.0f) * Radius;

            //�p�x�𑝉�������
            Angle += RotateSpeed;
            //�U�����Ԃ��o�߂�����
            VibrationTime -= Time.deltaTime;
            if (VibrationTime <= 0.0f)
            {
                VibrationTime = 0.0f;
                RotatePosY = 0.0f;
                RotatePosX = 0.0f;
                VibrationFlag = false;
            }
            //���a�����������Ă���
            Radius = Radius / 1.08f;
        }
    }

    /// <summary>
    ///�֐����FVibrationCamera()<br />
    ///�������F�U�����ԁ@�U�����a	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�J������h�炷
    /// </summary>
    public void VibrationCamera(float SetVibrationTime, float SetRadius)
    {
        //�U�����ԂƐU�����a��ݒ�
        VibrationTime = SetVibrationTime;
        Radius = SetRadius;

        //�t���O��true��
        VibrationFlag = true;
    }

    /// <summary>
    ///�֐����FSetPlayerObject()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�J������ݒ�
    /// </summary>
    public void SetPlayerObject(GameObject SetPlayer)
    {
        player = SetPlayer;
    }
    /// <summary>
    ///�֐����FGetPlayerObject()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�J�������擾
    /// </summary>
    public GameObject GetPlayerObject()
    {
        return player;
    }

    /// <summary>
    ///�֐����FCameraLock()<br />
    ///�������F���W���A��	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�J���������̈ʒu�ɌŒ肵������
    /// </summary>
    public void CameraLock(float x, float y)
    {
        CameraLockFlag = true;
        CameraLockPos = new Vector2(x, y);
    }

    /// <summary>
    ///�֐����FCameraUnlock()<br />
    ///�������F�Ȃ�<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�Ȃ�
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
