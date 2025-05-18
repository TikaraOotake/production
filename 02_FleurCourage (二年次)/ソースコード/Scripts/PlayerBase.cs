using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public GameObject SpritePrefab;//Sprite��\������I�u�W�F�N�g�̃v���n�u
    public GameObject Sprite;//���������X�v���C�g�I�u�W�F�N�g��ۑ�����
    public GameObject Camera;//�J�����̃I�u�W�F�N�g��ۑ�����

    public float MoveSpeed; //�ړ����x
    public float JumpPower;//�W�����v��
    public int HP;//�̗�
    protected int AtkPower;//�U����
    public float VulneraRate;//�Ǝ㗦

    public float SpecialAtkCoolTimer;//����U���N�[���^�C�}�[
    public float SpecialAtkCoolTime;//����U���N�[���^�C��
    public int EP_Cost;//����U������EP
    public int EP_HealRate;//EP�񕜗�

    protected float StopTimer;//�قڑS�Ă̏����̒�~���Ԃ��v��
    protected float InvincibleTimer;//���G���Ԃ��v��(�_���[�W���󂯂Ȃ�����)
    protected float StiffnessTimer;//�d�����Ԃ��v��(��؂̑�����󂯕t���Ȃ�����)

    public float ActionIncapacityTimer;//�s�����̍d�����Ԃ��v��
    public float AddInputInterval;//�s�����̒ǉ����͎�t���Ԃ��v��

    public float GravityMaxSpeed;//�ō��������x
    public float RepeatCounter;//�J��Ԃ��񐔂��v��
    public bool RepeatCountFlag;//�J��Ԃ��񐔂����邩�̃t���O
    public bool dead;
    public float HitDamagePosX;//��Q(�_���[�W)��^���Ă����Ώە��̍��W��ۑ�����
    protected bool IsHitBigPillbug;//���������Ώۂ�����_���S���V���L�^

    public int SpecialAttackStep;//����U���̒i�K���L�^

    public bool SlotShiftSign;//�I������X���b�g���ǂ���ɂ��炷���w��

    public LayerMask GroundLayer; //�n�ʂƂ��Ĕ��肷��I�u�W�F�N�g�̃��C���[

    protected float fadeTimer = 0.0f;
    protected SpriteRenderer _sr; //SpriteRenderer�̊i�[�p
    protected Rigidbody2D _rb; //Rigidbody2D�̊i�[�p

    protected Vector2 FallReturn_Pos;
    protected float FallReturnJamp_Height;
    protected bool FallReturnFlag;//�������A���������s���邩�̃t���O

    private bool _isMoving;

    [SerializeField]
    protected GameObject DamageEffect;

    [SerializeField]
    protected GameObject DamageEffect2;

    public enum Status
    {
        Fine, //�ʏ�
        Dead, //���S
        Respawn, //���A
        Clear, //�N���A
        NoControll,//����s�\
        DamageStiffness,//��e�d��
        FallReturn,//�������A
    }
    public Status st;

    public enum ActionStatus
    {
        NoAction,//�������Ă��Ȃ�
        Parry,//�p���B
        Jump,//�W�����v
        NormalAttack,//�ʏ�U��
        SpecialAttack,//����U��
        ChangeStart,//�؂�ւ��J�n
        ChangeEnd,//�؂�ւ��I��
    }
    public ActionStatus Ast;

    void Awake()
    {
        _isMoving = true;
    }
    /// <summary>
    ///�֐����FUpdateHP_toManager()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�FPlayerManager��Player��HP�����X�V����
    /// </summary>
    public void UpdateHP_toManager()
    {
        if (Camera != null)
        {
            Camera.GetComponent<PlayerManager>().UpdatePlayerHP(HP);
        }
    }

    /// <summary>
    ///�֐����Fmove()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�v���C���[�̍��E�ړ�����
    /// </summary>
    public void Move()
    {
        //��������
        if (InputGetAction("LeftMove", "Stay") && InputGetAction("RightMove", "Stay"))
        {
            return;
        }

        //A�������ꂽ��
        if (InputGetAction("LeftMove", "Stay") && IsWalledL() == false)
        {
            //�ړ�
            transform.Translate(-MoveSpeed, 0.0f, 0.0f);
            //X���ɑ΂��Ĕ��]������
            _sr.flipX = true;

            //�ڒn���Ă��鎞�̓A�j���[�V�������u���s�v�ɂ���
            if (IsGrounded() == true)
            {
                AnimationPass(2);
            }
        }
        else
        //D�������ꂽ��
        if (InputGetAction("RightMove", "Stay") && IsWalledR() == false)
        {
            //�ړ�
            transform.Translate(MoveSpeed, 0.0f, 0.0f);
            //X���ɑ΂��Ĕ��]������
            _sr.flipX = false;

            //�ڒn���Ă��鎞�̓A�j���[�V�������u���s�v�ɂ���
            if (IsGrounded() == true)
            {
                AnimationPass(2);
            }
        }
    }

    public void SetMoveFlag(bool ret)
    {
        _isMoving = ret;
    }

    public void ActionProduce()
    {
        //�W�����v
        if (InputGetAction("Jump", "Down") && IsGrounded())
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
            //�N�[���^�C�}�[��0�̏ꍇ and EP���R�X�g�ȏ�c���Ă���ꍇ
            if (SpecialAtkCoolTimer <= 0.0f && EP_Cost <= Camera.GetComponent<PlayerManager>().GetEP())
            {
                Ast = ActionStatus.SpecialAttack;
                st = Status.NoControll;
            }

        }
        //�Ԑ؂�ւ��E
        if (InputGetAction("ChangeSlotR", "Down"))
        {
            SlotShiftSign = true;
            Ast = ActionStatus.ChangeStart;
            st = Status.NoControll;
        }
        //�Ԑ؂�ւ���
        if (InputGetAction("ChangeSlotL", "Down"))
        {
            SlotShiftSign = false;
            Ast = ActionStatus.ChangeStart;
            st = Status.NoControll;
        }

    }

    /// <summary>
    ///�֐����FJump()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�W�����v����
    /// </summary>
    public void Jump()
    {
        if (RepeatCounter == 0)
        {
            //�A�j���[�V�������W�����v�ҋ@��Ԃ�
            AnimationPass(8);
            //�J��Ԃ��񐔂��L�^
            RepeatCountFlag = true;
        }

        //transform.Translate(0.0f, jumpMomentum, 0.0f);
        if (RepeatCounter >= 2)
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
        }
        RepeatCount();
    }

    /// <summary>
    ///�֐����FChangeStart()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�؂�ւ��J�n����
    /// </summary>
    public void ChangeStart()
    {
        InvincibleTimer = 0.1f;

        if (RepeatCounter == 0)
        {
            //�A�j���[�V������؂�ւ��J�n��Ԃ�
            AnimationPass(6);
            Debug.Log("�A�j���[�V�������Đ�");
            //�J��Ԃ��񐔂��L�^
            RepeatCountFlag = true;
            //�󒆂Ȃ�c���𐧌�
            if (IsGrounded() == false) {
                //Y���̉����x��������
                Vector2 v = _rb.velocity;
                v.y = 0.0f;
                _rb.velocity = v;

                //�d�͂�0��
                _rb.gravityScale = 0;
            }
        }

        if (RepeatCounter >= 25)
        {
            //Player�̏���Manager�ɑ���
            //Camera.GetComponent<PlayerManager>().GetPlayerStatus(new Vector2(transform.position.x, transform.position.y),HP);

            if (Camera != null)
            {
                //Player�̏���Manager�ɑ���
                Camera.GetComponent<PlayerManager>().ChangePlayer(new Vector2(transform.position.x, transform.position.y), SlotShiftSign, HP, _sr.flipX, this.gameObject);

            }
            else
            {
                Debug.Log("�J�����I�u�W�F�N�g���ۑ�����Ă��܂���");
            }

            //���g��j��
            Destroy(this.gameObject);

            //�d�͂�߂�
            _rb.gravityScale = 5;

            //�؂�ւ��̃v���O�������I�����@���Œʏ��Ԃɖ߂�
            ResetStatus();
        }
        RepeatCount();
    }

    /// <summary>
    ///�֐����FChangeEnd()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�؂�ւ��I������
    /// </summary>
    public void ChangeEnd()
    {
        InvincibleTimer = 0.1f;

        if (RepeatCounter == 0)
        {
            //�A�j���[�V������؂�ւ��I����Ԃ�
            AnimationPass(7);
            //�J��Ԃ��񐔂��L�^
            RepeatCountFlag = true;
            //�󒆂Ȃ�c���𐧌�
            if (IsGrounded() == false)
            { 
                //Y���̉����x��������
                Vector2 v = _rb.velocity;
                v.y = 0.0f;
                _rb.velocity = v;

                //�d�͂�0��
                _rb.gravityScale = 0;
            }
        }

        if (RepeatCounter >= 15)
        {
            Debug.Log("�؂�ւ�����");

            //�d�͂�߂�
            _rb.gravityScale = 5;

            //�؂�ւ��I���̃v���O�������I�����ʏ��Ԃɖ߂�
            ResetStatus();
        }
        RepeatCount();
    }

    /// <summary>
    ///�֐����FDamageBounce()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F��e���̐�����я���
    /// </summary>
    public void DamageBounce()
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
    ///�֐����FDamageBounce()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F��e���̐�����я���
    /// </summary>
    public void DamageBounce_BigPillbug()
    {
        if (HitDamagePosX > this.transform.position.x)
        {
            this.transform.position = new Vector2(HitDamagePosX + 3.0f, this.transform.position.y);
        }

        //�����x��������
        Vector2 v = new Vector2();
        v.x = 0.0f;
        v.y = 0.0f;
        _rb.velocity = v;
        //��Ɖ��ւ̗͂������ăW�����v������
        _rb.AddForce(Vector2.up * JumpPower / 1.5f * 1.05f);
        _rb.AddForce(Vector2.right * JumpPower / 2.0f * 5.0f);

    }

    /// <summary>
    ///�֐����FMaxGravitySpeed()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�������x������߂�h���֐�
    /// </summary>
    public void MaxGravitySpeed()
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
    ///�֐����FIsGrounded() 	<br />
    ///�������F�Ȃ�		<br />
    ///�߂�l�F�ڒn���Ă���ꍇ��true,���Ă��Ȃ��ꍇ��false		<br />
    ///�p�@�r�F�ڒn���菈��
    /// </summary>
    public bool IsGrounded()
    {
        bool ret = false;
        //��������Ray���΂��āA�w�肵�����C���[�̃I�u�W�F�N�g�ƐڐG���Ă��邩�ǂ����𔻕ʂ���
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, Vector2.down, 1.8f, GroundLayer);
        //�q�b�g���Ă��Ȃ��ꍇ��null���������
        if (hit.collider != null)
        {
            ret = true;
        }
        hit = Physics2D.Raycast(transform.position - new Vector3(0.2f, 0.0f, 0.0f), Vector2.down, 1.8f, GroundLayer);
        //�q�b�g���Ă��Ȃ��ꍇ��null���������
        if (hit.collider != null)
        {
            ret = true;
        }
        hit = Physics2D.Raycast(transform.position - new Vector3(-0.2f, 0.0f, 0.0f), Vector2.down, 1.8f, GroundLayer);

        //�q�b�g���Ă��Ȃ��ꍇ��null���������
        if (hit.collider != null)
        {
            ret = true;
        }
        return ret;
    }

    /// <summary>
    ///�֐����FIsGrounded() 	<br />
    ///�������F�Ȃ�		<br />
    ///�߂�l�F�ڒn���Ă���ꍇ��true,���Ă��Ȃ��ꍇ��false		<br />
    ///�p�@�r�F�L���ڒn���菈��
    /// </summary>
    public bool IsWideGrounded()
    {
        bool ret = false;
        //��������Ray���΂��āA�w�肵�����C���[�̃I�u�W�F�N�g�ƐڐG���Ă��邩�ǂ����𔻕ʂ���
        RaycastHit2D hit1 = Physics2D.Raycast(transform.position, Vector2.down, 1.8f, GroundLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position - new Vector3(0.5f, 0.0f, 0.0f), Vector2.down, 1.8f, GroundLayer);
        RaycastHit2D hit3 = Physics2D.Raycast(transform.position - new Vector3(-0.5f, 0.0f, 0.0f), Vector2.down, 1.8f, GroundLayer);
        //�q�b�g���Ă��Ȃ��ꍇ��null���������
        if (hit1.collider != null && hit2.collider != null && hit3.collider != null)
        {
            ret = true;
        }
        return ret;
    }


    /// <summary>
    ///�֐����FIsWalled() 	<br />
    ///�������F�Ȃ�		<br />
    ///�߂�l�F�ڒn���Ă���ꍇ��true,���Ă��Ȃ��ꍇ��false		<br />
    ///�p�@�r�F�ڕǔ��菈��
    /// </summary>
    public bool IsWalledR()
    {
        bool ret = false;
        //��������Ray���΂��āA�w�肵�����C���[�̃I�u�W�F�N�g�ƐڐG���Ă��邩�ǂ����𔻕ʂ���
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, Vector2.right, 0.3f, GroundLayer);
        //�q�b�g���Ă��Ȃ��ꍇ��null���������
        if (hit.collider != null)
        {
            ret = true;
        }
        else if(!IsWideGrounded())//�󒆂ɋ���ꍇ�͍X�ɉ��̕����݂�
        {
            hit = Physics2D.Raycast(transform.position + new Vector3(0.0f, -0.9f, 0.0f), Vector2.right, 0.3f, GroundLayer);
        }
        return ret;
    }

    /// <summary>
    ///�֐����FIsWalledL() 	<br />
    ///�������F�Ȃ�		<br />
    ///�߂�l�F�ڒn���Ă���ꍇ��true,���Ă��Ȃ��ꍇ��false		<br />
    ///�p�@�r�F�ڕǔ��菈��
    /// </summary>
    public bool IsWalledL()
    {
        bool ret = false;
        //��������Ray���΂��āA�w�肵�����C���[�̃I�u�W�F�N�g�ƐڐG���Ă��邩�ǂ����𔻕ʂ���
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, Vector2.left, 0.3f, GroundLayer);
        //�q�b�g���Ă��Ȃ��ꍇ��null���������
        if (hit.collider != null)
        {
            ret = true;
        }
        else if (!IsWideGrounded())//�󒆂ɋ���ꍇ�͍X�ɉ��̕����݂�
        {
            hit = Physics2D.Raycast(transform.position + new Vector3(0.0f, -0.9f, 0.0f), Vector2.left, 0.3f, GroundLayer);
        }
        return ret;
    }

    /// <summary>
    ///�֐����FFallReturn_Instant() 	<br />
    ///�������F�Ȃ�		<br />
    ///�߂�l�F�Ȃ�		<br />
    ///�p�@�r�F�������̕��A����(�u�Ԉړ�)
    /// </summary>
    public void  FallReturn_Instant()
    {
        //�Ō�ɒn�ʂƐڐG���Ă������W���L�^
        if (IsWideGrounded())
        {
            if (Camera != null)
            { Camera.GetComponent<PlayerManager>().SetReturnPos(this.transform.position);  }
            FallReturn_Pos = this.transform.position;
        }
        //���������ꍇ�Ō�ɒn�ʂɂ������W�ɔ�΂�
        if (this.transform.position.y < -10.0f)
        {
            Return_Teleport();
        }
    }

    public void Return_Teleport()
    {
        if (Camera != null)
        { this.transform.position = Camera.GetComponent<PlayerManager>().GetReturnPos(); }

        //HP��1���炷
        HP -= 1;
        //���G���Ԃ�ݒ�
        InvincibleTimer = 2.0f;
        //���G���ԕ��_�ł�����
        Sprite.GetComponent<PlayerSprite>().UpdateFlicker(InvincibleTimer);
    }

    /// <summary>
    ///�֐����FResetInterval()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�e���ԊԊu�n��̕ϐ���������(���G�A��e���ԈȊO)
    /// </summary>
    public void ResetInterval()
    {
        //�J��Ԃ��񐔂�������
        RepeatCounter = 0;
        //�J��Ԃ��񐔂��L�^���邩�̃t���O��false��
        RepeatCountFlag = false;
    }

    /// <summary>
    ///�֐����FSetFallReturn()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�������A�̏����ݒ�
    /// </summary>
    protected void SetFallReturn()
    {
        //�󒆂ɂ���ۂ̍ő�̍������L�^
        if (IsGrounded() == false)
        {
            if (FallReturnJamp_Height < this.transform.position.y)
            {
                FallReturnJamp_Height = this.transform.position.y;
            }

        }
        else//�n��ɂ��鎞�͋L�^�����݂̍����ɑ�����
        {
            FallReturnJamp_Height = this.transform.position.y;
        }

        //���������ꍇ�ڕW�̍����܂ŃW�����v
        if (this.transform.position.y < -10.0f)
        {
            if (FallReturnFlag == false)
            {
                //�������A��Ԃ�
                st = Status.FallReturn;

                //HP��1���炷
                HP -= 1;
                //���G���Ԃ�ݒ�
                InvincibleTimer = 3.0f;
                //���G���ԕ��_�ł�����
                Sprite.GetComponent<PlayerSprite>().UpdateFlicker(InvincibleTimer);
                //�J������h�炷
                if (Camera != null)
                { Camera.GetComponent<NewCameraMove>().VibrationCamera(0.2f, 0.4f); }

                //�J��Ԃ��񐔂��L�^
                RepeatCountFlag = true;
                FallReturnFlag = true;
            }

        }
    }
    /// <summary>
    ///�֐����FFallReturn()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�������A����
    /// </summary>
    protected void FallReturn()
    {
        Move();

        //�ڒn���Ă��鎞�̓A�j���[�V�������u�ҋ@�v�ɂ���
        if (IsGrounded() == true)
        {
            AnimationPass(0);
        }
        //�ڒn���Ă��Ȃ��ꍇ�̓A�j���[�V�������u�󒆁v�ɂ���
        else
        {
            AirAnimation();
        }

        Debug.Log("�ڕW�̍���" + FallReturnJamp_Height);
        Debug.Log("�v���C���[�̍���" + this.transform.position.y);
        Vector2 v = new Vector2();
        v.x = 0.0f;
        v.y = 0.0f;
        _rb.velocity = v;
        //��ւ̗͂������ăW�����v������
        _rb.AddForce(Vector2.up * JumpPower);
        if (FallReturnJamp_Height < this.transform.position.y || RepeatCounter >= 60)
        {
            FallReturnFlag = false;
            st = Status.Fine;
            ResetInterval();
        }
        RepeatCount();
    }

    /// <summary>
    ///�֐����FDamageStiffnessFunction()<br />
    ///�������F�Ȃ�<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F��e�d�����ԏ���
    /// </summary>
    public void DamageStiffnessFunction()
    {
        if (RepeatCounter == 0)
        {
            //�A�j���[�V�������_���[�W��
            AnimationPass(1);
            //�m�b�N�o�b�N����
            DamageBounce();
            if(IsHitBigPillbug)
            {
                DamageBounce_BigPillbug();
                IsHitBigPillbug = false;
            }
            //�J��Ԃ��񐔂��L�^
            RepeatCountFlag = true;
        }

        //�����x��X���̂�N����1�Ɍ���������;
        Vector2 v = _rb.velocity;
        v.x = v.x / 1.1f;
        _rb.velocity = v;

        StiffnessTimer -= Time.deltaTime;
        if (StiffnessTimer <= 0)
        {
            //�����x��X���̂ݏ�����
            Vector2 v2 = _rb.velocity;
            v2.x = 0.0f;
            _rb.velocity = v2;

            StiffnessTimer = 0.0f;
            st = Status.Fine;
            //�ʏ��Ԃɖ߂�
            ResetStatus();
            //���G���ԕ��_�ł�����
            Sprite.GetComponent<PlayerSprite>().UpdateFlicker(InvincibleTimer);
        }

        RepeatCount();
    }

    /// <summary>
    ///�֐����FResetStatus()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�ړ���W�����v���ł����Ԃɖ߂�����
    /// </summary>
    protected void ResetStatus()
    {
        ResetInterval();
        Ast = ActionStatus.NoAction;
        st = Status.Fine;
        SpecialAttackStep = 0;
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.constraints = RigidbodyConstraints2D.None;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    /// <summary>
    ///�֐����FAnimationPass()<br />
    ///�������FActionType<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�󒆃A�j���[�V�������㏸�؋󉺍~���Ƃɐ؂�ւ��鏈���@<br />
    /// </summary>
    public void AirAnimation()
    {
        Vector2 v = _rb.velocity;
        //�㏸
        if (v.y > 5.0f)
        {
            AnimationPass(3);
        }
        else
        //���~
        if (v.y < -5.0f)
        {
            AnimationPass(5);
        }
        //�؋�
        else
        {
            AnimationPass(4);
        }
    }

    /// <summary>
    ///�֐����FAnimationPass()<br />
    ///�������FActionType<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�Đ�����A�j���[�V�����̎�ނ�SpriteObject�ɓn�������@<br />
    /// </summary>
    public void AnimationPass(int ActionType)
    {
        if (Sprite != null)
        {
            Sprite.GetComponent<PlayerSprite>().UpdateAnimation(ActionType);
            Sprite.GetComponent<PlayerSprite>().UpdatePlayerWey(_sr.flipX);
        }
    }

    /// <summary>
    ///�֐����FRepeatCount()<br />
    ///�������F�Ȃ��@<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�J��Ԃ��񐔂����Z���Ă�������<br />
    /// </summary>
    public void RepeatCount()
    {
        if (RepeatCountFlag)
        {
            ++RepeatCounter;
        }
    }

    /// <summary>
    ///�֐����FInputGetAction()<br />
    ///�������FInputActionType(�f�[�^�^string) Timing"Down","Stay","Up"(�f�[�^�^string)<br />
    ///�߂�l�Fbool <br />
    ///�p�@�r�F�s�����Ƃɓ��͂𑍊�����֐��@<br />
    /// </summary>
    public bool InputGetAction(string InputActionType, string Timing)
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
                        if (Input.GetKeyDown(KeyCode.Joystick1Button3) || Input.GetKeyDown(KeyCode.DownArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Stay")
                    {
                        if (Input.GetKey(KeyCode.Joystick1Button3) || Input.GetKey(KeyCode.DownArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Up")
                    {
                        if (Input.GetKeyUp(KeyCode.Joystick1Button3) || Input.GetKeyUp(KeyCode.DownArrow))
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
                        if (Input.GetKeyDown(KeyCode.Joystick1Button5) || Input.GetKeyDown(KeyCode.RightArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Stay")
                    {
                        if (Input.GetKey(KeyCode.Joystick1Button5) || Input.GetKey(KeyCode.RightArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Up")
                    {
                        if (Input.GetKeyUp(KeyCode.Joystick1Button5) || Input.GetKeyUp(KeyCode.RightArrow))
                        { Signal = true; }
                    }
                }
                break;
            case "ChangeSlotL":
                {
                    if (Timing == "Down")
                    {
                        if (Input.GetKeyDown(KeyCode.Joystick1Button4) || Input.GetKeyDown(KeyCode.LeftArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Stay")
                    {
                        if (Input.GetKey(KeyCode.Joystick1Button4) || Input.GetKey(KeyCode.LeftArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Up")
                    {
                        if (Input.GetKeyUp(KeyCode.Joystick1Button4) || Input.GetKeyUp(KeyCode.LeftArrow))
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
                        if (h >= 0.1f)
                        {
                            { Signal = true; }
                        }
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
                        if (-h >= 0.1f)
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
        if (!_isMoving) { Signal = false; }
        return Signal;
    }

    /// <summary>
    ///�֐����FInheritStatus()<br />
    ///�������F�v���C���\�̃X�e�[�^�X<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�}�l�W���[�ɑ���ꂽPlayer�̏��������p���@<br />
    /// </summary>
    public void InheritStatus(int GetHP, bool GetWay)
    {
        //�A�^�b�`����Ă���Component���擾���Ă���
        _sr = GetComponent<SpriteRenderer>();

        //�󂯎���������̏���Ή�����e�ϐ��ɑ��
        HP = GetHP;
        _sr.flipX = GetWay;

        Debug.Log("�}�l�[�W���[����󂯎�����������J��" + GetWay);
        //�؂�ւ��I���̒i�K��
        Ast = ActionStatus.ChangeEnd;
        st = Status.NoControll;
    }

    /// <summary>
    ///�֐����Fcheck()<br />
    ///�������F�Ȃ�<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�Ăяo���ł��邩�̃e�X�g�@<br />
    /// </summary>
    public void check()
    {
        Debug.Log("�Ăяo������");
    }

    /// <summary>
    ///�֐����FOnCollisionEnter2D()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�����蔻��
    /// </summary>
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Abyss")
        {
            Return_Teleport();

            //�J������h�炷
            if (Camera != null)
            { Camera.GetComponent<NewCameraMove>().VibrationCamera(0.2f, 0.2f); }
        }
    }

    /// <summary>
    ///�֐����FOnCollisionEnter2D()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�����蔻��
    /// </summary>
    public void OnCollisionStay2D(Collision2D collision)
    {
        //���G���ԂłȂ��Ȃ�
        if (InvincibleTimer <= 0)
        {
            if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBoss" || collision.gameObject.tag == "TutorialBoss")
            {
                //�A�j���[�V�������e��Ԃ�
                AnimationPass(1);

                //�J������h�炷
                if (Camera != null)
                { Camera.GetComponent<NewCameraMove>().VibrationCamera(0.2f, 0.2f); }
                //�X�v���C�g��U��������
                if (Sprite != null)
                { Sprite.GetComponent<PlayerSprite>().UpdateVibStatus(0.1f, 1.2f, 0.3f, 0.3f, 500.0f); }


                //�d�����Ԃ�ݒ�
                StiffnessTimer = 0.5f;
                //���G���Ԃ�ݒ�
                InvincibleTimer = 1.0f;
                //�e���ԊԊu�n��̕ϐ���������
                ResetInterval();

                //��~���Ԃ�ݒ�(�q�b�g�X�g�b�v)
                StopTimer = 0.1f;

                //HP�����炷
                HP -= 1* (int)VulneraRate;

                if (_sr.flipX == true)
                {
                    //�_���[�W�̃G�t�F�N�g�𔭐�������
                    Instantiate(DamageEffect2, new Vector3(transform.position.x + 2.5f, transform.position.y + 1.5f, 0.0f), Quaternion.identity, transform);
                }
                else
                {
                    //�_���[�W�̃G�t�F�N�g�𔭐�������
                    Instantiate(DamageEffect, new Vector3(transform.position.x - 2.5f, transform.position.y + 1.5f, 0.0f), Quaternion.identity, transform);
                }


                //�ڐG�Ώۂ̍��W���擾
                Vector3 tmp = GameObject.Find(collision.gameObject.name).transform.position;

                Debug.Log("�Փˑ���̖��O�́A" + collision.gameObject.name);
                Debug.Log(tmp.x);

                HitDamagePosX = tmp.x;
                st = Status.DamageStiffness;
                if(collision.gameObject.tag == "TutorialBoss")
                { IsHitBigPillbug = true; }
            }
            if (collision.gameObject.tag == "EnemyAttack" )
            {
                //�A�j���[�V�������e��Ԃ�
                AnimationPass(1);

                //�J������h�炷
                if (Camera != null)
                { Camera.GetComponent<NewCameraMove>().VibrationCamera(0.2f, 0.2f); }
                //�X�v���C�g��U��������
                if (Sprite != null)
                { Sprite.GetComponent<PlayerSprite>().UpdateVibStatus(0.1f, 1.2f, 0.3f, 0.3f, 500.0f); }

                //�d�����Ԃ�ݒ�
                StiffnessTimer = 0.5f;
                //���G���Ԃ�ݒ�
                InvincibleTimer = 1.0f;
                //�e���ԊԊu�n��̕ϐ���������
                ResetInterval();

                //��~���Ԃ�ݒ�(�q�b�g�X�g�b�v)
                StopTimer = 0.1f;


                //HP�����炷
                HP -= 2 * (int)VulneraRate;


                //�ڐG�Ώۂ̍��W���擾
                Vector3 tmp = GameObject.Find(collision.gameObject.name).transform.position;

                Debug.Log("�Փˑ���̖��O�́A" + collision.gameObject.name);
                Debug.Log(tmp.x);


                HitDamagePosX = tmp.x;
                st = Status.DamageStiffness;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //���G���ԂłȂ��Ȃ�
        if (InvincibleTimer <= 0)
        {
            if (collision.gameObject.tag == "TutorialBoss")
            {
                //�A�j���[�V�������e��Ԃ�
                AnimationPass(1);

                //�J������h�炷
                if (Camera != null)
                { Camera.GetComponent<NewCameraMove>().VibrationCamera(0.2f, 0.2f); }
                //�X�v���C�g��U��������
                if (Sprite != null)
                { Sprite.GetComponent<PlayerSprite>().UpdateVibStatus(0.1f, 1.2f, 0.3f, 0.3f, 500.0f); }

                //�d�����Ԃ�ݒ�
                StiffnessTimer = 0.5f;
                //���G���Ԃ�ݒ�
                InvincibleTimer = 1.0f;
                //�e���ԊԊu�n��̕ϐ���������
                ResetInterval();

                //��~���Ԃ�ݒ�(�q�b�g�X�g�b�v)
                StopTimer = 0.1f;


                //HP�����炷
                HP -= 1 * (int)VulneraRate;


                //�ڐG�Ώۂ̍��W���擾
                Vector3 tmp = GameObject.Find(collision.gameObject.name).transform.position;

                Debug.Log("�Փˑ���̖��O�́A" + collision.gameObject.name);
                Debug.Log(tmp.x);


                HitDamagePosX = tmp.x;
                st = Status.DamageStiffness;

                IsHitBigPillbug = true;
            }

            if (collision.gameObject.tag == "Enemy"|| collision.gameObject.tag == "EnemyBoss")
            {
                //�A�j���[�V�������e��Ԃ�
                AnimationPass(1);

                //�J������h�炷
                if (Camera != null)
                { Camera.GetComponent<NewCameraMove>().VibrationCamera(0.2f, 0.2f); }
                //�X�v���C�g��U��������
                if (Sprite != null)
                { Sprite.GetComponent<PlayerSprite>().UpdateVibStatus(0.1f, 1.2f, 0.3f, 0.3f, 500.0f); }


                //�d�����Ԃ�ݒ�
                StiffnessTimer = 0.5f;
                //���G���Ԃ�ݒ�
                InvincibleTimer = 1.0f;
                //�e���ԊԊu�n��̕ϐ���������
                ResetInterval();

                //��~���Ԃ�ݒ�(�q�b�g�X�g�b�v)
                StopTimer = 0.1f;

                //HP�����炷
                HP -= 1 * (int)VulneraRate;

                if (_sr.flipX == true)
                {
                    //�_���[�W�̃G�t�F�N�g�𔭐�������
                    Instantiate(DamageEffect2, new Vector3(transform.position.x + 2.5f, transform.position.y + 1.5f, 0.0f), Quaternion.identity, transform);
                }
                else
                {
                    //�_���[�W�̃G�t�F�N�g�𔭐�������
                    Instantiate(DamageEffect, new Vector3(transform.position.x - 2.5f, transform.position.y + 1.5f, 0.0f), Quaternion.identity, transform);
                }


                //�ڐG�Ώۂ̍��W���擾
                Vector3 tmp = GameObject.Find(collision.gameObject.name).transform.position;

                Debug.Log("�Փˑ���̖��O�́A" + collision.gameObject.name);
                Debug.Log(tmp.x);

                HitDamagePosX = tmp.x;
                st = Status.DamageStiffness;
            }
        }
    }

    //�y�f�o�b�O�p�֐��z
    //Ray��\��
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, new Vector3(0, -1.8f, 0));
        Gizmos.DrawRay(transform.position - new Vector3(0, 0, 0), new Vector3(0.3f, 0, 0));
        Gizmos.DrawRay(transform.position - new Vector3(0.2f, 0.0f, 0.0f), new Vector3(0, -1.8f, 0));
        Gizmos.DrawRay(transform.position - new Vector3(-0.2f, 0.0f, 0.0f), new Vector3(0, -1.8f, 0));
    }
}
