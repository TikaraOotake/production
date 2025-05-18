using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprite : MonoBehaviour
{
    public GameObject Player;//プレイヤーのオブジェクトを保存する
    public bool VibFlag;//振動させるフラグ
    public float VibTimer;//振動時間
    public float VibAttenuation;//振動減衰度
    public float AmplitudeX;//X軸の振幅
    public float AmplitudeY;//Y軸の振幅
    public float VivSpeed;//振動速度
    public float Angle;//現在の角度
    public float RadiusX;//半径
    public float RadiusY;//半径

    public int Animation;//確認用

    public bool FlickerFlag;//明滅フラグ
    public float FlickerTimer;//明滅時間

    SpriteRenderer _sr; //SpriteRendererの格納用
    Rigidbody2D _rb; //Rigidbody2Dの格納用
    Animator _animator; //Animatorの格納用



    // Start is called before the first frame update
    void Start()
    {
        //アタッチされているComponentを取得しておく
        _sr = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        Angle = 1130;

        //自身のオブジェクトを透明に
        GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //自身のオブジェクトを不透明に
        GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);

        if (FlickerFlag)
        {
            if (FlickerTimer <= 0.0f)
            {
                GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
                FlickerTimer = 0.0f;
                FlickerFlag = false;
            }

            if ((int)(FlickerTimer * 15.0f) % 2 == 1)
            {
                GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 128);
            }
            else
            {
                GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
            }

            FlickerTimer -= Time.deltaTime;
        }

        if (VibFlag)
        {
            //角度をループさせる
            if (Angle > 2260)
            {
                Angle = 0;
            }
            if (Angle < 0)
            {
                Angle = 2260;
            }

            //角度と半径から座標を算出
            AmplitudeY = Mathf.Sin((float)(Angle) / 360.0f) * RadiusY;
            AmplitudeX = Mathf.Cos((float)(Angle) / 360.0f) * RadiusX;

            //角度を増加させる
            Angle += VivSpeed;
            //振動時間を経過させる
            VibTimer -= Time.deltaTime;
            if (VibTimer <= 0.0f)
            {
                VibTimer = 0.0f;
                AmplitudeY = 0.0f;
                AmplitudeX = 0.0f;
                VibFlag = false;
            }
            //半径を小さくしていく
            RadiusX = RadiusX / VibAttenuation;
            RadiusY = RadiusY / VibAttenuation;


        }
        if (Player != null)
        {
            transform.position = new Vector3(Player.transform.position.x + AmplitudeX, Player.transform.position.y - AmplitudeY);
        }
    }
    /// <summary>
    ///関数名：UpdatePlayerObject()<br />
    ///引数名：プレイヤーのオブジェクト<br />
    ///戻り値：なし <br />
    ///用　途：プレイヤーのオブジェクトを取得する　<br />
    /// </summary>
    public void UpdatePlayerObject(GameObject Object)
    {
        Player = Object;
    }

    /// <summary>
    ///関数名：UpdateVibStatus()<br />
    ///引数名：振幅時間 振幅減衰度(推奨1.08f) 半径X 半径Y 振動速度<br />
    ///引数例：0.1f, 1.2f, 0.3f, 0.3f, 500.0f　<br />
    ///戻り値：なし <br />
    ///用　途：振動情報を更新する関数　<br />
    /// </summary>
    public void UpdateVibStatus(float Timer, float Attenuation, float SetRadiusX, float SetRadiusY, float SetVivSpeed)
    {
        VibFlag = true;
        VibTimer = Timer;           //振動時間を設定
        VibAttenuation = Attenuation;//振動減衰度を設定
        VivSpeed = SetVivSpeed;      //振動速度を設定
        RadiusX = SetRadiusX;        //振動半径Xを設定
        RadiusY = SetRadiusY;        //振動半径Yを設定
    }
    public void UpdateAnimation(int PlayAnimation)
    {
        _animator.SetInteger("Action", PlayAnimation);

        Animation = PlayAnimation;

    }

    /// <summary>
    ///関数名：UpdatePlayerWey()<br />
    ///引数名：プレイヤーの方向<br />
    ///戻り値：なし <br />
    ///用　途：プレイヤーの方向を更新する関数　<br />
    /// </summary>
    public void UpdatePlayerWey(bool Wey)
    {
        _sr.flipX = Wey;
    }

    /// <summary>
    ///関数名：UpdateFlicker()<br />
    ///引数名：明滅時間<br />
    ///戻り値：なし <br />
    ///用　途：プレイヤーの明滅処理の各変数を設定する処理　<br />
    /// </summary>
    public void UpdateFlicker(float SetFlickerTimer)
    {
        FlickerFlag = true;
        FlickerTimer = SetFlickerTimer;
    }
}
