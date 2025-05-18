using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public GameObject SpritePrefab;//Spriteを表示するオブジェクトのプレハブ
    public GameObject Sprite;//生成したスプライトオブジェクトを保存する
    public GameObject Camera;//カメラのオブジェクトを保存する

    public float MoveSpeed; //移動速度
    public float JumpPower;//ジャンプ力
    public int HP;//体力
    protected int AtkPower;//攻撃力
    public float VulneraRate;//脆弱率

    public float SpecialAtkCoolTimer;//特殊攻撃クールタイマー
    public float SpecialAtkCoolTime;//特殊攻撃クールタイム
    public int EP_Cost;//特殊攻撃消費EP
    public int EP_HealRate;//EP回復率

    protected float StopTimer;//ほぼ全ての処理の停止時間を計測
    protected float InvincibleTimer;//無敵時間を計測(ダメージを受けない時間)
    protected float StiffnessTimer;//硬直時間を計測(一切の操作を受け付けない時間)

    public float ActionIncapacityTimer;//行動中の硬直時間を計測
    public float AddInputInterval;//行動中の追加入力受付時間を計測

    public float GravityMaxSpeed;//最高落下速度
    public float RepeatCounter;//繰り返し回数を計る
    public bool RepeatCountFlag;//繰り返し回数をするかのフラグ
    public bool dead;
    public float HitDamagePosX;//被害(ダメージ)を与えてきた対象物の座標を保存する
    protected bool IsHitBigPillbug;//当たった対象が巨大ダンゴムシか記録

    public int SpecialAttackStep;//特殊攻撃の段階を記録

    public bool SlotShiftSign;//選択するスロットをどちらにずらすか指定

    public LayerMask GroundLayer; //地面として判定するオブジェクトのレイヤー

    protected float fadeTimer = 0.0f;
    protected SpriteRenderer _sr; //SpriteRendererの格納用
    protected Rigidbody2D _rb; //Rigidbody2Dの格納用

    protected Vector2 FallReturn_Pos;
    protected float FallReturnJamp_Height;
    protected bool FallReturnFlag;//落下復帰処理を実行するかのフラグ

    private bool _isMoving;

    [SerializeField]
    protected GameObject DamageEffect;

    [SerializeField]
    protected GameObject DamageEffect2;

    public enum Status
    {
        Fine, //通常
        Dead, //死亡
        Respawn, //復帰
        Clear, //クリア
        NoControll,//操作不能
        DamageStiffness,//被弾硬直
        FallReturn,//落下復帰
    }
    public Status st;

    public enum ActionStatus
    {
        NoAction,//何もしていない
        Parry,//パリィ
        Jump,//ジャンプ
        NormalAttack,//通常攻撃
        SpecialAttack,//特殊攻撃
        ChangeStart,//切り替え開始
        ChangeEnd,//切り替え終了
    }
    public ActionStatus Ast;

    void Awake()
    {
        _isMoving = true;
    }
    /// <summary>
    ///関数名：UpdateHP_toManager()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：PlayerManagerにPlayerのHP情報を更新する
    /// </summary>
    public void UpdateHP_toManager()
    {
        if (Camera != null)
        {
            Camera.GetComponent<PlayerManager>().UpdatePlayerHP(HP);
        }
    }

    /// <summary>
    ///関数名：move()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：プレイヤーの左右移動操作
    /// </summary>
    public void Move()
    {
        //同時押し
        if (InputGetAction("LeftMove", "Stay") && InputGetAction("RightMove", "Stay"))
        {
            return;
        }

        //Aが押された時
        if (InputGetAction("LeftMove", "Stay") && IsWalledL() == false)
        {
            //移動
            transform.Translate(-MoveSpeed, 0.0f, 0.0f);
            //X軸に対して反転させる
            _sr.flipX = true;

            //接地している時はアニメーションを「歩行」にする
            if (IsGrounded() == true)
            {
                AnimationPass(2);
            }
        }
        else
        //Dが押された時
        if (InputGetAction("RightMove", "Stay") && IsWalledR() == false)
        {
            //移動
            transform.Translate(MoveSpeed, 0.0f, 0.0f);
            //X軸に対して反転させる
            _sr.flipX = false;

            //接地している時はアニメーションを「歩行」にする
            if (IsGrounded() == true)
            {
                AnimationPass(2);
            }
        }
    }

    public void SetMoveFlag(bool ret)
    {
        _isMoving = ret;
    }

    public void ActionProduce()
    {
        //ジャンプ
        if (InputGetAction("Jump", "Down") && IsGrounded())
        {
            Ast = ActionStatus.Jump;
            st = Status.NoControll;
        }
        /*/空中ジャンプ
        if (InputGetAction("Jump", "Down") && IsGrounded() == false && JumpRemainCount > 0)
        {
            Ast = ActionStatus.Jump;
            st = Status.NoControll;
            --JumpRemainCount;
        }/*/
        //通常攻撃
        if (InputGetAction("NormalAttack", "Down"))
        {
            Ast = ActionStatus.NormalAttack;
            st = Status.NoControll;
        }
        //特殊攻撃
        if (InputGetAction("SpecialAttack", "Down"))
        {
            //クールタイマーが0の場合 and EPがコスト以上残っている場合
            if (SpecialAtkCoolTimer <= 0.0f && EP_Cost <= Camera.GetComponent<PlayerManager>().GetEP())
            {
                Ast = ActionStatus.SpecialAttack;
                st = Status.NoControll;
            }

        }
        //花切り替え右
        if (InputGetAction("ChangeSlotR", "Down"))
        {
            SlotShiftSign = true;
            Ast = ActionStatus.ChangeStart;
            st = Status.NoControll;
        }
        //花切り替え左
        if (InputGetAction("ChangeSlotL", "Down"))
        {
            SlotShiftSign = false;
            Ast = ActionStatus.ChangeStart;
            st = Status.NoControll;
        }

    }

    /// <summary>
    ///関数名：Jump()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：ジャンプ処理
    /// </summary>
    public void Jump()
    {
        if (RepeatCounter == 0)
        {
            //アニメーションをジャンプ待機状態に
            AnimationPass(8);
            //繰り返し回数を記録
            RepeatCountFlag = true;
        }

        //transform.Translate(0.0f, jumpMomentum, 0.0f);
        if (RepeatCounter >= 2)
        {
            //加速度を初期化
            Vector2 v = new Vector2();
            v.x = 0.0f;
            v.y = 0.0f;
            _rb.velocity = v;
            //上への力を加えてジャンプさせる
            _rb.AddForce(Vector2.up * JumpPower);

            //ジャンプのプログラムを終了し通常状態に戻す(※これがないとジャンプが終了されず動けなくなる)
            ResetStatus();
        }
        RepeatCount();
    }

    /// <summary>
    ///関数名：ChangeStart()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：切り替え開始処理
    /// </summary>
    public void ChangeStart()
    {
        InvincibleTimer = 0.1f;

        if (RepeatCounter == 0)
        {
            //アニメーションを切り替え開始状態に
            AnimationPass(6);
            Debug.Log("アニメーションを再生");
            //繰り返し回数を記録
            RepeatCountFlag = true;
            //空中なら縦軸を制限
            if (IsGrounded() == false) {
                //Y軸の加速度を初期化
                Vector2 v = _rb.velocity;
                v.y = 0.0f;
                _rb.velocity = v;

                //重力を0に
                _rb.gravityScale = 0;
            }
        }

        if (RepeatCounter >= 25)
        {
            //Playerの情報をManagerに送る
            //Camera.GetComponent<PlayerManager>().GetPlayerStatus(new Vector2(transform.position.x, transform.position.y),HP);

            if (Camera != null)
            {
                //Playerの情報をManagerに送る
                Camera.GetComponent<PlayerManager>().ChangePlayer(new Vector2(transform.position.x, transform.position.y), SlotShiftSign, HP, _sr.flipX, this.gameObject);

            }
            else
            {
                Debug.Log("カメラオブジェクトが保存されていません");
            }

            //自身を破棄
            Destroy(this.gameObject);

            //重力を戻す
            _rb.gravityScale = 5;

            //切り替えのプログラムを終了し　仮で通常状態に戻す
            ResetStatus();
        }
        RepeatCount();
    }

    /// <summary>
    ///関数名：ChangeEnd()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：切り替え終了処理
    /// </summary>
    public void ChangeEnd()
    {
        InvincibleTimer = 0.1f;

        if (RepeatCounter == 0)
        {
            //アニメーションを切り替え終了状態に
            AnimationPass(7);
            //繰り返し回数を記録
            RepeatCountFlag = true;
            //空中なら縦軸を制限
            if (IsGrounded() == false)
            { 
                //Y軸の加速度を初期化
                Vector2 v = _rb.velocity;
                v.y = 0.0f;
                _rb.velocity = v;

                //重力を0に
                _rb.gravityScale = 0;
            }
        }

        if (RepeatCounter >= 15)
        {
            Debug.Log("切り替え完了");

            //重力を戻す
            _rb.gravityScale = 5;

            //切り替え終了のプログラムを終了し通常状態に戻す
            ResetStatus();
        }
        RepeatCount();
    }

    /// <summary>
    ///関数名：DamageBounce()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：被弾時の吹っ飛び処理
    /// </summary>
    public void DamageBounce()
    {
        if ((HitDamagePosX - transform.position.x) < 0)
        {
            //加速度を初期化
            Vector2 v = new Vector2();
            v.x = 0.0f;
            v.y = 0.0f;
            _rb.velocity = v;
            //上と横への力を加えてジャンプさせる
            _rb.AddForce(Vector2.up * JumpPower / 1.5f);
            _rb.AddForce(Vector2.right * JumpPower / 2.0f);
        }
        else
        if ((HitDamagePosX - transform.position.x) > 0)
        {
            //加速度を初期化
            Vector2 v = new Vector2();
            v.x = 0.0f;
            v.y = 0.0f;
            _rb.velocity = v;
            //上と横への力を加えてジャンプさせる
            _rb.AddForce(Vector2.up * JumpPower / 1.5f);
            _rb.AddForce(Vector2.left * JumpPower / 2.0f);
        }
        else
        {

        }
    }
    /// <summary>
    ///関数名：DamageBounce()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：被弾時の吹っ飛び処理
    /// </summary>
    public void DamageBounce_BigPillbug()
    {
        if (HitDamagePosX > this.transform.position.x)
        {
            this.transform.position = new Vector2(HitDamagePosX + 3.0f, this.transform.position.y);
        }

        //加速度を初期化
        Vector2 v = new Vector2();
        v.x = 0.0f;
        v.y = 0.0f;
        _rb.velocity = v;
        //上と横への力を加えてジャンプさせる
        _rb.AddForce(Vector2.up * JumpPower / 1.5f * 1.05f);
        _rb.AddForce(Vector2.right * JumpPower / 2.0f * 5.0f);

    }

    /// <summary>
    ///関数名：MaxGravitySpeed()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：落下速度上限超過を防ぐ関数
    /// </summary>
    public void MaxGravitySpeed()
    {
        if (_rb.velocity.y < -GravityMaxSpeed)
        {
            //現在の加速を一時的に保存する
            Vector2 v = _rb.velocity;
            v.y = -GravityMaxSpeed;
            _rb.velocity = v;
        }
    }

    /// <summary>
    ///関数名：IsGrounded() 	<br />
    ///引数名：なし		<br />
    ///戻り値：接地している場合はtrue,していない場合はfalse		<br />
    ///用　途：接地判定処理
    /// </summary>
    public bool IsGrounded()
    {
        bool ret = false;
        //下方向にRayを飛ばして、指定したレイヤーのオブジェクトと接触しているかどうかを判別する
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, Vector2.down, 1.8f, GroundLayer);
        //ヒットしていない場合はnullが解される
        if (hit.collider != null)
        {
            ret = true;
        }
        hit = Physics2D.Raycast(transform.position - new Vector3(0.2f, 0.0f, 0.0f), Vector2.down, 1.8f, GroundLayer);
        //ヒットしていない場合はnullが解される
        if (hit.collider != null)
        {
            ret = true;
        }
        hit = Physics2D.Raycast(transform.position - new Vector3(-0.2f, 0.0f, 0.0f), Vector2.down, 1.8f, GroundLayer);

        //ヒットしていない場合はnullが解される
        if (hit.collider != null)
        {
            ret = true;
        }
        return ret;
    }

    /// <summary>
    ///関数名：IsGrounded() 	<br />
    ///引数名：なし		<br />
    ///戻り値：接地している場合はtrue,していない場合はfalse		<br />
    ///用　途：広い接地判定処理
    /// </summary>
    public bool IsWideGrounded()
    {
        bool ret = false;
        //下方向にRayを飛ばして、指定したレイヤーのオブジェクトと接触しているかどうかを判別する
        RaycastHit2D hit1 = Physics2D.Raycast(transform.position, Vector2.down, 1.8f, GroundLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position - new Vector3(0.5f, 0.0f, 0.0f), Vector2.down, 1.8f, GroundLayer);
        RaycastHit2D hit3 = Physics2D.Raycast(transform.position - new Vector3(-0.5f, 0.0f, 0.0f), Vector2.down, 1.8f, GroundLayer);
        //ヒットしていない場合はnullが解される
        if (hit1.collider != null && hit2.collider != null && hit3.collider != null)
        {
            ret = true;
        }
        return ret;
    }


    /// <summary>
    ///関数名：IsWalled() 	<br />
    ///引数名：なし		<br />
    ///戻り値：接地している場合はtrue,していない場合はfalse		<br />
    ///用　途：接壁判定処理
    /// </summary>
    public bool IsWalledR()
    {
        bool ret = false;
        //下方向にRayを飛ばして、指定したレイヤーのオブジェクトと接触しているかどうかを判別する
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, Vector2.right, 0.3f, GroundLayer);
        //ヒットしていない場合はnullが解される
        if (hit.collider != null)
        {
            ret = true;
        }
        else if(!IsWideGrounded())//空中に居る場合は更に下の方もみる
        {
            hit = Physics2D.Raycast(transform.position + new Vector3(0.0f, -0.9f, 0.0f), Vector2.right, 0.3f, GroundLayer);
        }
        return ret;
    }

    /// <summary>
    ///関数名：IsWalledL() 	<br />
    ///引数名：なし		<br />
    ///戻り値：接地している場合はtrue,していない場合はfalse		<br />
    ///用　途：接壁判定処理
    /// </summary>
    public bool IsWalledL()
    {
        bool ret = false;
        //下方向にRayを飛ばして、指定したレイヤーのオブジェクトと接触しているかどうかを判別する
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, Vector2.left, 0.3f, GroundLayer);
        //ヒットしていない場合はnullが解される
        if (hit.collider != null)
        {
            ret = true;
        }
        else if (!IsWideGrounded())//空中に居る場合は更に下の方もみる
        {
            hit = Physics2D.Raycast(transform.position + new Vector3(0.0f, -0.9f, 0.0f), Vector2.left, 0.3f, GroundLayer);
        }
        return ret;
    }

    /// <summary>
    ///関数名：FallReturn_Instant() 	<br />
    ///引数名：なし		<br />
    ///戻り値：なし		<br />
    ///用　途：落下時の復帰処理(瞬間移動)
    /// </summary>
    public void  FallReturn_Instant()
    {
        //最後に地面と接触していた座標を記録
        if (IsWideGrounded())
        {
            if (Camera != null)
            { Camera.GetComponent<PlayerManager>().SetReturnPos(this.transform.position);  }
            FallReturn_Pos = this.transform.position;
        }
        //落下した場合最後に地面にいた座標に飛ばす
        if (this.transform.position.y < -10.0f)
        {
            Return_Teleport();
        }
    }

    public void Return_Teleport()
    {
        if (Camera != null)
        { this.transform.position = Camera.GetComponent<PlayerManager>().GetReturnPos(); }

        //HPを1減らす
        HP -= 1;
        //無敵時間を設定
        InvincibleTimer = 2.0f;
        //無敵時間分点滅させる
        Sprite.GetComponent<PlayerSprite>().UpdateFlicker(InvincibleTimer);
    }

    /// <summary>
    ///関数名：ResetInterval()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：各時間間隔系列の変数を初期化(無敵、被弾時間以外)
    /// </summary>
    public void ResetInterval()
    {
        //繰り返し回数を初期化
        RepeatCounter = 0;
        //繰り返し回数を記録するかのフラグをfalseに
        RepeatCountFlag = false;
    }

    /// <summary>
    ///関数名：SetFallReturn()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：落下復帰の初期設定
    /// </summary>
    protected void SetFallReturn()
    {
        //空中にいる際の最大の高さを記録
        if (IsGrounded() == false)
        {
            if (FallReturnJamp_Height < this.transform.position.y)
            {
                FallReturnJamp_Height = this.transform.position.y;
            }

        }
        else//地上にいる時は記録を現在の高さに揃える
        {
            FallReturnJamp_Height = this.transform.position.y;
        }

        //落下した場合目標の高さまでジャンプ
        if (this.transform.position.y < -10.0f)
        {
            if (FallReturnFlag == false)
            {
                //落下復帰状態に
                st = Status.FallReturn;

                //HPを1減らす
                HP -= 1;
                //無敵時間を設定
                InvincibleTimer = 3.0f;
                //無敵時間分点滅させる
                Sprite.GetComponent<PlayerSprite>().UpdateFlicker(InvincibleTimer);
                //カメラを揺らす
                if (Camera != null)
                { Camera.GetComponent<NewCameraMove>().VibrationCamera(0.2f, 0.4f); }

                //繰り返し回数を記録
                RepeatCountFlag = true;
                FallReturnFlag = true;
            }

        }
    }
    /// <summary>
    ///関数名：FallReturn()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：落下復帰処理
    /// </summary>
    protected void FallReturn()
    {
        Move();

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

        Debug.Log("目標の高さ" + FallReturnJamp_Height);
        Debug.Log("プレイヤーの高さ" + this.transform.position.y);
        Vector2 v = new Vector2();
        v.x = 0.0f;
        v.y = 0.0f;
        _rb.velocity = v;
        //上への力を加えてジャンプさせる
        _rb.AddForce(Vector2.up * JumpPower);
        if (FallReturnJamp_Height < this.transform.position.y || RepeatCounter >= 60)
        {
            FallReturnFlag = false;
            st = Status.Fine;
            ResetInterval();
        }
        RepeatCount();
    }

    /// <summary>
    ///関数名：DamageStiffnessFunction()<br />
    ///引数名：なし<br />
    ///戻り値：なし <br />
    ///用　途：被弾硬直時間処理
    /// </summary>
    public void DamageStiffnessFunction()
    {
        if (RepeatCounter == 0)
        {
            //アニメーションをダメージに
            AnimationPass(1);
            //ノックバック処理
            DamageBounce();
            if(IsHitBigPillbug)
            {
                DamageBounce_BigPillbug();
                IsHitBigPillbug = false;
            }
            //繰り返し回数を記録
            RepeatCountFlag = true;
        }

        //加速度をX軸のみN分の1に減少させる;
        Vector2 v = _rb.velocity;
        v.x = v.x / 1.1f;
        _rb.velocity = v;

        StiffnessTimer -= Time.deltaTime;
        if (StiffnessTimer <= 0)
        {
            //加速度をX軸のみ初期化
            Vector2 v2 = _rb.velocity;
            v2.x = 0.0f;
            _rb.velocity = v2;

            StiffnessTimer = 0.0f;
            st = Status.Fine;
            //通常状態に戻す
            ResetStatus();
            //無敵時間分点滅させる
            Sprite.GetComponent<PlayerSprite>().UpdateFlicker(InvincibleTimer);
        }

        RepeatCount();
    }

    /// <summary>
    ///関数名：ResetStatus()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：移動やジャンプができる状態に戻す処理
    /// </summary>
    protected void ResetStatus()
    {
        ResetInterval();
        Ast = ActionStatus.NoAction;
        st = Status.Fine;
        SpecialAttackStep = 0;
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.constraints = RigidbodyConstraints2D.None;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    /// <summary>
    ///関数名：AnimationPass()<br />
    ///引数名：ActionType<br />
    ///戻り値：なし <br />
    ///用　途：空中アニメーションを上昇滞空下降ごとに切り替える処理　<br />
    /// </summary>
    public void AirAnimation()
    {
        Vector2 v = _rb.velocity;
        //上昇
        if (v.y > 5.0f)
        {
            AnimationPass(3);
        }
        else
        //下降
        if (v.y < -5.0f)
        {
            AnimationPass(5);
        }
        //滞空
        else
        {
            AnimationPass(4);
        }
    }

    /// <summary>
    ///関数名：AnimationPass()<br />
    ///引数名：ActionType<br />
    ///戻り値：なし <br />
    ///用　途：再生するアニメーションの種類をSpriteObjectに渡す処理　<br />
    /// </summary>
    public void AnimationPass(int ActionType)
    {
        if (Sprite != null)
        {
            Sprite.GetComponent<PlayerSprite>().UpdateAnimation(ActionType);
            Sprite.GetComponent<PlayerSprite>().UpdatePlayerWey(_sr.flipX);
        }
    }

    /// <summary>
    ///関数名：RepeatCount()<br />
    ///引数名：なし　<br />
    ///戻り値：なし <br />
    ///用　途：繰り返し回数を加算していく処理<br />
    /// </summary>
    public void RepeatCount()
    {
        if (RepeatCountFlag)
        {
            ++RepeatCounter;
        }
    }

    /// <summary>
    ///関数名：InputGetAction()<br />
    ///引数名：InputActionType(データ型string) Timing"Down","Stay","Up"(データ型string)<br />
    ///戻り値：bool <br />
    ///用　途：行動ごとに入力を総括する関数　<br />
    /// </summary>
    public bool InputGetAction(string InputActionType, string Timing)
    {
        //信号があるかないかを保存する変数
        bool Signal = false;

        switch (InputActionType)
        {
            case "Jump":
                {
                    if (Timing == "Down")
                    {
                        if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Space))
                        { Signal = true; }
                    }
                    if (Timing == "Stay")
                    {
                        if (Input.GetKey(KeyCode.Joystick1Button0) || Input.GetKey(KeyCode.Space))
                        { Signal = true; }
                    }
                    if (Timing == "Up")
                    {
                        if (Input.GetKeyUp(KeyCode.Joystick1Button0) || Input.GetKeyUp(KeyCode.Space))
                        { Signal = true; }
                    }
                }
                break;
            case "NormalAttack":
                {
                    if (Timing == "Down")
                    {
                        if (Input.GetKeyDown(KeyCode.Joystick1Button2) || Input.GetKeyDown(KeyCode.UpArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Stay")
                    {
                        if (Input.GetKey(KeyCode.Joystick1Button2) || Input.GetKey(KeyCode.UpArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Up")
                    {
                        if (Input.GetKeyUp(KeyCode.Joystick1Button2) || Input.GetKeyUp(KeyCode.UpArrow))
                        { Signal = true; }
                    }
                }
                break;
            case "SpecialAttack":
                {
                    if (Timing == "Down")
                    {
                        if (Input.GetKeyDown(KeyCode.Joystick1Button3) || Input.GetKeyDown(KeyCode.DownArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Stay")
                    {
                        if (Input.GetKey(KeyCode.Joystick1Button3) || Input.GetKey(KeyCode.DownArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Up")
                    {
                        if (Input.GetKeyUp(KeyCode.Joystick1Button3) || Input.GetKeyUp(KeyCode.DownArrow))
                        { Signal = true; }
                    }
                }
                break;
            case "attack1":
                {

                }
                break;
            case "attack2":
                {
                }
                break;
            case "attack3":
                {
                }
                break;
            case "ChangeSlotR":
                {
                    if (Timing == "Down")
                    {
                        if (Input.GetKeyDown(KeyCode.Joystick1Button5) || Input.GetKeyDown(KeyCode.RightArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Stay")
                    {
                        if (Input.GetKey(KeyCode.Joystick1Button5) || Input.GetKey(KeyCode.RightArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Up")
                    {
                        if (Input.GetKeyUp(KeyCode.Joystick1Button5) || Input.GetKeyUp(KeyCode.RightArrow))
                        { Signal = true; }
                    }
                }
                break;
            case "ChangeSlotL":
                {
                    if (Timing == "Down")
                    {
                        if (Input.GetKeyDown(KeyCode.Joystick1Button4) || Input.GetKeyDown(KeyCode.LeftArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Stay")
                    {
                        if (Input.GetKey(KeyCode.Joystick1Button4) || Input.GetKey(KeyCode.LeftArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Up")
                    {
                        if (Input.GetKeyUp(KeyCode.Joystick1Button4) || Input.GetKeyUp(KeyCode.LeftArrow))
                        { Signal = true; }
                    }
                }
                break;
            case "RightMove":
                {
                    if (Timing == "Down")
                    {
                        if (Input.GetKeyDown(KeyCode.D))
                        { Signal = true; }
                    }
                    if (Timing == "Stay")
                    {
                        float h = Input.GetAxis("Horizontal");
                        if (h >= 0.1f)
                        {
                            { Signal = true; }
                        }
                        if (Input.GetKey(KeyCode.D))
                        { Signal = true; }
                    }
                    if (Timing == "Up")
                    {
                        if (Input.GetKeyUp(KeyCode.D))
                        { Signal = true; }
                    }
                }
                break;
            case "LeftMove":
                {
                    if (Timing == "Down")
                    {
                        if (Input.GetKeyDown(KeyCode.A))
                        { Signal = true; }
                    }
                    if (Timing == "Stay")
                    {
                        float h = Input.GetAxis("Horizontal");
                        if (-h >= 0.1f)
                        { Signal = true; }
                        if (Input.GetKey(KeyCode.A))
                        { Signal = true; }
                    }
                    if (Timing == "Up")
                    {
                        if (Input.GetKeyUp(KeyCode.A))
                        { Signal = true; }
                    }
                }
                break;
        }
        if (!_isMoving) { Signal = false; }
        return Signal;
    }

    /// <summary>
    ///関数名：InheritStatus()<br />
    ///引数名：プレイヤ―のステータス<br />
    ///戻り値：なし <br />
    ///用　途：マネジャーに送られたPlayerの情報を引き継ぐ　<br />
    /// </summary>
    public void InheritStatus(int GetHP, bool GetWay)
    {
        //アタッチされているComponentを取得しておく
        _sr = GetComponent<SpriteRenderer>();

        //受け取った引数の情報を対応する各変数に代入
        HP = GetHP;
        _sr.flipX = GetWay;

        Debug.Log("マネージャーから受け取った方向を開示" + GetWay);
        //切り替え終了の段階へ
        Ast = ActionStatus.ChangeEnd;
        st = Status.NoControll;
    }

    /// <summary>
    ///関数名：check()<br />
    ///引数名：なし<br />
    ///戻り値：なし <br />
    ///用　途：呼び出しできるかのテスト　<br />
    /// </summary>
    public void check()
    {
        Debug.Log("呼び出し成功");
    }

    /// <summary>
    ///関数名：OnCollisionEnter2D()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：当たり判定
    /// </summary>
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Abyss")
        {
            Return_Teleport();

            //カメラを揺らす
            if (Camera != null)
            { Camera.GetComponent<NewCameraMove>().VibrationCamera(0.2f, 0.2f); }
        }
    }

    /// <summary>
    ///関数名：OnCollisionEnter2D()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：当たり判定
    /// </summary>
    public void OnCollisionStay2D(Collision2D collision)
    {
        //無敵時間でないなら
        if (InvincibleTimer <= 0)
        {
            if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBoss" || collision.gameObject.tag == "TutorialBoss")
            {
                //アニメーションを被弾状態に
                AnimationPass(1);

                //カメラを揺らす
                if (Camera != null)
                { Camera.GetComponent<NewCameraMove>().VibrationCamera(0.2f, 0.2f); }
                //スプライトを振動させる
                if (Sprite != null)
                { Sprite.GetComponent<PlayerSprite>().UpdateVibStatus(0.1f, 1.2f, 0.3f, 0.3f, 500.0f); }


                //硬直時間を設定
                StiffnessTimer = 0.5f;
                //無敵時間を設定
                InvincibleTimer = 1.0f;
                //各時間間隔系列の変数を初期化
                ResetInterval();

                //停止時間を設定(ヒットストップ)
                StopTimer = 0.1f;

                //HPを減らす
                HP -= 1* (int)VulneraRate;

                if (_sr.flipX == true)
                {
                    //ダメージのエフェクトを発生させる
                    Instantiate(DamageEffect2, new Vector3(transform.position.x + 2.5f, transform.position.y + 1.5f, 0.0f), Quaternion.identity, transform);
                }
                else
                {
                    //ダメージのエフェクトを発生させる
                    Instantiate(DamageEffect, new Vector3(transform.position.x - 2.5f, transform.position.y + 1.5f, 0.0f), Quaternion.identity, transform);
                }


                //接触対象の座標を取得
                Vector3 tmp = GameObject.Find(collision.gameObject.name).transform.position;

                Debug.Log("衝突相手の名前は、" + collision.gameObject.name);
                Debug.Log(tmp.x);

                HitDamagePosX = tmp.x;
                st = Status.DamageStiffness;
                if(collision.gameObject.tag == "TutorialBoss")
                { IsHitBigPillbug = true; }
            }
            if (collision.gameObject.tag == "EnemyAttack" )
            {
                //アニメーションを被弾状態に
                AnimationPass(1);

                //カメラを揺らす
                if (Camera != null)
                { Camera.GetComponent<NewCameraMove>().VibrationCamera(0.2f, 0.2f); }
                //スプライトを振動させる
                if (Sprite != null)
                { Sprite.GetComponent<PlayerSprite>().UpdateVibStatus(0.1f, 1.2f, 0.3f, 0.3f, 500.0f); }

                //硬直時間を設定
                StiffnessTimer = 0.5f;
                //無敵時間を設定
                InvincibleTimer = 1.0f;
                //各時間間隔系列の変数を初期化
                ResetInterval();

                //停止時間を設定(ヒットストップ)
                StopTimer = 0.1f;


                //HPを減らす
                HP -= 2 * (int)VulneraRate;


                //接触対象の座標を取得
                Vector3 tmp = GameObject.Find(collision.gameObject.name).transform.position;

                Debug.Log("衝突相手の名前は、" + collision.gameObject.name);
                Debug.Log(tmp.x);


                HitDamagePosX = tmp.x;
                st = Status.DamageStiffness;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //無敵時間でないなら
        if (InvincibleTimer <= 0)
        {
            if (collision.gameObject.tag == "TutorialBoss")
            {
                //アニメーションを被弾状態に
                AnimationPass(1);

                //カメラを揺らす
                if (Camera != null)
                { Camera.GetComponent<NewCameraMove>().VibrationCamera(0.2f, 0.2f); }
                //スプライトを振動させる
                if (Sprite != null)
                { Sprite.GetComponent<PlayerSprite>().UpdateVibStatus(0.1f, 1.2f, 0.3f, 0.3f, 500.0f); }

                //硬直時間を設定
                StiffnessTimer = 0.5f;
                //無敵時間を設定
                InvincibleTimer = 1.0f;
                //各時間間隔系列の変数を初期化
                ResetInterval();

                //停止時間を設定(ヒットストップ)
                StopTimer = 0.1f;


                //HPを減らす
                HP -= 1 * (int)VulneraRate;


                //接触対象の座標を取得
                Vector3 tmp = GameObject.Find(collision.gameObject.name).transform.position;

                Debug.Log("衝突相手の名前は、" + collision.gameObject.name);
                Debug.Log(tmp.x);


                HitDamagePosX = tmp.x;
                st = Status.DamageStiffness;

                IsHitBigPillbug = true;
            }

            if (collision.gameObject.tag == "Enemy"|| collision.gameObject.tag == "EnemyBoss")
            {
                //アニメーションを被弾状態に
                AnimationPass(1);

                //カメラを揺らす
                if (Camera != null)
                { Camera.GetComponent<NewCameraMove>().VibrationCamera(0.2f, 0.2f); }
                //スプライトを振動させる
                if (Sprite != null)
                { Sprite.GetComponent<PlayerSprite>().UpdateVibStatus(0.1f, 1.2f, 0.3f, 0.3f, 500.0f); }


                //硬直時間を設定
                StiffnessTimer = 0.5f;
                //無敵時間を設定
                InvincibleTimer = 1.0f;
                //各時間間隔系列の変数を初期化
                ResetInterval();

                //停止時間を設定(ヒットストップ)
                StopTimer = 0.1f;

                //HPを減らす
                HP -= 1 * (int)VulneraRate;

                if (_sr.flipX == true)
                {
                    //ダメージのエフェクトを発生させる
                    Instantiate(DamageEffect2, new Vector3(transform.position.x + 2.5f, transform.position.y + 1.5f, 0.0f), Quaternion.identity, transform);
                }
                else
                {
                    //ダメージのエフェクトを発生させる
                    Instantiate(DamageEffect, new Vector3(transform.position.x - 2.5f, transform.position.y + 1.5f, 0.0f), Quaternion.identity, transform);
                }


                //接触対象の座標を取得
                Vector3 tmp = GameObject.Find(collision.gameObject.name).transform.position;

                Debug.Log("衝突相手の名前は、" + collision.gameObject.name);
                Debug.Log(tmp.x);

                HitDamagePosX = tmp.x;
                st = Status.DamageStiffness;
            }
        }
    }

    //【デバッグ用関数】
    //Rayを表示
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, new Vector3(0, -1.8f, 0));
        Gizmos.DrawRay(transform.position - new Vector3(0, 0, 0), new Vector3(0.3f, 0, 0));
        Gizmos.DrawRay(transform.position - new Vector3(0.2f, 0.0f, 0.0f), new Vector3(0, -1.8f, 0));
        Gizmos.DrawRay(transform.position - new Vector3(-0.2f, 0.0f, 0.0f), new Vector3(0, -1.8f, 0));
    }
}
