using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackScroll : MonoBehaviour
{
    //public GameObject Camera;//�J�����̃I�u�W�F�N�g��ۑ�����
    public float Multiple;//�X�N���[�����x
    public Vector2 CameraPos;//�J�����̍��W
    public Vector2 BackScrollPos;//�w�i�̖ڕW���W

    public Vector2 InitialPos;//�������W

    // Start is called before the first frame update
    void Start()
    {
        GameObject Camera = GameObject.FindGameObjectWithTag("MainCamera");
        if (Camera == null)
        {
            Debug.Log("�J�����I�u�W�F�N�g���ݒ肳��Ă܂���");
        }
        if (Camera != null)
        {
            Debug.Log("�J�����I�u�W�F�N�g�̎擾����");
        }Debug.Log(Camera.transform.position.x);

        //�������W���L��
        InitialPos = this.gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //�I�u�W�F�N�g��T��
        GameObject Camera = GameObject.FindGameObjectWithTag("MainCamera");
           
        if (Camera == null)
        {
            Debug.Log("�J�����I�u�W�F�N�g���ݒ肳��Ă܂���");
        }
        if (Camera != null)
        {
            CameraPos = Camera.transform.position;
        }

        BackScrollPos.x = CameraPos.x * Multiple;

        transform.position = new Vector2(BackScrollPos.x + InitialPos.x, CameraPos.y);
        //Debug.Log(Camera.transform.position.x);
        //Debug.Log(BackScrollPos.x);
    }
}
