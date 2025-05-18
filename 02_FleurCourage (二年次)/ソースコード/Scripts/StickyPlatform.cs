using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyPlatform : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 床の上側コライダーの中に入ったときに実行
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 衝突したオブジェクト名がPlayerなら、床の子オブジェクトにする
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.SetParent(transform);
        }
    }

    // 床の上側コライダーから離れたときに実行
    private void OnTriggerExit2D(Collider2D collision)
    {
        // 衝突したオブジェクト名がPlayerなら、床の子オブジェクトから解除する
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.SetParent(null);
        }
    }
}
