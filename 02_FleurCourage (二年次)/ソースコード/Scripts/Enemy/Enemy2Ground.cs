using UnityEngine;

public class Enemy2Ground : MonoBehaviour
{
    [SerializeField]
    private float _rLength;         //

    private Vector2 _myPos;         //
    [SerializeField]
    private GameObject _enemy2;     //

    void Start()
    {
        //������
        _myPos = transform.position;
    }

    void Update()
    {
        //try
        //{
        //    transform.position = new Vector2(_enemy2.transform.position.x, _enemy2.transform.position.y);
        //}
        //catch (MissingReferenceException)
        //{

        //}
        if ( _enemy2 == null ) { return; }
        //�ړ�����
        PositionControl();
    }

    /// <summary>
    /// �֐����FControlPotion   <br />
    /// ���@���F����  <br />
    /// �߂�l�F����  <br />
    /// �ړ�����̊֐� <br />
    /// </summary>
    void PositionControl()
    {
        //�ړ�����
        if (transform.position.x >= _myPos.x + _rLength)
        {
            //����ȏ�s���Ȃ��悤�ɂ���
            transform.position = new Vector3(_myPos.x + _rLength, transform.position.y, transform.position.z);
        }
        if (transform.position.x <= _myPos.x - _rLength)
        {
            //����ȏ�s���Ȃ��悤�ɂ���
            transform.position = new Vector3(_myPos.x - _rLength, transform.position.y, transform.position.z);
        }
    }

    // ���̏㑤�R���C�_�[�̒��ɓ������Ƃ��Ɏ��s
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Player���q�I�u�W�F�N�g�ɂ���
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.SetParent(transform);
        }
    }

    // ���̏㑤�R���C�_�[���痣�ꂽ�Ƃ��Ɏ��s
    private void OnTriggerExit2D(Collider2D collision)
    {
        //Player���q�ԃW�F�N�g�����������
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.SetParent(null);
        }
    }
}
