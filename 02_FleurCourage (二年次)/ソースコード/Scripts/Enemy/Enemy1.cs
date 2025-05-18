using UnityEngine;

public class Enemy1 : Enemy
{
    private bool _walkFlag;                 //�������ǂ����̃t���O
    private bool _getPosFlag;               //��x�������W���擾����ׂ̃t���O

    [SerializeField, Tooltip("���Ԃ肩������ɐ؂�ւ�鎞��")]
    private float _walkTimer;               //�����ɐ؂�ւ��܂ł̎���

    [SerializeField, Tooltip("Ray�̒���")]
    private float _rayLength;

    private Vector2 _turnOverPos;           //���Ԃ������̍��W

    [SerializeField, Tooltip("�n�ʂ̃��C���[")]
    private LayerMask GroundLayer;          //


    void Start()
    {
        //������
        _walkFlag = false;
        _getPosFlag = true;

        EnemyHP = 2;
    }

    void Update()
    {
        //�]����̃A�j���[�V����
        MyAnimator.SetInteger("Action", 1);

        //�Ȗтɓ����������̏���
        EnemyFly();

        if (_isKnockedBack)
        {
            WaitTimer -= Time.deltaTime;
            if (WaitTimer <= 0.0f)
            {
                ChangeAlpha();
                WaitTimer = _firstwaitTime;
            }
        }

        //�J�����ɓ��������瓮���o������Enemy��Dead����Ȃ������瓮���o��
        if (_moveFlag && !Dead() && !_isFly && !_isStun)
        {
            //�̗͂�2�̎�
            if (EnemyHP == 2 && !_isKnockedBack)
            {
                //�����̊֐�
                Move();
            }
            //�̗͂�1�̎�
            else if (EnemyHP == 1 && !_isKnockedBack)
            {
                //�����̃t���O
                if (!_walkFlag && IsGrounded())
                {
                    _walkTimer -= Time.deltaTime;
                    MyRigidbody.velocity = Vector2.zero;
                    //���Ԃ�̃A�j���[�V������������֐�
                    TurnOver();

                    if (_getPosFlag)
                    {
                        //��x�������W���擾
                        _turnOverPos = transform.position;
                        _getPosFlag = false;
                    }

                    //�����̍��W�𗠕Ԃ������̍��W�ŏ㏑��
                    transform.position = _turnOverPos;
                }

                if (_walkTimer <= 0.0f)
                {
                    //���Ԃ����㉽�b����ɕ���
                    MyAnimator.SetInteger("Action", 0);
                    _walkFlag = true;
                }
            }
        }
    }
    /// <summary>
    /// �֐����FMove    <br />
    /// ���@���F����  <br />
    /// �߂�l�F����  <br />
    /// ���Ɍ������ē]����֐�
    /// </summary>
    override protected void Move()
    {
        //�����̃A�j���[�V����
        MyAnimator.SetInteger("Action", 1);

        //�����̏���
        MyRigidbody.velocity = new Vector2(-MoveSpeed, MyRigidbody.velocity.y);
    }
    /// <summary>
    /// �֐����FTurnOver    <br />
    /// ���@���F����  <br />
    /// �߂�l�F����  <br />
    /// ���Ԃ�̃A�j���[�V������������֐� <br />
    /// </summary>
    private void TurnOver()
    {
        //���Ԃ�̃A�j���[�V����
        MyAnimator.SetInteger("Action", 2);
        gameObject.tag = "Invincible";
    }
    /// <summary>
    /// �֐����FWalk
    /// ���@���F����
    /// �߂�l�F����
    /// �����̃A�j���[�V�����ɂȂ����瓮�����֐�
    /// (�A�j���[�V�����C�x���g�ɂĎg�p)
    /// </summary>
    private void Walk()
    {
        //tag��Enemy�ɕύX
        gameObject.tag = "Enemy";
        //�����̏���
        MyRigidbody.velocity = new Vector2(-MoveSpeed * 0.2f, MyRigidbody.velocity.y);
    }
    public bool IsGrounded()
    {
        bool ret = false;
        //��������Ray���΂��āA�w�肵�����C���[�̃I�u�W�F�N�g�ƐڐG���Ă��邩�ǂ����𔻕ʂ���
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _rayLength, GroundLayer);
        //�q�b�g���Ă��Ȃ��ꍇ��null���������
        if (hit.collider != null)
        {
            ret = true;
        }
        return ret;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, new Vector3(0, -_rayLength, 0));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //�J�����Ɍ�����悤�ɂȂ����瓮���o��
        if (collision.gameObject.tag == "MainCamera")
        {
            _moveFlag = true;
        }
        //Player�̍U���ɓ���������Enemy��HP�����炷
        if (collision.gameObject.tag == "PlayerAtk")
        {
            //�_���[�W����
            TakeDamage();
        }
        //�^���|�|�̖Ȗтɓ���������
        OnTriggerDandelionBullet(collision);

        //�A�T�K�I�̉����ɓ���������
        OnTriggerMorninggloryBullet(collision);
    }
}
