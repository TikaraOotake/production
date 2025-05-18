using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMorninggloryControl : PlayerBase
{
    public GameObject AtkObjPrefab;
    public GameObject AtkObjPrefab2;
    public GameObject SPAtkObjPrefab;
    public GameObject SpEffectPrefab;
    public GameObject SpEffectEndPrefab;
    public GameObject GameOverObj;

    public bool Atktriger;
    public bool EffectTrigger;

    void Start()
    {
        //アタッチされているComponentを取得しておく
        _sr = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();

        Camera = GameObject.Find("Main Camera");
        if (Camera != null)
        { Debug.Log("カメラオブジェクトの取得に成功"); }
        else
        { Debug.Log("カメラオブジェクトの取得に失敗"); }

        //Spriteを表示するオブジェクトをインスタンス化
        if (SpritePrefab != null)
        {
            GameObject childObject = Instantiate(SpritePrefab, transform);
            Sprite = childObject;
            Sprite.GetComponent<PlayerSprite>().UpdatePlayerObject(this.gameObject);
        }

        Atktriger = false;

    }

    void Update()
    {
        //落下時の復帰処理
        FallReturn_Instant();

        UpdateHP_toManager();

        //自身のオブジェクトを透明に
        GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 0);

        //停止時間が0より大きい場合ほぼすべての処理を停止
        if (StopTimer > 0.0f)
        {
            //停止時間を経過させる
            StopTimer -= Time.deltaTime;
            if (StopTimer < 0.0f)
            { StopTimer = 0.0f; }
        }
        else
        {
            switch (st)
            {
                //「通常」
                case Status.Fine:
                    {
                        //接地している時はアニメーションを「待機」にする
                        if (IsGrounded() == true)
                        {
                            AnimationPass(0);
                        }
                        //接地していない場合はアニメーションを「空中」にする
                        else
                        {
                            AirAnimation();
                        }

                        ActionProduce();
                        Move();
                        MaxGravitySpeed();

                        //無敵時間を経過させる
                        if (InvincibleTimer > 0.0f)
                        { InvincibleTimer -= Time.deltaTime; }
                        //無敵時間が0を下回ったら0にする
                        if (InvincibleTimer < 0.0f)
                        { InvincibleTimer = 0.0f; }

                        if (HP <= 0)
                        {
                            st = Status.Dead;
                        }

                    }
                    break;
                //「操作不能」
                case Status.NoControll:
                    {
                        //行動
                        switch (Ast)
                        {
                            //ジャンプ
                            case ActionStatus.Jump:
                                {
                                    Jump();
                                }
                                break;

                            //切り替え開始
                            case ActionStatus.ChangeStart:
                                {
                                    ChangeStart();
                                }
                                break;

                            //切り替え終了
                            case ActionStatus.ChangeEnd:
                                {
                                    ChangeEnd();
                                    //重力を戻す
                                    _rb.gravityScale = 5;
                                }
                                break;

                            //通常攻撃
                            case ActionStatus.NormalAttack:
                                {
                                    Debug.Log("通常攻撃処理");
                                    NormalAttack();
                                }
                                break;
                            //特殊攻撃
                            case ActionStatus.SpecialAttack:
                                {
                                    Debug.Log("特殊攻撃処理");
                                    SpecialAttack();
                                }
                                break;
                        }

                        //無敵時間を経過させる
                        if (InvincibleTimer > 0.0f)
                        { InvincibleTimer -= Time.deltaTime; }
                        //無敵時間が0を下回ったら0にする
                        if (InvincibleTimer < 0.0f)
                        { InvincibleTimer = 0.0f; }
                    }
                    break;
                //「被弾硬直」
                case Status.DamageStiffness:
                    {
                        DamageStiffnessFunction();
                    }
                    break;
                //「落下復帰」
                case Status.FallReturn:
                    {
                        FallReturn();
                    }
                    break;

                //「死亡判定」
                case Status.Dead:
                    {
                        dead = true;
                        fadeTimer += Time.deltaTime;
                        if (fadeTimer >= 0.3f)
                        {
                            Instantiate(GameOverObj, new Vector3(transform.position.x, transform.position.y, 0.0f), Quaternion.identity);
                            Destroy(gameObject);
                            //SceneManager.LoadScene("GameOver"); // (仮)
                        }
                    }
                    break;
            }
        }
        //落下復帰処理の初期設定
       // SetFallReturn();

        //特殊攻撃クールタイマーを進める
        if (SpecialAtkCoolTimer > 0.0f) { SpecialAtkCoolTimer -= Time.deltaTime; }
        //特殊攻撃クールタイマーが0を下回ったら0にする
        if (SpecialAtkCoolTimer < 0.0f) { SpecialAtkCoolTimer = 0.0f; }
        //特殊攻撃クールタイムをマネージャーに送る
        if (Camera != null) { Camera.GetComponent<PlayerManager>().SetCoolTimer(SpecialAtkCoolTimer, SpecialAtkCoolTime); }

        //EPを自然回復
        if (Camera.GetComponent<PlayerManager>().EP_CheckLimit() == false)
        { Camera.GetComponent<PlayerManager>().SetEP(Camera.GetComponent<PlayerManager>().GetEP() + EP_HealRate * 1); }
    }

    /// <summary>
    ///関数名：NormalAttack()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：通常攻撃の処理
    /// </summary>
    void NormalAttack()
    {
        if (RepeatCounter == 0)
        {
            //アニメーションを特殊攻撃に
            AnimationPass(12);
            //繰り返し回数を記録
            RepeatCountFlag = true;
            if (_sr.flipX == true)
            {
                Instantiate(AtkObjPrefab2, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(AtkObjPrefab, transform.position, Quaternion.identity);
            }

        }

        if (RepeatCounter >= 30)
        {
            //特殊技のプログラムを終了し通常状態に戻す(※これがないと特殊技が終了されず動けなくなる)
            ResetStatus();
        }
        RepeatCount();
    }

    /// <summary>
    ///関数名：SpecialAttack()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：特殊攻撃の処理
    /// </summary>
    void SpecialAttack()
    {
        if (IsGrounded() == true)
        {
            if (RepeatCounter == 0)
            {
                //アニメーションを特殊攻撃に
                AnimationPass(10);
                //繰り返し回数を記録
                RepeatCountFlag = true;
                //無敵時間を経過させる

                //EPを消費
                Camera.GetComponent<PlayerManager>().AddEP(-EP_Cost);
                //クールタイマーを加算
                SpecialAtkCoolTimer = SpecialAtkCoolTime;
            }

            if (RepeatCounter >= 30)
            {
                //アニメーションを特殊攻撃に
                AnimationPass(11);

                if (EffectTrigger == false)
                {
                    Instantiate(SpEffectPrefab, transform.position, Quaternion.identity);
                    EffectTrigger = true;
                }

                    //if (Atktriger == false)
                    {
                    Instantiate(SPAtkObjPrefab, transform.position, Quaternion.identity);
                    //Atktriger = true; 
                }
            }

            if (RepeatCounter >= 90)
            {

                Instantiate(SpEffectEndPrefab, transform.position, Quaternion.identity);
                //特殊技のプログラムを終了し通常状態に戻す(※これがないと特殊技が終了されず動けなくなる)
                ResetStatus();
                Atktriger = false;
                EffectTrigger = false;
            }
            RepeatCount();
        }
        else
        {
            ResetStatus();
            RepeatCount();
        }
    }    
}