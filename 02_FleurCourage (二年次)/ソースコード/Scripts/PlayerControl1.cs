using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl1 : MonoBehaviour
{
    //移動速度
    public float MoveSpeed;

    //ジャンプ力
    public float jumpPower;

    public float StopTime;

    public float StopTimer;

    //追加入力時間を格納する変数
    public float AddInputTime;

    //計測時間を格納する変数
    public float IntervalTime;

    public int ACnt;

    //地面として判断するオブジェクトのレイヤー
    public LayerMask groundLayer;

    //プレイヤーの状態
    public enum PlayerStutas
    {
        Fine,
        NoControl,
        Dead,
    }
    PlayerStutas PSt;

    public enum AttackStutas
    {
        NoAttack,
        Normal,
        Spcial,
    }
    AttackStutas ASt;

    //SpriteRendererを格納する変数
    public SpriteRenderer Sr;

    //RigidBodyを格納する変数
    public Rigidbody2D Rb;

    //Animatorを格納する変数
    public Animator Anim;

    //CapsuleCollider2Dを格納する変数
    public CapsuleCollider2D CCol;

    // Start is called before the first frame update
    void Start()
    {
        ACnt = 0;
        AddInputTime = 0.0f;
        IntervalTime = 0.0f;
        StopTimer = 0.0f;

        //Componentの取得
        Sr = GetComponent<SpriteRenderer>();
        Rb = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        CCol = GetComponent<CapsuleCollider2D>();

        //状態の設定
        PSt = PlayerStutas.Fine;

        ASt = AttackStutas.NoAttack;
    }

    // Update is called once per frame
    void Update()
    {
        switch(PSt)
        {
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

                    AtkSearch();

                    break;
                            
			    }

            case PlayerStutas.NoControl:
                {
                    switch (ASt)
                    {
                        case AttackStutas.Normal:
                            {
                                Debug.Log("通常攻撃");
                                this.tag = "PlayerAtk";
                                Anim.SetInteger("Action", 10);
                                AtkSupport();

                            }
                            break;
                        case AttackStutas.Spcial:
                            {
                                Debug.Log("特殊攻撃");
                                this.tag = "PlayerAtk";
                                Anim.SetInteger("Action", 20);
                                //StopTimer += Time.deltaTime;
                                //Vector2 power = (transform.forward + transform.right) * MoveSpeed;
                                //Rb.AddForce(power, ForceMode2D.Impulse);
                                //if (StopTimer >= StopTime)
                                //{
                                //    Anim.speed = 0.0f;
                                //    Rb.isKinematic = true;
                                //    Rb.velocity = new Vector3(0.0f, 2.0f, 0.0f);

                                //    float StopAir = 0.1f;
                                //    StopAir -= Time.deltaTime;
                                //    if (StopAir <= 0.0f )
                                //    {
                                //        Rb.velocity = Vector3.zero;
                                //        Rb.isKinematic = false;
                                //        Anim.SetInteger("Action", 21);
                                //        StopTimer = 0.0f;
                                //    }
                                    


                                //    StopTimer = 0.0f;
                                //}
                                

                                AtkSupport();
                            }
                            break;
                    }
                    break;
                }

            case PlayerStutas.Dead:
                {
                    Dead();
                    break;
                }
        }
    }

    //移動処理
    public void Move()
    {
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

    public void AtkSearch()
    {
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PSt = PlayerStutas.NoControl;
            ASt = AttackStutas.Normal;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            PSt = PlayerStutas.NoControl;
            ASt = AttackStutas.Spcial;
        }

    }

    public void AtkSupport()
    {
        if (ACnt == 0)
        {
            AddInputTime = 0.5f;
            IntervalTime = 0.8f;
        }
        ++ACnt;
        AddInputTime -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X))
        {
            AtkSearch();
            ACnt = 0;
        }
        if (IntervalTime <= 0.0f)
        {
            ResetStutas();
        }
        IntervalTime -= Time.deltaTime;
    }

    public void ResetStutas()
    {
        this.tag = "Player";
        ACnt = 0;
        Anim.SetInteger("Action", 0);
        PSt = PlayerStutas.Fine;
        ASt = AttackStutas.NoAttack;
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

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2.0f, groundLayer);

        //ヒットしていないならnullを返す
        if (hit.collider != null)
        {
            ret = true;
        }
        return ret;
    }

}
