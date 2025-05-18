using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprite : MonoBehaviour
{
    public GameObject Player;//�v���C���[�̃I�u�W�F�N�g��ۑ�����
    public bool VibFlag;//�U��������t���O
    public float VibTimer;//�U������
    public float VibAttenuation;//�U�������x
    public float AmplitudeX;//X���̐U��
    public float AmplitudeY;//Y���̐U��
    public float VivSpeed;//�U�����x
    public float Angle;//���݂̊p�x
    public float RadiusX;//���a
    public float RadiusY;//���a

    public int Animation;//�m�F�p

    public bool FlickerFlag;//���Ńt���O
    public float FlickerTimer;//���Ŏ���

    SpriteRenderer _sr; //SpriteRenderer�̊i�[�p
    Rigidbody2D _rb; //Rigidbody2D�̊i�[�p
    Animator _animator; //Animator�̊i�[�p



    // Start is called before the first frame update
    void Start()
    {
        //�A�^�b�`����Ă���Component���擾���Ă���
        _sr = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        Angle = 1130;

        //���g�̃I�u�W�F�N�g�𓧖���
        GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //���g�̃I�u�W�F�N�g��s������
        GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);

        if (FlickerFlag)
        {
            if (FlickerTimer <= 0.0f)
            {
                GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
                FlickerTimer = 0.0f;
                FlickerFlag = false;
            }

            if ((int)(FlickerTimer * 15.0f) % 2 == 1)
            {
                GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 128);
            }
            else
            {
                GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
            }

            FlickerTimer -= Time.deltaTime;
        }

        if (VibFlag)
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
            AmplitudeY = Mathf.Sin((float)(Angle) / 360.0f) * RadiusY;
            AmplitudeX = Mathf.Cos((float)(Angle) / 360.0f) * RadiusX;

            //�p�x�𑝉�������
            Angle += VivSpeed;
            //�U�����Ԃ��o�߂�����
            VibTimer -= Time.deltaTime;
            if (VibTimer <= 0.0f)
            {
                VibTimer = 0.0f;
                AmplitudeY = 0.0f;
                AmplitudeX = 0.0f;
                VibFlag = false;
            }
            //���a�����������Ă���
            RadiusX = RadiusX / VibAttenuation;
            RadiusY = RadiusY / VibAttenuation;


        }
        if (Player != null)
        {
            transform.position = new Vector3(Player.transform.position.x + AmplitudeX, Player.transform.position.y - AmplitudeY);
        }
    }
    /// <summary>
    ///�֐����FUpdatePlayerObject()<br />
    ///�������F�v���C���[�̃I�u�W�F�N�g<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�v���C���[�̃I�u�W�F�N�g���擾����@<br />
    /// </summary>
    public void UpdatePlayerObject(GameObject Object)
    {
        Player = Object;
    }

    /// <summary>
    ///�֐����FUpdateVibStatus()<br />
    ///�������F�U������ �U�������x(����1.08f) ���aX ���aY �U�����x<br />
    ///������F0.1f, 1.2f, 0.3f, 0.3f, 500.0f�@<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�U�������X�V����֐��@<br />
    /// </summary>
    public void UpdateVibStatus(float Timer, float Attenuation, float SetRadiusX, float SetRadiusY, float SetVivSpeed)
    {
        VibFlag = true;
        VibTimer = Timer;           //�U�����Ԃ�ݒ�
        VibAttenuation = Attenuation;//�U�������x��ݒ�
        VivSpeed = SetVivSpeed;      //�U�����x��ݒ�
        RadiusX = SetRadiusX;        //�U�����aX��ݒ�
        RadiusY = SetRadiusY;        //�U�����aY��ݒ�
    }
    public void UpdateAnimation(int PlayAnimation)
    {
        _animator.SetInteger("Action", PlayAnimation);

        Animation = PlayAnimation;

    }

    /// <summary>
    ///�֐����FUpdatePlayerWey()<br />
    ///�������F�v���C���[�̕���<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�v���C���[�̕������X�V����֐��@<br />
    /// </summary>
    public void UpdatePlayerWey(bool Wey)
    {
        _sr.flipX = Wey;
    }

    /// <summary>
    ///�֐����FUpdateFlicker()<br />
    ///�������F���Ŏ���<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�v���C���[�̖��ŏ����̊e�ϐ���ݒ肷�鏈���@<br />
    /// </summary>
    public void UpdateFlicker(float SetFlickerTimer)
    {
        FlickerFlag = true;
        FlickerTimer = SetFlickerTimer;
    }
}
