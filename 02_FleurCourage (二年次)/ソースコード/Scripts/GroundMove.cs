using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMove : MonoBehaviour
{
    public float Timer;//タイマー
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;

        if (Timer < 0.0f)
        {
            //移動
            transform.Translate(0.01f, 0.0f, 0.0f);
        }
        if (Timer > 0.0f)
        {
            //移動
            transform.Translate(-0.01f, 0.0f, 0.0f);
        }

        if(Timer>3.0f)
		{
            Timer = -3.0f;
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
        }
    }
}
