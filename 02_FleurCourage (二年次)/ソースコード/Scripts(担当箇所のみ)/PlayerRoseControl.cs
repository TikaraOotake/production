using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerRoseControl : PlayerBase
{
    [SerializeField]
    private GameObject RoseKickColliderPrefab;   //キック当たり判定のプレハブ
    public GameObject RoseKickCollider;         //キック当たり判定を保存する
    public GameObject GameOverObj;

    public float KickSpeedCoefficient;          //キック速度係数
    public bool KickCancelFlag;                 //キックを強制的に終了するかのフラグ

    private bool _isNormalAttack;               //通常攻撃中かどうかを示すフラグ
    [SerializeField]
    private GameObject RoseNormalCol;           //通常攻撃の当たり判定

    private CapsuleCollider2D _co;//CapsuleCollider2Dの格納


    void Start()
    {
        //アタッチされているComponentを取得しておく
        _co = GetComponent<CapsuleCollider2D>();
        _sr = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();

        dead = false;
        _isNormalAttack = false;

        Camera = GameObject.Find("Main Camera");

        //Spriteを表示するオブジェクトをインスタンス化
        if (SpritePrefab != null)
        {
            GameObject childObject = Instantiate(SpritePrefab, transform);
            Sprite = childObject;
            Sprite.GetComponent<PlayerSprite>().UpdatePlayerObject(this.gameObject);
        }
    }

    void Update()
    {
        //落下時の復帰処理
        FallReturn_Instant();






        //ManagerにPlayerのHP情報を更新
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
        //SetFallReturn();

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
            //繰り返し回数を記録
            RepeatCountFlag = true;
            _isNormalAttack = true;
            if (_isNormalAttack)
            {
                //当たり判定をインスタンス
                Instantiate(RoseNormalCol, transform.position, Quaternion.identity, transform);
                _isNormalAttack = false;
            }
        }

        Move();
        //アニメーションを通常攻撃に
        AnimationPass(14);

        if (RepeatCounter >= 30)
        {
            //当たり判定を消す
            Destroy(GameObject.Find("RoseNormalAttackCol(Clone)"));

            //特殊技のプログラムを終了し通常状態に戻す(※これがないと特殊技が終了されず動けなくなる)
            ResetStatus();
        }
        RepeatCount();
    }

    /// <summary>
    ///関数名：cialcialAttack()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：特殊攻撃の処理
    /// </summary>
    void SpecialAttack()
    {
        switch (SpecialAttackStep)
        {
            //地上か空中で次に派生
            case 0:
                {
                    //フラグを初期化
                    KickCancelFlag = false;

                    if (IsGrounded())
                    { SpecialAttackStep = 1; }
                    else { SpecialAttackStep = 2; }

                }
                break;
            //ジャンプ
            case 1:
                {
                    if (RepeatCounter == 0)
                    {
                        AnimationPass(11);
                        //加速度を初期化
                        Vector2 v = new Vector2();
                        v.x = 0.0f;
                        v.y = 0.0f;
                        _rb.velocity = v;
                        //上への力を加えてジャンプさせる
                        _rb.AddForce(Vector2.up * JumpPower);

                        //繰り返し回数を記録
                        RepeatCountFlag = true;
                    }

                    if (_sr.flipX)
                    {
                        //移動
                        transform.Translate(-0.05f, 0.0f, 0.0f);
                    }
                    else
                    {
                        //移動
                        transform.Translate(0.05f, 0.0f, 0.0f);
                    }

                    //30回繰り返すか落下し始めたら
                    Vector2 vel = _rb.velocity;
                    if (RepeatCounter >= 30 || vel.y < -1.0f)
                    {
                        SpecialAttackStep = 2;
                        ResetInterval();
                    }
                    RepeatCount();
                }
                break;
            //キック
            case 2:
                {
                    if (RepeatCounter == 0)
                    {
                        AnimationPass(12);
                        //キック速度係数を初期化
                        KickSpeedCoefficient = 0.0f;

                        //加速度を初期化
                        Vector2 v = new Vector2();
                        v.x = 0.0f;
                        v.y = 0.0f;
                        _rb.velocity = v;

                        //重力を0に
                        _rb.gravityScale = 0;

                        //無敵時間を設定
                        InvincibleTimer = 1.0f;

                        //キック当たり判定をインスタンス化

                        if (RoseKickColliderPrefab != null)
                        {
                            GameObject childObject = Instantiate(RoseKickColliderPrefab, transform.position, Quaternion.identity, transform);
                            RoseKickCollider = childObject;
                            RoseKickCollider.GetComponent<RoseKickCollider>().GetPlayerObject(this.gameObject);
                            Debug.Log("キック当たり判定のインスタンス化に成功");
                        }
                        else { Debug.Log("キック当たり判定のプレハブが見つかりません"); }

                        //繰り返し回数を記録
                        RepeatCountFlag = true;

                        //EPを消費
                        Camera.GetComponent<PlayerManager>().AddEP(-EP_Cost);
                    }

                    if (KickSpeedCoefficient < 1.0f)
                    {
                        KickSpeedCoefficient += 0.005f + KickSpeedCoefficient / 3.0f;
                    }

                    if (_sr.flipX)
                    {
                        //移動
                        transform.Translate(-0.2f * KickSpeedCoefficient, -0.1f * KickSpeedCoefficient, 0.0f);
                    }
                    else
                    {
                        //移動
                        transform.Translate(0.2f * KickSpeedCoefficient, -0.1f * KickSpeedCoefficient, 0.0f);
                    }

                    //指定F経過かフラグがtrueになったら
                    //if (RepeatCounter >= 50 || KickCancelFlag == true)
                    if (KickCancelFlag == true)
                    {
                        //攻撃判定を生成
                        if (RoseKickCollider != null)
                        {
                            RoseKickCollider.GetComponent<RoseKickCollider>().GeneratePlayerAttack();
                        }

                        //クールタイマーを加算
                        SpecialAtkCoolTimer = SpecialAtkCoolTime;

                        //判定を削除しNULLを代入
                        Destroy(RoseKickCollider);
                        RoseKickCollider = null;

                        //重力を戻す
                        _rb.gravityScale = 5;
                        _co.isTrigger = false;

                        _rb.bodyType = RigidbodyType2D.Dynamic;
                        SpecialAttackStep = 3;
                        ResetInterval();
                    }
                    RepeatCount();
                }
                break;
            //跳ね返り
            case 3:
                {
                    if (RepeatCounter == 0)
                    {
                        AnimationPass(13);
                        //加速度を初期化
                        Vector2 v = new Vector2();
                        v.x = 0.0f;
                        v.y = 0.0f;
                        _rb.velocity = v;
                        //上への力を加えてジャンプさせる
                        _rb.AddForce(Vector2.up * JumpPower / 3.0f);

                        //プレイヤーの向きに応じて横方向に力を加える
                        if (_sr.flipX)
                        { _rb.AddForce(Vector2.right * 400); }
                        else
                        { _rb.AddForce(Vector2.left * 400); }

                        //繰り返し回数を記録
                        RepeatCountFlag = true;

                        //カメラを揺らす
                        if (Camera != null)
                        { Camera.GetComponent<NewCameraMove>().VibrationCamera(0.5f, 0.2f); }
                    }

                    //加速度をX軸のみN分の1に減少させる;
                    Vector2 vel = _rb.velocity;
                    vel.x = vel.x / 1.1f;
                    _rb.velocity = vel;

                    //入力があった場合跳ね返り時間を無視して特殊攻撃
                    if (InputGetAction("SpecialAttack", "Down_No"))
                    {
                        //加速度を初期化
                        Vector2 v = new Vector2();
                        v.x = 0.0f;
                        v.y = 0.0f;
                        _rb.velocity = v;

                        //フラグを初期化
                        KickCancelFlag = false;

                        ResetInterval();

                        SpecialAttackStep = 0;
                    }

                    if (RepeatCounter >= 15)
                    {
                        SpecialAttackStep = 0;
                        ResetStatus();
                    }
                    RepeatCount();
                }
                break;
        }
    }
    public void UpdateKickCancelFlag(bool Flag)
    {
        KickCancelFlag = Flag;
    }
}
