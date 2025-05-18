using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackScroll : MonoBehaviour
{
    //public GameObject Camera;//カメラのオブジェクトを保存する
    public float Multiple;//スクロール速度
    public Vector2 CameraPos;//カメラの座標
    public Vector2 BackScrollPos;//背景の目標座標

    public Vector2 InitialPos;//初期座標

    // Start is called before the first frame update
    void Start()
    {
        GameObject Camera = GameObject.FindGameObjectWithTag("MainCamera");
        if (Camera == null)
        {
            Debug.Log("カメラオブジェクトが設定されてません");
        }
        if (Camera != null)
        {
            Debug.Log("カメラオブジェクトの取得成功");
        }Debug.Log(Camera.transform.position.x);

        //初期座標を記憶
        InitialPos = this.gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //オブジェクトを探す
        GameObject Camera = GameObject.FindGameObjectWithTag("MainCamera");
           
        if (Camera == null)
        {
            Debug.Log("カメラオブジェクトが設定されてません");
        }
        if (Camera != null)
        {
            CameraPos = Camera.transform.position;
        }

        BackScrollPos.x = CameraPos.x * Multiple;

        transform.position = new Vector2(BackScrollPos.x + InitialPos.x, CameraPos.y);
        //Debug.Log(Camera.transform.position.x);
        //Debug.Log(BackScrollPos.x);
    }
}
