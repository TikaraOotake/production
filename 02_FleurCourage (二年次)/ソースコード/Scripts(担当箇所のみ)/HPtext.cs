using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPtext : MonoBehaviour
{
    public Text Scoretext;
    public int HP;
    public float CoolTime;
    public int FlowerID;

    //public GameObject Cam;//カメラの情報

    //public float AdjustPosX;//X座標を補正する
    //public float AdjustPosY;//Y座標を補正する

    SpriteRenderer _sr; //SpriteRendererの格納用
    Animator _animator; //Animatorの格納用

    // Start is called before the first frame update
    void Start()
    {
        if (Scoretext != null)
        {
            Scoretext.text = "HP" + HP + "CoolTime" + CoolTime;
        }

        _sr = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

    }


    // Update is called once per frame
    void Update()
    {
        //UIをカメラの位置と同期させる
        //transform.position = new Vector3(Cam.transform.position.x+ AdjustPosX, Cam.transform.position.y+ AdjustPosY, 0.0f);
        Scoretext.text = "HP" + HP + " CoolTime" + CoolTime;

        _animator.SetInteger("ActionUI", FlowerID);
    }

    public void UpdateHP(int SetHP)
    {
        HP=SetHP;
    }
    public void UpdateFlowerID(int SetID)
    {
        FlowerID = SetID;
    }
    public void UpdateCoolTime(float SetCoolTime)
    {
        CoolTime = SetCoolTime;
    }
}
