using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DandelionBullet : MonoBehaviour
{
    public int Wey;//’e‚ÌŒü‚«‚ğ•Û‘¶
    public float MoveSpeed;//’e‚Ì‘¬“x
    public float InitialSpeed;//’e‚Ì‰Šú‘¬“x
    public float Magnification;//”{—¦
    public float BulletDuration;//’e‚Ì‘±ŠÔ

    public float Angle;//Œ»İ‚ÌŠp“x
    public float Radius;//”¼Œa
    public float VibrationTime;//U“®ŠÔ
    public float RotateSpeed;//‰ñ“]‘¬“x
    public float RotatePosY;//‰ñ“]YÀ•W
    public bool VibrationFlag;//U“®‚µ‚Ä‚¢‚é‚©

    //Á‚¦‚éÛ‹}ã¸‚³‚¹‚é•Ï”
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
        //ˆÚ“®
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

        //Šp“x‚ğƒ‹[ƒv‚³‚¹‚é
        if (Angle > 2260)
        {
            Angle = 0;
        }
        if (Angle < 0)
        {
            Angle = 2260;
        }

        

        //Šp“x‚Æ”¼Œa‚©‚çÀ•W‚ğZo
        RotatePosY = Mathf.Sin((float)(Angle) / 360.0f) * Radius;

        //Šp“x‚ğ‘‰Á‚³‚¹‚é
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
            Debug.Log("ÚG");
            Destroy(this.gameObject);
        }
    }
    /*/public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            Debug.Log("ÚG");
            Destroy(this.gameObject);
=======
            Destroy(gameObject);
>>>>>>> 71b97008aba918173571f503c40bc3fff7beee24
        }
    }/*/
}
