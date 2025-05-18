using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public float Timer = 0;　　　//足場出現の時間計測のためのタイマー
    public float pointX = 0.0f;　//足場出現のｘ座標を調整する変数
    public float pointY = 0.0f;  //足場出現のｙ座標を調整する変数
    public GameObject Platform;  //出現させるオブジェクト
    public int cnt = 0;          //足場が出現した回数をカウントする変数

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //タイマーをスタートさせる
        Timer += Time.deltaTime;

        //タイマーが指定時間になったら足場を出現させる
        if(Timer >= 1 && cnt == 0)
        {
            Instantiate(Platform, new Vector3(pointX, pointY,0.0f), Quaternion.identity);
            ++cnt;
        }

        if(Timer >= 2 && cnt == 1)
        {
            Instantiate(Platform, new Vector3(pointX + 3, pointY - 1, 0.0f), Quaternion.identity);
            ++cnt;
        }

        if(Timer >= 3 && cnt == 2)
        {
            Instantiate(Platform, new Vector3(pointX + 6, pointY , 0.0f), Quaternion.identity);
            ++cnt;
        }

        //タイマーが指定時間を超えたらタイマーとカウントを元に戻す
        if(Timer >= 5 && cnt >= 3)
        {
            Timer = 0;
            cnt = 0;
        }
    }
}
