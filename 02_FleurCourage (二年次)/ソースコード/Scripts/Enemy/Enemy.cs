using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour
{
    #region bool
    protected bool _moveFlag;                       //動きを切り替えるスイッチ
	protected bool _isKnockedBack;                  //ノックバック中か
	protected bool _isFly;                          //浮遊中か
	protected bool _isStun;                         //スタン中か
    #endregion

    #region int
    [SerializeField, Tooltip("Enemyの体力")]
	protected int EnemyHP;                          //EnemyのHP
	[SerializeField, Tooltip("浮かす弾の個数を設定")]
	protected int FlyCnt;                           //綿毛に当たった数
	private int _firstFlyCnt;                       //FlyCntの初期値
    #endregion

    #region float
    [SerializeField, Tooltip("Enemy3用")]
	private float _knockBackTimer;                  //
	[SerializeField, Tooltip("タンポポの浮遊時間")]
	private float _flyTimer;                        //
	[SerializeField, Tooltip("アサガオのスタン時間")]
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
	[SerializeField, Tooltip("ノックバックの力")]
	private KnockBackPower knockBackPower;          //ノックバックする力

	[SerializeField, Tooltip("移動速度")]
	protected float MoveSpeed;                      //移動速度
	[SerializeField, Tooltip("浮遊速度")]
	protected float FlyingSpeed;                    //浮遊速度
    #endregion

    private string[] _playerTags = {
		"PlayerRose",
		"PlayerRose(Clone)",
		"PlayerDandelion(Clone)",
		"PlayerMorningglory(Clone)",
		"PlayerRose - Copy(Clone)"
	};                                              //Playerが持っているtag

	private Vector3 _randomDandelionBulletPos;      //

	[SerializeField, Tooltip("Enemyが死んだときのアニメーション")]
	protected GameObject EnemyExplosion;            //死んだ時のオブジェクト

	[SerializeField, Tooltip("綿毛のオブジェクト")]
	protected GameObject DandelionBullet;           //綿毛のオブジェクト

	protected GameObject Player;                    //Playerのオブジェクト

	[SerializeField]
	protected GameObject DamageEffect;              //ダメージを食らった際のエフェクト

	[SerializeField]
	protected GameObject StunEffect;                //スタン時のエフェクト

	[SerializeField, Tooltip("自身のAnimator")]
	protected Animator MyAnimator;                      //アニメーション

	[SerializeField, Tooltip("自身のRigidbody2D")]
	protected Rigidbody2D MyRigidbody;                     //自身のRigidBody2D

	[SerializeField]
	private SpriteRenderer _mySpriteRenderer;                   //自身のSpriteRenderer

	private Color _firstColor;                      //最初の色

	void Awake()
	{
		//初期化
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
	/// 関数名：Dead()  <br />
	/// 引　数：無し  <br />
	/// 戻り値：bool  <br />
	/// Playerに攻撃されたらDestroyする関数
	/// </summary>
	protected bool Dead()
	{
		bool ret = false;
		//EnemyのHPが0以下になったらtrue
		if (EnemyHP <= 0)
		{
			Instantiate(EnemyExplosion, transform.position, Quaternion.identity);

			Destroy(gameObject);

			ret = true;
		}
		return ret;
	}

	/// <summary>
	/// 関数名：EnemyFly    <br />
	/// 引　数：無し    <br />
	/// 戻り値：無し  <br />
	/// 綿毛にｎ個当たったらEnemyを浮かす関数
	/// </summary>
	protected void EnemyFly()
	{
		//綿毛がｎ個当たったら処理をする
		if (FlyCnt <= 0 && !_isFly)
		{
			//綿毛が当たった数Enemyの周りにインスタンス化する
			for (int i = 0; i < _firstFlyCnt; i++)
			{
				//±１の範囲でrandomな値を取得する
				//Enemyを中心に加えたいX,Yの値
				_randomDandelionBulletPos = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
				//綿毛をインスタンス化
				//Enemyを中心に±１（X,Y座標）にインスタンス化
				Instantiate(DandelionBullet, transform.position + _randomDandelionBulletPos, Quaternion.identity, transform);
			}
			_isFly = true;

			//初期値にリセット
			FlyCnt = _firstFlyCnt;
		}
		// Enemyを浮かす処理
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
	/// 関数名：OnTriggerDandelionBullet <br />
	/// 引　数：Collider2D    <br />
	/// 戻り値：無し  <br />
	/// 綿毛に当たったらカウントを増やす関数
	/// </summary>
	protected void OnTriggerDandelionBullet(Collider2D collision)
	{
		if (collision.gameObject.tag == "DandelionBullet" && !_isFly)
		{
			FlyCnt--;
		}
	}

	/// <summary>
	/// 浮遊する時間を取得
	/// </summary>
	public float FlyTimer
	{
		get => _flyTimer;
	}

	/// <summary>
	/// 関数名：KnockBack    <br />
	/// 引　数：無し    <br />
	/// 戻り値：無し  <br />
	/// ノックバックをする前の準備
	/// </summary>
	protected void KnockBack()
	{
		if (!_isKnockedBack)
		{
			//Playerを取得
			FindPlayer();

			//Playerがいる方向を取得
			Vector3 knockbackDirection = transform.position - Player.transform.position;

			//コルーチンを実行
			StartCoroutine(ApplyKnockBack(knockbackDirection.normalized));
		}
	}

	/// <summary>
	/// 関数名：ApplyKnockBack    <br />
	/// 引　数：Vector3    <br />
	/// 戻り値：無し  <br />
	/// ノックバックをした後に一定時間待つ関数
	/// </summary>
	IEnumerator ApplyKnockBack(Vector3 knockbackDirection)
	{
		//Playerが上にいたらノックバックしない
		if (knockbackDirection.y > -0.8f)
		{
			//ノックバック開始
			_isKnockedBack = true;

			//velocityを0にする
			MyRigidbody.velocity = Vector3.zero;

			//KinematicだとAddForceが機能しない為
			MyRigidbody.bodyType = RigidbodyType2D.Dynamic;

			//ノックバック
			MyRigidbody.AddForce(new Vector2(knockBackPower.X * knockbackDirection.x, knockBackPower.Y));

			//一定時間待つ
			yield return new WaitForSeconds(_knockBackTimer);

			_mySpriteRenderer.color = _firstColor;

			//ノックバック終了
			_isKnockedBack = false;
		}
	}

	/// <summary>
	/// 関数名：ChangeAlpha    <br />
	/// 引　数：無し    <br />
	/// 戻り値：無し  <br />
	/// Enemyを点滅させる
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
	/// 関数名：FindPlayer    <br />
	/// 引　数：無し    <br />
	/// 戻り値：無し  <br />
	/// Playerを見つける関数
	/// </summary>
	protected void FindPlayer()
	{
		//Playerを取得
		for (int i = 0; i < _playerTags.Length; i++)
		{
			//PlayerがHierarchyにいたら取得
			if (GameObject.Find(_playerTags[i]) != null)
			{
				Player = GameObject.Find(_playerTags[i]);
				break;
			}
		}
		//var player = GameObject.FindGameObjectsWithTag("");
		
		//Playerがいなかったら自身を取得
		if (Player == null)
		{
			Player = gameObject;
		}
	}

	/// <summary>
	/// 関数名：StunEnemy<br />
	/// 引　数：無し    <br />
	/// 戻り値：無し  <br />
	/// スタンの実行
	/// </summary>
	IEnumerator StunEnemy()
	{
		_isStun = true;

		MyRigidbody.velocity = Vector3.zero;

		_moveFlag = true;

		//スタン時の画像をインスタンス
		Instantiate(StunEffect, transform.position, Quaternion.identity, transform);

		//スタン時間
		yield return new WaitForSeconds(StunTime);

		//スタン時の画像を破棄


		_isStun = false;
	}
	/// <summary>
	/// 関数名：TakeDamage<br />
	/// 引　数：無し  <br />
	/// 戻り値：無し  <br />
	/// Playerの攻撃を受けた際の共通の処理
	/// </summary>
	protected void TakeDamage()
	{
		EnemyHP--;

		Instantiate(DamageEffect, transform.position, Quaternion.identity);

		KnockBack();
	}
	/// <summary>
	/// 関数名：OnTriggerMorningglory<br />
	/// 引　数：Collider2D    <br />
	/// 戻り値：無し  <br />
	/// アサガオの特殊攻撃に当たった時の処理
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
