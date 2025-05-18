using UnityEngine;
using System.Collections;

public class Enemy3 : Enemy
{
	private bool _startFlag;                        //カメラに当たったら動きだす為のスイッチ
	private bool _waitFlag;                         //

    #region Rayの変数
    [SerializeField]
    private float _groundCheckRadius;
    [SerializeField]
    private float _groundCheckOffsetY;
    [SerializeField]
    private float _groundCheckDistance;
	#endregion

    [SerializeField, Tooltip("Playerがダメージを受けた際の待ち時間")]
	private float _waitTime;
	[SerializeField, Tooltip("上下する速さ")]
	private float _sinSpeed;                        //上下する速さ
	[SerializeField, Tooltip("上下する幅")]
	private float _sinLength;                       //上下の幅
	private float _sin;                             //上下させる為の変数
	private float _sinTimer;                        //_sin用のタイマー(上下させる為のタイマー)

	[SerializeField, Tooltip("Rayの長さ")]
	private float _reyLength;                       //Reyの長さ

	private Vector2 _myPos;                         //初期位置

	[SerializeField, Tooltip("Playerのレイヤー")]
	private LayerMask _playerLayer;                 //PlayerのレイヤーS

	void Start()
	{
		//初期化
		_myPos = transform.position;
		_moveFlag = false;
		_startFlag = false;
		_waitFlag = false;
	}

	void Update()
	{
		if (Time.timeScale == 1)
		{
			//綿毛に当たった時の処理
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

			//Playerをセット
			FindPlayer();

			if (!_isKnockedBack)
			{
				MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
				MyRigidbody.velocity = Vector3.zero;
			}

			//カメラに当たったら動き出すかつEnemyがDeadじゃなかったら動き出す
			if (_startFlag && !Dead() && !_isFly && !_isKnockedBack && !_isStun)
			{
				//このコードだとY座標が4.0fより高かったら追尾してしまうから高い位置にEnemyを置けない
				if (transform.position.y >= 4.0f)
				{
					_moveFlag = true;
				}

				//RayがPlayerに当たっていなかったら待機かつ動きのスイッチがfalseだったら待機
				if (!PlayerHit() && !_moveFlag)
				{
					//待機の関数
					Idle();
				}
				//RayがPlayerに当たったら動きのスイッチをtrueにする
				else
				{
					_moveFlag = true;
				}

				//Playerに当たったら常に追尾する
				if (_moveFlag && Player != null && !_waitFlag)
				{
					//追尾する
					Move();

					//画像の左右反転
					RotationControl();
				}
			}
		}
	}

