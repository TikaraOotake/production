using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    [SerializeField]
    private AudioSource a;//AudioSource�^�̕ϐ�a��錾

    [SerializeField]
    private AudioSource b;

    public float Timer = 0.0f;
    private int Cnt = 0;
    // Start is called before the first frame update
    void Start()
    {
        a.Play();//AudioSource�^�������Ă���ϐ�a���Đ�
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Cnt++;
            a.Pause();
            /*a = b;
            a.Play();*/
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            a = b;
            a.Play();
        }
    }
}
