using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl1 : MonoBehaviour
{
    //�ړ����x
    public float MoveSpeed;

    //�W�����v��
    public float jumpPower;

    public float StopTime;

    public float StopTimer;

    //�ǉ����͎��Ԃ��i�[����ϐ�
    public float AddInputTime;

    //�v�����Ԃ��i�[����ϐ�
    public float IntervalTime;

    public int ACnt;

    //�n�ʂƂ��Ĕ��f����I�u�W�F�N�g�̃��C���[
    public LayerMask groundLayer;

    //�v���C���[�̏��
    public enum PlayerStutas
    {
        Fine,
        NoControl,
        Dead,
    }
    PlayerStutas PSt;

    public enum AttackStutas
    {
        NoAttack,
        Normal,
        Spcial,
    }
    AttackStutas ASt;

    //SpriteRenderer���i�[����ϐ�
    public SpriteRenderer Sr;

    //RigidBody���i�[����ϐ�
    public Rigidbody2D Rb;

    //Animator���i�[����ϐ�
    public Animator Anim;

    //CapsuleCollider2D���i�[����ϐ�
    public CapsuleCollider2D CCol;

    // Start is called before the first frame update
    void Start()
    {
        ACnt = 0;
        AddInputTime = 0.0f;
        IntervalTime = 0.0f;
        StopTimer = 0.0f;

        //Component�̎擾
        Sr = GetComponent<SpriteRenderer>();
        Rb = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        CCol = GetComponent<CapsuleCollider2D>();

        //��Ԃ̐ݒ�
        PSt = PlayerStutas.Fine;

        ASt = AttackStutas.NoAttack;
    }

    // Update is called once per frame
    void Update()
    {
        switch(PSt)
        {
            case PlayerStutas.Fine:
                {
                      
                    
                    //�ݒu���Ă��鎞�́u�ҋ@�v�ɂ���
                    if (IsGrounded() == true)
                    {
                        Anim.SetInteger("Action", 0);
                    }
                    //�󒆂ɂ��鎞�́u�W�����v�v�ɂ���
                    else
                    {
                        Anim.SetInteger("Action", 2);
                    }

                    //�ړ�����
                    Move();

                    if (IsGrounded() == true)
                    {
                        //�W�����v����
                        Jump();
                    }

                    AtkSearch();

                    break;
                            
			    }

            case PlayerStutas.NoControl:
                {
                    switch (ASt)
                    {
                        case AttackStutas.Normal:
                            {
                                Debug.Log("�ʏ�U��");
                                this.tag = "PlayerAtk";
                                Anim.SetInteger("Action", 10);
                                AtkSupport();

                            }
                            break;
                        case AttackStutas.Spcial:
                            {
                                Debug.Log("����U��");
                                this.tag = "PlayerAtk";
                                Anim.SetInteger("Action", 20);
                                //StopTimer += Time.deltaTime;
                                //Vector2 power = (transform.forward + transform.right) * MoveSpeed;
                                //Rb.AddForce(power, ForceMode2D.Impulse);
                                //if (StopTimer >= StopTime)
                                //{
                                //    Anim.speed = 0.0f;
                                //    Rb.isKinematic = true;
                                //    Rb.velocity = new Vector3(0.0f, 2.0f, 0.0f);

                                //    float StopAir = 0.1f;
                                //    StopAir -= Time.deltaTime;
                                //    if (StopAir <= 0.0f )
                                //    {
                                //        Rb.velocity = Vector3.zero;
                                //        Rb.isKinematic = false;
                                //        Anim.SetInteger("Action", 21);
                                //        StopTimer = 0.0f;
                                //    }
                                    


                                //    StopTimer = 0.0f;
                                //}
                                

                                AtkSupport();
                            }
                            break;
                    }
                    break;
                }

            case PlayerStutas.Dead:
                {
                    Dead();
                    break;
                }
        }
    }

    //�ړ�����
    public void Move()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(-MoveSpeed, 0.0f, 0.0f);
            //�ݒu���Ă��鎞�́u�ҋ@�v
            if (IsGrounded() == true)
            {
                Anim.SetInteger("Action", 1);
            }
            //�󒆂ɂ��鎞�́u�W�����v�v
            else
            {
                Anim.SetInteger("Action", 2);
            }

            //X���ɑ΂��Ĕ��]������
            Sr.flipX = true;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(MoveSpeed, 0.0f, 0.0f);
            //�ݒu���Ă��鎞�́u�ҋ@�v
            if (IsGrounded() == true)
            {
                Anim.SetInteger("Action", 1);
            }
            //�󒆂ɂ��鎞�́u�W�����v�v
            else
            {
                Anim.SetInteger("Action", 2);
            }

            //X���ɑ΂��Ĕ��]������
            Sr.flipX = false;
        }
    }

    //�W�����v����
    public void Jump()
    {
        //�X�y�[�X�L�[�������ꂽ�Ƃ�
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //��̕��ɃW�����v������
            Rb.AddForce(Vector2.up * jumpPower);
            //�v���C���[�̃A�j���[�V�������u�W�����v�v�ɂ���
            Anim.SetInteger("Action", 2);
        }
    }

    public void AtkSearch()
    {
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PSt = PlayerStutas.NoControl;
            ASt = AttackStutas.Normal;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            PSt = PlayerStutas.NoControl;
            ASt = AttackStutas.Spcial;
        }

    }

    public void AtkSupport()
    {
        if (ACnt == 0)
        {
            AddInputTime = 0.5f;
            IntervalTime = 0.8f;
        }
        ++ACnt;
        AddInputTime -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X))
        {
            AtkSearch();
            ACnt = 0;
        }
        if (IntervalTime <= 0.0f)
        {
            ResetStutas();
        }
        IntervalTime -= Time.deltaTime;
    }

    public void ResetStutas()
    {
        this.tag = "Player";
        ACnt = 0;
        Anim.SetInteger("Action", 0);
        PSt = PlayerStutas.Fine;
        ASt = AttackStutas.NoAttack;
    }

    //���S����
    void Dead()
    {
        //�����蔻��𖳌���
        CCol.enabled = false;
        Destroy(gameObject);
    }

    //�v���C���[�̐ڒn�����̊֐�
    bool IsGrounded()
    {
        bool ret = false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2.0f, groundLayer);

        //�q�b�g���Ă��Ȃ��Ȃ�null��Ԃ�
        if (hit.collider != null)
        {
            ret = true;
        }
        return ret;
    }

}
