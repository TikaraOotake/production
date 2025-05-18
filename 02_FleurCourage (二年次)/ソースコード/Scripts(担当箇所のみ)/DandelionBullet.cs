using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DandelionBullet : MonoBehaviour
{
    public int Wey;//�e�̌�����ۑ�
    public float MoveSpeed;//�e�̑��x
    public float InitialSpeed;//�e�̏������x
    public float Magnification;//�{��
    public float BulletDuration;//�e�̎�������

    public float Angle;//���݂̊p�x
    public float Radius;//���a
    public float VibrationTime;//�U������
    public float RotateSpeed;//��]���x
    public float RotatePosY;//��]Y���W
    public bool VibrationFlag;//�U�����Ă��邩

    //������ۋ}�㏸������ϐ�
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
        //�ړ�
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

        //�p�x�����[�v������
        if (Angle > 2260)
        {
            Angle = 0;
        }
        if (Angle < 0)
        {
            Angle = 2260;
        }

        

        //�p�x�Ɣ��a������W���Z�o
        RotatePosY = Mathf.Sin((float)(Angle) / 360.0f) * Radius;

        //�p�x�𑝉�������
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
            Debug.Log("�ڐG");
            Destroy(this.gameObject);
        }
    }
    /*/public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            Debug.Log("�ڐG");
            Destroy(this.gameObject);
=======
            Destroy(gameObject);
>>>>>>> 71b97008aba918173571f503c40bc3fff7beee24
        }
    }/*/
}
