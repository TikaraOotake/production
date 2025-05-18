using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{

    [SerializeField] private AudioSource a1;//AudioSource�^�̕ϐ�a1��錾 �g�p����AudioSource�R���|�[�l���g���A�^�b�`�K�v
    [SerializeField] private AudioSource a2;//AudioSource�^�̕ϐ�a2��錾 �g�p����AudioSource�R���|�[�l���g���A�^�b�`�K�v
    [SerializeField] private AudioSource a3;//AudioSource�^�̕ϐ�a3��錾 �g�p����AudioSource�R���|�[�l���g���A�^�b�`�K�v

    [SerializeField] private AudioClip b1;//AudioClip�^�̕ϐ�b1��錾 �g�p����AudioClip���A�^�b�`�K�v
    [SerializeField] private AudioClip b2;//AudioClip�^�̕ϐ�b2��錾 �g�p����AudioClip���A�^�b�`�K�v 
    [SerializeField] private AudioClip b3;//AudioClip�^�̕ϐ�b3��錾 �g�p����AudioClip���A�^�b�`�K�v 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
		{
            a1.PlayOneShot(b1);//a1�ɃA�^�b�`����AudioSource�̐ݒ�l��b1�ɃA�^�b�`�������ʉ����Đ�
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            a2.PlayOneShot(b2);//a2�ɃA�^�b�`����AudioSource�̐ݒ�l��b2�ɃA�^�b�`�������ʉ����Đ�
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            a3.PlayOneShot(b3);//a3�ɃA�^�b�`����AudioSource�̐ݒ�l��b3�ɃA�^�b�`�������ʉ����Đ�
        }
    }

    /*/����̊֐�1
    public void SE1()
    {
        a1.PlayOneShot(b1);//a1�ɃA�^�b�`����AudioSource�̐ݒ�l��b1�ɃA�^�b�`�������ʉ����Đ�
    }

    //����̊֐�2
    public void SE2()
    {
        a2.PlayOneShot(b2);//a2�ɃA�^�b�`����AudioSource�̐ݒ�l��b2�ɃA�^�b�`�������ʉ����Đ�
    }

    //����̊֐�3
    public void SE3()
    {
        a3.PlayOneShot(b3);//a3�ɃA�^�b�`����AudioSource�̐ݒ�l��b3�ɃA�^�b�`�������ʉ����Đ�
    }/*/
}
