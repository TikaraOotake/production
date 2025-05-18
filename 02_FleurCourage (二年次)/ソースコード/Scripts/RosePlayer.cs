using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RosePlayer : MonoBehaviour
{
    //移動速度
    public float MoveSpeed;

    //ジャンプ力
    public float jumpPower;

    public float StopTimer;
    //特殊攻撃時、経過時間を格納する変数
    public float SpTime;

    //追加入力時間を格納する変数
    public float AddInputTime;

    //計測時間を格納する変数
    public float IntervalTime;

    //特殊攻撃の高さの上限
    public float High;

    //特殊攻撃中、空中待機の時間
    public float AirStop;

    //特殊攻撃時、プレイヤーの現在位置
    public Vector3 pos;

    //攻撃に関する回数を格納する変数
    public int ACnt;

    //特殊攻撃時の着地判定に使用する変数
    //0:true  1:false　　とする。
    public int landingCnt;

    //特殊攻撃時の着地判定に切り替える変数
    public bool landingSwitch;

    //地面として判断するオブジェクトのレイヤー
    public LayerMask groundLayer;

    //プレイヤー全体の状態
    public enum PlayerStutas
    {
        Fine,       //通常
        NoControl,      //攻撃ボタンが押された際
        Dead,       //死亡時
    }
    public PlayerStutas PSt;

    //攻撃の種類別状態
    public enum AttackStutas
    {
        NoAttack,       //攻撃されていない時
        Normal,     //通常攻撃
        Spcial,     //特殊攻撃
    }
    public AttackStutas ASt;

    //特殊攻撃専用の段階別状態
    public enum SpAtk
    {
        SpNo,       //特殊攻撃がない時
        AtkUp,      //特殊攻撃・上昇中
        SpWait,     //特殊攻撃・空中待機中
        AtkDown,        //特殊攻撃・下降中
    }
    public SpAtk spAtk;

    //SpriteRendererを格納する変数
    SpriteRenderer Sr;

    //RigidBodyを格納する変数
    Rigidbody2D Rb;

    //Animatorを格納する変数
    Animator Anim;

    //CapsuleCollider2Dを格納する変数
    CapsuleCollider2D CCol;

    // Start is called before the first frame update
    void Start()
    {
        //変数初期化
        ACnt = 0;
        landingCnt = 0;
        landingSwitch = false;
        AddInputTime = 0.0f;
        IntervalTime = 0.0f;
        AirStop = 0.2f;
        StopTimer = 0.0f;
        pos = transform.position;

        //Componentの取得
        Sr = GetComponent<SpriteRenderer>();
        Rb = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        CCol = GetComponent<CapsuleCollider2D>();

        //状態の初期設定
        PSt = PlayerStutas.Fine;
        ASt = AttackStutas.NoAttack;
        spAtk = SpAtk.SpNo;

    }

    // Update is called once per frame
    void Update()
    {
        switch(PSt)
        {
            //通常
            case PlayerStutas.Fine:
                {
                      
                    
                    //設置している時は「待機」にする
                    if (IsGrounded() == true)
                    {
                        Anim.SetInteger("Action", 0);
                    }
                    //空中にいる時は「ジャンプ」にする
                    else
                    {
                        Anim.SetInteger("Action", 2);
                    }

                    //移動処理
                    Move();

                    if (IsGrounded() == true)
                    {
                        //ジャンプ処理
                        Jump();
                    }

                    //攻撃ボタンを押されたかどうかの確認を行う
                    AtkSearch();

                    break;
                            
			    }

            //攻撃を行う際は他のアニメーションを停止させて、攻撃専用のアニメーションのみに移行させる。
            //入力が行われなかった場合、PlayerStutas.Fineに移行される。
            case PlayerStutas.NoControl:
                {
                    switch (ASt)
                    {
                        //通常攻撃の処理
                        case AttackStutas.Normal:
                            {
                                Debug.Log("通常攻撃");
                                //tagを"PlayerAtk"に変更。敵に当たった場合、敵を破壊できる。
                                this.tag = "PlayerAtk";
                                //攻撃アニメーションの再生
                                Anim.SetInteger("Action", 10);
                                //再度入力が行われるかチェックを行う。
                                AtkSupport();

                            }
                            break;
                        //特殊攻撃の処理
                        case AttackStutas.Spcial:
                            {
                                Debug.Log("特殊攻撃");
                                //tagを"PlayerAtk"に変更。敵に当たった場合、敵を破壊できる。
                                this.tag = "PlayerAtk";
                                Rb.gravityScale = 1.0f;
                                pos = transform.position;
                                
                                //攻撃アニメーションの再生
                                switch (spAtk)
                                {
                                    //特殊攻撃ボタンが押されていない
                                    case SpAtk.SpNo:
                                        {
                                        }
                                        break;

                                    //特殊攻撃・上昇中
                                    case SpAtk.AtkUp:
                                        {
                                            Anim.SetInteger("Action", 20);
                                            //座標を取得
                                            //画像の向きによって方向を変える
                                            if (Sr.flipX == false && pos.y <= High)
                                            {
                                                pos.x += 0.2f;    // x座標へ0.3加算
                                                pos.y += 0.5f;    // y座標へ0.4加算
                                                transform.position = pos;  // 座標を設定 
                                            }
                                            else if (Sr.flipX == true && pos.y <= High)
                                            {
                                                pos.x -= 0.2f;    // x座標へ0.3減算
                                                pos.y += 0.5f;    // y座標へ0.4加算
                                                transform.position = pos;  // 座標を設定
                                            }
                                            Debug.Log("上昇中");

                                            //pos.yが高さ上限を超えたら滞空に切り替え
                                            if (pos.y >= High)
                                            {
                                                spAtk = SpAtk.SpWait;
                                                
                                                Debug.Log("切り替え");
                                            }
                                        }
                                        break;

                                    //特殊攻撃・空中滞空
                                    case SpAtk.SpWait:
                                        {
                                            //空中停止しながらアニメーションを行う
                                            Rb.isKinematic = true;
                                            AirStop -= Time.deltaTime;
                                            Anim.SetInteger("Action", 21);

                                            //空中滞空時間を過ぎたら下降に切り替える
                                            //空中停止を解除
                                            if (AirStop <= 0.0f)
                                            {
                                                
                                                spAtk = SpAtk.AtkDown;    
                                                Debug.Log("耐空切り替え");
                                                
                                            }
                                            Debug.Log("耐空中");

                                        }
                                        break;

                                    //特殊攻撃・下降中
                                    case SpAtk.AtkDown:
                                        {
                                            if (pos.y > -1.0f && landingSwitch == false)
                                            {
                                                Anim.SetInteger("Action", 22);
                                                Rb.isKinematic = false;
                                            }
                                            else if (pos.y <= -1.0f && landingSwitch == false)
                                            {
                                                Anim.SetInteger("Action", 23);

                                                if (Rb.isKinematic == true)
                                                {
                                                    Rb.isKinematic = false;
                                                }
                                            }
                                            
                                            //座標を取得
                                            //画像の向きによって方向を変える
                                            if (Sr.flipX == false)
                                            {
                                                pos.x += 0.1f;    // x座標へ0.15加算
                                                pos.y -= 0.1f;    // y座標へ0.15加算
                                                transform.position = pos;  // 座標を設定
                                            }
                                            else
                                            {
                                                pos.x -= 0.1f;    // x座標へ0.15加算
                                                pos.y -= 0.1f;    // y座標へ0.15加算
                                                transform.position = pos;  // 座標を設定
                                            }

                                            Debug.Log("下降中");
                                        }
                                        break;
                                }
                                //再度入力が行われるかチェックを行う。
                                AtkSupport();
                            }
                            break;
                        //攻撃を行わない時
                        case AttackStutas.NoAttack:
                            {
                            }
                            break;
                    }
                    break;
                }
            //死亡処理
            case PlayerStutas.Dead:
                {
                    Dead();
                    break;
                }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        //接触したgameObject.tagが"Ground"もしくは"enemy"だった場合
        //地面か敵に接触した際の接触判定
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "enemy")
        {
            //特殊攻撃かどうか判定を行う。（ただし特殊攻撃・下降かどうかで判定を行う）
            if (ASt == AttackStutas.Spcial)
            {

                //特殊攻撃、着地用の処理
                //今現在移行に苦戦の為、一時的に保留中

                //Debug.Log("反応している");
                //Anim.SetInteger("Action", 30);
                //landingSwitch = true;
                //if (landingSwitch == true)
                //{
                //    Anim.SetInteger("Action", 30);
                //}

                //半歩下がるアニメーションを作動、
                //画像の向きによって少し移動する方向を変える
                if (Sr.flipX == false)
                {
                    transform.Translate(-0.5f, 0.0f, 0.0f);
                    //パラメーターリセットを行う
                    ResetStutas();
                }
                else
                {
                    transform.Translate(0.5f, 0.0f, 0.0f);
                    //パラメーターリセットを行う
                    ResetStutas();
                }
            }
        }
    }

    //移動処理
    public void Move()
    {
        //左移動
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(-MoveSpeed, 0.0f, 0.0f);
            //設置している時は「待機」
            if (IsGrounded() == true)
            {
                Anim.SetInteger("Action", 1);
            }
            //空中にいる時は「ジャンプ」
            else
            {
                Anim.SetInteger("Action", 2);
            }

            //X軸に対して反転させる
            Sr.flipX = true;
        }
        //右移動
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(MoveSpeed, 0.0f, 0.0f);
            //設置している時は「待機」
            if (IsGrounded() == true)
            {
                Anim.SetInteger("Action", 1);
            }
            //空中にいる時は「ジャンプ」
            else
            {
                Anim.SetInteger("Action", 2);
            }

            //X軸に対して反転させる
            Sr.flipX = false;
        }
    }

    //ジャンプ処理
    public void Jump()
    {
        //スペースキーが押されたとき
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //上の方にジャンプさせる
            Rb.AddForce(Vector2.up * jumpPower);
            //プレイヤーのアニメーションを「ジャンプ」にする
            Anim.SetInteger("Action", 2);
        }
    }

    //入力を参照して攻撃種類を変える関数
    public void AtkSearch()
    {
        //通常攻撃だった場合
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PSt = PlayerStutas.NoControl;
            ASt = AttackStutas.Normal;
        }
        //特殊攻撃だった場合
        if (Input.GetKeyDown(KeyCode.X))
        {
            PSt = PlayerStutas.NoControl;
            ASt = AttackStutas.Spcial;
            spAtk = SpAtk.AtkUp;
        }

    }

    //追加攻撃入力に関しての関数
    public void AtkSupport()
    {
        if (ACnt == 0)
        {
            //追加攻撃入力受付時間
            AddInputTime = 0.5f;
            //関数全体の受付時間（終了すると自動でリセットされる）
            IntervalTime = 0.8f;
        }
        ++ACnt;
        AddInputTime -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X))
        {
            //再度、入力されたボタンによって攻撃を行う。
            AtkSearch();
            ACnt = 0;
            
        }
        if (IntervalTime <= 0.0f)
        {
            //状態を自動リセット
            ResetStutas();
        }
        IntervalTime -= Time.deltaTime;
    }

    //状態の自動リセットを行う関数
    public void ResetStutas()
    {
        this.tag = "Player";
        ACnt = 0;
        landingSwitch = false;
        Anim.SetInteger("Action", 0);
        PSt = PlayerStutas.Fine;
        ASt = AttackStutas.NoAttack;
        spAtk = SpAtk.SpNo;
    }

    //死亡処理
    void Dead()
        {
            //当たり判定を無効に
            CCol.enabled = false;
            Destroy(gameObject);
        }

    //プレイヤーの接地処理の関数
    bool IsGrounded()
    {
        bool ret = false;

        //Ray(光線)を地面に2.0f発射
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 6.0f, groundLayer);

        //ヒットしていないならnullを返す
        if (hit.collider != null)
        {
            ret = true;
        }
        
        return ret;
    }

    //【デバッグ用関数】
    //Rayを表示

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, new Vector3(0, -0.6f, 0));
    }

}
