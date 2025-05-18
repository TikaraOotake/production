using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddleBoss : MonoBehaviour
{
    //���{�X�̗̑�
    public int Hp;

    public int HalfHp;

    //�ړ��؂�ւ��p�̕ϐ�
    public int num;

    //�v���C���[�̍U���Ő�����Ԕ����͂�����ϐ�
    public int RepulsiveForce;

    //�ړ��̑���
    public float MoveSpeed;

    //���̍s���܂ł̑ҋ@����
    public float WaiTime;

    //���̍s���܂ł̎��Ԍv��
    public float WaiTimer;

    //�t�F�[�h�A�E�g����܂ł̎���
    public float FadeTime;

    //�t�F�[�h�A�E�g���鎞�Ԍv��
    public float FadeTimer;

    //�ʒu���i�[�p�̕ϐ�
    public Vector2 pos;

    //�e�̃v���n�u������ϐ�
    public GameObject BulletPrefab;

    //�����̃v���n�u������ϐ�
    public GameObject ExPrefab;

    //�n�ʂƂ��Ĕ��f����I�u�W�F�N�g�̃��C���[
    public LayerMask groundLayer;

    //���{�X�̍s�����
    public enum MBoss_Status
    {
        //�ʏ�̎�
        Fine, 
        //HP�������ȉ��̎�
        Half,
        //HP���Ȃ��Ȃ�����
        Dead,
    }
    //MBoss_Stutas��ϐ�St�ŊǗ�����B
    MBoss_Status St;

    public enum Atk_Stutas
	{
        //�ړ���
        Move,
        //�U���O�P�̎�
        Atk1,
        //�U���O�Q�̎�
        Atk2,
        //��Z�U���̎�
        SpAtk,
	}
    Atk_Stutas A_St;

    //SpriteRenderer���i�[����ϐ�
    SpriteRenderer Sr;

    //RigidBody2D���i�[����ϐ�
    Rigidbody2D Rb;

    // �����x
    Vector3 initialVelocity;

    // Start is called before the first frame update
    void Start()
    {
        //�ϐ��̏�����
        WaiTimer = 0.0f;
        FadeTimer = 0.0f;
        num = 1;
        HalfHp = Hp / 2;

        
        // ���ˊp�x�i�����ł�45�x�j
        float angle = 90f;
        // �����x�i�����ł�10m/s�j
        float speed = 10f;
        //���������̑��x�A���������̑��x
        //�I�u�W�F�N�g�ɑ΂��āA�d�͂ƕ����^���ɂ��͂������Ă��܂��B
        float vx = speed * Mathf.Cos(angle * Mathf.Deg2Rad);
        float vy = speed * Mathf.Sin(angle * Mathf.Deg2Rad);
        initialVelocity = new Vector3(vx, vy, 0f);

        //��Ԃ̏�����
        St = MBoss_Status.Fine;

        //Component�̎擾
        Sr = GetComponent<SpriteRenderer>();
        Rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //���{�X�̍s���p�^�[��
        switch (St)
        {
            //�ʏ펞
            case MBoss_Status.Fine:
                {

                    //�ړ�����
                    MBMove();

                    if (pos.x > 6.5f)
					{
                        //�U������
                        MBAtk1();
                    }

                    break;
                }

            //Hp�������ȉ��̎�
            case MBoss_Status.Half:
                {
                    //�ړ�����
                    MBMove();
                    break;
                }

            //Hp���Ȃ��Ȃ�����
            case MBoss_Status.Dead:
                {
                    //�t�F�[�h�A�E�g�̎��Ԍv��
                    FadeTimer += Time.deltaTime;

                    //FadeTime�𒴂���܂łɏ��X�ɓ����ɂ��Ă���
                    if (FadeTimer < FadeTime)
                    {
                        //���l��(FadeTimer / FadeTime)�̕��A�����������l�ɂ���B
                        //alpha : �ύX��̃��l�̒l
                        float alpha = 1.0f - FadeTimer / FadeTime;
                        //�F���̎擾�y�у��l�̕ύX
                        Color color = Sr.color;
                        color.a = alpha;
                        Sr.color = color;
                    }
                    //(FadeTimer > FadeTime)�ɂȂ������ADead()�𔭓�����B
                    else
                    {
                        //�����̃A�j���[�V�����v���n�u���C���X�^���X��
                        Instantiate(ExPrefab, this.transform.position, Quaternion.identity);
                        Dead();
                    }
                    break;
                }
        }
    }

    //�ړ��̊֐�
    public void MBMove()
    {
        //�ʒu�̎擾
        pos = transform.position;

        //���}�C�i�X�������邱�Ƃŋt�����Ɉړ����� �� �ϐ�num�ōs��
        //transform.right = ���[���h��Ԃ̃��[�J���itransform�jx���@�E�E�E�Ԏ�(MoveTool)
        //�ړ� =�i���[�J��x�� * �o�ߎ��� * ���x * �ϐ�num�q���E�؂�ւ��p�r�j
        transform.Translate(transform.right * Time.deltaTime * MoveSpeed * num);

        //pos.x��6.5f���傫���ʒu�ɂ����ꍇ�A���Ɉړ��]��������(�E �� ��)
        if (pos.x > 6.5f)
        {
            num = -1;
            Sr.flipX = false;
        }
        //pos.x��-6.5f��菬�����ʒu�ɂ����ꍇ�A�E�Ɉړ��]��������(�� �� �E)
        else if (pos.x < -6.5f)
        {
            num = 1;
            Sr.flipX = true;
        }
        
    }


    //�ʏ�U���P�̊֐�
    public void MBAtk1()
    {
        if (num == 1)
		{
            
            
            Vector3 gravity = new Vector3(0f, 9.8f, 0f);  // �d��
            Rb.AddForce(gravity, ForceMode2D.Force);
            Rb.AddForce(initialVelocity, ForceMode2D.Impulse);
        }
        else if (num == -1)
		{
            Instantiate(BulletPrefab, new Vector3(pos.x + 1.0f, pos.y, transform.position.z), transform.rotation);
        } 
    }

    //�ʏ�U���Q�̊֐�
    public void MBAtk2()
    {

    }

    //����U���̊֐�
    public void SpAtk()
    {

    }

    //���S�������̊֐�
    public void Dead()
    {
        //���g��j������B
        Destroy(gameObject);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("�ڐG������s��");
        //�v���C���[����U�����󂯂��ۂ̃_���[�W����
        if (collision.gameObject.tag == "PlayerAtk")
        {
            Hp--;
            if (num == -1)
            {
                Rb.AddForce((this.transform.up + this.transform.right) * RepulsiveForce);
            }
            else
            {
                Rb.AddForce((this.transform.up + this.transform.right) * RepulsiveForce);
            }
            Debug.Log("�ڐG����");

            if (Hp == HalfHp)
            {
                St = MBoss_Status.Half;
            }
            else if (Hp <= 0)
            {
                Hp = 0;
                St = MBoss_Status.Dead;
            }
        }
    }
    
}
