using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMorninggloryControl : PlayerBase
{
    public GameObject AtkObjPrefab;
    public GameObject AtkObjPrefab2;
    public GameObject SPAtkObjPrefab;
    public GameObject SpEffectPrefab;
    public GameObject SpEffectEndPrefab;
    public GameObject GameOverObj;

    public bool Atktriger;
    public bool EffectTrigger;

    void Start()
    {
        //�A�^�b�`����Ă���Component���擾���Ă���
        _sr = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();

        Camera = GameObject.Find("Main Camera");
        if (Camera != null)
        { Debug.Log("�J�����I�u�W�F�N�g�̎擾�ɐ���"); }
        else
        { Debug.Log("�J�����I�u�W�F�N�g�̎擾�Ɏ��s"); }

        //Sprite��\������I�u�W�F�N�g���C���X�^���X��
        if (SpritePrefab != null)
        {
            GameObject childObject = Instantiate(SpritePrefab, transform);
            Sprite = childObject;
            Sprite.GetComponent<PlayerSprite>().UpdatePlayerObject(this.gameObject);
        }

        Atktriger = false;

    }

    void Update()
    {
        //�������̕��A����
        FallReturn_Instant();

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
                                    //�d�͂�߂�
                                    _rb.gravityScale = 5;
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
                                    Debug.Log("����U������");
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
       // SetFallReturn();

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
            //�A�j���[�V���������U����
            AnimationPass(12);
            //�J��Ԃ��񐔂��L�^
            RepeatCountFlag = true;
            if (_sr.flipX == true)
            {
                Instantiate(AtkObjPrefab2, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(AtkObjPrefab, transform.position, Quaternion.identity);
            }

        }

        if (RepeatCounter >= 30)
        {
            //����Z�̃v���O�������I�����ʏ��Ԃɖ߂�(�����ꂪ�Ȃ��Ɠ���Z���I�����ꂸ�����Ȃ��Ȃ�)
            ResetStatus();
        }
        RepeatCount();
    }

    /// <summary>
    ///�֐����FSpecialAttack()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F����U���̏���
    /// </summary>
    void SpecialAttack()
    {
        if (IsGrounded() == true)
        {
            if (RepeatCounter == 0)
            {
                //�A�j���[�V���������U����
                AnimationPass(10);
                //�J��Ԃ��񐔂��L�^
                RepeatCountFlag = true;
                //���G���Ԃ��o�߂�����

                //EP������
                Camera.GetComponent<PlayerManager>().AddEP(-EP_Cost);
                //�N�[���^�C�}�[�����Z
                SpecialAtkCoolTimer = SpecialAtkCoolTime;
            }

            if (RepeatCounter >= 30)
            {
                //�A�j���[�V���������U����
                AnimationPass(11);

                if (EffectTrigger == false)
                {
                    Instantiate(SpEffectPrefab, transform.position, Quaternion.identity);
                    EffectTrigger = true;
                }

                    //if (Atktriger == false)
                    {
                    Instantiate(SPAtkObjPrefab, transform.position, Quaternion.identity);
                    //Atktriger = true; 
                }
            }

            if (RepeatCounter >= 90)
            {

                Instantiate(SpEffectEndPrefab, transform.position, Quaternion.identity);
                //����Z�̃v���O�������I�����ʏ��Ԃɖ߂�(�����ꂪ�Ȃ��Ɠ���Z���I�����ꂸ�����Ȃ��Ȃ�)
                ResetStatus();
                Atktriger = false;
                EffectTrigger = false;
            }
            RepeatCount();
        }
        else
        {
            ResetStatus();
            RepeatCount();
        }
    }    
}