using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01Ground : MonoBehaviour
{
    public GameObject Parent;//親のオブジェクトを保存する

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Parent != null)
        {
            transform.position = new Vector2(Parent.transform.position.x , Parent.transform.position.y);
        }
    }

    /// <summary>
    ///関数名：UpdatePlayerObject()<br />
    ///引数名：親のオブジェクト<br />
    ///戻り値：なし <br />
    ///用　途：親のオブジェクトを取得する　<br />
    /// </summary>
    public void UpdateParentObject(GameObject Object)
    {
        Parent = Object;
    }

    // 床の上側コライダーの中に入ったときに実行
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 衝突したオブジェクト名がPlayerなら、床の子オブジェクトにする
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.SetParent(transform);
            Parent.GetComponent<Enemy01>().UpdateGroundOnFlag(true);
        }
    }

    // 床の上側コライダーから離れたときに実行
    private void OnTriggerExit2D(Collider2D collision)
    {
        // 衝突したオブジェクト名がPlayerなら、床の子オブジェクトから解除する
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.SetParent(null);
            Parent.GetComponent<Enemy01>().UpdateGroundOnFlag(false);
        }
    }
    /// <summary>
    /// 移動床のコライダーがobjに触れた時の処理
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // 触れたobjの親を移動床にする
            other.transform.SetParent(transform);
            Parent.GetComponent<Enemy01>().UpdateGroundOnFlag(true);
        }
    }

    /// <summary>
    /// 移動床のコライダーがobjから離れた時の処理
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // 触れたobjの親をなくす
            other.transform.SetParent(null);
            Parent.GetComponent<Enemy01>().UpdateGroundOnFlag(false);
        }
    }
}
