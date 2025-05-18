using UnityEngine;

public class NormalBulletMoves : MonoBehaviour
{
    private bool _upFlag;

    //0:����
    //1:��������
    //2:������
    [SerializeField, Tooltip("0:����,1:��������,2:������")]
    private int MoveFlag;

    //�ړ����x
    [SerializeField, Tooltip("�ړ����x")]
    private float MoveSpeed;

    //Y���̒���
    [SerializeField, Tooltip("Y���̒���")]
    private float SinLength;
    //Y���̑��x
    [SerializeField, Tooltip("Y���̑��x")]
    private float SinSpeed;
    //Sin�̓���
    private float _sin;
    //sin�̑傫����ς���^�C�}�[
    private float sinTimer;

    //�X�P�[����ς���ϐ�
    [SerializeField]
    private float Scale;

    //�A���t�@�l�������鑬��
    [SerializeField, Tooltip("�A���t�@�l�������鑬��")]
    private float DownAlphaSpeed;
    //�A���t�@�l��������
    private float DownAlpha;

    //�����̏������W
    private Vector2 _myPos;

    private Vector2 _myScale;

    //����Rigidbody2D
    [SerializeField]
    private Rigidbody2D Rb;
    //������SpriteRenderer
    [SerializeField]
    private SpriteRenderer Sr;

    public GameObject PlayerObj;
    private SpriteRenderer PlayerSr;

    // Start is called before the first frame update
    void Start()
    {
        //������
        _upFlag = true;
        DownAlpha = 1;
        _myPos = transform.position;
        _myScale = transform.localScale;
        Scale = 0.0f;
        sinTimer = 0.0f;

        PlayerSr = PlayerObj.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 1)
        {
            //�A���t�@�l��������֐�
            DownAlphaValue();

            //Scale��ς���֐�
            //ChangeScale();

            //�U���̎��
            switch (MoveFlag)
            {
                //����
                case 0:
                    {
                        //�����̒e
                        StraightMove();
                    }
                    break;
                //��������
                case 1:
                    {
                        //Sin�̒e
                        SinMove();
                    }
                    break;
                //������
                case 2:
                    {
                        //�������̒e
                        CurveMove();
                    }
                    break;
            }

            //�A���t�@�l��0�ɂȂ�����Destroy
            if (DownAlpha <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
    /// <summary>
    /// �֐����FStraightMove
    /// ���@���F����
    /// �߂�l�F����
    /// �����̓���������֐�
    /// </summary>
    void StraightMove()
    {
        //if (PlayerSr.flipX == true)
        {
            //transform.Translate(-MoveSpeed, 0.0f, 0.0f, Space.Self);
        }
        //else
        {
            transform.Translate(MoveSpeed, 0.0f, 0.0f, Space.Self);
        }
        //Rb.AddForce(Vector2.right * MoveSpeed);
        
    }
    /// <summary>
    /// �֐����FSinMove
    /// ���@���F����
    /// �߂�l�F����
    /// Sin�̓���������֐�
    /// </summary>
    void SinMove()
    {
        sinTimer += Time.deltaTime; 
        _sin = Mathf.Sin(sinTimer * SinSpeed);
        transform.position = new Vector2(transform.position.x, _myPos.y - _sin * SinLength);

        Rb.velocity = new Vector2(MoveSpeed, 0.0f);
    }
    /// <summary>
    /// �֐����FCurveMove
    /// ���@���F����
    /// �߂�l�F����
    /// �������̓���������֐�
    /// </summary>
    void CurveMove()
    {
        if (_upFlag)
        {
            Rb.AddForce(Vector2.up * MoveSpeed);
            Rb.AddForce(Vector2.right * MoveSpeed);
            _upFlag = false;
        }
    }
    /// <summary>
    /// �֐����FChangeAlphaValue
    /// ���@���F����
    /// �߂�l�F����
    /// �A���t�@�l�����񂾂�Ɖ�����֐�
    /// </summary>
    void DownAlphaValue()
    {
        DownAlpha -= Time.deltaTime * DownAlphaSpeed;
        Sr.color = new Color(1, 1, 1, DownAlpha);
    }
    /// <summary>
    /// �A�j���[�V�����C�x���g�p�̊֐�
    /// </summary>
    void ChangeScale()
    {        
        //������
        Scale = Mathf.Sin(Time.deltaTime * 25);
        Debug.Log(Scale);
        transform.localScale = new Vector3(_myScale.x + Scale,_myScale.y + Scale, transform.localScale.z);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //
        if (collision.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
        }
    }
}
