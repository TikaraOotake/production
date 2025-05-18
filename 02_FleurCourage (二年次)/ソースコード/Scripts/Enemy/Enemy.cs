using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour
{
    #region bool
    protected bool _moveFlag;                       //������؂�ւ���X�C�b�`
	protected bool _isKnockedBack;                  //�m�b�N�o�b�N����
	protected bool _isFly;                          //���V����
	protected bool _isStun;                         //�X�^������
    #endregion

    #region int
    [SerializeField, Tooltip("Enemy�̗̑�")]
	protected int EnemyHP;                          //Enemy��HP
	[SerializeField, Tooltip("�������e�̌���ݒ�")]
	protected int FlyCnt;                           //�Ȗтɓ���������
	private int _firstFlyCnt;                       //FlyCnt�̏����l
    #endregion

    #region float
    [SerializeField, Tooltip("Enemy3�p")]
	private float _knockBackTimer;                  //
	[SerializeField, Tooltip("�^���|�|�̕��V����")]
	private float _flyTimer;                        //
	[SerializeField, Tooltip("�A�T�K�I�̃X�^������")]
	private float StunTime;                         //
	[SerializeField]
	protected float WaitTimer;                      //

	protected float _firstwaitTime;                 //

	private float _firstFlyTime;                    //

	[System.Serializable]
	private struct KnockBackPower
	{
		public float X;
		public float Y;
	}
	[SerializeField, Tooltip("�m�b�N�o�b�N�̗�")]
	private KnockBackPower knockBackPower;          //�m�b�N�o�b�N�����

	[SerializeField, Tooltip("�ړ����x")]
	protected float MoveSpeed;                      //�ړ����x
	[SerializeField, Tooltip("���V���x")]
	protected float FlyingSpeed;                    //���V���x
    #endregion

    private string[] _playerTags = {
		"PlayerRose",
		"PlayerRose(Clone)",
		"PlayerDandelion(Clone)",
		"PlayerMorningglory(Clone)",
		"PlayerRose - Copy(Clone)"
	};                                              //Player�������Ă���tag

	private Vector3 _randomDandelionBulletPos;      //

	[SerializeField, Tooltip("Enemy�����񂾂Ƃ��̃A�j���[�V����")]
	protected GameObject EnemyExplosion;            //���񂾎��̃I�u�W�F�N�g

	[SerializeField, Tooltip("�Ȗт̃I�u�W�F�N�g")]
	protected GameObject DandelionBullet;           //�Ȗт̃I�u�W�F�N�g

	protected GameObject Player;                    //Player�̃I�u�W�F�N�g

	[SerializeField]
	protected GameObject DamageEffect;              //�_���[�W��H������ۂ̃G�t�F�N�g

	[SerializeField]
	protected GameObject StunEffect;                //�X�^�����̃G�t�F�N�g

	[SerializeField, Tooltip("���g��Animator")]
	protected Animator MyAnimator;                      //�A�j���[�V����

	[SerializeField, Tooltip("���g��Rigidbody2D")]
	protected Rigidbody2D MyRigidbody;                     //���g��RigidBody2D

	[SerializeField]
	private SpriteRenderer _mySpriteRenderer;                   //���g��SpriteRenderer

	private Color _firstColor;                      //�ŏ��̐F

	void Awake()
	{
		//������
		_firstFlyCnt = FlyCnt;
		_moveFlag = false;
		_isFly = false;
		_isKnockedBack = false;
		_isStun = false;

		_firstFlyTime = _flyTimer;
		_firstwaitTime = WaitTimer;

		_firstColor = _mySpriteRenderer.color;
	}

	/// <summary>
	/// �֐����FDead()  <br />
	/// ���@���F����  <br />
	/// �߂�l�Fbool  <br />
	/// Player�ɍU�����ꂽ��Destroy����֐�
	/// </summary>
	protected bool Dead()
	{
		bool ret = false;
		//Enemy��HP��0�ȉ��ɂȂ�����true
		if (EnemyHP <= 0)
		{
			Instantiate(EnemyExplosion, transform.position, Quaternion.identity);

			Destroy(gameObject);

			ret = true;
		}
		return ret;
	}

	/// <summary>
	/// �֐����FEnemyFly    <br />
	/// ���@���F����    <br />
	/// �߂�l�F����  <br />
	/// �Ȗтɂ�����������Enemy�𕂂����֐�
	/// </summary>
	protected void EnemyFly()
	{
		//�Ȗт������������珈��������
		if (FlyCnt <= 0 && !_isFly)
		{
			//�Ȗт�����������Enemy�̎���ɃC���X�^���X������
			for (int i = 0; i < _firstFlyCnt; i++)
			{
				//�}�P�͈̔͂�random�Ȓl���擾����
				//Enemy�𒆐S�ɉ�������X,Y�̒l
				_randomDandelionBulletPos = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
				//�Ȗт��C���X�^���X��
				//Enemy�𒆐S�Ɂ}�P�iX,Y���W�j�ɃC���X�^���X��
				Instantiate(DandelionBullet, transform.position + _randomDandelionBulletPos, Quaternion.identity, transform);
			}
			_isFly = true;

			//�����l�Ƀ��Z�b�g
			FlyCnt = _firstFlyCnt;
		}
		// Enemy�𕂂�������
		if (_isFly)
		{
			MyRigidbody.velocity = Vector2.zero;

			_flyTimer -= Time.deltaTime;
			if (_flyTimer > 0.0f)
			{
				transform.position = new Vector3(transform.position.x, transform.position.y + FlyingSpeed, transform.position.z);
			}
			else
			{
				_isFly = false;
				_flyTimer = _firstFlyTime;
			}
		}
	}

	/// <summary>
	/// �֐����FOnTriggerDandelionBullet <br />
	/// ���@���FCollider2D    <br />
	/// �߂�l�F����  <br />
	/// �Ȗтɓ���������J�E���g�𑝂₷�֐�
	/// </summary>
	protected void OnTriggerDandelionBullet(Collider2D collision)
	{
		if (collision.gameObject.tag == "DandelionBullet" && !_isFly)
		{
			FlyCnt--;
		}
	}

	/// <summary>
	/// ���V���鎞�Ԃ��擾
	/// </summary>
	public float FlyTimer
	{
		get => _flyTimer;
	}

	/// <summary>
	/// �֐����FKnockBack    <br />
	/// ���@���F����    <br />
	/// �߂�l�F����  <br />
	/// �m�b�N�o�b�N������O�̏���
	/// </summary>
	protected void KnockBack()
	{
		if (!_isKnockedBack)
		{
			//Player���擾
			FindPlayer();

			//Player������������擾
			Vector3 knockbackDirection = transform.position - Player.transform.position;

			//�R���[�`�������s
			StartCoroutine(ApplyKnockBack(knockbackDirection.normalized));
		}
	}

	/// <summary>
	/// �֐����FApplyKnockBack    <br />
	/// ���@���FVector3    <br />
	/// �߂�l�F����  <br />
	/// �m�b�N�o�b�N��������Ɉ�莞�ԑ҂֐�
	/// </summary>
	IEnumerator ApplyKnockBack(Vector3 knockbackDirection)
	{
		//Player����ɂ�����m�b�N�o�b�N���Ȃ�
		if (knockbackDirection.y > -0.8f)
		{
			//�m�b�N�o�b�N�J�n
			_isKnockedBack = true;

			//velocity��0�ɂ���
			MyRigidbody.velocity = Vector3.zero;

			//Kinematic����AddForce���@�\���Ȃ���
			MyRigidbody.bodyType = RigidbodyType2D.Dynamic;

			//�m�b�N�o�b�N
			MyRigidbody.AddForce(new Vector2(knockBackPower.X * knockbackDirection.x, knockBackPower.Y));

			//��莞�ԑ҂�
			yield return new WaitForSeconds(_knockBackTimer);

			_mySpriteRenderer.color = _firstColor;

			//�m�b�N�o�b�N�I��
			_isKnockedBack = false;
		}
	}

	/// <summary>
	/// �֐����FChangeAlpha    <br />
	/// ���@���F����    <br />
	/// �߂�l�F����  <br />
	/// Enemy��_�ł�����
	/// </summary>    
	protected void ChangeAlpha()
	{
		if (_mySpriteRenderer.color.a == 1)
		{
			_mySpriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
		}
		else
		{
			_mySpriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
	}

	/// <summary>
	/// �֐����FFindPlayer    <br />
	/// ���@���F����    <br />
	/// �߂�l�F����  <br />
	/// Player��������֐�
	/// </summary>
	protected void FindPlayer()
	{
		//Player���擾
		for (int i = 0; i < _playerTags.Length; i++)
		{
			//Player��Hierarchy�ɂ�����擾
			if (GameObject.Find(_playerTags[i]) != null)
			{
				Player = GameObject.Find(_playerTags[i]);
				break;
			}
		}
		//var player = GameObject.FindGameObjectsWithTag("");
		
		//Player�����Ȃ������玩�g���擾
		if (Player == null)
		{
			Player = gameObject;
		}
	}

	/// <summary>
	/// �֐����FStunEnemy<br />
	/// ���@���F����    <br />
	/// �߂�l�F����  <br />
	/// �X�^���̎��s
	/// </summary>
	IEnumerator StunEnemy()
	{
		_isStun = true;

		MyRigidbody.velocity = Vector3.zero;

		_moveFlag = true;

		//�X�^�����̉摜���C���X�^���X
		Instantiate(StunEffect, transform.position, Quaternion.identity, transform);

		//�X�^������
		yield return new WaitForSeconds(StunTime);

		//�X�^�����̉摜��j��


		_isStun = false;
	}
	/// <summary>
	/// �֐����FTakeDamage<br />
	/// ���@���F����  <br />
	/// �߂�l�F����  <br />
	/// Player�̍U�����󂯂��ۂ̋��ʂ̏���
	/// </summary>
	protected void TakeDamage()
	{
		EnemyHP--;

		Instantiate(DamageEffect, transform.position, Quaternion.identity);

		KnockBack();
	}
	/// <summary>
	/// �֐����FOnTriggerMorningglory<br />
	/// ���@���FCollider2D    <br />
	/// �߂�l�F����  <br />
	/// �A�T�K�I�̓���U���ɓ����������̏���
	/// </summary>
	protected void OnTriggerMorninggloryBullet(Collider2D collision)
	{
		if (collision.gameObject.tag == "SpMorninggloryAtk")
		{
			StartCoroutine(StunEnemy());
		}
	}

	virtual protected void Move() { }
}
