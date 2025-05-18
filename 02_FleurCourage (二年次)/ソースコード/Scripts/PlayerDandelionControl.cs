    using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDandelionControl : PlayerBase
{
    public GameObject DandelionBulletPrefab;//�^���|�|�̒e�̃v���n�u
    public GameObject DandelionAttackPrefab;

    public float GlideSpeed; //���󗎉����x
    public float UpPower; //�㏸���x

    private bool IsBlower;//���t���O
    public int FloatTimer;//���V�^�C�}�[

    public GameObject GameOverObj;

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
                        if (FloatTimer == 0)
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
                        }


                        ActionProduce();
                        Move();
                        MaxGravitySpeed();
                        GlideGravitySpeed();
                        FloatBlower();

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
                                    if (_rb.gravityScale < 3)
                                    {
                                        _rb.gravityScale = 3;
                                    }
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
                                    _rb.gravityScale = 3;
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


        if (FloatTimer > 0) { --FloatTimer; }
        if (FloatTimer < 0) { FloatTimer = 0; }

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
            AnimationPass(10);
            //�J��Ԃ��񐔂��L�^
            RepeatCountFlag = true;
        }

        if (RepeatCounter == 15)
        {
            Instantiate(DandelionAttackPrefab, transform.position, Quaternion.identity);
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
        if (RepeatCounter == 0)
        {
            //�A�j���[�V���������U����
            AnimationPass(15);
            //�J��Ԃ��񐔂��L�^
            RepeatCountFlag = true;
            //EP������
            Camera.GetComponent<PlayerManager>().AddEP(-EP_Cost);
            //�N�[���^�C�}�[�����Z
            SpecialAtkCoolTimer = SpecialAtkCoolTime;

        }
        if (RepeatCounter == 5)
        {
            //5�`9�̒e���C���X�^���X��
            for (int i = 0; i < UnityEngine.Random.Range(5, 9); ++i)
            {
                //�^���|�|�̒e���C���X�^���X��
                GameObject DandelionBullet = Instantiate(DandelionBulletPrefab, transform.position, Quaternion.identity);
                if (DandelionBullet != null)
                {
                    DandelionBullet.GetComponent<DandelionBullet>().UpdateWey(_sr.flipX);
                    DandelionBullet = null;
                }
            }//
        }
        if (RepeatCounter >= 40)
        {
            //����Z�̃v���O�������I�����ʏ��Ԃɖ߂�(�����ꂪ�Ȃ��Ɠ���Z���I�����ꂸ�����Ȃ��Ȃ�)
            ResetStatus();
        }
        RepeatCount();
    }

    /// <summary>
    ///�֐����FGlideGravitySpeed()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F���󎞂̏���
    /// </summary>
    void GlideGravitySpeed()
    {
        if (InputGetAction("Jump", "Stay") && IsGrounded() == false)
        {
            //���̗������x��ۂ�
            if (_rb.velocity.y <= -GlideSpeed)
            {
                //�A�j���[�V�����������Ԃ�
                AnimationPass(9);

                //���݂̉������ꎞ�I�ɕۑ�����
                Vector2 v = _rb.velocity;
                v.y = -GlideSpeed;
                _rb.velocity = v;
            }
        }
    }

    void FloatBlower()
    {
        if (InputGetAction("Jump", "Stay") && IsGrounded() == false && IsBlower)
        {
            FloatTimer = 20;
            
        }
        if (FloatTimer > 0)
        {
            //���̑��x��ۂ�
            if (_rb.velocity.y <= 5)
            {
                //���݂̉������ꎞ�I�ɕۑ�����
                Vector2 v = _rb.velocity;
                v.y += 1.0f;
                _rb.velocity = v;

                //�A�j���[�V�����������Ԃ�
                AnimationPass(9);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //�͈͓��ɂ���Ƃ�
        if (collision.gameObject.tag == "Brower")
        {
            IsBlower = true;
            
            /* if (InputGetAction("Jump", "Stay") && IsGrounded() == false)
            {
                //�v���C���[���㏸������
                _rb.AddForce(Vector2.up * UpPower);
                _rb.gravityScale = 6;

                //�A�j���[�V�����������Ԃ�
                AnimationPass(9);
            }
            else
            {
                //�͈͊O�ɏo���Ƃ��̋�������
                _rb.gravityScale = 2;
            }
             */
           
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Brower")
        {
            IsBlower = false;

            //�͈͊O�ɏo���Ƃ��̋�������
            //_rb.gravityScale = 2;
        }
    }
}

