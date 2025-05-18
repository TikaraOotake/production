using System.Collections;
using UnityEngine;

public class TutorialBOSS : MonoBehaviour
{
	private bool _isMoving;                 //
	private bool _isWithinRange;            //
	private bool _isTreeHit;                //
	private bool _isHit;                    //

	[SerializeField]
	private int _hp;                        //
	private int _targetPointNum;            //

	[SerializeField]
	private float _moveSpeed;               //
	[SerializeField]
	private float _threshold;               //
	[SerializeField]
	private float _moveStartTime;           //
	[System.Serializable]
	struct KnockBackPower
	{
		public float X;
		public float Y;
	}
	[SerializeField, Tooltip("�m�b�N�o�b�N�̗�")]
	private KnockBackPower _knockBackPower; //�m�b�N�o�b�N�����

	private string[] _playerTags = {
		"PlayerRose",
		"PlayerRose(Clone)",
		"PlayerDandelion(Clone)",
		"PlayerMorningglory(Clone)",
		"PlayerRose - Copy(Clone)"
	};                                      //

	private GameObject _player;             //

	[SerializeField]
	private GameObject[] _targetPoints;     //

	private Vector2 _myFirstPos;            //

	private Rigidbody2D _myRb;              //

	private CircleCollider2D _circleCollider;//

	private Animator _myAnim;               //

	private PlayerBase _playerBase;         //

	[SerializeField]
	private ReturnPosPoint _returnPosPoint; //Player���R�ɗ������ۂɖ߂�Point

	[SerializeField]
	private TutorialBOSSInstance _instance; //BOSS���C���X�^���X����I�u�W�F�N�g�̃X�N���v�g

	void Start()
	{
		//������
		_isMoving = false;
		_isWithinRange = false;
		_isTreeHit = false;
		_isHit = true;

		_targetPointNum = 0;

		_myFirstPos = transform.position;

		_myRb = GetComponent<Rigidbody2D>();
		_circleCollider = GetComponent<CircleCollider2D>();
		_myAnim = GetComponent<Animator>();
    }

	void Update()
	{
		FindPlayer();
		_playerBase = _player.GetComponent<PlayerBase>();

		if (_isMoving)
		{
			if (!_isTreeHit)
			{
				if (_player != null)
				{
					_playerBase.SetMoveFlag(true);
				}

				//�^�[�Q�b�g�Ɉ��͈͋߂Â�����true�ɂ���
				_isWithinRange = Vector2.Distance(transform.position, _targetPoints[_targetPointNum].transform.position) <= _threshold;

				int _targetPointMAXNum = _targetPoints.Length - 1;

				//�^�[�Q�b�g�̋߂��ɒ������玟�̃^�[�Q�b�g�ɂ���
				if (_isWithinRange && _targetPointNum < _targetPointMAXNum)
				{
					_targetPointNum++;
				}
				Move();
			}
			//�؂ɓ���������
			else
			{
				//AddForce���g�����߂�Dynamic��
				_myRb.bodyType = RigidbodyType2D.Dynamic;

				if (_isHit)
				{
					_myRb.AddForce(new Vector2(-_knockBackPower.X, _knockBackPower.Y));
					_isHit = false;
				}
				_myAnim.Play("TurnOver");
			}
		}
		else
		{
			StartCoroutine(WaitMoveStart());
		}

		//Player���R�ɗ������珈��
		if (_returnPosPoint.StayPoint == true)
		{
			transform.position = _myFirstPos;

			_instance.SetIsInstance(false);

			_isMoving = false;

			_targetPointNum = 0;

			gameObject.SetActive(false);
		}
	}
	/// <summary>
	/// �֐����FMove    <br />
	/// ���@���F����    <br />
	/// �߂�l�F����  <br />
	/// �`���[�g���A���{�X�̓���
	/// </summary>
	void Move()
	{
		//TargetPoint��ǔ�����
		transform.position =
		Vector2.MoveTowards(transform.position,
		new Vector2(_targetPoints[_targetPointNum].transform.position.x,
		_targetPoints[_targetPointNum].transform.position.y),
		_moveSpeed * Time.deltaTime);
	}
	/// <summary>
	/// �֐����FWaitMoveStart    <br />
	/// ���@���F����    <br />
	/// �߂�l�F����  <br />
	/// �����o���܂łɑ҂���
	/// </summary>
	IEnumerator WaitMoveStart()
	{
		_myRb.bodyType = RigidbodyType2D.Dynamic;
		_circleCollider.isTrigger = false;
		yield return new WaitForSeconds(_moveStartTime);
		_myRb.bodyType = RigidbodyType2D.Kinematic;
		_circleCollider.isTrigger = true;
		_isMoving = true;
	}
	/// <summary>
	/// �֐����FFindPlayer    <br />
	/// ���@���F����    <br />
	/// �߂�l�F����  <br />
	/// Player��������֐�
	/// </summary>
	void FindPlayer()
	{
		//Player���擾
		for (int i = 0; i < _playerTags.Length; i++)
		{
			//Player��Hierarchy�ɂ�����擾
			if (GameObject.Find(_playerTags[i]) != null)
			{
				_player = GameObject.Find(_playerTags[i]);
				break;
			}
		}
		//Player�����Ȃ������玩�g���擾
		if (_player == null)
		{
			_player = gameObject;
		}
	}
	/// <summary>
	/// ���g�̑��x��ύX����
	/// </summary>
	public void SetMoveSpeed(float Movespeed)
	{
		_moveSpeed = Movespeed;
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Tree")
		{
			_isTreeHit = true;
		}
	}
}
