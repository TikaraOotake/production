using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01Ground : MonoBehaviour
{
    public GameObject Parent;//�e�̃I�u�W�F�N�g��ۑ�����

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Parent != null)
        {
            transform.position = new Vector2(Parent.transform.position.x , Parent.transform.position.y);
        }
    }

    /// <summary>
    ///�֐����FUpdatePlayerObject()<br />
    ///�������F�e�̃I�u�W�F�N�g<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�e�̃I�u�W�F�N�g���擾����@<br />
    /// </summary>
    public void UpdateParentObject(GameObject Object)
    {
        Parent = Object;
    }

    // ���̏㑤�R���C�_�[�̒��ɓ������Ƃ��Ɏ��s
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �Փ˂����I�u�W�F�N�g����Player�Ȃ�A���̎q�I�u�W�F�N�g�ɂ���
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.SetParent(transform);
            Parent.GetComponent<Enemy01>().UpdateGroundOnFlag(true);
        }
    }

    // ���̏㑤�R���C�_�[���痣�ꂽ�Ƃ��Ɏ��s
    private void OnTriggerExit2D(Collider2D collision)
    {
        // �Փ˂����I�u�W�F�N�g����Player�Ȃ�A���̎q�I�u�W�F�N�g�����������
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.SetParent(null);
            Parent.GetComponent<Enemy01>().UpdateGroundOnFlag(false);
        }
    }
    /// <summary>
    /// �ړ����̃R���C�_�[��obj�ɐG�ꂽ���̏���
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // �G�ꂽobj�̐e���ړ����ɂ���
            other.transform.SetParent(transform);
            Parent.GetComponent<Enemy01>().UpdateGroundOnFlag(true);
        }
    }

    /// <summary>
    /// �ړ����̃R���C�_�[��obj���痣�ꂽ���̏���
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // �G�ꂽobj�̐e���Ȃ���
            other.transform.SetParent(null);
            Parent.GetComponent<Enemy01>().UpdateGroundOnFlag(false);
        }
    }
}
