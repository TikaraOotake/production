using System.Collections.Generic;
using UnityEngine;
public class Enemy2 : Enemy
{
    private bool _startFlag;                        //カメラに当たったら動きだす為のスイッチ
    private bool _angryFlag;                        //

    [SerializeField, Tooltip("行き来する半径の長さ")]
    private float _rLength;                         //半径の長さ
    [SerializeField, Tooltip("Rayの長さ")]
    private float _reyLength;                       //Rayの長さ
    
    [SerializeField, Tooltip("背中に乗っていて良い時間")]
    private float _stayTimer;                       //背中に乗っていて良い時間
    private float _firstStayTime;

    [SerializeField, Tooltip("攻撃モーション時の往復の早さ")]
    private float _shakingSpeed;
    [SerializeField, Tooltip("攻撃モーション時の往復の長さ")]
    private float _shakingLength;

    [SerializeField, Tooltip("攻撃モーションをする時間")]
    private float _motionTimer;                     //モーション時間
    private float _firstMotionTime;

    [SerializeField, Tooltip("怒る時間")]
    private float angryTimer;                       //怒る時間
    private float _firstAngryTime;                  //

    private float sinX;                             //

    private Vector2 _myPos;                         //初期位置

    [SerializeField, Tooltip("Playerのレイヤー")]
    private LayerMask _playerLayer;                 //PlayerのLayer

    private GameObject _parentObj;                  //親オブジェクト

    void Start()
    {
        //初期化
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
                // 親オブジェクトを削除
                Destroy(_parentObj);
            }

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

            //カメラに当たったら動き出すかつEnemyがDeadじゃなかったら処理する
            if (_startFlag && !Dead() && !_angryFlag && !_isFly && !_isKnockedBack && !_isStun)
            {
                //動きの処理
                if (_moveFlag)
                {
                    //動きのアニメーション
                    MyAnimator.SetInteger("Action", 0);

                    //動き
                    Move();
                }

                //移動範囲
                if (transform.position.x >= _myPos.x + _rLength || transform.position.x <= _myPos.x - _rLength)
                {
                    //Speedの正と負を反転
                    MoveSpeed *= -1.0f;
                }

                //移動制御
                PositionControl();

                //画像の左右反転
                RotationControl();
            }

            //1秒間Playerが背中に乗っていたら
            if (_stayTimer <= 0.0f)
            {
                _angryFlag = true;

                //攻撃前モーション
                ShakeMotion();

                _motionTimer -= Time.deltaTime;

                //モージョンが終わったら処理
                if (_motionTimer <= 0.0f)
                {
                    MyAnimator.SetInteger("Action", 3);
                    angryTimer -= Time.deltaTime;
                }

                //怒りが終わったら処理
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
    /// 関数名：ControlPotion   <br />
    /// 引　数：無し  <br />
    /// 戻り値：無し  <br />
    /// 移動制御の関数 <br />
    /// </summary>
    private void PositionControl()
    {
        //移動制御
        if (transform.position.x >= _myPos.x + _rLength)
        {
            //これ以上行かないようにする
            transform.position = new Vector3(_myPos.x + _rLength, transform.position.y, transform.position.z);
        }
        if (transform.position.x <= _myPos.x - _rLength)
        {
            //これ以上行かないようにする
            transform.position = new Vector3(_myPos.x - _rLength, transform.position.y, transform.position.z);
        }
    }
    /// <summary>
    /// 関数名：RotationControl <br />
    /// 引　数：無し  <br />
    /// 戻り値：無し  <br />
    /// 画像の向きを制御する関数
    /// </summary>
    private void RotationControl()
    {
        if (MoveSpeed <= 0)
        {
            //画像を左向きにする
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            //画像を右向きにする
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
    /// <summary>
    /// 関数名：OnDrawGizmos    <br />
    /// 引　数：無し  <br />
    /// 戻り値：無し  <br />
    /// Rayを可視化する関数
    /// </summary>
    private void OnDrawGizmos()
    {
        //***PlayerHit関数と同じ長さのReyにする***

        //Rayを可視化する色
        Gizmos.color = Color.red;

        //右のRayの可視化
        Gizmos.DrawRay(transform.position, new Vector3(-_reyLength, 0, 0));

        //左のRayの可視化
        Gizmos.DrawRay(transform.position, new Vector3(_reyLength, 0, 0));
    }

    /// <summary>
    /// 関数名：ChangeEnemyTag
    /// 引　数：無し
    /// 戻り値：無し
    /// tagをEnemyに変える関数(Animation eventのみで使用)
    /// </summary>
    private void ShakeMotion()
    {
        //揺れる
        sinX = Mathf.Sin(2 * Mathf.PI * _shakingSpeed * Time.time);

        transform.position = new Vector3(transform.position.x + sinX * _shakingLength, transform.position.y, transform.position.z);

    }

    /// <summary>
    /// 関数名：ChangeEnemyTag  <br />
    /// 引　数：無し  <br />
    /// 戻り値：無し  <br />
    /// tagをEnemyに変える関数(Animation eventのみで使用)
    /// </summary>
    private void ChangeEnemyTag()
    {
        //tagをEnemyに変える
        gameObject.tag = "Enemy";
    }
    /// <summary>
    /// 関数名：ChangeUntaggedTag
    /// 引　数：無し
    /// 戻り値：無し
    /// tagをUntaggedに変える関数(Animation eventのみで使用)
    /// </summary>
    private void ChangeInvincibleTag()
    {
        //tagをUntaggedに変える
        gameObject.tag = "Invincible";
    }
    /*
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
    */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerAtk")
        {
            //ダメージ処理
            TakeDamage();

            _myPos = transform.position;
        }
        if (collision.gameObject.tag == "MainCamera")
        {
            _startFlag = true;
        }

        if (collision.gameObject.name == "Player")
        {
            //Playerを子オブジェクトにする
            collision.gameObject.transform.SetParent(transform);
        }
        //タンポポの綿毛に当たったら
        OnTriggerDandelionBullet(collision);

        //アサガオの音符に当たったら
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
