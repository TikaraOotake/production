using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpLeaf : MonoBehaviour
{
    public int UpCnt = 0;
    public float UpSpeed = 0.0f;
    private bool Effect = false;

    private Vector3 randomDandelionBulletPos;

    [SerializeField, Tooltip("�Ȗт̃I�u�W�F�N�g")]
    protected GameObject DandelionBullet;           //�Ȗт̃I�u�W�F�N�g

    [SerializeField]
    protected Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //���ʒu�܂ŗ����Ă�����
        if(transform.position.y <= -0.5f)
        {
            //���x��������Kinematic�ɕύX
            rb.velocity = Vector3.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        //�J�E���g����萔�ȏ��
        if (UpCnt >= 1)
        {
            if (Effect == false)
            {
                //�Ȗт�����������Enemy�̎���ɃC���X�^���X������
                for (int i = 0; i < 3; i++)
                {
                    //�}�P�͈̔͂�random�Ȓl���擾����
                    //Enemy�𒆐S�ɉ�������X,Y�̒l
                    randomDandelionBulletPos = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
                    //�Ȗт��C���X�^���X��
                    //Enemy�𒆐S�Ɂ}�P�iX,Y���W�j�ɃC���X�^���X��
                    Instantiate(DandelionBullet, transform.position + randomDandelionBulletPos, Quaternion.identity, transform);
                }
                Effect = true;
            }

            //�w��̍����܂ŏ㏸
            if (transform.position.y <= 10.0f)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + UpSpeed, transform.position.z);
            }
            //�w��̍����ɂ�����
            else
            {
                //Dynamic�ɕύX���ė����A�J�E���g���O��
                rb.bodyType = RigidbodyType2D.Dynamic;
                UpCnt = 0;
                Effect = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //�^���|�|�̖Ȗтɓ���������
        if (collision.gameObject.tag == "DandelionBullet") 
        {
            //�J�E���g�v���X
            UpCnt++;
        }
    }
}
