using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WoodBreak : MonoBehaviour
{
    public GameObject BreakWood;
    public GameObject Effect;
    public GameObject Camera;

    // Start is called before the first frame update
    void Start()
    {
        //カメラが設定されてない場合取得
        if (Camera == null)
        { Camera = GameObject.Find("Main Camera"); if (Camera == null) { Debug.Log("カメラの取得に失敗"); } }

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "SpRoseAtk")
        {
            if(Camera!=null)
            { Camera.GetComponent<NewCameraMove>().VibrationCamera(1.0f, 0.4f); }

            Instantiate(BreakWood, new Vector3(transform.position.x,transform.position.y-2.0f,0.0f), Quaternion.identity);
            Instantiate(Effect, new Vector3(transform.position.x, transform.position.y, 0.0f), Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
