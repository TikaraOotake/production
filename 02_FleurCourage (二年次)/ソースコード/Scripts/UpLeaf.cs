using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpLeaf : MonoBehaviour
{
    public int UpCnt = 0;
    public float UpSpeed = 0.0f;
    private bool Effect = false;

    private Vector3 randomDandelionBulletPos;

    [SerializeField, Tooltip("綿毛のオブジェクト")]
    protected GameObject DandelionBullet;           //綿毛のオブジェクト

    [SerializeField]
    protected Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //一定位置まで落ちてきたら
        if(transform.position.y <= -0.5f)
        {
            //速度を消してKinematicに変更
            rb.velocity = Vector3.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        //カウントが一定数以上で
        if (UpCnt >= 1)
        {
            if (Effect == false)
            {
                //綿毛が当たった数Enemyの周りにインスタンス化する
                for (int i = 0; i < 3; i++)
                {
                    //±１の範囲でrandomな値を取得する
                    //Enemyを中心に加えたいX,Yの値
                    randomDandelionBulletPos = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
                    //綿毛をインスタンス化
                    //Enemyを中心に±１（X,Y座標）にインスタンス化
                    Instantiate(DandelionBullet, transform.position + randomDandelionBulletPos, Quaternion.identity, transform);
                }
                Effect = true;
            }

            //指定の高さまで上昇
            if (transform.position.y <= 10.0f)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + UpSpeed, transform.position.z);
            }
            //指定の高さについたら
            else
            {
                //Dynamicに変更して落下、カウントを０に
                rb.bodyType = RigidbodyType2D.Dynamic;
                UpCnt = 0;
                Effect = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //タンポポの綿毛に当たったら
        if (collision.gameObject.tag == "DandelionBullet") 
        {
            //カウントプラス
            UpCnt++;
        }
    }
}
