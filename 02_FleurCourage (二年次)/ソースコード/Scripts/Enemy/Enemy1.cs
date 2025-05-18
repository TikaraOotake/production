using UnityEngine;

public class Enemy1 : Enemy
{
    private bool _walkFlag;                 //歩きかどうかのフラグ
    private bool _getPosFlag;               //一度だけ座標を取得する為のフラグ

    [SerializeField, Tooltip("裏返りから歩きに切り替わる時間")]
    private float _walkTimer;               //歩きに切り替わるまでの時間

    [SerializeField, Tooltip("Rayの長さ")]
    private float _rayLength;

    private Vector2 _turnOverPos;           //裏返った時の座標

    [SerializeField, Tooltip("地面のレイヤー")]
    private LayerMask GroundLayer;          //


    void Start()
    {
        //初期化
        _walkFlag = false;
        _getPosFlag = true;

        EnemyHP = 2;
    }

    void Update()
    {
        //転がりのアニメーション
        MyAnimator.SetInteger("Action", 1);

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

        //カメラに当たったら動き出すかつEnemyがDeadじゃなかったら動き出す
        if (_moveFlag && !Dead() && !_isFly && !_isStun)
        {
            //体力が2の時
            if (EnemyHP == 2 && !_isKnockedBack)
            {
                //動きの関数
                Move();
            }
            //体力が1の時
            else if (EnemyHP == 1 && !_isKnockedBack)
            {
                //動きのフラグ
                if (!_walkFlag && IsGrounded())
                {
                    _walkTimer -= Time.deltaTime;
                    MyRigidbody.velocity = Vector2.zero;
                    //裏返りのアニメーションをさせる関数
                    TurnOver();

                    if (_getPosFlag)
                    {
                        //一度だけ座標を取得
                        _turnOverPos = transform.position;
                        _getPosFlag = false;
                    }

                    //自分の座標を裏返った時の座標で上書き
                    transform.position = _turnOverPos;
                }

                if (_walkTimer <= 0.0f)
                {
                    //裏返った後何秒か後に歩く
                    MyAnimator.SetInteger("Action", 0);
                    _walkFlag = true;
                }
            }
        }
    }
    /// <summary>
    /// 関数名：Move    <br />
    /// 引　数：無し  <br />
    /// 戻り値：無し  <br />
    /// 左に向かって転がる関数
    /// </summary>
    override protected void Move()
    {
        //動きのアニメーション
        MyAnimator.SetInteger("Action", 1);

        //動きの処理
        MyRigidbody.velocity = new Vector2(-MoveSpeed, MyRigidbody.velocity.y);
    }
    /// <summary>
    /// 関数名：TurnOver    <br />
    /// 引　数：無し  <br />
    /// 戻り値：無し  <br />
    /// 裏返りのアニメーションをさせる関数 <br />
    /// </summary>
    private void TurnOver()
    {
        //裏返りのアニメーション
        MyAnimator.SetInteger("Action", 2);
        gameObject.tag = "Invincible";
    }
    /// <summary>
    /// 関数名：Walk
    /// 引　数：無し
    /// 戻り値：無し
    /// 歩きのアニメーションになったら動かす関数
    /// (アニメーションイベントにて使用)
    /// </summary>
    private void Walk()
    {
        //tagをEnemyに変更
        gameObject.tag = "Enemy";
        //動きの処理
        MyRigidbody.velocity = new Vector2(-MoveSpeed * 0.2f, MyRigidbody.velocity.y);
    }
    public bool IsGrounded()
    {
        bool ret = false;
        //下方向にRayを飛ばして、指定したレイヤーのオブジェクトと接触しているかどうかを判別する
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _rayLength, GroundLayer);
        //ヒットしていない場合はnullが解される
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
        //カメラに見えるようになったら動き出す
        if (collision.gameObject.tag == "MainCamera")
        {
            _moveFlag = true;
        }
        //Playerの攻撃に当たったらEnemyのHPを減らす
        if (collision.gameObject.tag == "PlayerAtk")
        {
            //ダメージ処理
            TakeDamage();
        }
        //タンポポの綿毛に当たったら
        OnTriggerDandelionBullet(collision);

        //アサガオの音符に当たったら
        OnTriggerMorninggloryBullet(collision);
    }
}
