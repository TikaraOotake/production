using UnityEngine;

public class NormalBulletMoves : MonoBehaviour
{
    private bool _upFlag;

    //0:直線
    //1:ぎざぎざ
    //2:放物線
    [SerializeField, Tooltip("0:直線,1:ぎざぎざ,2:放物線")]
    private int MoveFlag;

    //移動速度
    [SerializeField, Tooltip("移動速度")]
    private float MoveSpeed;

    //Y軸の長さ
    [SerializeField, Tooltip("Y軸の長さ")]
    private float SinLength;
    //Y軸の速度
    [SerializeField, Tooltip("Y軸の速度")]
    private float SinSpeed;
    //Sinの動き
    private float _sin;
    //sinの大きさを変えるタイマー
    private float sinTimer;

    //スケールを変える変数
    [SerializeField]
    private float Scale;

    //アルファ値を下げる速さ
    [SerializeField, Tooltip("アルファ値を下げる速さ")]
    private float DownAlphaSpeed;
    //アルファ値を下げる
    private float DownAlpha;

    //自分の初期座標
    private Vector2 _myPos;

    private Vector2 _myScale;

    //自分Rigidbody2D
    [SerializeField]
    private Rigidbody2D Rb;
    //自分のSpriteRenderer
    [SerializeField]
    private SpriteRenderer Sr;

    public GameObject PlayerObj;
    private SpriteRenderer PlayerSr;

    // Start is called before the first frame update
    void Start()
    {
        //初期化
        _upFlag = true;
        DownAlpha = 1;
        _myPos = transform.position;
        _myScale = transform.localScale;
        Scale = 0.0f;
        sinTimer = 0.0f;

        PlayerSr = PlayerObj.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 1)
        {
            //アルファ値を下げる関数
            DownAlphaValue();

            //Scaleを変える関数
            //ChangeScale();

            //攻撃の種類
            switch (MoveFlag)
            {
                //直線
                case 0:
                    {
                        //直線の弾
                        StraightMove();
                    }
                    break;
                //ぎざぎざ
                case 1:
                    {
                        //Sinの弾
                        SinMove();
                    }
                    break;
                //放物線
                case 2:
                    {
                        //放物線の弾
                        CurveMove();
                    }
                    break;
            }

            //アルファ値が0になったらDestroy
            if (DownAlpha <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
    /// <summary>
    /// 関数名：StraightMove
    /// 引　数：無し
    /// 戻り値：無し
    /// 直線の動きをする関数
    /// </summary>
    void StraightMove()
    {
        //if (PlayerSr.flipX == true)
        {
            //transform.Translate(-MoveSpeed, 0.0f, 0.0f, Space.Self);
        }
        //else
        {
            transform.Translate(MoveSpeed, 0.0f, 0.0f, Space.Self);
        }
        //Rb.AddForce(Vector2.right * MoveSpeed);
        
    }
    /// <summary>
    /// 関数名：SinMove
    /// 引　数：無し
    /// 戻り値：無し
    /// Sinの動きをする関数
    /// </summary>
    void SinMove()
    {
        sinTimer += Time.deltaTime; 
        _sin = Mathf.Sin(sinTimer * SinSpeed);
        transform.position = new Vector2(transform.position.x, _myPos.y - _sin * SinLength);

        Rb.velocity = new Vector2(MoveSpeed, 0.0f);
    }
    /// <summary>
    /// 関数名：CurveMove
    /// 引　数：無し
    /// 戻り値：無し
    /// 放物線の動きをする関数
    /// </summary>
    void CurveMove()
    {
        if (_upFlag)
        {
            Rb.AddForce(Vector2.up * MoveSpeed);
            Rb.AddForce(Vector2.right * MoveSpeed);
            _upFlag = false;
        }
    }
    /// <summary>
    /// 関数名：ChangeAlphaValue
    /// 引　数：無し
    /// 戻り値：無し
    /// アルファ値をだんだんと下げる関数
    /// </summary>
    void DownAlphaValue()
    {
        DownAlpha -= Time.deltaTime * DownAlphaSpeed;
        Sr.color = new Color(1, 1, 1, DownAlpha);
    }
    /// <summary>
    /// アニメーションイベント用の関数
    /// </summary>
    void ChangeScale()
    {        
        //未完成
        Scale = Mathf.Sin(Time.deltaTime * 25);
        Debug.Log(Scale);
        transform.localScale = new Vector3(_myScale.x + Scale,_myScale.y + Scale, transform.localScale.z);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //
        if (collision.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
        }
    }
}
