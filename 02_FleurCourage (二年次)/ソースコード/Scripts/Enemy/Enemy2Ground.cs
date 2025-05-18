using UnityEngine;

public class Enemy2Ground : MonoBehaviour
{
    [SerializeField]
    private float _rLength;         //

    private Vector2 _myPos;         //
    [SerializeField]
    private GameObject _enemy2;     //

    void Start()
    {
        //初期化
        _myPos = transform.position;
    }

    void Update()
    {
        //try
        //{
        //    transform.position = new Vector2(_enemy2.transform.position.x, _enemy2.transform.position.y);
        //}
        //catch (MissingReferenceException)
        //{

        //}
        if ( _enemy2 == null ) { return; }
        //移動制御
        PositionControl();
    }

    /// <summary>
    /// 関数名：ControlPotion   <br />
    /// 引　数：無し  <br />
    /// 戻り値：無し  <br />
    /// 移動制御の関数 <br />
    /// </summary>
    void PositionControl()
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

    // 床の上側コライダーの中に入ったときに実行
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Playerを子オブジェクトにする
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.SetParent(transform);
        }
    }

    // 床の上側コライダーから離れたときに実行
    private void OnTriggerExit2D(Collider2D collision)
    {
        //Playerを子ぶジェクトから解除する
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.SetParent(null);
        }
    }
}
