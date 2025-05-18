using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockPlatform : MonoBehaviour
{
    public float Timer = 0; //足場が消えるまでの時間を計測するタイマー

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //インスタンスされた時にタイマー起動
        Timer += Time.deltaTime;

        //指定時間を超えたら足場を消去する
        if(Timer >= 2.0f)
        {
            Destroy(gameObject);
        }
    }
}
