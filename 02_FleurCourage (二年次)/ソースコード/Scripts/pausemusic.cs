using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pausemusic : MonoBehaviour
{
    [SerializeField] private AudioSource a;//AudioSource�^�̕ϐ�a��錾 �Đ�����AudioSource���A�^�b�`�K�v

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
		{
            a.Pause();//AudioSource�^�������Ă���ϐ�a���ꎞ��~
        }

        if(Input.GetKeyDown(KeyCode.H))
		{
            a.UnPause();//AudioSource�^�������Ă���ϐ�a���ꎞ��~����
        }
    }

    /*/�{�^��1�ڂɊ��蓖�Ă鎩��̊֐�
    public void PushButtonPause()
    {
        a.Pause();//AudioSource�^�������Ă���ϐ�a���ꎞ��~ 
    }

    //�{�^��2�ڂɊ��蓖�Ă鎩��̊֐�
    public void PushButtonUnPause()
    {
        a.UnPause();//AudioSource�^�������Ă���ϐ�a���ꎞ��~���� 
    }/*/
}
