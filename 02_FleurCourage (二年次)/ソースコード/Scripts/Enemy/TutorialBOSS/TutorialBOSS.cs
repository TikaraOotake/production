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
	[SerializeField, Tooltip("ノックバックの力")]
	private KnockBackPower _knockBackPower; //ノックバックする力

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
	private ReturnPosPoint _returnPosPoint; //Playerが崖に落ちた際に戻るPoint

	[SerializeField]
	private TutorialBOSSInstance _instance; //BOSSをインスタンスするオブジェクトのスクリプト

	void Start()
	{
		//初期化
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

				//ターゲットに一定範囲近づいたらtrueにする
				_isWithinRange = Vector2.Distance(transform.position, _targetPoints[_targetPointNum].transform.position) <= _threshold;

				int _targetPointMAXNum = _targetPoints.Length - 1;

				//ターゲットの近くに着いたら次のターゲットにする
				if (_isWithinRange && _targetPointNum < _targetPointMAXNum)
				{
					_targetPointNum++;
				}
				Move();
			}
			//木に当たったら
			else
			{
				//AddForceを使うためにDynamicへ
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

		//Playerが崖に落ちたら処理
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
	/// 関数名：Move    <br />
	/// 引　数：無し    <br />
	/// 戻り値：無し  <br />
	/// チュートリアルボスの動き
	/// </summary>
	void Move()
	{
		//TargetPointを追尾する
		transform.position =
		Vector2.MoveTowards(transform.position,
		new Vector2(_targetPoints[_targetPointNum].transform.position.x,
		_targetPoints[_targetPointNum].transform.position.y),
		_moveSpeed * Time.deltaTime);
	}
	/// <summary>
	/// 関数名：WaitMoveStart    <br />
	/// 引　数：無し    <br />
	/// 戻り値：無し  <br />
	/// 動き出すまでに待つ時間
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
	/// 関数名：FindPlayer    <br />
	/// 引　数：無し    <br />
	/// 戻り値：無し  <br />
	/// Playerを見つける関数
	/// </summary>
	void FindPlayer()
	{
		//Playerを取得
		for (int i = 0; i < _playerTags.Length; i++)
		{
			//PlayerがHierarchyにいたら取得
			if (GameObject.Find(_playerTags[i]) != null)
			{
				_player = GameObject.Find(_playerTags[i]);
				break;
			}
		}
		//Playerがいなかったら自身を取得
		if (_player == null)
		{
			_player = gameObject;
		}
	}
	/// <summary>
	/// 自身の速度を変更する
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
