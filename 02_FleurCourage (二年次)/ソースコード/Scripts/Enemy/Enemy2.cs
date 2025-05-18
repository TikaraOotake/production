using System.Collections.Generic;
using UnityEngine;
public class Enemy2 : Enemy
{
    private bool _startFlag;                        //�J�����ɓ��������瓮�������ׂ̃X�C�b�`
    private bool _angryFlag;                        //

    [SerializeField, Tooltip("�s�������锼�a�̒���")]
    private float _rLength;                         //���a�̒���
    [SerializeField, Tooltip("Ray�̒���")]
    private float _reyLength;                       //Ray�̒���
    
    [SerializeField, Tooltip("�w���ɏ���Ă��ėǂ�����")]
    private float _stayTimer;                       //�w���ɏ���Ă��ėǂ�����
    private float _firstStayTime;

    [SerializeField, Tooltip("�U�����[�V�������̉����̑���")]
    private float _shakingSpeed;
    [SerializeField, Tooltip("�U�����[�V�������̉����̒���")]
    private float _shakingLength;

    [SerializeField, Tooltip("�U�����[�V���������鎞��")]
    private float _motionTimer;                     //���[�V��������
    private float _firstMotionTime;

    [SerializeField, Tooltip("�{�鎞��")]
    private float angryTimer;                       //�{�鎞��
    private float _firstAngryTime;                  //

    private float sinX;                             //

    private Vector2 _myPos;                         //�����ʒu

    [SerializeField, Tooltip("Player�̃��C���[")]
    private LayerMask _playerLayer;                 //Player��Layer

    private GameObject _parentObj;                  //�e�I�u�W�F�N�g

    void Start()
    {
        //������
        _moveFlag = true;
        _startFlag = false;
        _angryFlag = false;

        _firstStayTime = _stayTimer;
        _firstAngryTime = angryTimer;
        _firstMotionTime = _motionTimer;

        sinX = 0.0f;

        _myPos = transform.position;
        _parentObj = transform.parent.gameObject;
    }

    void Update()
    {
        if (Time.timeScale == 1)
        {
            if (EnemyHP == 0)
            {
                // �e�I�u�W�F�N�g���폜
                Destroy(_parentObj);
            }

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

            //�J�����ɓ��������瓮���o������Enemy��Dead����Ȃ������珈������
            if (_startFlag && !Dead() && !_angryFlag && !_isFly && !_isKnockedBack && !_isStun)
            {
                //�����̏���
                if (_moveFlag)
                {
                    //�����̃A�j���[�V����
                    MyAnimator.SetInteger("Action", 0);

                    //����
                    Move();
                }

                //�ړ��͈�
                if (transform.position.x >= _myPos.x + _rLength || transform.position.x <= _myPos.x - _rLength)
                {
                    //Speed�̐��ƕ��𔽓]
                    MoveSpeed *= -1.0f;
                }

                //�ړ�����
                PositionControl();

                //�摜�̍��E���]
                RotationControl();
            }

            //1�b��Player���w���ɏ���Ă�����
            if (_stayTimer <= 0.0f)
            {
                _angryFlag = true;

                //�U���O���[�V����
                ShakeMotion();

                _motionTimer -= Time.deltaTime;

                //���[�W�������I������珈��
                if (_motionTimer <= 0.0f)
                {
                    MyAnimator.SetInteger("Action", 3);
                    angryTimer -= Time.deltaTime;
                }

                //�{�肪�I������珈��
                if (angryTimer <= 0.0f)
                {
                    _angryFlag = false;
                    _stayTimer = _firstStayTime;
                    _motionTimer = _firstMotionTime;
                    angryTimer = _firstAngryTime;
                }
            }
        }
    }
    override protected void Move()
    {
        transform.position = new Vector3(transform.position.x + MoveSpeed, transform.position.y, transform.position.z);
    }
    /// <summary>
    /// �֐����FControlPotion   <br />
    /// ���@���F����  <br />
    /// �߂�l�F����  <br />
    /// �ړ�����̊֐� <br />
    /// </summary>
    private void PositionControl()
    {
        //�ړ�����
        if (transform.position.x >= _myPos.x + _rLength)
        {
            //����ȏ�s���Ȃ��悤�ɂ���
            transform.position = new Vector3(_myPos.x + _rLength, transform.position.y, transform.position.z);
        }
        if (transform.position.x <= _myPos.x - _rLength)
        {
            //����ȏ�s���Ȃ��悤�ɂ���
            transform.position = new Vector3(_myPos.x - _rLength, transform.position.y, transform.position.z);
        }
    }
    /// <summary>
    /// �֐����FRotationControl <br />
    /// ���@���F����  <br />
    /// �߂�l�F����  <br />
    /// �摜�̌����𐧌䂷��֐�
    /// </summary>
    private void RotationControl()
    {
        if (MoveSpeed <= 0)
        {
            //�摜���������ɂ���
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            //�摜���E�����ɂ���
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
    /// <summary>
    /// �֐����FOnDrawGizmos    <br />
    /// ���@���F����  <br />
    /// �߂�l�F����  <br />
    /// Ray����������֐�
    /// </summary>
    private void OnDrawGizmos()
    {
        //***PlayerHit�֐��Ɠ���������Rey�ɂ���***

        //Ray����������F
        Gizmos.color = Color.red;

        //�E��Ray�̉���
        Gizmos.DrawRay(transform.position, new Vector3(-_reyLength, 0, 0));

        //����Ray�̉���
        Gizmos.DrawRay(transform.position, new Vector3(_reyLength, 0, 0));
    }

    /// <summary>
    /// �֐����FChangeEnemyTag
    /// ���@���F����
    /// �߂�l�F����
    /// tag��Enemy�ɕς���֐�(Animation event�݂̂Ŏg�p)
    /// </summary>
    private void ShakeMotion()
    {
        //�h���
        sinX = Mathf.Sin(2 * Mathf.PI * _shakingSpeed * Time.time);

        transform.position = new Vector3(transform.position.x + sinX * _shakingLength, transform.position.y, transform.position.z);

    }

    /// <summary>
    /// �֐����FChangeEnemyTag  <br />
    /// ���@���F����  <br />
    /// �߂�l�F����  <br />
    /// tag��Enemy�ɕς���֐�(Animation event�݂̂Ŏg�p)
    /// </summary>
    private void ChangeEnemyTag()
    {
        //tag��Enemy�ɕς���
        gameObject.tag = "Enemy";
    }
    /// <summary>
    /// �֐����FChangeUntaggedTag
    /// ���@���F����
    /// �߂�l�F����
    /// tag��Untagged�ɕς���֐�(Animation event�݂̂Ŏg�p)
    /// </summary>
    private void ChangeInvincibleTag()
    {
        //tag��Untagged�ɕς���
        gameObject.tag = "Invincible";
    }
    /*
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
    */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerAtk")
        {
            //�_���[�W����
            TakeDamage();

            _myPos = transform.position;
        }
        if (collision.gameObject.tag == "MainCamera")
        {
            _startFlag = true;
        }

        if (collision.gameObject.name == "Player")
        {
            //Player���q�I�u�W�F�N�g�ɂ���
            collision.gameObject.transform.SetParent(transform);
        }
        //�^���|�|�̖Ȗтɓ���������
        OnTriggerDandelionBullet(collision);

        //�A�T�K�I�̉����ɓ���������
        OnTriggerMorninggloryBullet(collision);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _stayTimer -= Time.deltaTime;
        }
    }
}