	/// <summary>
	/// 関数名：PlayerHit   <br />
	/// 引　数：無し  <br />
	/// 戻り値：bool  <br />
	/// Playerに当たったかRayで判定する関数
	/// </summary>
	private bool PlayerHit()
	{
		bool ret = false;

        //Rayを飛ばしPlayerLayerにヒットしたらtrueを返す
#if false
		//右側のRay
		RaycastHit2D[] lefthit = new RaycastHit2D[3];
		lefthit[0] = Physics2D.Raycast(transform.position - new Vector3(0.0f, -1.0f, 0.0f), Vector2.left, _reyLength, _playerLayer);
		lefthit[1] = Physics2D.Raycast(transform.position, Vector2.left, _reyLength, _playerLayer);
		lefthit[2] = Physics2D.Raycast(transform.position - new Vector3(0.0f, 1.5f, 0.0f), Vector2.left, _reyLength, _playerLayer);

		//左側のRay
		RaycastHit2D[] righthit = new RaycastHit2D[3];
		righthit[0] = Physics2D.Raycast(transform.position - new Vector3(0.0f, -1.0f, 0.0f), Vector2.right, _reyLength, _playerLayer);
		righthit[1] = Physics2D.Raycast(transform.position, Vector2.right, _reyLength, _playerLayer);
		righthit[2] = Physics2D.Raycast(transform.position - new Vector3(0.0f, 1.5f, 0.0f), Vector2.right, _reyLength, _playerLayer);

		        for (int a = 0; a < 3; a++)
		{
			//lefthit又はrighthitがNULLじゃなかったらretをtrueにする
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
	/// 関数名：Idle
	/// 引　数：無し
	/// 戻り値：無し
	/// 待機の状態
	/// </summary>
	private void Idle()
	{
		//移動のアニメーション
		MyAnimator.SetInteger("Action", 0);

		//Sinの動き
		_sinTimer += Time.deltaTime;
		_sin = Mathf.Sin(_sinTimer * _sinSpeed);

		//初期位置を中心に上下に動く
		transform.position = new Vector3(transform.position.x, _myPos.y + _sin * _sinLength, transform.position.z);
	}
	/// <summary>
	/// 関数名：RotationControl <br />
	/// 引　数：無し  <br />
	/// 戻り値：無し  <br />
	/// 画像の向きを制御する関数
	/// </summary>
	private void RotationControl()
	{
		if (Player.transform.position.x > transform.position.x)
		{
			//画像を左向きにする
			transform.rotation = Quaternion.Euler(0, 180, 0);
		}
		else
		{
			//画像を右向きにする
			transform.rotation = Quaternion.Euler(0, 0, 0);
		}
	}
	/// <summary>
	/// 関数名：TrackingMove
	/// 引　数：無し
	/// 戻り値：無し
	/// Playerを追尾する関数
	/// </summary>
	override protected void Move()
	{
		//攻撃のアニメーション
		MyAnimator.SetInteger("Action", 1);
		FindPlayer();
		//MoveTowards()
		//現在地から目的地まで一定速度で移動させてくれる関数
		//MoveTowards(現在地（Vector3), 目的地（Vector3), １フレームの距離(一定速度) (float))
		transform.position = Vector2.MoveTowards(transform.position, new Vector2(Player.transform.position.x, Player.transform.position.y), MoveSpeed * Time.deltaTime);
	}
	private IEnumerator Wait()
	{
		_waitFlag = true;

		yield return new WaitForSeconds(_waitTime);

		_waitFlag = false;
	}
	/// <summary>
	/// 関数名：OnDrawGizmos   <br />
	/// 引　数：無し  <br />
	/// 戻り値：無し  <br />
	/// Rayを可視化する関数
	/// </summary>
	private void OnDrawGizmos()
	{
		//***PlayerHit関数と同じ長さのReyにする***

		//Rayを可視化する色
		Gizmos.color = Color.red;
#if false
		//右のRayの可視化
		Gizmos.DrawRay(transform.position - new Vector3(0.0f, -1.0f, 0.0f), new Vector3(_reyLength, 0, 0));
		Gizmos.DrawRay(transform.position, new Vector3(_reyLength, 0, 0));
		Gizmos.DrawRay(transform.position - new Vector3(0.0f, 1.5f, 0.0f), new Vector3(_reyLength, 0, 0));

		//左のRayの可視化
		Gizmos.DrawRay(transform.position - new Vector3(0.0f, -1.0f, 0.0f), new Vector3(-_reyLength, 0, 0));
		Gizmos.DrawRay(transform.position, new Vector3(-_reyLength, 0, 0));
		Gizmos.DrawRay(transform.position - new Vector3(0.0f, 1.5f, 0.0f), new Vector3(-_reyLength, 0, 0));
#else
		Gizmos.DrawWireSphere((Vector2)transform.position + _groundCheckOffsetY * Vector2.up + Vector2.down * _groundCheckDistance, _groundCheckRadius);
#endif
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		//Playerの球に当たったらEnemyの体力を1減らす
		if (collision.gameObject.tag == "PlayerAtk")
		{
			//ダメージ処理
			TakeDamage();

			_moveFlag = true;
		}
		//cameraのtagに当たったら動き始める
		if (collision.gameObject.tag == "MainCamera")
		{
			_startFlag = true;
		}
		//タンポポの綿毛に当たったら
		OnTriggerDandelionBullet(collision);

		//アサガオの音符に当たったら
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
