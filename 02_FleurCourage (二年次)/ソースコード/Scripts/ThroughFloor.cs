using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThroughFloor : MonoBehaviour
{
    public float Timer = 0; //判定を戻すためのタイマー
    private PlatformEffector2D _platformEffector;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Awake()
    {
        //コンポーネントを取得
        _platformEffector = GetComponent<PlatformEffector2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //タイマーで戻るまでの時間計測
        Timer += Time.deltaTime;

        //Sキーが押されたとき乗っている判定を消してすり抜けるように
        if (Input.GetKey(KeyCode.S))
            _platformEffector.rotationalOffset = 180f;

        //指定した時間になったら判定を上に戻す
        if (Timer >= 0.3)
        {
            _platformEffector.rotationalOffset = 0f;
            Timer = 0;
        }
    }
}
