using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    private bool _specialFlag;          //特殊攻撃をしたか見るFlag
    private bool _isUntaggedFlag;       //tagがUntaggedか見るFlag

    public int HP;                      //Playerの体力
    private int BulletSelect;           //弾の選択

    public float MoveSpeed;             //Playerの移動速度
    public float JumpPower;             //ジャンプの大きさ

    public float ReyLength;             //Rayの長さ

    [SerializeField,Tooltip("特殊攻撃の時間")]
    private float SpecialTime;          //特殊攻撃の時間
    private float _specialTimer;        //特殊攻撃のタイマー

    [SerializeField]
    private Rigidbody2D Rb;             //PlayerのRigidbody2D

    [SerializeField,Tooltip("通常攻撃の弾")]
    private GameObject[] BulletObj
        = new GameObject[3];            //通常攻撃の弾を格納する
    [SerializeField,Tooltip("特殊攻撃の弾")]
    private GameObject SpecialObj;      //特殊攻撃の弾を格納する

    public LayerMask groundLayer;       //レイヤーの判定		

    // Start is called before the first frame update
    void Start()
    {
        _specialFlag = true;
        _isUntaggedFlag = false;

        BulletSelect = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //特殊攻撃中じゃなかったら処理
        if (_specialFlag)
        {
            //地面にRayが当たっていたらかつtagがUntaggedだったら処理する
            if (IsGrounded() && _isUntaggedFlag)
            {
                //Cが押されていたらかつSpaceが押されていなかったら処理
                if (Input.GetKey(KeyCode.C) && !Input.GetKey(KeyCode.Space))
                {
                    //特殊攻撃
                    SpecialAtk();
                }
                //Spaceが押されていたらかつCが押されていなかったら処理
                else if (Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.C))
                {
                    //ジャンプ
                    Jump();
                }
            }
            //動き
            Move();
            //攻撃
            Attack();
        }
        //特殊攻撃中だったら処理
        else
        {
            //タイマーを加算
            _specialTimer += Time.deltaTime;
            //一定時間たったら処理
            if (_specialTimer >= SpecialTime)
            {
                //特殊攻撃を終了
                _specialFlag = true;

                //タイマーを初期化
                _specialTimer = 0.0f;

                //無敵状態を解除
                gameObject.tag = "Player";
            }
        }
        //Playerの体力が0になったら処理
        if (HP <= 0)
        {
            //死亡
            Dead();
        }
    }

    /// <summary>
    /// 関数名：Dead	<br />
    /// 引　数：無し	<br />
    /// 戻り値：無し	<br />
    /// 死亡の時の処理をする関数	<br />
    /// </summary>
    void Dead()
    {
        //オブジェクトを削除
        Destroy(gameObject);
    }
    /// <summary>
    ///関数名：Move <br />
    ///引　数：無し <br />
    ///戻り値：無し <br />
    ///動きの処理   <br />
    /// </summary>
    void Move()
    {
        Vector2 a = Rb.velocity;
        //「←」が押されていたら処理をする
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //動きの処理
            //transform.Translate(-speed, 0.0f, 0.0f);
            Rb.velocity = new Vector2(-MoveSpeed, a.y);
        }

        //「→」が押されていたら処理をする
        if (Input.GetKey(KeyCode.RightArrow))
        {
            //動きの処理
            //transform.Translate(speed, 0.0f, 0.0f);
            Rb.velocity = new Vector2(MoveSpeed, a.y);
        }
    }
    /// <summary>
    /// 関数名：Jump    <br />
    /// 引　数：無し  <br />
    /// 戻り値：無し  <br />
    /// ジャンプの処理をする関数    
    /// </summary>
    void Jump()
    {
        //Spaceが押されたらジャンプさせる
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //上向きの力を加える
            Rb.AddForce(Vector2.up * JumpPower);
        }
    }
    /// <summary>
    /// 関数名：Attack  <br />
    /// 引　数：無し  <br />
    /// 戻り値：無し  <br />
    /// 通常攻撃の関数
    /// </summary>
    void Attack()
    {
        //selectの値を1加算
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            BulletSelect++;
        }
        //selectの値を1加減
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            BulletSelect--;
        }
        //selectの数値制御
        if (BulletSelect > 2)
        {
            BulletSelect = 0;
        }
        else if (BulletSelect < 0)
        {
            BulletSelect = 2;
        }
        //Xを押したら処理
        if (Input.GetKeyDown(KeyCode.X))
        {
            for (int i = 0; i < 3; i++)
            {
                if (BulletSelect == i)
                {
                    //インスタンス化する場所
                    Vector3 _instanceBulletPos;
                    _instanceBulletPos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);

                    //インスタンス化
                    Instantiate(BulletObj[i], _instanceBulletPos, Quaternion.identity);
                    break;
                }
            }
        }
    }
    /// <summary>
    /// 関数名：ApecialAtk
    /// 引　数：無し
    /// 戻り値：無し
    /// 特殊攻撃の関数
    /// </summary>
    void SpecialAtk()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _specialFlag = false;
            //位置を固定
            Rb.velocity = Vector2.zero;

            //無敵
            gameObject.tag = "Invincible";

            //衝撃波
            Instantiate(SpecialObj, transform.position, Quaternion.identity);
        }
    }
    // <summary>
    // 関数名：IsGrounded   <br />
    // 引　数：無し   <br />
    // 戻り値：無し   <br />
    // 地面か判定する関数
    // </summary>
    bool IsGrounded()
    {
        bool ret = false;

        //Rayを飛ばしgroundLayerにヒットしたらtrueを返す
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, ReyLength, groundLayer);
        //hitがNULLじゃなかったらretをtrueにする
        if (hit.collider != null)
        {
            ret = true;
        }
        return ret;
    }
    // <summary>
    // 関数名：IsGrounded   <br />
    // 引　数：無し           <br />
    // 戻り値：無し           <br />
    // Rayを可視化する関数      <br />
    // </summary>
    void OnDrawGizmos()
    {
        //***IsGrounded関数と同じ長さのReyにする***
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, new Vector3(0, -ReyLength, 0));
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && gameObject.tag == "Player")
        {
            HP--;
        }
        //当たったオブジェクトのtagがUntaggedだったら
        if (collision.gameObject.tag == "Untagged")
        {
            _isUntaggedFlag = true;
        }
        //当たったオブジェクトのtagがUntaggedじゃなかったら
        else
        {
            _isUntaggedFlag = false;
        }
    }
}
