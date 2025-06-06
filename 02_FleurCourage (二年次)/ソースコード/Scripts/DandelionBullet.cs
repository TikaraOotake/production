using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DandelionBullet : MonoBehaviour
{
    public int Wey;//弾の向きを保存
    public float MoveSpeed;//弾の速度
    public float InitialSpeed;//弾の初期速度
    public float Magnification;//倍率
    public float BulletDuration;//弾の持続時間

    public float Angle;//現在の角度
    public float Radius;//半径
    public float VibrationTime;//振動時間
    public float RotateSpeed;//回転速度
    public float RotatePosY;//回転Y座標
    public bool VibrationFlag;//振動しているか

    //消える際急上昇させる変数
    public float RiseY;
    // Start is called before the first frame update
    void Start()
    {
        Angle = Random.Range(0, 2261);
        InitialSpeed = Random.Range(100, 2000);
        InitialSpeed /= 10000.0f;
    }

    // Update is called once per frame
    void Update()
    {
        InitialSpeed /= 1.08f;
        //移動
        transform.Translate((MoveSpeed + InitialSpeed) * Wey, 0.0f, 0.0f);

        if (BulletDuration <= 4)
        {
            RiseY+=0.001f;
        }
        if (BulletDuration <= 0)
        {
            Destroy(this.gameObject);
        }
        BulletDuration -= Time.deltaTime;

        //角度をループさせる
        if (Angle > 2260)
        {
            Angle = 0;
        }
        if (Angle < 0)
        {
            Angle = 2260;
        }

        

        //角度と半径から座標を算出
        RotatePosY = Mathf.Sin((float)(Angle) / 360.0f) * Radius;

        //角度を増加させる
        Angle += RotateSpeed;

        transform.position = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y + RotatePosY+ RiseY);
    }
    public void UpdateWey(bool PlayerWey)
    {
        if (PlayerWey == true)
        {
            Wey = -1;
        }
        if (PlayerWey == false)
        {
            Wey = 1;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            Debug.Log("接触");
            Destroy(this.gameObject);
        }
    }
    /*/public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            Debug.Log("接触");
            Destroy(this.gameObject);
=======
            Destroy(gameObject);
>>>>>>> 71b97008aba918173571f503c40bc3fff7beee24
        }
    }/*/
}
