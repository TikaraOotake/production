using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //�ϐ��錾
    public GameObject SpritePrefab;//Sprite��\������I�u�W�F�N�g�̃v���n�u
    public GameObject AttackCollider1Prefab;//�U������1�̃v���n�u
    public GameObject AttackCollider2Prefab;//�U������2�̃v���n�u
    public GameObject AttackCollider3Prefab;//�U������3�̃v���n�u
    public GameObject CloudPrefab;//�_�̃v���n�u
    public GameObject DandelionBulletPrefab;//�^���|�|�̒e�̃v���n�u
    public GameObject ParryEffectPrefab;//�p���B�̉��o�̃v���n�u
    public GameObject Sprite;//���������X�v���C�g�I�u�W�F�N�g��ۑ�����
    public GameObject Cloud;//���������I�u�W�F�N�g��ۑ�����
    public GameObject Camera;//�J�����̃I�u�W�F�N�g��ۑ�����
    public GameObject HPUI;//HP��\������I�u�W�F�N�g
    public GameObject SpecialBullet;//����̓���U���̒e

    //�ǉ���
    //-----------------------------------------
    [SerializeField, Tooltip("�ʏ�U���̒e")]
    private GameObject[] BulletObj
       = new GameObject[3];            //�ʏ�U���̒e���i�[����
    [SerializeField, Tooltip("����U���̒e")]
    private GameObject SpecialObj;      //����U���̒e���i�[����

    public int BulletSelect;           //�e�̑I��
    [SerializeField, Tooltip("����U���̎���")]
    private float SpecialTime;          //����U���̎���
    private float _specialTimer;        //����U���̃^�C�}�[

    public float UncontrollableTimer = 0;
    public float CoolDownTimer = 0;

    public GameObject RoseAttack;
    //----------------------------------------



    public float Speed; //�ړ����x
    public int HP;//�̗�

    public float GravitySpeed; //�������x
    public float GravityMaxSpeed; //�����ō����x
    public float DandelionGravityMaxSpeed;//�^���|�|��Ԃ̗����ō����x
    public float GravityScale; //�d�͉����x

    public float JumpPower;//�W�����v��
    public float JumpMomentum;//�W�����v����
    public int JumpRemainCount;//�W�����v��
    public float JumpStartTimer;//�W�����v��������

    public float StiffnessTime;//��e�d������
    public float InvincibleTime;//���G����
    public float HitDamagePosX;//�Ō�Ɏ󂯂��_���[�W��X���W��ۑ�����

    public bool AddInputFlag;//�ǉ����͂��������ǂ����̃t���O
    public float AddInputInterval;//�ǉ����͎�t����
    public float ActionIncapacityTime;//�s���d������
    public int RepeatCount;//�J��Ԃ���

    public int SelectSlot;//���Ԗڂ̃X���b�g���g���Ă��邩�̕ϐ�
    public int SaveSelectSlot;//�؂�ւ���O�ɑI��ł��X���b�g���L�����Ă����ϐ�
    public float ParryCoolTime;//�p���B�̃N�[���^�C��

    //�X���b�g���ɉԂ�ID��ۑ�����
    public int[] SlotID = new int[4];
    
    //�X���b�g����HP��ۑ�����
    public int[] SlotHP = new int[4];
    public int PlayerHP;

    public float StopTime;//��~����
    public bool StopFlag;//��~���邩

    public float oldPosX;//�v���C���[�̉ߋ��̍��W������ϐ�
    public float oldPosY;//�v���C���[�̉ߋ��̍��W������ϐ�

    SpriteRenderer _sr; //SpriteRenderer�̊i�[�p
    Rigidbody2D _rb; //Rigidbody2D�̊i�[�p
    Animator _animator; //Animator�̊i�[�p

    public LayerMask GroundLayer; //�n�ʂƂ��Ĕ��肷��I�u�W�F�N�g�̃��C���[

    enum Status
    {
        Fine, //�ʏ�
        Dead, //���S
        Respawn, //���A
        Clear, //�N���A
        NoControll,//����s�\
        DamageStiffness,//��e�d��
    }
    Status st;

    enum ActionStatus
    {
        NoAction,//�������Ă��Ȃ�
        Parry,//�p���B
        Jump,//�W�����v
        NormalAttack1,//�ʏ�U����i
        NormalAttack2,//�ʏ�U����i
        NormalAttack3,//�ʏ�U���O�i
        NormalAttack,//�ʏ�U��
        SpecialAttack,//����U��
    }
    ActionStatus Ast;

    enum FlourSlot
    {
        NoFlour,//�����҂�
        Slot1,//�X���b�g1
        Slot2,//�X���b�g2
        Slot3,//�X���b�g3
    }
    FlourSlot fs;





    // Start is called before the first frame update
    void Start()
    {
        //�A�^�b�`����Ă���Component���擾���Ă���
        _sr = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        //�ϐ���������
        AddInputFlag = false;
        int initialHP = 6;//����HP
        SlotHP[1] = initialHP;
        SlotHP[2] = initialHP;
        SlotHP[3] = initialHP;

        HPUI = GameObject.Find("HPtext");

        //Sprite��\������I�u�W�F�N�g���C���X�^���X��
        if (SpritePrefab != null)
        {
            GameObject childObject = Instantiate(SpritePrefab, transform);
            Sprite = childObject;
            Sprite.GetComponent<PlayerSprite>().UpdatePlayerObject(this.gameObject);
        }

        BulletSelect = 0;
    }

    // Update is called once per frame
    void Update()
    {
        UncontrollableTimer -= Time.deltaTime;
        CoolDownTimer -= Time.deltaTime;
        if (UncontrollableTimer < 0.0f)
        {
            UncontrollableTimer = 0.0f;
        }
        if(CoolDownTimer < 0.0f)
        {
            CoolDownTimer = 0.0f;
        }

        //Vector2 v = new Vector2();
        //v.x = 0.0f;
        // v.y = 0.0f;
        //_rb.velocity = v;

        //��~�t���O��true�Ȃ�قڑS�Ă̏������~
        if (StopFlag)
        {
            //�󒆂ŐÎ~������
            //ReposeAir();
            ReposeAirX();

            //��~���Ԃ��o�߂�����
            StopTime -= Time.deltaTime;

            //��~���Ԃ�0�ȉ��̂Ƃ��t���O��false��
            if (StopTime <= 0)
            {
                StopFlag = false;
                StopTime = 0;
            }
        }
        else
        {


            switch (st)
            {
                //�u�ʏ�v
                case Status.Fine:
                    {
                        //�ڒn���Ă��鎞�̓A�j���[�V�������u�ҋ@�v�ɂ���
                        if (IsGrounded() == true)
                        {
                            AnimationSummary(SlotID[SelectSlot], 0);
                        }

                        


                        GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 0);

                        /*/�e�X�g
                        if (Input.GetKeyDown(KeyCode.O))
                        {
                            ++HP;
                            HPUI.GetComponent<HPtext>().UpdateHP(HP);
                        }
                        if (Input.GetKey(KeyCode.I))
                        {
                            GetComponent<Renderer>().material.color = new Color32(0, 0, 0, 0);
                        }
                        if (Input.GetKey(KeyCode.U))
                        {
                            ReposeAir();
                        }
                        if (Input.GetKeyDown(KeyCode.R))
                        {
                            //�X�v���C�g��U��������
                            Sprite.GetComponent<PlayerSprite>().UpdateVibStatus(0.1f, 1.08f, 0.3f, 0.3f, 500.0f);
                        }
                        if (Input.GetKeyDown(KeyCode.L))
                        {
                            if (Cloud == null)
                            {
                                Sprite.GetComponent<PlayerSprite>().UpdateAnimation(100);
                            }
                        }
                        if (Input.GetKeyDown(KeyCode.K))
                        {
                            if (Cloud == null)
                            {
                                Sprite.GetComponent<PlayerSprite>().UpdateAnimation(101);
                            }
                        }


                        if (Input.GetKey(KeyCode.Y))
                        {
                            if (Cloud == null)
                            {
                                Cloud = Instantiate(CloudPrefab, transform.position, Quaternion.identity);
                                Cloud.GetComponent<Cloud>().UpdateGameObject(this.gameObject);
                            }
                        }/*/


                        TestController();


                        //float h = Input.GetAxis("Horizontal");
                        //transform.Translate(h * 0.01f, 0.0f, 0.0f);


                        //�I�𒆂̃X���b�g��HP��\��
                        HPUI.GetComponent<HPtext>().UpdateHP(SlotHP[SelectSlot]);

                        //�v���C���[�̌���HP��\��
                        HPUI.GetComponent<HPtext>().UpdateHP(PlayerHP);

                        //���G���Ԃ��o�߂�����
                        ElapsedInvincibleTime();

                        ActionProduce();
                        Move();
                        MaxGravitySpeed();
                        ChangeSlot();
                        TransitionGameOvere();
                        ResetJumpAirCount();

                        //�󒆂ɂ��鎞�̓A�j���[�V�������u�W�����v�v�ɂ���
                        if (IsGrounded() == false)
                        {
                            AirAnimation();
                        }
                    }
                    break;
            }
            switch (st)
            {
                //�u����s�\�v
                case Status.NoControll:
                    {
                        //HPUI.GetComponent<HPtext>().UpdateHP(SlotHP[SelectSlot]);�v
                        HPUI.GetComponent<HPtext>().UpdateHP(PlayerHP);

                        //���G���Ԃ��o�߂�����
                        ElapsedInvincibleTime();
                        //�������x���
                        MaxGravitySpeed();

                        TransitionGameOvere();


                        //�s��
                        switch (Ast)
                        {
                            //�W�����v
                            case ActionStatus.Jump:
                                {
                                    Jump();
                                }
                                break;

                            //�p���B
                            case ActionStatus.Parry:
                                {
                                    Parry();
                                }
                                break;
                            //�ʏ�U��
                            case ActionStatus.NormalAttack:
                                {
                                    switch (SelectSlot)
                                    {
                                        case 1:
                                            {
                                                NormalAttack(SlotID[1]);
                                            }
                                            break;
                                        case 2:
                                            {
                                                NormalAttack(SlotID[2]);
                                            }
                                            break;
                                        case 3:
                                            {
                                                NormalAttack(SlotID[3]);
                                            }
                                            break;
                                    }
                                }
                                break;
                            //����U��
                            case ActionStatus.SpecialAttack:
                                {
                                    switch (SelectSlot)
                                    {
                                        case 1:
                                            {
                                                SpecialAttack(SlotID[1]);
                                            }
                                            break;
                                        case 2:
                                            {
                                                SpecialAttack(SlotID[2]);
                                            }
                                            break;
                                        case 3:
                                            {
                                                SpecialAttack(SlotID[3]);
                                            }
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
            switch (st)
            {
                //�u��e�d���v
                case Status.DamageStiffness:
                    {
                        //HPUI.GetComponent<HPtext>().UpdateHP(SlotHP[SelectSlot]);�v
                        HPUI.GetComponent<HPtext>().UpdateHP(PlayerHP);
                        MaxGravitySpeed();
                        DamageStiffnessFunction();

                        //�A�j���[�V�������e��Ԃ�
                        AnimationSummary(SlotID[SelectSlot], 2);

                        /*/�_�ł̃v���O�����i�e�X�g�j
                        �R���|�[�l���g���擾
                        var blinker = GetComponent<Blinker>();
                        // �_�ŊJ�n
                        blinker.BeginBlink();
                        //�o�ߎ��Ԃ��v��
                        float damageTimer = 0;
                        damageTimer += Time.deltaTime;
                        //�w��̎��Ԃ𒴂�����
                        if (damageTimer >= 1.5)
                        {
                            //�_�ŏI��
                            blinker.EndBlink();
                            //�^�C�}�[�����Z�b�g����
                            damageTimer = 0;
                        }
                        /*/
                    }
                    break;
            }
            switch (st)
            {
                //�u���S�v
                case Status.Dead:
                    {
                        //�A�j���[�V���������S��Ԃ�
                        AnimationSummary(0, 11);
                    }
                    break;
            }
            switch (st)
            {
                //�u���A�v
                case Status.Respawn:
                    {

                    }
                    break;
            }
            switch (st)
            {
                //�u�N���A�v
                case Status.Clear:
                    {

                    }
                    break;
            }
        }
        //�p���B�N�[���^�C����\��
        HPUI.GetComponent<HPtext>().UpdateCoolTime(ParryCoolTime);

        //�^���|�|�Ȃ畂�V����������
        DandelionFall();
        //�A�W�T�C��ԂȂ�_����������
        //SummonCloud();

        //1F�O��Y���W���K����������
        oldPosY = this.transform.localPosition.y;
        oldPosX = this.transform.localPosition.x;
        //�N�[���^�C�����o�߂�����
        ParryCoolTime -= Time.deltaTime;
        if (ParryCoolTime <= 0.0f)
        { ParryCoolTime = 0.0f; }

        CheckStatus();
    }

    /// <summary>
    ///�֐����Fmove()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�v���C���[�̍��E�ړ�����
    /// </summary>
    void Move()
    {
        if (UncontrollableTimer > 0.0f)
        {
            return;
        }
        //A�������ꂽ��
        if (InputGetAction("LeftMove", "Stay"))
        {
            //�ړ�
            transform.Translate(-Speed, 0.0f, 0.0f);
            //X���ɑ΂��Ĕ��]������
            _sr.flipX = true;
            AnimationSummary(SlotID[SelectSlot], 1);
        }
        else
        //D�������ꂽ��
        if (InputGetAction("RightMove", "Stay"))
        {
            //�ړ�
            transform.Translate(Speed, 0.0f, 0.0f);
            //X���ɑ΂��Ĕ��]������
            _sr.flipX = false;
            AnimationSummary(SlotID[SelectSlot], 1);
        }
        else 
        {
            //AnimationSummary(SlotID[SelectSlot], 0);
        }
    }
    /// <summary>
    ///�֐����FAttackMove()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�O�ɏ����ړ��ł���
    /// </summary>
    void AttackMove()
    {
        if(UncontrollableTimer > 0.0f)
        {
            return;
        }
        //A�������ꂽ��
        if (InputGetAction("LeftMove", "Stay") && _sr.flipX == true)
        {
            //�ړ�
            transform.Translate(-Speed / 3, 0.0f, 0.0f);
        }
        //D�������ꂽ��
        if (InputGetAction("RightMove", "Stay") && _sr.flipX == false)
        {
            //�ړ�
            transform.Translate(Speed / 3, 0.0f, 0.0f);
        }
    }
    /// <summary>
    ///�֐����FJump()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�W�����v����
    /// </summary>
    void Jump()
    {
        if (UncontrollableTimer > 0.0f)
        {
            return;
        }

        if (RepeatCount == 0)
        {
            //�A�j���[�V�������W�����v�ҋ@��Ԃ�
            AnimationSummary(SlotID[SelectSlot], 6);
        }

        //transform.Translate(0.0f, jumpMomentum, 0.0f);
        if (RepeatCount >= 2)
        {
            //�����x��������
            Vector2 v = new Vector2();
            v.x = 0.0f;
            v.y = 0.0f;
            _rb.velocity = v;
            //��ւ̗͂������ăW�����v������
            _rb.AddForce(Vector2.up * JumpPower);

            //�W�����v�̃v���O�������I�����ʏ��Ԃɖ߂�(�����ꂪ�Ȃ��ƃW�����v���I�����ꂸ�����Ȃ��Ȃ�)
            ResetStatus();
            //gravitySpeed = -jumpPower;
            //jumpMomentum = jumpPower;
        }
        ++RepeatCount;

    }
    /// <summary>
    ///�֐����FJumpAir()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�󒆃W�����v����
    /// </summary>
    /*/void JumpAir()
    {
        //�W�����v�������ꂽ�ۂɁh�󒆁h���c��W�����v�񐔂�����ꍇ
        if (InputGetAction("Jump", "Down") && IsGrounded() == false && JumpRemainCount > 0)
        {
            //�����x��������
            Vector2 v = new Vector2();
            v.x = 0.0f;
            v.y = 0.0f;
            _rb.velocity = v;
            //��ւ̗͂������ăW�����v������
            _rb.AddForce(Vector2.up * JumpPower);

            //�󒆃W�����v�񐔂����炷
            JumpRemainCount -= 1;
        }
    }/*/

    /// <summary>
    ///�֐����FResetJumpAirCount()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�󒆃W�����v�񐔃��Z�b�g����
    /// </summary>
    void ResetJumpAirCount()
    {
        //�n��ɂ���ꍇ
        if (IsGrounded())
        {
            JumpRemainCount = 1;
        }
    }



    /// <summary>
    ///�֐����FGravityCalculation<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�d�͂̌v�Z������֐�
    /// </summary>
    void GravityCalculation()
    {

        //�����ō����x�𒴂����瓙���ɂ��鏈��
        if (GravityMaxSpeed <= GravitySpeed)
        {
            GravitySpeed = GravityMaxSpeed;
        }

        //�n�ʂƐڒn���Ă�����d�͂�0��
        if (IsGrounded() == true)
        {
            if (GravitySpeed > 0)
            {
                GravitySpeed = 0.0f;
            }

            //transform.Translate(0.0f, 0.01f, 0.0f);
        }
        else
        {
            //gravitySpeed += gravityScale;
        }
    }
    /// <summary>
    ///�֐����FMaxGravitySpeed()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�������x������߂�h���֐�
    /// </summary>
    void MaxGravitySpeed()
    {
        if (_rb.velocity.y < -GravityMaxSpeed)
        {
            //���݂̉������ꎞ�I�ɕۑ�����
            Vector2 v = _rb.velocity;
            v.y = -GravityMaxSpeed;
            _rb.velocity = v;
        }
    }

    /// <summary>
    ///�֐����FGravity()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�d�͂𔭐�������֐�
    /// </summary>
    void Gravity()
    {
        transform.Translate(0.0f, -GravitySpeed, 0.0f);
    }

    /// <summary>
    ///�֐����FReposeAir()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�Ăяo���Ă����Y���W���Œ肷��֐�
    /// </summary>
    /*/void ReposeAir()
    {
        //�󒆂Ȃ�Y���W���Œ肷��
        if (IsGrounded() == false)
        {
            Vector2 v = _rb.velocity;
            v.y = 0.0f;
            _rb.velocity = v;
            transform.position = new Vector2(this.transform.localPosition.x, oldPosY);
        }
    }/*/
    /// <summary>
    ///�֐����FReposeAirX()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�Ăяo���Ă����X���W���Œ肷��֐�
    /// </summary>
    void ReposeAirX()
    {
        //�󒆂Ȃ�X���W���Œ肷��
        if (IsGrounded() == false)
        {
            Vector2 v = _rb.velocity;
            v.x = 0.0f;
            _rb.velocity = v;
            transform.position = new Vector2(oldPosX, this.transform.localPosition.y);
        }
    }

    /// <summary>
    ///�֐����FIsGrounded() 	<br />
    ///�������F�Ȃ�		<br />
    ///�߂�l�F�ڒn���Ă���ꍇ��true,���Ă��Ȃ��ꍇ��false		<br />
    ///�p�@�r�F�ڒn���菈��
    /// </summary>

    bool IsGrounded()
    {
        bool ret = false;
        //��������Ray���΂��āA�w�肵�����C���[�̃I�u�W�F�N�g�ƐڐG���Ă��邩�ǂ����𔻕ʂ���
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.2f, GroundLayer);
        //�q�b�g���Ă��Ȃ��ꍇ��null���������
        if (hit.collider != null)
        {
            ret = true;
        }
        return ret;
    }

    /// <summary>
    ///�֐����FDamageBounce()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F��e���̐�����я���
    /// </summary>
    void DamageBounce()
    {
        if ((HitDamagePosX - transform.position.x) < 0)
        {
            //�����x��������
            Vector2 v = new Vector2();
            v.x = 0.0f;
            v.y = 0.0f;
            _rb.velocity = v;
            //��Ɖ��ւ̗͂������ăW�����v������
            _rb.AddForce(Vector2.up * JumpPower / 1.5f);
            _rb.AddForce(Vector2.right * JumpPower / 2.0f);
        }
        else
        if ((HitDamagePosX - transform.position.x) > 0)
        {
            //�����x��������
            Vector2 v = new Vector2();
            v.x = 0.0f;
            v.y = 0.0f;
            _rb.velocity = v;
            //��Ɖ��ւ̗͂������ăW�����v������
            _rb.AddForce(Vector2.up * JumpPower / 1.5f);
            _rb.AddForce(Vector2.left * JumpPower / 2.0f);
        }
        else
        {

        }
    }

    /// <summary>
    ///�֐����FDamageStiffnessFunction()<br />
    ///�������F�Ȃ�<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F��e�d�����ԏ���
    /// </summary>
    void DamageStiffnessFunction()
    {
        if (RepeatCount == 0)
        {
            DamageBounce();
            
        }
        ++RepeatCount;
        //�����x��X���̂�N����1�Ɍ���������;
        Vector2 v = _rb.velocity;
        v.x = v.x / 1.1f;
        _rb.velocity = v;

        StiffnessTime -= Time.deltaTime;
        if (StiffnessTime <= 0)
        {
            //�����x��X���̂ݏ�����
            Vector2 v2 = _rb.velocity;
            v2.x = 0.0f;
            _rb.velocity = v2;

            StiffnessTime = 0.0f;
            st = Status.Fine;
            //�ʏ��Ԃɖ߂�
            ResetStatus();
            //���G���ԕ��_�ł�����
            Sprite.GetComponent<PlayerSprite>().UpdateFlicker(InvincibleTime);
        }
    }
    /// <summary>
    ///�֐����FElapsedInvincibleTime()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F���G���Ԃ��o�߂����鏈��
    /// </summary>
    void ElapsedInvincibleTime()
    {
        //���G���Ԃ��o�߂�����
        if (InvincibleTime > 0)
        {
            InvincibleTime -= Time.deltaTime;
        }
        //���G���Ԃ�0�����������0�ɂ���
        if (InvincibleTime < 0)
        {
            InvincibleTime = 0;
        }
    }

    /// <summary>
    ///�֐����FChangeSlot()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�Ԃ̐؂�ւ�����
    /// </summary>
    void ChangeSlot()
    {
        if (ParryCoolTime <= 0.0f)
        {
            if (InputGetAction("ChangeSlotR", "Down"))
            {
                SaveSelectSlot = SelectSlot;
                ++SelectSlot;
                SelectSlotLoop();
                if (SelectSlot == 1)
                {
                    //�A�j���[�V������ύX
                    AnimationSummary(0, 10);
                }
                else if (SelectSlot == 2)
                {
                    //�A�j���[�V������ύX
                    AnimationSummary(0, 12);
                }
                else if (SelectSlot == 3)
                {
                    //�A�j���[�V������ύX
                    AnimationSummary(0, 10);
                }
                /*/if(SelectSlot == 1)
                {
                    ChangeDandelion();
                }/*/
                /*/if (SlotHP[SelectSlot] <= 0)
                {
                    ++SelectSlot;
                    SelectSlotLoop();
                }
                if (SlotHP[SelectSlot] <= 0)
                {
                    ++SelectSlot;
                    SelectSlotLoop();
                }/*/
            }
            if (InputGetAction("ChangeSlotL", "Down"))
            {
                SaveSelectSlot = SelectSlot;
                --SelectSlot;
                SelectSlotLoop();
                if (SelectSlot == 1)
                {
                    //�A�j���[�V������ύX
                    AnimationSummary(0, 22);
                }
                else if (SelectSlot == 2)
                {
                    //�A�j���[�V������ύX
                    AnimationSummary(0, 10);
                }
                else if (SelectSlot == 3)
                {
                    //�A�j���[�V������ύX
                    AnimationSummary(0, 10);
                }
                /*/if (SlotHP[SelectSlot] <= 0)
                {
                    --SelectSlot;
                    SelectSlotLoop();
                }
                if (SlotHP[SelectSlot] <= 0)
                {
                    --SelectSlot;
                    SelectSlotLoop();
                }/*/
            }


            /*/HP��0�ɂȂ�����ʂ̃X���b�g�Ɉڍs
            if (SlotHP[SelectSlot] <= 0)
            {
                ++SelectSlot;
                SelectSlotLoop();
                if (SlotHP[SelectSlot] <= 0)
                {
                    ++SelectSlot;
                    SelectSlotLoop();
                }
                if (SlotHP[SelectSlot] <= 0)
                {
                    ++SelectSlot;
                    SelectSlotLoop();
                }
            }/*/

            SelectSlotLoop();

            if (InputGetAction("ChangeSlotR", "Down") || InputGetAction("ChangeSlotL", "Down"))
            {
                Ast = ActionStatus.Parry;
                st = Status.NoControll;

                //�A�j���[�V������ύX
                AnimationSummary(SlotID[SaveSelectSlot], 10);
            }

        }

    }
    void Parry()
    {
        //ReposeAir();

        if (RepeatCount == 0)
        {
            
            

            //�d�����Ԃ�ݒ�(�S�̃t���[��)
            ActionIncapacityTime = 0.5f;
            //���G���Ԃ�ݒ�
            InvincibleTime = 0.5f;

            //Instantiate(ParryEffectPrefab, transform.position, Quaternion.identity);
        }

        

        if (RepeatCount == 10)
        {
            //�A�j���[�V������ύX
            AnimationSummary(SlotID[SelectSlot], 11);
        }
        
        ++RepeatCount;

            //    �ǉ����͎�t���Ԃ�0�����������
            if (ActionIncapacityTime <= 0)
        {
            //�p���B�̃N�[���^�C����ݒ�
            ParryCoolTime = 2.0f;
            //����Z�̃v���O�������I�����ʏ��Ԃɖ߂�(�����ꂪ�Ȃ��Ɠ���Z���I�����ꂸ�����Ȃ��Ȃ�)
            ResetStatus();
        }
        ActionIncapacityTime -= Time.deltaTime;
    }

    /*/void ChangeDandelion()
    {
        if (RepeatCount == 0)
        {
            //�A�j���[�V������ύX
            AnimationSummary(0, 12);
            //�d�����Ԃ�ݒ�(�S�̃t���[��)
            ActionIncapacityTime = 0.5f;
            //���G���Ԃ�ݒ�
            InvincibleTime = 0.5f;
            //�p���B�̃N�[���^�C����ݒ�
            ParryCoolTime = 2.0f;
            //����Z�̃v���O�������I�����ʏ��Ԃɖ߂�(�����ꂪ�Ȃ��Ɠ���Z���I�����ꂸ�����Ȃ��Ȃ�)
            ResetStatus();
        }
    }/*/

    void TransitionGameOvere()
    {
        //�S�ẴX���b�g��HP��0�ɂȂ����玀�S��Ԃ�  �v
        //if (SlotHP[1] <= 0 && SlotHP[2] <= 0 && SlotHP[3] <= 0)

        //HP��0�ɂȂ����玀�S��Ԃ�
        if (PlayerHP <= 0)
        {
            ResetStatus();
            st = Status.Dead;
        }

    }

    /// <summary>
    ///�֐����FSelectSlotLoop()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�FSelectSlot�̃��[�v����
    /// </summary>
    void SelectSlotLoop()
    {
        //���[�v������
        if (SelectSlot <= 0)
        {
            SelectSlot += 3;
        }
        if (SelectSlot >= 4)
        {
            SelectSlot -= 3;
        }
    }

    /// <summary>
    ///�֐����FActionProduce()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�e�s���ֈڍs����v���O�����𑍊�����֐�
    /// </summary>
    void ActionProduce()
    {
        //�W�����v
        if(InputGetAction("Jump", "Down") && IsGrounded())
        {
            Ast = ActionStatus.Jump;
            st = Status.NoControll;
        }
        /*/�󒆃W�����v
        if (InputGetAction("Jump", "Down") && IsGrounded() == false && JumpRemainCount > 0)
        {
            Ast = ActionStatus.Jump;
            st = Status.NoControll;
            --JumpRemainCount;
        }/*/
        //�ʏ�U��
        if (InputGetAction("NormalAttack", "Down"))
        {
            Ast = ActionStatus.NormalAttack;
            st = Status.NoControll;
        }
        //����U��
        if (InputGetAction("SpecialAttack", "Down"))
        {
            Ast = ActionStatus.SpecialAttack;
            st = Status.NoControll;
        }
    }

    /// <summary>
    ///�֐����FNormalAttack()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�ʏ�U���S�̂𓮂����֐�
    /// </summary>
    void NormalAttack(int ID)
    {
        switch (ID)
        {
            //-----------------------------------------------------------
            //
            //if (RepeatCount == 0)
            //{
            //    //�d�����Ԃƒǉ����͎�t���Ԃ�ݒ�(��̍��v���S�̃t���[��)
            //    ActionIncapacityTime = 0.5f;  �d������
            //    AddInputInterval = 0.5f;�@�@�@�ǉ����͎�t����
            //
            //    (�ŏ��ɕϐ��Ȃǂ��������������ꍇ��������)
            //}++RepeatCount;


            //                  ��N�ɍD���Ȓl����͂��邱�Ƃ�N�t���[���o�ߌ��񂾂��v���O�����𓮂�����
            //if (RepeatCount == N) 
            //{
            //    //��@�U������̃v���n�u���C���X�^���X��
            //    Instantiate(�v���C���[�̒e��Prefab, transform.position, Quaternion.identity);
            //}


            //    �d�����Ԃ�0�����������
            //if (ActionIncapacityTime <= 0)
            //{
            //�@�@�ǉ����͎�t���Ԃ�i�܂���
            //    AddInputInterval -= Time.deltaTime;
            //
            //        ����̃{�^���������ꂽ��ǉ��̏������s��(���i�ڂ̍U�������i�ڂɈڂ�)
            //    if (Input.GetKey(KeyCode.???))
            //    {
            //        
            //    }
            //}


            //    �ǉ����͎�t���Ԃ�0�����������
            //if (AddInputInterval <= 0)
            //{
            //    ����Z�̃v���O�������I�����ʏ��Ԃɖ߂�(�����ꂪ�Ȃ��Ɠ���Z���I�����ꂸ�����Ȃ��Ȃ�)
            //    ResetStatus();
            //}
            //ActionIncapacityTime -= Time.deltaTime;
            //
            //-----------------------------------------------------------

            //�u�o���v
            case 1:
                {
                    Debug.Log("�o���ʏ�U��");
                    AnimationSummary(SlotID[SelectSlot], 8);

                    //Ast = ActionStatus.NoAction;
                    //st = Status.Fine;
                    //this.tag = "PlayerAtk";

                    Instantiate(RoseAttack, new Vector3(transform.position.x,transform.position.y,0.0f), Quaternion.identity);
                    AtkSupport();
                }
                break;
            //�u�^���|�|�v
            case 2:
                {
                    if (RepeatCount == 0)
                    {
                        Debug.Log("�^���|�|�ʏ�U��");
                        AnimationSummary(SlotID[SelectSlot], 8);
                      
                        //�d�����Ԃƒǉ����͎�t���Ԃ�ݒ�(��̍��v���S�̃t���[��)
                        ActionIncapacityTime = 0.3f; //�d������
                        AddInputInterval = 0.5f; //�ǉ����͎�t����
                    }
                    ++RepeatCount;
                    //�d�����Ԃ�0�����������
                    if (ActionIncapacityTime <= 0)
                    {
                        //�ǉ����͎�t���Ԃ�i�܂���
                        AddInputInterval -= Time.deltaTime;

                        //����̃{�^���������ꂽ��ǉ��̏������s��
                        if (InputGetAction("NormalAttack", "Down"))
                        {
                            //�^���|�|�U�����ŏ�����
                            RepeatCount = 0;
                        }
                    }
                    //�ǉ����͎�t���Ԃ�0�����������
                    if (AddInputInterval <= 0)
                    {
                        //����Z�̃v���O�������I�����ʏ��Ԃɖ߂�(�����ꂪ�Ȃ��Ɠ���Z���I�����ꂸ�����Ȃ��Ȃ�)
                        ResetStatus();
                    }
                    ActionIncapacityTime -= Time.deltaTime;
                }
                break;
            //�u�A�T�K�I�v
            case 4:
                {
                    if (RepeatCount == 0)
                    {
                        Debug.Log("�A�T�K�I�ʏ�U��"); 

                        //�d�����Ԃƒǉ����͎�t���Ԃ�ݒ�(��̍��v���S�̃t���[��)
                        ActionIncapacityTime = 0.3f; //�d������
                        AddInputInterval = 0.5f; //�ǉ����͎�t����
                    }
                    ++RepeatCount;

                    MorningGloryAttack();

                    //�d�����Ԃ�0�����������
                    if (ActionIncapacityTime <= 0)
                    {
                        //�ǉ����͎�t���Ԃ�i�܂���
                        AddInputInterval -= Time.deltaTime;

                        //����̃{�^���������ꂽ��ǉ��̏������s��
                        if (InputGetAction("NormalAttack", "Down"))
                        {
                            //�A�T�K�I�U�����ŏ�����
                            RepeatCount = 0;
                        }
                    }
                    
                    //�ʏ��Ԃɖ߂�
                    ResetStatus();
                }
                break;
        }

        //�v
        //switch (Ast)
        //{
        //    //�u�ʏ�U����i�v
        //    case ActionStatus.NormalAttack1:
        //        {
        //            AttackMove();
        //            NormalAttack1();
        //        }
        //        break;
        //    //�u�ʏ�U����i�v
        //    case ActionStatus.NormalAttack2:
        //        {
        //            AttackMove();
        //            NormalAttack2();
        //        }
        //        break;
        //    //�u�ʏ�U���O�i�v
        //    case ActionStatus.NormalAttack3:
        //        {
        //            AttackMove();
        //            NormalAttack3();
        //        }
        //        break;
        //}
    }

    /*/
    /// <summary>
    ///�֐����FNormalAttack1()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�ʏ�U����i�ڂ̊֐�
    /// </summary>
    void NormalAttack1()
    {
        //ReposeAir();
        if (RepeatCount == 0)
        {
            //�A�j���[�V������ʏ�U��1��
            AnimationSummary(SlotID[SelectSlot], 11);
            //�U������̃v���n�u���C���X�^���X��
            Instantiate(AttackCollider1Prefab, transform.position, Quaternion.identity);
            //�d�����Ԃƒǉ����͎�t���Ԃ�ݒ�(��̍��v���S�̃t���[��)
            ActionIncapacityTime = 0.4f;
            AddInputInterval = 0.5f;
            //�t���O��������
            AddInputFlag = false;
        }
        ++RepeatCount;

        if (ActionIncapacityTime <= 0)
        {
            if (InputGetAction("NormalAttack", "Down"))
            {
                AddInputFlag = true;
                //�J��Ԃ��񐔂�������
                RepeatCount = 0;
                Ast = ActionStatus.NormalAttack2;
            }
            //�W�����v�L�����Z������
            if (InputGetAction("Jump", "Down"))
            {
                //�ʏ��Ԃɖ߂�
                Jump();
                ResetStatus();
            }
                AddInputInterval -= Time.deltaTime;
        }
        if (AddInputInterval <= 0)
        {
            //�ʏ��Ԃɖ߂�
            ResetStatus();
        }

        ActionIncapacityTime -= Time.deltaTime;
    }

    /// <summary>
    ///�֐����FNormalAttack2()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�ʏ�U����i�ڂ̊֐�
    /// </summary>
    void NormalAttack2()
    {
        //ReposeAir();
        if (RepeatCount == 0)
        {
            //�A�j���[�V������ʏ�U��2��
            AnimationSummary(SlotID[SelectSlot], 12);
            //�U������̃v���n�u���C���X�^���X��
            Instantiate(AttackCollider2Prefab, transform.position, Quaternion.identity);
            //�d�����Ԃƒǉ����͎�t���Ԃ�ݒ�(��̍��v���S�̃t���[��)
            ActionIncapacityTime = 0.4f;
            AddInputInterval = 0.5f;
            //�t���O��������
            AddInputFlag = false;
        }
        ++RepeatCount;

        if (ActionIncapacityTime <= 0)
        {
            if (InputGetAction("NormalAttack", "Down"))
            {
                AddInputFlag = true;
                //�J��Ԃ��񐔂�������
                RepeatCount = 0;
                Ast = ActionStatus.NormalAttack3;
            }
            //�W�����v�L�����Z������
            if (InputGetAction("Jump", "Down"))
            {
                //�ʏ��Ԃɖ߂�
                Jump();
                ResetStatus();
            }
            AddInputInterval -= Time.deltaTime;
        }
        if (AddInputInterval <= 0)
        {
            //�ʏ��Ԃɖ߂�
            ResetStatus();
        }

        ActionIncapacityTime -= Time.deltaTime;
    }

    /// <summary>
    ///�֐����FNormalAttack3()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�ʏ�U���O�i�ڂ̊֐�
    /// </summary>
    void NormalAttack3()
    {
        //ReposeAir();
        if (RepeatCount == 0)
        {
            //�A�j���[�V������ʏ�U��3��
            AnimationSummary(SlotID[SelectSlot], 13);
            //�U������̃v���n�u���C���X�^���X��
            Instantiate(AttackCollider3Prefab, transform.position, Quaternion.identity);
            //�d�����Ԃƒǉ����͎�t���Ԃ�ݒ�(��̍��v���S�̃t���[��)
            ActionIncapacityTime = 0.5f;
            AddInputInterval = 0.5f;
            //�t���O��������
            AddInputFlag = false;
        }
        ++RepeatCount;

        if (ActionIncapacityTime <= 0)
        {
            //�W�����v�L�����Z������
            if (InputGetAction("Jump", "Down"))
            {
                //�ʏ��Ԃɖ߂�
                Jump();
                ResetStatus();
            }
            AddInputInterval -= Time.deltaTime;
        }
        if (AddInputInterval <= 0)
        {
            //�ʏ��Ԃɖ߂�
            ResetStatus();
        }

        ActionIncapacityTime -= Time.deltaTime;
    }
    /*/

    /// <summary>
    ///�֐����FSpecialAttack()<br />
    ///�������FID	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F���ꂼ��̓���U�����܂Ƃ߂��Ă���֐�
    /// </summary>
    void SpecialAttack(int ID)
    {
        switch (ID)
        {
            //-----------------------------------------------------------
            //
            //if (RepeatCount == 0)
            //{
            //    //�d�����Ԃƒǉ����͎�t���Ԃ�ݒ�(��̍��v���S�̃t���[��)
            //    ActionIncapacityTime = 0.5f;  �d������
            //    AddInputInterval = 0.5f;�@�@�@�ǉ����͎�t����
            //
            //    (�ŏ��ɕϐ��Ȃǂ��������������ꍇ��������)
            //}++RepeatCount;


            //                  ��N�ɍD���Ȓl����͂��邱�Ƃ�N�t���[���o�ߌ��񂾂��v���O�����𓮂�����
            //if (RepeatCount == N) 
            //{
            //    //��@�U������̃v���n�u���C���X�^���X��
            //    Instantiate(�v���C���[�̒e��Prefab, transform.position, Quaternion.identity);
            //}


            //    �d�����Ԃ�0�����������
            //if (ActionIncapacityTime <= 0)
            //{
            //�@�@�ǉ����͎�t���Ԃ�i�܂���
            //    AddInputInterval -= Time.deltaTime;
            //
            //        ����̃{�^���������ꂽ��ǉ��̏������s��(���i�ڂ̍U�������i�ڂɈڂ�)
            //    if (Input.GetKey(KeyCode.???))
            //    {
            //        
            //    }
            //}


            //    �ǉ����͎�t���Ԃ�0�����������
            //if (AddInputInterval <= 0)
            //{
            //    ����Z�̃v���O�������I�����ʏ��Ԃɖ߂�(�����ꂪ�Ȃ��Ɠ���Z���I�����ꂸ�����Ȃ��Ȃ�)
            //    ResetStatus();
            //}
            //ActionIncapacityTime -= Time.deltaTime;
            //
            //-----------------------------------------------------------

            //�u�o���v
            case 1:
                {
                    Debug.Log("�o������U��");
                    Ast = ActionStatus.NoAction;
                    st = Status.Fine;
                }
                break;
            //�u�^���|�|�v
            case 2:
                {
                    if (RepeatCount == 0)
                    {
                        if (CoolDownTimer > 0.0f)
                        {
                            Debug.Log("�N�[���^�C����");
                            ResetStatus();
                            break;
                        }
                        Debug.Log("�^���|�|����U��");
                        _rb.AddForce(Vector2.up * (JumpPower/2));

                        //5�`9�̒e���C���X�^���X��
                        for (int i = 0; i < Random.Range(5, 9); ++i)
                        {
                            //�^���|�|�̒e���C���X�^���X��
                            GameObject DandelionBullet = Instantiate(DandelionBulletPrefab, transform.position, Quaternion.identity);
                            if (DandelionBullet != null)
                            {
                                DandelionBullet.GetComponent<DandelionBullet>().UpdateWey(_sr.flipX);
                                DandelionBullet = null;
                            }
                        }

                        //�A�j���[�V���������U����
                        AnimationSummary(SlotID[SelectSlot], 20);
                        CoolDownTimer = 1.5f;

                        //�d�����Ԃƒǉ����͎�t���Ԃ�ݒ�(��̍��v���S�̃t���[��)
                        ActionIncapacityTime = 0.3f; //�d������
                        AddInputInterval = 0.5f; //�ǉ����͎�t����
                    }
                    ++RepeatCount;
                    //�d�����Ԃ�0�����������
                    if (ActionIncapacityTime <= 0)
                    {
                        //�ǉ����͎�t���Ԃ�i�܂���
                        AddInputInterval -= Time.deltaTime;

                        //����̃{�^���������ꂽ��ǉ��̏������s��
                        if (InputGetAction("SpecialAttack", "Down"))
                        {
                            //�^���|�|�U�����ŏ�����
                            RepeatCount = 0;
                        }
                    }
                    //�ǉ����͎�t���Ԃ�0�����������
                    if (AddInputInterval <= 0)
                    {
                        //����Z�̃v���O�������I�����ʏ��Ԃɖ߂�(�����ꂪ�Ȃ��Ɠ���Z���I�����ꂸ�����Ȃ��Ȃ�)
                        ResetStatus();
                    }
                    ActionIncapacityTime -= Time.deltaTime;
                    
                    
                }
                break;

            //�u�A�T�K�I�v
            case 4:
                {
                    Debug.Log("�A�T�K�I����U��");

                    //����U��������Ȃ������珈��
                    //if (_specialFlag)
                    //{

                    //�d�����Ԃ�ݒ�(�S�̃t���[��)
                    ActionIncapacityTime = 2.0f;
                    //���G���Ԃ�ݒ�
                    InvincibleTime = 2.0f;
                    //�n�ʂ�Ray���������Ă����珈������
                    if (IsGrounded())
                        {
                            //����U��
                            SpecialAtk();
                        }
                    //}
                    //����U�����������珈��
                    //else
                    //{

                    //�^�C�}�[�����Z
                    _specialTimer += Time.deltaTime;
                        //��莞�Ԃ������珈��
                        if (_specialTimer >= SpecialTime)
                        {
                            //�^�C�}�[��������
                            _specialTimer = 0.0f;

                            //���G��Ԃ�����
                            gameObject.tag = "Player";
                        }
                    //}
                    //�ʏ��Ԃɖ߂�
                    ResetStatus();
                }
                break;
        }
    }

    //�ǉ����͎��Ԃ��i�[����ϐ�
    public float AddInputTime;

    //�v�����Ԃ��i�[����ϐ�
    public float IntervalTime;

    public int ACnt;

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
            ACnt = 0;
        }
        if (IntervalTime <= 0.0f)
        {
            ResetStutas();
        }
        IntervalTime -= Time.deltaTime;
    }
    
    

   

    /// <summary>
    /// �֐����FMorningGloryAttack  <br />
    /// ���@���F����  <br />
    /// �߂�l�F����  <br />
    /// �ʏ�U���̊֐�
    /// </summary>
    void MorningGloryAttack()
    {
        //X���������珈��
        //if (Input.GetKeyDown(KeyCode.X))
        {
            for (int i = 0; i < 3; i++)
            {
                if (BulletSelect == i)
                {
                    //�C���X�^���X������ꏊ
                    Vector3 _instanceBulletPos;
                    _instanceBulletPos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);

                    //�C���X�^���X��
                    Instantiate(BulletObj[i], _instanceBulletPos, Quaternion.identity);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// �֐����FSpecialAtk
    /// ���@���F����
    /// �߂�l�F����
    /// �A�T�K�I�̓���U���̊֐�
    /// </summary>
    void SpecialAtk()
    {
        UncontrollableTimer = 1.5f;
        //���G
        gameObject.tag = "Invincible";

        //�Ռ��g
        Instantiate(SpecialObj, transform.position, Quaternion.identity);
    }

    public void ResetStutas()
     {
           this.tag = "Player";
           ACnt = 0;
           st = Status.Fine;
        Ast = ActionStatus.NoAction;
     }

    /// <summary>
    ///�֐����FDandelionFall()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�^���|�|�̕��V����
    /// </summary>
    void DandelionFall()
    {
        //�^���|�|��ԂȂ畂�V�������s��
        if (SlotID[SelectSlot]==2)
        {
            //�W�����v�������ꂽ�ۋ󒆂ł���Ȃ�
            if(InputGetAction("Jump", "Stay") && IsGrounded() == false)
            {
                if (_rb.velocity.y < -DandelionGravityMaxSpeed)
                {
                    //���݂̉������ꎞ�I�ɕۑ�����
                    Vector2 v = _rb.velocity;
                    v.y = -DandelionGravityMaxSpeed;
                    _rb.velocity = v;
                }
            }
        } 
    }

    /// <summary>
    ///�֐����FInputGetAction()<br />
    ///�������FInputActionType(�f�[�^�^string) Timing"Down","Stay","Up"(�f�[�^�^string)<br />
    ///�߂�l�Fbool <br />
    ///�p�@�r�F�s�����Ƃɓ��͂𑍊�����֐��@<br />
    /// </summary>
    bool InputGetAction(string InputActionType, string Timing)
    {
        //�M�������邩�Ȃ�����ۑ�����ϐ�
        bool Signal = false;

        switch (InputActionType)
        {
            case "Jump":
                {
                    if (Timing == "Down")
                    {
                        if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Space))
                        { Signal = true; }
                    }
                    if (Timing == "Stay")
                    {
                        if (Input.GetKey(KeyCode.Joystick1Button0) || Input.GetKey(KeyCode.Space))
                        { Signal = true; }
                    }
                    if (Timing == "Up")
                    {
                        if (Input.GetKeyUp(KeyCode.Joystick1Button0) || Input.GetKeyUp(KeyCode.Space))
                        { Signal = true; }
                    }
                }
                break;
            case "NormalAttack":
                {
                    if (Timing == "Down")
                    {
                        if (Input.GetKeyDown(KeyCode.Joystick1Button2) || Input.GetKeyDown(KeyCode.UpArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Stay")
                    {
                        if (Input.GetKey(KeyCode.Joystick1Button2) || Input.GetKey(KeyCode.UpArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Up")
                    {
                        if (Input.GetKeyUp(KeyCode.Joystick1Button2) || Input.GetKeyUp(KeyCode.UpArrow))
                        { Signal = true; }
                    }
                }
                break;
            case "SpecialAttack":
                {
                    if (Timing == "Down")
                    {
                        if (Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.DownArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Stay")
                    {
                        if (Input.GetKey(KeyCode.Joystick1Button1) || Input.GetKey(KeyCode.DownArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Up")
                    {
                        if (Input.GetKeyUp(KeyCode.Joystick1Button1) || Input.GetKeyUp(KeyCode.DownArrow))
                        { Signal = true; }
                    }
                }
                break;
            case "attack1":
                {
                    
                }
                break;
            case "attack2":
                {
                }
                break;
            case "attack3":
                {
                }
                break;
            case "ChangeSlotR":
                {
                    if (Timing == "Down")
                    {
                        if (Input.GetKeyDown(KeyCode.Joystick1Button4) || Input.GetKeyDown(KeyCode.RightArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Stay")
                    {
                        if (Input.GetKey(KeyCode.Joystick1Button4) || Input.GetKey(KeyCode.RightArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Up")
                    {
                        if (Input.GetKeyUp(KeyCode.Joystick1Button4) || Input.GetKeyUp(KeyCode.RightArrow))
                        { Signal = true; }
                    }
                }
                break;
            case "ChangeSlotL":
                {
                    if (Timing == "Down")
                    {
                        if (Input.GetKeyDown(KeyCode.Joystick1Button5) || Input.GetKeyDown(KeyCode.LeftArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Stay")
                    {
                        if (Input.GetKey(KeyCode.Joystick1Button5) || Input.GetKey(KeyCode.LeftArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Up")
                    {
                        if (Input.GetKeyUp(KeyCode.Joystick1Button5) || Input.GetKeyUp(KeyCode.LeftArrow))
                        { Signal = true; }
                    }
                }
                break;
            case "RightMove":
                {
                    if (Timing == "Down")
                    {
                        if (Input.GetKeyDown(KeyCode.D))
                        { Signal = true; }
                    }
                    if (Timing == "Stay")
                    {
                        float h = Input.GetAxis("Horizontal");
                        if (h == 1)
                        { Signal = true; }
                        if (Input.GetKey(KeyCode.D))
                        { Signal = true; }
                    }
                    if (Timing == "Up")
                    {
                        if (Input.GetKeyUp(KeyCode.D))
                        { Signal = true; }
                    }
                }
                break;
            case "LeftMove":
                {
                    if (Timing == "Down")
                    {
                        if (Input.GetKeyDown(KeyCode.A))
                        { Signal = true; }
                    }
                    if (Timing == "Stay")
                    {
                        float h = Input.GetAxis("Horizontal");
                        if (h == -1)
                        { Signal = true; }
                        if (Input.GetKey(KeyCode.A))
                        { Signal = true; }
                    }
                    if (Timing == "Up")
                    {
                        if (Input.GetKeyUp(KeyCode.A))
                        { Signal = true; }
                    }
                }
                break;
        }
        
        return Signal;
    }

    void AirAnimation()
    {
        Vector2 v = _rb.velocity;
        //�㏸
        if (v.y > 5.0f)
        {
            AnimationSummary(SlotID[SelectSlot], 3);
        }
        else
        //���~
        if (v.y < -5.0f)
        {
            AnimationSummary(SlotID[SelectSlot], 5);
        }
        //�؋�
        else
        {
            AnimationSummary(SlotID[SelectSlot], 4);
        }
    }

    /// <summary>
    ///�֐����FAnimationSummary()<br />
    ///�������FFlourID ActionType<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�A�j���[�V�����𑍊�����֐��@<br />
    /// </summary>
    void AnimationSummary(int FlourID, int ActionType)
    {
        int PlayAnimation = 0;
        switch (FlourID)
        {
            
            case 0:
                {
                    PlayAnimation += 000 + ActionType;
                }
                break;
            //�o��
            case 1:
                {
                    PlayAnimation += 100 + ActionType;
                }
                break;
            case 2:
                {
                    PlayAnimation += 200 + ActionType;
                }
                break;
            //�A�T�K�I
            case 3:
                {
                    PlayAnimation = +300 + ActionType;
                }
                break;
            case 4:
                {
                    PlayAnimation = +400 + ActionType;
                }
                break;
        }


        if (Sprite != null)
        {
            Sprite.GetComponent<PlayerSprite>().UpdateAnimation(PlayAnimation);
            Sprite.GetComponent<PlayerSprite>().UpdatePlayerWey(_sr.flipX);
        }

        _animator.SetInteger("Action", PlayAnimation);
    }

    /// <summary>
    ///�֐����FResetInterval()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�e���ԊԊu�n��̕ϐ���������(���G�A��e���ԈȊO)
    /// </summary>
    void ResetInterval()
    {
        //�J��Ԃ��񐔂�������
        RepeatCount = 0;
    }

    /// <summary>
    ///�֐����FResetStatus()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�ړ���W�����v���ł����Ԃɖ߂�����
    /// </summary>
    void ResetStatus()
    {
        //�A�j���[�V������ҋ@��Ԃ�
        AnimationSummary(SlotID[SelectSlot], 0);

        ResetInterval();
        Ast = ActionStatus.NoAction;
        st = Status.Fine;
        _rb.bodyType = RigidbodyType2D.Dynamic;
    }

    /// <summary>
    ///�֐����FVibrationCamera()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�J������h�炷�֐����Ăяo��
    /// </summary>
    void VibrationCamera()
    {
        if (Camera != null)
        {
            Camera.GetComponent<NewCameraMove>().VibrationCamera(0.2f, 0.2f);
        }
    }

    /// <summary>
    ///�֐����FGetFindObject()<br />
    ///�������F�T�������I�u�W�F�N�g�̖��O	<br />
    ///�߂�l�F�T�����I�u�W�F�N�g <br />
    ///�p�@�r�F���O����I�u�W�F�N�g��T���擾�̐������s���m�F����
    /// </summary>
    Object GetFindObject(string ObjectName)
    {
        GameObject Object= GameObject.Find(ObjectName);
        if (Object == null)
        {
            Debug.Log(ObjectName+"�͑��݂��܂���");
        }
        else
        {
            Debug.Log(ObjectName + "�̃I�u�W�F�N�g�擾�ɐ���");
        }
        return Object;
    }

    /// <summary>
    ///�֐����FOnCollisionEnter2D()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�����蔻��
    /// </summary>
    void OnCollisionStay2D(Collision2D collision)
    {
        //���G���ԂłȂ��Ȃ�
        if (InvincibleTime <= 0)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                
                //�J������h�炷
                VibrationCamera();
                //�X�v���C�g��U��������
                Sprite.GetComponent<PlayerSprite>().UpdateVibStatus(0.1f, 1.2f, 0.3f, 0.3f, 500.0f);

                //�d�����Ԃ�ݒ�
                StiffnessTime = 0.5f;
                //���G���Ԃ�ݒ�
                InvincibleTime = 1.0f;
                //�e���ԊԊu�n��̕ϐ���������
                ResetInterval();

                //��~�t���O�Ǝ��Ԃ�ݒ�(�q�b�g�X�g�b�v)
                StopFlag = true;
                StopTime = 0.1f;

                //HP��1���炷
                //--SlotHP[SelectSlot];
                PlayerHP -= 1;


                //�ڐG�Ώۂ̍��W���擾
                Vector3 tmp = GameObject.Find(collision.gameObject.name).transform.position;

                Debug.Log("�Փˑ���̖��O�́A" + collision.gameObject.name);
                Debug.Log(tmp.x);

                HitDamagePosX = tmp.x;
                st = Status.DamageStiffness;
            }
            if (collision.gameObject.tag == "EnemyAttack")
            {
                //�A�j���[�V�������e��Ԃ�
                AnimationSummary(SlotID[SelectSlot], 2);
                //�J������h�炷
                VibrationCamera();
                //�X�v���C�g��U��������
                Sprite.GetComponent<PlayerSprite>().UpdateVibStatus(0.1f, 1.2f, 0.3f, 0.3f, 500.0f);

                //�d�����Ԃ�ݒ�
                StiffnessTime = 0.5f;
                //���G���Ԃ�ݒ�
                InvincibleTime = 1.0f;
                //�e���ԊԊu�n��̕ϐ���������
                ResetInterval();

                //��~�t���O�Ǝ��Ԃ�ݒ�(�q�b�g�X�g�b�v)
                StopFlag = true;
                StopTime = 0.1f;


                //HP��2���炷
                //SlotHP[SelectSlot]-=2;
                PlayerHP -= 2;


                //�ڐG�Ώۂ̍��W���擾
                Vector3 tmp = GameObject.Find(collision.gameObject.name).transform.position;

                Debug.Log("�Փˑ���̖��O�́A" + collision.gameObject.name);
                Debug.Log(tmp.x);


                HitDamagePosX = tmp.x;
                st = Status.DamageStiffness;
            }
        }

    }
    void TestController()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            Debug.Log("Joystick1Button0");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            Debug.Log("Joystick1Button1");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button2))
        {
            Debug.Log("Joystick1Button2");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button3))
        {
            Debug.Log("Joystick1Button3");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button4))
        {
            Debug.Log("Joystick1Button4");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button5))
        {
            Debug.Log("Joystick1Button5");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button6))
        {
            Debug.Log("Joystick1Button6");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button7))
        {
            Debug.Log("Joystick1Button7");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button8))
        {
            Debug.Log("Joystick1Button8");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button9))
        {
            Debug.Log("Joystick1Button9");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button10))
        {
            Debug.Log("Joystick1Button10");
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button11))
        {
            Debug.Log("Joystick1Button11");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button12))
        {
            Debug.Log("Joystick1Button12");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button13))
        {
            Debug.Log("Joystick1Button13");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button14))
        {
            Debug.Log("Joystick1Button14");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button15))
        {
            Debug.Log("Joystick1Button15");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button16))
        {
            Debug.Log("Joystick1Button16");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button17))
        {
            Debug.Log("Joystick1Button17");
        }
    }
    void CheckStatus()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            switch (st)
            {
                case Status.Fine:
                    { Debug.Log("�ʏ펞"); }
                    break;
                case Status.Dead:
                    { Debug.Log("���S"); }
                    break;
                case Status.Respawn:
                    { Debug.Log("���A"); }
                    break;
                case Status.Clear:
                    { Debug.Log("�N���A"); }
                    break;
                case Status.NoControll:
                    { Debug.Log("����s�\"); }
                    break;
                case Status.DamageStiffness:
                    { Debug.Log("��e�d��"); }
                    break;
            }
            switch (Ast)
            {
                //�p���B
                case ActionStatus.Parry:
                    {
                        //Parry();
                    }
                    break;
                //�W�����v
                case ActionStatus.Jump:
                    { Debug.Log("�W�����v"); }
                    break;
                //�m�[�A�N�V����
                case ActionStatus.NoAction:
                    { Debug.Log("�m�[�A�N�V����"); }
                    break;
            }

        }

    }
    //�y�f�o�b�O�p�֐��z
    //Ray��\��
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, new Vector3(0, -1.2f, 0));
    }
}