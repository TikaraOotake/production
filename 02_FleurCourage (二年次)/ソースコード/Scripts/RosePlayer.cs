using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RosePlayer : MonoBehaviour
{
    //�ړ����x
    public float MoveSpeed;

    //�W�����v��
    public float jumpPower;

    public float StopTimer;
    //����U�����A�o�ߎ��Ԃ��i�[����ϐ�
    public float SpTime;

    //�ǉ����͎��Ԃ��i�[����ϐ�
    public float AddInputTime;

    //�v�����Ԃ��i�[����ϐ�
    public float IntervalTime;

    //����U���̍����̏��
    public float High;

    //����U�����A�󒆑ҋ@�̎���
    public float AirStop;

    //����U�����A�v���C���[�̌��݈ʒu
    public Vector3 pos;

    //�U���Ɋւ���񐔂��i�[����ϐ�
    public int ACnt;

    //����U�����̒��n����Ɏg�p����ϐ�
    //0:true  1:false�@�@�Ƃ���B
    public int landingCnt;

    //����U�����̒��n����ɐ؂�ւ���ϐ�
    public bool landingSwitch;

    //�n�ʂƂ��Ĕ��f����I�u�W�F�N�g�̃��C���[
    public LayerMask groundLayer;

    //�v���C���[�S�̂̏��
    public enum PlayerStutas
    {
        Fine,       //�ʏ�
        NoControl,      //�U���{�^���������ꂽ��
        Dead,       //���S��
    }
    public PlayerStutas PSt;

    //�U���̎�ޕʏ��
    public enum AttackStutas
    {
        NoAttack,       //�U������Ă��Ȃ���
        Normal,     //�ʏ�U��
        Spcial,     //����U��
    }
    public AttackStutas ASt;

    //����U����p�̒i�K�ʏ��
    public enum SpAtk
    {
        SpNo,       //����U�����Ȃ���
        AtkUp,      //����U���E�㏸��
        SpWait,     //����U���E�󒆑ҋ@��
        AtkDown,        //����U���E���~��
    }
    public SpAtk spAtk;

    //SpriteRenderer���i�[����ϐ�
    SpriteRenderer Sr;

    //RigidBody���i�[����ϐ�
    Rigidbody2D Rb;

    //Animator���i�[����ϐ�
    Animator Anim;

    //CapsuleCollider2D���i�[����ϐ�
    CapsuleCollider2D CCol;

    // Start is called before the first frame update
    void Start()
    {
        //�ϐ�������
        ACnt = 0;
        landingCnt = 0;
        landingSwitch = false;
        AddInputTime = 0.0f;
        IntervalTime = 0.0f;
        AirStop = 0.2f;
        StopTimer = 0.0f;
        pos = transform.position;

        //Component�̎擾
        Sr = GetComponent<SpriteRenderer>();
        Rb = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        CCol = GetComponent<CapsuleCollider2D>();

        //��Ԃ̏����ݒ�
        PSt = PlayerStutas.Fine;
        ASt = AttackStutas.NoAttack;
        spAtk = SpAtk.SpNo;

    }

    // Update is called once per frame
    void Update()
    {
        switch(PSt)
        {
            //�ʏ�
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

                    //�U���{�^���������ꂽ���ǂ����̊m�F���s��
                    AtkSearch();

                    break;
                            
			    }

            //�U�����s���ۂ͑��̃A�j���[�V�������~�����āA�U����p�̃A�j���[�V�����݂̂Ɉڍs������B
            //���͂��s���Ȃ������ꍇ�APlayerStutas.Fine�Ɉڍs�����B
            case PlayerStutas.NoControl:
                {
                    switch (ASt)
                    {
                        //�ʏ�U���̏���
                        case AttackStutas.Normal:
                            {
                                Debug.Log("�ʏ�U��");
                                //tag��"PlayerAtk"�ɕύX�B�G�ɓ��������ꍇ�A�G��j��ł���B
                                this.tag = "PlayerAtk";
                                //�U���A�j���[�V�����̍Đ�
                                Anim.SetInteger("Action", 10);
                                //�ēx���͂��s���邩�`�F�b�N���s���B
                                AtkSupport();

                            }
                            break;
                        //����U���̏���
                        case AttackStutas.Spcial:
                            {
                                Debug.Log("����U��");
                                //tag��"PlayerAtk"�ɕύX�B�G�ɓ��������ꍇ�A�G��j��ł���B
                                this.tag = "PlayerAtk";
                                Rb.gravityScale = 1.0f;
                                pos = transform.position;
                                
                                //�U���A�j���[�V�����̍Đ�
                                switch (spAtk)
                                {
                                    //����U���{�^����������Ă��Ȃ�
                                    case SpAtk.SpNo:
                                        {
                                        }
                                        break;

                                    //����U���E�㏸��
                                    case SpAtk.AtkUp:
                                        {
                                            Anim.SetInteger("Action", 20);
                                            //���W���擾
                                            //�摜�̌����ɂ���ĕ�����ς���
                                            if (Sr.flipX == false && pos.y <= High)
                                            {
                                                pos.x += 0.2f;    // x���W��0.3���Z
                                                pos.y += 0.5f;    // y���W��0.4���Z
                                                transform.position = pos;  // ���W��ݒ� 
                                            }
                                            else if (Sr.flipX == true && pos.y <= High)
                                            {
                                                pos.x -= 0.2f;    // x���W��0.3���Z
                                                pos.y += 0.5f;    // y���W��0.4���Z
                                                transform.position = pos;  // ���W��ݒ�
                                            }
                                            Debug.Log("�㏸��");

                                            //pos.y����������𒴂�����؋�ɐ؂�ւ�
                                            if (pos.y >= High)
                                            {
                                                spAtk = SpAtk.SpWait;
                                                
                                                Debug.Log("�؂�ւ�");
                                            }
                                        }
                                        break;

                                    //����U���E�󒆑؋�
                                    case SpAtk.SpWait:
                                        {
                                            //�󒆒�~���Ȃ���A�j���[�V�������s��
                                            Rb.isKinematic = true;
                                            AirStop -= Time.deltaTime;
                                            Anim.SetInteger("Action", 21);

                                            //�󒆑؋󎞊Ԃ��߂����牺�~�ɐ؂�ւ���
                                            //�󒆒�~������
                                            if (AirStop <= 0.0f)
                                            {
                                                
                                                spAtk = SpAtk.AtkDown;    
                                                Debug.Log("�ϋ�؂�ւ�");
                                                
                                            }
                                            Debug.Log("�ϋ�");

                                        }
                                        break;

                                    //����U���E���~��
                                    case SpAtk.AtkDown:
                                        {
                                            if (pos.y > -1.0f && landingSwitch == false)
                                            {
                                                Anim.SetInteger("Action", 22);
                                                Rb.isKinematic = false;
                                            }
                                            else if (pos.y <= -1.0f && landingSwitch == false)
                                            {
                                                Anim.SetInteger("Action", 23);

                                                if (Rb.isKinematic == true)
                                                {
                                                    Rb.isKinematic = false;
                                                }
                                            }
                                            
                                            //���W���擾
                                            //�摜�̌����ɂ���ĕ�����ς���
                                            if (Sr.flipX == false)
                                            {
                                                pos.x += 0.1f;    // x���W��0.15���Z
                                                pos.y -= 0.1f;    // y���W��0.15���Z
                                                transform.position = pos;  // ���W��ݒ�
                                            }
                                            else
                                            {
                                                pos.x -= 0.1f;    // x���W��0.15���Z
                                                pos.y -= 0.1f;    // y���W��0.15���Z
                                                transform.position = pos;  // ���W��ݒ�
                                            }

                                            Debug.Log("���~��");
                                        }
                                        break;
                                }
                                //�ēx���͂��s���邩�`�F�b�N���s���B
                                AtkSupport();
                            }
                            break;
                        //�U�����s��Ȃ���
                        case AttackStutas.NoAttack:
                            {
                            }
                            break;
                    }
                    break;
                }
            //���S����
            case PlayerStutas.Dead:
                {
                    Dead();
                    break;
                }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        //�ڐG����gameObject.tag��"Ground"��������"enemy"�������ꍇ
        //�n�ʂ��G�ɐڐG�����ۂ̐ڐG����
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "enemy")
        {
            //����U�����ǂ���������s���B�i����������U���E���~���ǂ����Ŕ�����s���j
            if (ASt == AttackStutas.Spcial)
            {

                //����U���A���n�p�̏���
                //�����݈ڍs�ɋ��ׁ̈A�ꎞ�I�ɕۗ���

                //Debug.Log("�������Ă���");
                //Anim.SetInteger("Action", 30);
                //landingSwitch = true;
                //if (landingSwitch == true)
                //{
                //    Anim.SetInteger("Action", 30);
                //}

                //����������A�j���[�V�������쓮�A
                //�摜�̌����ɂ���ď����ړ����������ς���
                if (Sr.flipX == false)
                {
                    transform.Translate(-0.5f, 0.0f, 0.0f);
                    //�p�����[�^�[���Z�b�g���s��
                    ResetStutas();
                }
                else
                {
                    transform.Translate(0.5f, 0.0f, 0.0f);
                    //�p�����[�^�[���Z�b�g���s��
                    ResetStutas();
                }
            }
        }
    }

    //�ړ�����
    public void Move()
    {
        //���ړ�
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
        //�E�ړ�
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

    //���͂��Q�Ƃ��čU����ނ�ς���֐�
    public void AtkSearch()
    {
        //�ʏ�U���������ꍇ
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PSt = PlayerStutas.NoControl;
            ASt = AttackStutas.Normal;
        }
        //����U���������ꍇ
        if (Input.GetKeyDown(KeyCode.X))
        {
            PSt = PlayerStutas.NoControl;
            ASt = AttackStutas.Spcial;
            spAtk = SpAtk.AtkUp;
        }

    }

    //�ǉ��U�����͂Ɋւ��Ă̊֐�
    public void AtkSupport()
    {
        if (ACnt == 0)
        {
            //�ǉ��U�����͎�t����
            AddInputTime = 0.5f;
            //�֐��S�̂̎�t���ԁi�I������Ǝ����Ń��Z�b�g�����j
            IntervalTime = 0.8f;
        }
        ++ACnt;
        AddInputTime -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X))
        {
            //�ēx�A���͂��ꂽ�{�^���ɂ���čU�����s���B
            AtkSearch();
            ACnt = 0;
            
        }
        if (IntervalTime <= 0.0f)
        {
            //��Ԃ��������Z�b�g
            ResetStutas();
        }
        IntervalTime -= Time.deltaTime;
    }

    //��Ԃ̎������Z�b�g���s���֐�
    public void ResetStutas()
    {
        this.tag = "Player";
        ACnt = 0;
        landingSwitch = false;
        Anim.SetInteger("Action", 0);
        PSt = PlayerStutas.Fine;
        ASt = AttackStutas.NoAttack;
        spAtk = SpAtk.SpNo;
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

        //Ray(����)��n�ʂ�2.0f����
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 6.0f, groundLayer);

        //�q�b�g���Ă��Ȃ��Ȃ�null��Ԃ�
        if (hit.collider != null)
        {
            ret = true;
        }
        
        return ret;
    }

    //�y�f�o�b�O�p�֐��z
    //Ray��\��

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, new Vector3(0, -0.6f, 0));
    }

}
