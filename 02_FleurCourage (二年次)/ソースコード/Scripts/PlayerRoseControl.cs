using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerRoseControl : PlayerBase
{
    [SerializeField]
    private GameObject RoseKickColliderPrefab;   //�L�b�N�����蔻��̃v���n�u
    public GameObject RoseKickCollider;         //�L�b�N�����蔻���ۑ�����
    public GameObject GameOverObj;

    public float KickSpeedCoefficient;          //�L�b�N���x�W��
    public bool KickCancelFlag;                 //�L�b�N�������I�ɏI�����邩�̃t���O

    private bool _isNormalAttack;               //�ʏ�U�������ǂ����������t���O
    [SerializeField]
    private GameObject RoseNormalCol;           //�ʏ�U���̓����蔻��

    private CapsuleCollider2D _co;//CapsuleCollider2D�̊i�[


    void Start()
    {
        //�A�^�b�`����Ă���Component���擾���Ă���
        _co = GetComponent<CapsuleCollider2D>();
        _sr = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();

        dead = false;
        _isNormalAttack = false;

        Camera = GameObject.Find("Main Camera");

        //Sprite��\������I�u�W�F�N�g���C���X�^���X��
        if (SpritePrefab != null)
        {
            GameObject childObject = Instantiate(SpritePrefab, transform);
            Sprite = childObject;
            Sprite.GetComponent<PlayerSprite>().UpdatePlayerObject(this.gameObject);
        }
    }

    void Update()
    {
        //�������̕��A����
        FallReturn_Instant();






        //Manager��Player��HP�����X�V
        UpdateHP_toManager();

        //���g�̃I�u�W�F�N�g�𓧖���
        GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 0);

        //��~���Ԃ�0���傫���ꍇ�قڂ��ׂĂ̏������~
        if (StopTimer > 0.0f)
        {
            //��~���Ԃ��o�߂�����
            StopTimer -= Time.deltaTime;
            if (StopTimer < 0.0f)
            { StopTimer = 0.0f; }
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
                            AnimationPass(0);
                        }
                        //�ڒn���Ă��Ȃ��ꍇ�̓A�j���[�V�������u�󒆁v�ɂ���
                        else
                        {
                            AirAnimation();
                        }

                        ActionProduce();
                        Move();
                        MaxGravitySpeed();

                        //���G���Ԃ��o�߂�����
                        if (InvincibleTimer > 0.0f)
                        { InvincibleTimer -= Time.deltaTime; }
                        //���G���Ԃ�0�����������0�ɂ���
                        if (InvincibleTimer < 0.0f)
                        { InvincibleTimer = 0.0f; }

                        if (HP <= 0)
                        {
                            st = Status.Dead;
                        }
                    }
                    break;
                //�u����s�\�v
                case Status.NoControll:
                    {
                        //�s��
                        switch (Ast)
                        {
                            //�W�����v
                            case ActionStatus.Jump:
                                {
                                    Jump();
                                }
                                break;

                            //�؂�ւ��J�n
                            case ActionStatus.ChangeStart:
                                {
                                    ChangeStart();
                                }
                                break;

                            //�؂�ւ��I��
                            case ActionStatus.ChangeEnd:
                                {
                                    ChangeEnd();
                                }
                                break;

                            //�ʏ�U��
                            case ActionStatus.NormalAttack:
                                {
                                    Debug.Log("�ʏ�U������");
                                    NormalAttack();
                                }
                                break;
                            //����U��
                            case ActionStatus.SpecialAttack:
                                {
                                    SpecialAttack();
                                }
                                break;
                        }

                        //���G���Ԃ��o�߂�����
                        if (InvincibleTimer > 0.0f)
                        { InvincibleTimer -= Time.deltaTime; }
                        //���G���Ԃ�0�����������0�ɂ���
                        if (InvincibleTimer < 0.0f)
                        { InvincibleTimer = 0.0f; }
                    }
                    break;
                //�u��e�d���v
                case Status.DamageStiffness:
                    {
                        DamageStiffnessFunction();
                    }
                    break;
                //�u�������A�v
                case Status.FallReturn:
                    {
                        FallReturn();
                    }
                    break;
                //�u���S����v
                case Status.Dead:
                    {
                        dead = true;
                        fadeTimer += Time.deltaTime;
                        if (fadeTimer >= 0.3f)
                        {
                            Instantiate(GameOverObj, new Vector3(transform.position.x, transform.position.y, 0.0f), Quaternion.identity);
                            Destroy(gameObject);
                            //SceneManager.LoadScene("GameOver"); // (��)
                        }
                    }
                    break;
            }
        }

        //�������A�����̏����ݒ�
        //SetFallReturn();

        //����U���N�[���^�C�}�[��i�߂�
        if (SpecialAtkCoolTimer > 0.0f) { SpecialAtkCoolTimer -= Time.deltaTime; }
        //����U���N�[���^�C�}�[��0�����������0�ɂ���
        if (SpecialAtkCoolTimer < 0.0f) { SpecialAtkCoolTimer = 0.0f; }
        //����U���N�[���^�C�����}�l�[�W���[�ɑ���
        if (Camera != null) { Camera.GetComponent<PlayerManager>().SetCoolTimer(SpecialAtkCoolTimer, SpecialAtkCoolTime); }

        //EP�����R��
        if (Camera.GetComponent<PlayerManager>().EP_CheckLimit() == false)
        { Camera.GetComponent<PlayerManager>().SetEP(Camera.GetComponent<PlayerManager>().GetEP() + EP_HealRate * 1); }

    }

    /// <summary>
    ///�֐����FNormalAttack()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�ʏ�U���̏���
    /// </summary>
    void NormalAttack()
    {
        if (RepeatCounter == 0)
        {
            //�J��Ԃ��񐔂��L�^
            RepeatCountFlag = true;
            _isNormalAttack = true;
            if (_isNormalAttack)
            {
                //�����蔻����C���X�^���X
                Instantiate(RoseNormalCol, transform.position, Quaternion.identity, transform);
                _isNormalAttack = false;
            }
        }

        Move();
        //�A�j���[�V������ʏ�U����
        AnimationPass(14);

        if (RepeatCounter >= 30)
        {
            //�����蔻�������
            Destroy(GameObject.Find("RoseNormalAttackCol(Clone)"));

            //����Z�̃v���O�������I�����ʏ��Ԃɖ߂�(�����ꂪ�Ȃ��Ɠ���Z���I�����ꂸ�����Ȃ��Ȃ�)
            ResetStatus();
        }
        RepeatCount();
    }

    /// <summary>
    ///�֐����FcialcialAttack()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F����U���̏���
    /// </summary>
    void SpecialAttack()
    {
        switch (SpecialAttackStep)
        {
            //�n�ォ�󒆂Ŏ��ɔh��
            case 0:
                {
                    //�t���O��������
                    KickCancelFlag = false;

                    if (IsGrounded())
                    { SpecialAttackStep = 1; }
                    else { SpecialAttackStep = 2; }

                }
                break;
            //�W�����v
            case 1:
                {
                    if (RepeatCounter == 0)
                    {
                        AnimationPass(11);
                        //�����x��������
                        Vector2 v = new Vector2();
                        v.x = 0.0f;
                        v.y = 0.0f;
                        _rb.velocity = v;
                        //��ւ̗͂������ăW�����v������
                        _rb.AddForce(Vector2.up * JumpPower);

                        //�J��Ԃ��񐔂��L�^
                        RepeatCountFlag = true;
                    }

                    if (_sr.flipX)
                    {
                        //�ړ�
                        transform.Translate(-0.05f, 0.0f, 0.0f);
                    }
                    else
                    {
                        //�ړ�
                        transform.Translate(0.05f, 0.0f, 0.0f);
                    }

                    //30��J��Ԃ����������n�߂���
                    Vector2 vel = _rb.velocity;
                    if (RepeatCounter >= 30 || vel.y < -1.0f)
                    {
                        SpecialAttackStep = 2;
                        ResetInterval();
                    }
                    RepeatCount();
                }
                break;
            //�L�b�N
            case 2:
                {
                    if (RepeatCounter == 0)
                    {
                        AnimationPass(12);
                        //�L�b�N���x�W����������
                        KickSpeedCoefficient = 0.0f;

                        //�����x��������
                        Vector2 v = new Vector2();
                        v.x = 0.0f;
                        v.y = 0.0f;
                        _rb.velocity = v;

                        //�d�͂�0��
                        _rb.gravityScale = 0;

                        //���G���Ԃ�ݒ�
                        InvincibleTimer = 1.0f;

                        //�L�b�N�����蔻����C���X�^���X��

                        if (RoseKickColliderPrefab != null)
                        {
                            GameObject childObject = Instantiate(RoseKickColliderPrefab, transform.position, Quaternion.identity, transform);
                            RoseKickCollider = childObject;
                            RoseKickCollider.GetComponent<RoseKickCollider>().GetPlayerObject(this.gameObject);
                            Debug.Log("�L�b�N�����蔻��̃C���X�^���X���ɐ���");
                        }
                        else { Debug.Log("�L�b�N�����蔻��̃v���n�u��������܂���"); }

                        //�J��Ԃ��񐔂��L�^
                        RepeatCountFlag = true;

                        //EP������
                        Camera.GetComponent<PlayerManager>().AddEP(-EP_Cost);
                    }

                    if (KickSpeedCoefficient < 1.0f)
                    {
                        KickSpeedCoefficient += 0.005f + KickSpeedCoefficient / 3.0f;
                    }

                    if (_sr.flipX)
                    {
                        //�ړ�
                        transform.Translate(-0.2f * KickSpeedCoefficient, -0.1f * KickSpeedCoefficient, 0.0f);
                    }
                    else
                    {
                        //�ړ�
                        transform.Translate(0.2f * KickSpeedCoefficient, -0.1f * KickSpeedCoefficient, 0.0f);
                    }

                    //�w��F�o�߂��t���O��true�ɂȂ�����
                    //if (RepeatCounter >= 50 || KickCancelFlag == true)
                    if (KickCancelFlag == true)
                    {
                        //�U������𐶐�
                        if (RoseKickCollider != null)
                        {
                            RoseKickCollider.GetComponent<RoseKickCollider>().GeneratePlayerAttack();
                        }

                        //�N�[���^�C�}�[�����Z
                        SpecialAtkCoolTimer = SpecialAtkCoolTime;

                        //������폜��NULL����
                        Destroy(RoseKickCollider);
                        RoseKickCollider = null;

                        //�d�͂�߂�
                        _rb.gravityScale = 5;
                        _co.isTrigger = false;

                        _rb.bodyType = RigidbodyType2D.Dynamic;
                        SpecialAttackStep = 3;
                        ResetInterval();
                    }
                    RepeatCount();
                }
                break;
            //���˕Ԃ�
            case 3:
                {
                    if (RepeatCounter == 0)
                    {
                        AnimationPass(13);
                        //�����x��������
                        Vector2 v = new Vector2();
                        v.x = 0.0f;
                        v.y = 0.0f;
                        _rb.velocity = v;
                        //��ւ̗͂������ăW�����v������
                        _rb.AddForce(Vector2.up * JumpPower / 3.0f);

                        //�v���C���[�̌����ɉ����ĉ������ɗ͂�������
                        if (_sr.flipX)
                        { _rb.AddForce(Vector2.right * 400); }
                        else
                        { _rb.AddForce(Vector2.left * 400); }

                        //�J��Ԃ��񐔂��L�^
                        RepeatCountFlag = true;

                        //�J������h�炷
                        if (Camera != null)
                        { Camera.GetComponent<NewCameraMove>().VibrationCamera(0.5f, 0.2f); }
                    }

                    //�����x��X���̂�N����1�Ɍ���������;
                    Vector2 vel = _rb.velocity;
                    vel.x = vel.x / 1.1f;
                    _rb.velocity = vel;

                    //���͂��������ꍇ���˕Ԃ莞�Ԃ𖳎����ē���U��
                    if (InputGetAction("SpecialAttack", "Down_No"))
                    {
                        //�����x��������
                        Vector2 v = new Vector2();
                        v.x = 0.0f;
                        v.y = 0.0f;
                        _rb.velocity = v;

                        //�t���O��������
                        KickCancelFlag = false;

                        ResetInterval();

                        SpecialAttackStep = 0;
                    }

                    if (RepeatCounter >= 15)
                    {
                        SpecialAttackStep = 0;
                        ResetStatus();
                    }
                    RepeatCount();
                }
                break;
        }
    }
    public void UpdateKickCancelFlag(bool Flag)
    {
        KickCancelFlag = Flag;
    }
}
