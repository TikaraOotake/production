using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMove : MonoBehaviour
{
    public float Timer;//�^�C�}�[
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;

        if (Timer < 0.0f)
        {
            //�ړ�
            transform.Translate(0.01f, 0.0f, 0.0f);
        }
        if (Timer > 0.0f)
        {
            //�ړ�
            transform.Translate(-0.01f, 0.0f, 0.0f);
        }

        if(Timer>3.0f)
		{
            Timer = -3.0f;
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
        }
    }
}
