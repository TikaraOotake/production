using UnityEngine;
using System.Collections;

public class Enemy3 : Enemy
{
	private bool _startFlag;                        //�J�����ɓ��������瓮�������ׂ̃X�C�b�`
	private bool _waitFlag;                         //

    #region Ray�̕ϐ�
    [SerializeField]
    private float _groundCheckRadius;
    [SerializeField]
    private float _groundCheckOffsetY;
    [SerializeField]
    private float _groundCheckDistance;
	#endregion

    [SerializeField, Tooltip("Player���_���[�W���󂯂��ۂ̑҂�����")]
	private float _waitTime;
	[SerializeField, Tooltip("�㉺���鑬��")]
	private float _sinSpeed;                        //�㉺���鑬��
	[SerializeField, Tooltip("�㉺���镝")]
	private float _sinLength;                       //�㉺�̕�
	private float _sin;                             //�㉺������ׂ̕ϐ�
	private float _sinTimer;                        //_sin�p�̃^�C�}�[(�㉺������ׂ̃^�C�}�[)

	[SerializeField, Tooltip("Ray�̒���")]
	private float _reyLength;                       //Rey�̒���

	private Vector2 _myPos;                         //�����ʒu

	[SerializeField, Tooltip("Player�̃��C���[")]
	private LayerMask _playerLayer;                 //Player�̃��C���[S

	void Start()
	{
		//������
		_myPos = transform.position;
		_moveFlag = false;
		_startFlag = false;
		_waitFlag = false;
	}

	void Update()
	{
		if (Time.timeScale == 1)
		{
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

			//Player���Z�b�g
			FindPlayer();

			if (!_isKnockedBack)
			{
				MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
				MyRigidbody.velocity = Vector3.zero;
			}

			//�J�����ɓ��������瓮���o������Enemy��Dead����Ȃ������瓮���o��
			if (_startFlag && !Dead() && !_isFly && !_isKnockedBack && !_isStun)
			{
				//���̃R�[�h����Y���W��4.0f��荂��������ǔ����Ă��܂����獂���ʒu��Enemy��u���Ȃ�
				if (transform.position.y >= 4.0f)
				{
					_moveFlag = true;
				}

				//Ray��Player�ɓ������Ă��Ȃ�������ҋ@�������̃X�C�b�`��false��������ҋ@
				if (!PlayerHit() && !_moveFlag)
				{
					//�ҋ@�̊֐�
					Idle();
				}
				//Ray��Player�ɓ��������瓮���̃X�C�b�`��true�ɂ���
				else
				{
					_moveFlag = true;
				}

				//Player�ɓ����������ɒǔ�����
				if (_moveFlag && Player != null && !_waitFlag)
				{
					//�ǔ�����
					Move();

					//�摜�̍��E���]
					RotationControl();
				}
			}
		}
	}

