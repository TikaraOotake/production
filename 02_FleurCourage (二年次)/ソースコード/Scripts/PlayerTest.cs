using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    private bool _specialFlag;          //����U��������������Flag
    private bool _isUntaggedFlag;       //tag��Untagged������Flag

    public int HP;                      //Player�̗̑�
    private int BulletSelect;           //�e�̑I��

    public float MoveSpeed;             //Player�̈ړ����x
    public float JumpPower;             //�W�����v�̑傫��

    public float ReyLength;             //Ray�̒���

    [SerializeField,Tooltip("����U���̎���")]
    private float SpecialTime;          //����U���̎���
    private float _specialTimer;        //����U���̃^�C�}�[

    [SerializeField]
    private Rigidbody2D Rb;             //Player��Rigidbody2D

    [SerializeField,Tooltip("�ʏ�U���̒e")]
    private GameObject[] BulletObj
        = new GameObject[3];            //�ʏ�U���̒e���i�[����
    [SerializeField,Tooltip("����U���̒e")]
    private GameObject SpecialObj;      //����U���̒e���i�[����

    public LayerMask groundLayer;       //���C���[�̔���		

    // Start is called before the first frame update
    void Start()
    {
        _specialFlag = true;
        _isUntaggedFlag = false;

        BulletSelect = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //����U��������Ȃ������珈��
        if (_specialFlag)
        {
            //�n�ʂ�Ray���������Ă����炩��tag��Untagged�������珈������
            if (IsGrounded() && _isUntaggedFlag)
            {
                //C��������Ă����炩��Space��������Ă��Ȃ������珈��
                if (Input.GetKey(KeyCode.C) && !Input.GetKey(KeyCode.Space))
                {
                    //����U��
                    SpecialAtk();
                }
                //Space��������Ă����炩��C��������Ă��Ȃ������珈��
                else if (Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.C))
                {
                    //�W�����v
                    Jump();
                }
            }
            //����
            Move();
            //�U��
            Attack();
        }
        //����U�����������珈��
        else
        {
            //�^�C�}�[�����Z
            _specialTimer += Time.deltaTime;
            //��莞�Ԃ������珈��
            if (_specialTimer >= SpecialTime)
            {
                //����U�����I��
                _specialFlag = true;

                //�^�C�}�[��������
                _specialTimer = 0.0f;

                //���G��Ԃ�����
                gameObject.tag = "Player";
            }
        }
        //Player�̗̑͂�0�ɂȂ����珈��
        if (HP <= 0)
        {
            //���S
            Dead();
        }
    }

    /// <summary>
    /// �֐����FDead	<br />
    /// ���@���F����	<br />
    /// �߂�l�F����	<br />
    /// ���S�̎��̏���������֐�	<br />
    /// </summary>
    void Dead()
    {
        //�I�u�W�F�N�g���폜
        Destroy(gameObject);
    }
    /// <summary>
    ///�֐����FMove <br />
    ///���@���F���� <br />
    ///�߂�l�F���� <br />
    ///�����̏���   <br />
    /// </summary>
    void Move()
    {
        Vector2 a = Rb.velocity;
        //�u���v��������Ă����珈��������
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //�����̏���
            //transform.Translate(-speed, 0.0f, 0.0f);
            Rb.velocity = new Vector2(-MoveSpeed, a.y);
        }

        //�u���v��������Ă����珈��������
        if (Input.GetKey(KeyCode.RightArrow))
        {
            //�����̏���
            //transform.Translate(speed, 0.0f, 0.0f);
            Rb.velocity = new Vector2(MoveSpeed, a.y);
        }
    }
    /// <summary>
    /// �֐����FJump    <br />
    /// ���@���F����  <br />
    /// �߂�l�F����  <br />
    /// �W�����v�̏���������֐�    
    /// </summary>
    void Jump()
    {
        //Space�������ꂽ��W�����v������
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //������̗͂�������
            Rb.AddForce(Vector2.up * JumpPower);
        }
    }
    /// <summary>
    /// �֐����FAttack  <br />
    /// ���@���F����  <br />
    /// �߂�l�F����  <br />
    /// �ʏ�U���̊֐�
    /// </summary>
    void Attack()
    {
        //select�̒l��1���Z
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            BulletSelect++;
        }
        //select�̒l��1����
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            BulletSelect--;
        }
        //select�̐��l����
        if (BulletSelect > 2)
        {
            BulletSelect = 0;
        }
        else if (BulletSelect < 0)
        {
            BulletSelect = 2;
        }
        //X���������珈��
        if (Input.GetKeyDown(KeyCode.X))
        {
            for (int i = 0; i < 3; i++)
            {
                if (BulletSelect == i)
                {
                    //�C���X�^���X������ꏊ
                    Vector3 _instanceBulletPos;
                    _instanceBulletPos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);

                    //�C���X�^���X��
                    Instantiate(BulletObj[i], _instanceBulletPos, Quaternion.identity);
                    break;
                }
            }
        }
    }
    /// <summary>
    /// �֐����FApecialAtk
    /// ���@���F����
    /// �߂�l�F����
    /// ����U���̊֐�
    /// </summary>
    void SpecialAtk()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _specialFlag = false;
            //�ʒu���Œ�
            Rb.velocity = Vector2.zero;

            //���G
            gameObject.tag = "Invincible";

            //�Ռ��g
            Instantiate(SpecialObj, transform.position, Quaternion.identity);
        }
    }
    // <summary>
    // �֐����FIsGrounded   <br />
    // ���@���F����   <br />
    // �߂�l�F����   <br />
    // �n�ʂ����肷��֐�
    // </summary>
    bool IsGrounded()
    {
        bool ret = false;

        //Ray���΂�groundLayer�Ƀq�b�g������true��Ԃ�
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, ReyLength, groundLayer);
        //hit��NULL����Ȃ�������ret��true�ɂ���
        if (hit.collider != null)
        {
            ret = true;
        }
        return ret;
    }
    // <summary>
    // �֐����FIsGrounded   <br />
    // ���@���F����           <br />
    // �߂�l�F����           <br />
    // Ray����������֐�      <br />
    // </summary>
    void OnDrawGizmos()
    {
        //***IsGrounded�֐��Ɠ���������Rey�ɂ���***
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, new Vector3(0, -ReyLength, 0));
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && gameObject.tag == "Player")
        {
            HP--;
        }
        //���������I�u�W�F�N�g��tag��Untagged��������
        if (collision.gameObject.tag == "Untagged")
        {
            _isUntaggedFlag = true;
        }
        //���������I�u�W�F�N�g��tag��Untagged����Ȃ�������
        else
        {
            _isUntaggedFlag = false;
        }
    }
}
