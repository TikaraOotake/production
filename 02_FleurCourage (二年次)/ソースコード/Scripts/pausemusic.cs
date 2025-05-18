using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pausemusic : MonoBehaviour
{
    [SerializeField] private AudioSource a;//AudioSource型の変数aを宣言 再生するAudioSourceをアタッチ必要

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
		{
            a.Pause();//AudioSource型が入っている変数aを一時停止
        }

        if(Input.GetKeyDown(KeyCode.H))
		{
            a.UnPause();//AudioSource型が入っている変数aを一時停止解除
        }
    }

    /*/ボタン1つ目に割り当てる自作の関数
    public void PushButtonPause()
    {
        a.Pause();//AudioSource型が入っている変数aを一時停止 
    }

    //ボタン2つ目に割り当てる自作の関数
    public void PushButtonUnPause()
    {
        a.UnPause();//AudioSource型が入っている変数aを一時停止解除 
    }/*/
}