	/// <summary>
	/// �֐����FPlayerHit   <br />
	/// ���@���F����  <br />
	/// �߂�l�Fbool  <br />
	/// Player�ɓ���������Ray�Ŕ��肷��֐�
	/// </summary>
	private bool PlayerHit()
	{
		bool ret = false;

        //Ray���΂�PlayerLayer�Ƀq�b�g������true��Ԃ�
#if false
		//�E����Ray
		RaycastHit2D[] lefthit = new RaycastHit2D[3];
		lefthit[0] = Physics2D.Raycast(transform.position - new Vector3(0.0f, -1.0f, 0.0f), Vector2.left, _reyLength, _playerLayer);
		lefthit[1] = Physics2D.Raycast(transform.position, Vector2.left, _reyLength, _playerLayer);
		lefthit[2] = Physics2D.Raycast(transform.position - new Vector3(0.0f, 1.5f, 0.0f), Vector2.left, _reyLength, _playerLayer);

		//������Ray
		RaycastHit2D[] righthit = new RaycastHit2D[3];
		righthit[0] = Physics2D.Raycast(transform.position - new Vector3(0.0f, -1.0f, 0.0f), Vector2.right, _reyLength, _playerLayer);
		righthit[1] = Physics2D.Raycast(transform.position, Vector2.right, _reyLength, _playerLayer);
		righthit[2] = Physics2D.Raycast(transform.position - new Vector3(0.0f, 1.5f, 0.0f), Vector2.right, _reyLength, _playerLayer);

		        for (int a = 0; a < 3; a++)
		{
			//lefthit����righthit��NULL����Ȃ�������ret��true�ɂ���
			if (lefthit[a].collider != null || righthit[a].collider != null)
			{
				ret = true;
			}
		}
#else
        RaycastHit2D rayHit;
		rayHit = Physics2D.CircleCast((Vector2)transform.position + _groundCheckOffsetY * Vector2.up, _groundCheckRadius, Vector2.down, _groundCheckDistance, _playerLayer);

		if (rayHit.collider != null)
		{
			ret = true;
		}
#endif


		return ret;
	}
	/// <summary>
	/// �֐����FIdle
	/// ���@���F����
	/// �߂�l�F����
	/// �ҋ@�̏��
	/// </summary>
	private void Idle()
	{
		//�ړ��̃A�j���[�V����
		MyAnimator.SetInteger("Action", 0);

		//Sin�̓���
		_sinTimer += Time.deltaTime;
		_sin = Mathf.Sin(_sinTimer * _sinSpeed);

		//�����ʒu�𒆐S�ɏ㉺�ɓ���
		transform.position = new Vector3(transform.position.x, _myPos.y + _sin * _sinLength, transform.position.z);
	}
	/// <summary>
	/// �֐����FRotationControl <br />
	/// ���@���F����  <br />
	/// �߂�l�F����  <br />
	/// �摜�̌����𐧌䂷��֐�
	/// </summary>
	private void RotationControl()
	{
		if (Player.transform.position.x > transform.position.x)
		{
			//�摜���������ɂ���
			transform.rotation = Quaternion.Euler(0, 180, 0);
		}
		else
		{
			//�摜���E�����ɂ���
			transform.rotation = Quaternion.Euler(0, 0, 0);
		}
	}
	/// <summary>
	/// �֐����FTrackingMove
	/// ���@���F����
	/// �߂�l�F����
	/// Player��ǔ�����֐�
	/// </summary>
	override protected void Move()
	{
		//�U���̃A�j���[�V����
		MyAnimator.SetInteger("Action", 1);
		FindPlayer();
		//MoveTowards()
		//���ݒn����ړI�n�܂ň�葬�x�ňړ������Ă����֐�
		//MoveTowards(���ݒn�iVector3), �ړI�n�iVector3), �P�t���[���̋���(��葬�x) (float))
		transform.position = Vector2.MoveTowards(transform.position, new Vector2(Player.transform.position.x, Player.transform.position.y), MoveSpeed * Time.deltaTime);
	}
	private IEnumerator Wait()
	{
		_waitFlag = true;

		yield return new WaitForSeconds(_waitTime);

		_waitFlag = false;
	}
	/// <summary>
	/// �֐����FOnDrawGizmos   <br />
	/// ���@���F����  <br />
	/// �߂�l�F����  <br />
	/// Ray����������֐�
	/// </summary>
	private void OnDrawGizmos()
	{
		//***PlayerHit�֐��Ɠ���������Rey�ɂ���***

		//Ray����������F
		Gizmos.color = Color.red;
#if false
		//�E��Ray�̉���
		Gizmos.DrawRay(transform.position - new Vector3(0.0f, -1.0f, 0.0f), new Vector3(_reyLength, 0, 0));
		Gizmos.DrawRay(transform.position, new Vector3(_reyLength, 0, 0));
		Gizmos.DrawRay(transform.position - new Vector3(0.0f, 1.5f, 0.0f), new Vector3(_reyLength, 0, 0));

		//����Ray�̉���
		Gizmos.DrawRay(transform.position - new Vector3(0.0f, -1.0f, 0.0f), new Vector3(-_reyLength, 0, 0));
		Gizmos.DrawRay(transform.position, new Vector3(-_reyLength, 0, 0));
		Gizmos.DrawRay(transform.position - new Vector3(0.0f, 1.5f, 0.0f), new Vector3(-_reyLength, 0, 0));
#else
		Gizmos.DrawWireSphere((Vector2)transform.position + _groundCheckOffsetY * Vector2.up + Vector2.down * _groundCheckDistance, _groundCheckRadius);
#endif
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		//Player�̋��ɓ���������Enemy�̗̑͂�1���炷
		if (collision.gameObject.tag == "PlayerAtk")
		{
			//�_���[�W����
			TakeDamage();

			_moveFlag = true;
		}
		//camera��tag�ɓ��������瓮���n�߂�
		if (collision.gameObject.tag == "MainCamera")
		{
			_startFlag = true;
		}
		//�^���|�|�̖Ȗтɓ���������
		OnTriggerDandelionBullet(collision);

		//�A�T�K�I�̉����ɓ���������
		OnTriggerMorninggloryBullet(collision);
	}
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "MainCamera")
		{
			_startFlag = false;
		}
	}
	private void OnCollisionStay2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			StartCoroutine(Wait());
		}
	}
}
