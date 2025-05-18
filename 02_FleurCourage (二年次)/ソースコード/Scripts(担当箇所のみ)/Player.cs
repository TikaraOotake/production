using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //変数宣言
    public GameObject SpritePrefab;//Spriteを表示するオブジェクトのプレハブ
    public GameObject AttackCollider1Prefab;//攻撃判定1のプレハブ
    public GameObject AttackCollider2Prefab;//攻撃判定2のプレハブ
    public GameObject AttackCollider3Prefab;//攻撃判定3のプレハブ
    public GameObject CloudPrefab;//雲のプレハブ
    public GameObject DandelionBulletPrefab;//タンポポの弾のプレハブ
    public GameObject ParryEffectPrefab;//パリィの演出のプレハブ
    public GameObject Sprite;//生成したスプライトオブジェクトを保存する
    public GameObject Cloud;//生成したオブジェクトを保存する
    public GameObject Camera;//カメラのオブジェクトを保存する
    public GameObject HPUI;//HPを表示するオブジェクト
    public GameObject SpecialBullet;//朝顔の特殊攻撃の弾

    //追加分
    //-----------------------------------------
    [SerializeField, Tooltip("通常攻撃の弾")]
    private GameObject[] BulletObj
       = new GameObject[3];            //通常攻撃の弾を格納する
    [SerializeField, Tooltip("特殊攻撃の弾")]
    private GameObject SpecialObj;      //特殊攻撃の弾を格納する

    public int BulletSelect;           //弾の選択
    [SerializeField, Tooltip("特殊攻撃の時間")]
    private float SpecialTime;          //特殊攻撃の時間
    private float _specialTimer;        //特殊攻撃のタイマー

    public float UncontrollableTimer = 0;
    public float CoolDownTimer = 0;

    public GameObject RoseAttack;
    //----------------------------------------



    public float Speed; //移動速度
    public int HP;//体力

    public float GravitySpeed; //落下速度
    public float GravityMaxSpeed; //落下最高速度
    public float DandelionGravityMaxSpeed;//タンポポ状態の落下最高速度
    public float GravityScale; //重力加速度

    public float JumpPower;//ジャンプ力
    public float JumpMomentum;//ジャンプ慣性
    public int JumpRemainCount;//ジャンプ回数
    public float JumpStartTimer;//ジャンプ発生時間

    public float StiffnessTime;//被弾硬直時間
    public float InvincibleTime;//無敵時間
    public float HitDamagePosX;//最後に受けたダメージのX座標を保存する

    public bool AddInputFlag;//追加入力をしたかどうかのフラグ
    public float AddInputInterval;//追加入力受付時間
    public float ActionIncapacityTime;//行動硬直時間
    public int RepeatCount;//繰り返し回数

    public int SelectSlot;//何番目のスロットを使っているかの変数
    public int SaveSelectSlot;//切り替える前に選んでたスロットを記憶しておく変数
    public float ParryCoolTime;//パリィのクールタイム

    //スロット毎に花のIDを保存する
    public int[] SlotID = new int[4];
    
    //スロット毎にHPを保存する
    public int[] SlotHP = new int[4];
    public int PlayerHP;

    public float StopTime;//停止時間
    public bool StopFlag;//停止するか

    public float oldPosX;//プレイヤーの過去の座標を入れる変数
    public float oldPosY;//プレイヤーの過去の座標を入れる変数

    SpriteRenderer _sr; //SpriteRendererの格納用
    Rigidbody2D _rb; //Rigidbody2Dの格納用
    Animator _animator; //Animatorの格納用

    public LayerMask GroundLayer; //地面として判定するオブジェクトのレイヤー

    enum Status
    {
        Fine, //通常
        Dead, //死亡
        Respawn, //復帰
        Clear, //クリア
        NoControll,//操作不能
        DamageStiffness,//被弾硬直
    }
    Status st;

    enum ActionStatus
    {
        NoAction,//何もしていない
        Parry,//パリィ
        Jump,//ジャンプ
        NormalAttack1,//通常攻撃一段
        NormalAttack2,//通常攻撃二段
        NormalAttack3,//通常攻撃三段
        NormalAttack,//通常攻撃
        SpecialAttack,//特殊攻撃
    }
    ActionStatus Ast;

    enum FlourSlot
    {
        NoFlour,//すっぴん
        Slot1,//スロット1
        Slot2,//スロット2
        Slot3,//スロット3
    }
    FlourSlot fs;





    // Start is called before the first frame update
    void Start()
    {
        //アタッチされているComponentを取得しておく
        _sr = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        //変数を初期化
        AddInputFlag = false;
        int initialHP = 6;//初期HP
        SlotHP[1] = initialHP;
        SlotHP[2] = initialHP;
        SlotHP[3] = initialHP;

        HPUI = GameObject.Find("HPtext");

        //Spriteを表示するオブジェクトをインスタンス化
        if (SpritePrefab != null)
        {
            GameObject childObject = Instantiate(SpritePrefab, transform);
            Sprite = childObject;
            Sprite.GetComponent<PlayerSprite>().UpdatePlayerObject(this.gameObject);
        }

        BulletSelect = 0;
    }

    // Update is called once per frame
    void Update()
    {
        UncontrollableTimer -= Time.deltaTime;
        CoolDownTimer -= Time.deltaTime;
        if (UncontrollableTimer < 0.0f)
        {
            UncontrollableTimer = 0.0f;
        }
        if(CoolDownTimer < 0.0f)
        {
            CoolDownTimer = 0.0f;
        }

        //Vector2 v = new Vector2();
        //v.x = 0.0f;
        // v.y = 0.0f;
        //_rb.velocity = v;

        //停止フラグがtrueならほぼ全ての処理を停止
        if (StopFlag)
        {
            //空中で静止させる
            //ReposeAir();
            ReposeAirX();

            //停止時間を経過させる
            StopTime -= Time.deltaTime;

            //停止時間が0以下のときフラグをfalseに
            if (StopTime <= 0)
            {
                StopFlag = false;
                StopTime = 0;
            }
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
                            AnimationSummary(SlotID[SelectSlot], 0);
                        }

                        


                        GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 0);

                        /*/テスト
                        if (Input.GetKeyDown(KeyCode.O))
                        {
                            ++HP;
                            HPUI.GetComponent<HPtext>().UpdateHP(HP);
                        }
                        if (Input.GetKey(KeyCode.I))
                        {
                            GetComponent<Renderer>().material.color = new Color32(0, 0, 0, 0);
                        }
                        if (Input.GetKey(KeyCode.U))
                        {
                            ReposeAir();
                        }
                        if (Input.GetKeyDown(KeyCode.R))
                        {
                            //スプライトを振動させる
                            Sprite.GetComponent<PlayerSprite>().UpdateVibStatus(0.1f, 1.08f, 0.3f, 0.3f, 500.0f);
                        }
                        if (Input.GetKeyDown(KeyCode.L))
                        {
                            if (Cloud == null)
                            {
                                Sprite.GetComponent<PlayerSprite>().UpdateAnimation(100);
                            }
                        }
                        if (Input.GetKeyDown(KeyCode.K))
                        {
                            if (Cloud == null)
                            {
                                Sprite.GetComponent<PlayerSprite>().UpdateAnimation(101);
                            }
                        }


                        if (Input.GetKey(KeyCode.Y))
                        {
                            if (Cloud == null)
                            {
                                Cloud = Instantiate(CloudPrefab, transform.position, Quaternion.identity);
                                Cloud.GetComponent<Cloud>().UpdateGameObject(this.gameObject);
                            }
                        }/*/


                        TestController();


                        //float h = Input.GetAxis("Horizontal");
                        //transform.Translate(h * 0.01f, 0.0f, 0.0f);


                        //選択中のスロットのHPを表示
                        HPUI.GetComponent<HPtext>().UpdateHP(SlotHP[SelectSlot]);

                        //プレイヤーの現在HPを表示
                        HPUI.GetComponent<HPtext>().UpdateHP(PlayerHP);

                        //無敵時間を経過させる
                        ElapsedInvincibleTime();

                        ActionProduce();
                        Move();
                        MaxGravitySpeed();
                        ChangeSlot();
                        TransitionGameOvere();
                        ResetJumpAirCount();

                        //空中にいる時はアニメーションを「ジャンプ」にする
                        if (IsGrounded() == false)
                        {
                            AirAnimation();
                        }
                    }
                    break;
            }
            switch (st)
            {
                //「操作不能」
                case Status.NoControll:
                    {
                        //HPUI.GetComponent<HPtext>().UpdateHP(SlotHP[SelectSlot]);没
                        HPUI.GetComponent<HPtext>().UpdateHP(PlayerHP);

                        //無敵時間を経過させる
                        ElapsedInvincibleTime();
                        //落下速度上限
                        MaxGravitySpeed();

                        TransitionGameOvere();


                        //行動
                        switch (Ast)
                        {
                            //ジャンプ
                            case ActionStatus.Jump:
                                {
                                    Jump();
                                }
                                break;

                            //パリィ
                            case ActionStatus.Parry:
                                {
                                    Parry();
                                }
                                break;
                            //通常攻撃
                            case ActionStatus.NormalAttack:
                                {
                                    switch (SelectSlot)
                                    {
                                        case 1:
                                            {
                                                NormalAttack(SlotID[1]);
                                            }
                                            break;
                                        case 2:
                                            {
                                                NormalAttack(SlotID[2]);
                                            }
                                            break;
                                        case 3:
                                            {
                                                NormalAttack(SlotID[3]);
                                            }
                                            break;
                                    }
                                }
                                break;
                            //特殊攻撃
                            case ActionStatus.SpecialAttack:
                                {
                                    switch (SelectSlot)
                                    {
                                        case 1:
                                            {
                                                SpecialAttack(SlotID[1]);
                                            }
                                            break;
                                        case 2:
                                            {
                                                SpecialAttack(SlotID[2]);
                                            }
                                            break;
                                        case 3:
                                            {
                                                SpecialAttack(SlotID[3]);
                                            }
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
            switch (st)
            {
                //「被弾硬直」
                case Status.DamageStiffness:
                    {
                        //HPUI.GetComponent<HPtext>().UpdateHP(SlotHP[SelectSlot]);没
                        HPUI.GetComponent<HPtext>().UpdateHP(PlayerHP);
                        MaxGravitySpeed();
                        DamageStiffnessFunction();

                        //アニメーションを被弾状態に
                        AnimationSummary(SlotID[SelectSlot], 2);

                        /*/点滅のプログラム（テスト）
                        コンポーネントを取得
                        var blinker = GetComponent<Blinker>();
                        // 点滅開始
                        blinker.BeginBlink();
                        //経過時間を計測
                        float damageTimer = 0;
                        damageTimer += Time.deltaTime;
                        //指定の時間を超えたら
                        if (damageTimer >= 1.5)
                        {
                            //点滅終了
                            blinker.EndBlink();
                            //タイマーをリセットする
                            damageTimer = 0;
                        }
                        /*/
                    }
                    break;
            }
            switch (st)
            {
                //「死亡」
                case Status.Dead:
                    {
                        //アニメーションを死亡状態に
                        AnimationSummary(0, 11);
                    }
                    break;
            }
            switch (st)
            {
                //「復帰」
                case Status.Respawn:
                    {

                    }
                    break;
            }
            switch (st)
            {
                //「クリア」
                case Status.Clear:
                    {

                    }
                    break;
            }
        }
        //パリィクールタイムを表示
        HPUI.GetComponent<HPtext>().UpdateCoolTime(ParryCoolTime);

        //タンポポなら浮遊処理をする
        DandelionFall();
        //アジサイ状態なら雲を召喚する
        //SummonCloud();

        //1F前のY座標を習得し続ける
        oldPosY = this.transform.localPosition.y;
        oldPosX = this.transform.localPosition.x;
        //クールタイムを経過させる
        ParryCoolTime -= Time.deltaTime;
        if (ParryCoolTime <= 0.0f)
        { ParryCoolTime = 0.0f; }

        CheckStatus();
    }

    /// <summary>
    ///関数名：move()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：プレイヤーの左右移動操作
    /// </summary>
    void Move()
    {
        if (UncontrollableTimer > 0.0f)
        {
            return;
        }
        //Aが押された時
        if (InputGetAction("LeftMove", "Stay"))
        {
            //移動
            transform.Translate(-Speed, 0.0f, 0.0f);
            //X軸に対して反転させる
            _sr.flipX = true;
            AnimationSummary(SlotID[SelectSlot], 1);
        }
        else
        //Dが押された時
        if (InputGetAction("RightMove", "Stay"))
        {
            //移動
            transform.Translate(Speed, 0.0f, 0.0f);
            //X軸に対して反転させる
            _sr.flipX = false;
            AnimationSummary(SlotID[SelectSlot], 1);
        }
        else 
        {
            //AnimationSummary(SlotID[SelectSlot], 0);
        }
    }
    /// <summary>
    ///関数名：AttackMove()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：前に少し移動できる
    /// </summary>
    void AttackMove()
    {
        if(UncontrollableTimer > 0.0f)
        {
            return;
        }
        //Aが押された時
        if (InputGetAction("LeftMove", "Stay") && _sr.flipX == true)
        {
            //移動
            transform.Translate(-Speed / 3, 0.0f, 0.0f);
        }
        //Dが押された時
        if (InputGetAction("RightMove", "Stay") && _sr.flipX == false)
        {
            //移動
            transform.Translate(Speed / 3, 0.0f, 0.0f);
        }
    }
    /// <summary>
    ///関数名：Jump()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：ジャンプ処理
    /// </summary>
    void Jump()
    {
        if (UncontrollableTimer > 0.0f)
        {
            return;
        }

        if (RepeatCount == 0)
        {
            //アニメーションをジャンプ待機状態に
            AnimationSummary(SlotID[SelectSlot], 6);
        }

        //transform.Translate(0.0f, jumpMomentum, 0.0f);
        if (RepeatCount >= 2)
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
            //gravitySpeed = -jumpPower;
            //jumpMomentum = jumpPower;
        }
        ++RepeatCount;

    }
    /// <summary>
    ///関数名：JumpAir()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：空中ジャンプ処理
    /// </summary>
    /*/void JumpAir()
    {
        //ジャンプが押された際に”空中”かつ残りジャンプ回数がある場合
        if (InputGetAction("Jump", "Down") && IsGrounded() == false && JumpRemainCount > 0)
        {
            //加速度を初期化
            Vector2 v = new Vector2();
            v.x = 0.0f;
            v.y = 0.0f;
            _rb.velocity = v;
            //上への力を加えてジャンプさせる
            _rb.AddForce(Vector2.up * JumpPower);

            //空中ジャンプ回数を減らす
            JumpRemainCount -= 1;
        }
    }/*/

    /// <summary>
    ///関数名：ResetJumpAirCount()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：空中ジャンプ回数リセット処理
    /// </summary>
    void ResetJumpAirCount()
    {
        //地上にいる場合
        if (IsGrounded())
        {
            JumpRemainCount = 1;
        }
    }



    /// <summary>
    ///関数名：GravityCalculation<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：重力の計算をする関数
    /// </summary>
    void GravityCalculation()
    {

        //落下最高速度を超えたら等速にする処理
        if (GravityMaxSpeed <= GravitySpeed)
        {
            GravitySpeed = GravityMaxSpeed;
        }

        //地面と接地していたら重力を0に
        if (IsGrounded() == true)
        {
            if (GravitySpeed > 0)
            {
                GravitySpeed = 0.0f;
            }

            //transform.Translate(0.0f, 0.01f, 0.0f);
        }
        else
        {
            //gravitySpeed += gravityScale;
        }
    }
    /// <summary>
    ///関数名：MaxGravitySpeed()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：落下速度上限超過を防ぐ関数
    /// </summary>
    void MaxGravitySpeed()
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
    ///関数名：Gravity()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：重力を発生させる関数
    /// </summary>
    void Gravity()
    {
        transform.Translate(0.0f, -GravitySpeed, 0.0f);
    }

    /// <summary>
    ///関数名：ReposeAir()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：呼び出している間Y座標を固定する関数
    /// </summary>
    /*/void ReposeAir()
    {
        //空中ならY座標を固定する
        if (IsGrounded() == false)
        {
            Vector2 v = _rb.velocity;
            v.y = 0.0f;
            _rb.velocity = v;
            transform.position = new Vector2(this.transform.localPosition.x, oldPosY);
        }
    }/*/
    /// <summary>
    ///関数名：ReposeAirX()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：呼び出している間X座標を固定する関数
    /// </summary>
    void ReposeAirX()
    {
        //空中ならX座標を固定する
        if (IsGrounded() == false)
        {
            Vector2 v = _rb.velocity;
            v.x = 0.0f;
            _rb.velocity = v;
            transform.position = new Vector2(oldPosX, this.transform.localPosition.y);
        }
    }

    /// <summary>
    ///関数名：IsGrounded() 	<br />
    ///引数名：なし		<br />
    ///戻り値：接地している場合はtrue,していない場合はfalse		<br />
    ///用　途：接地判定処理
    /// </summary>

    bool IsGrounded()
    {
        bool ret = false;
        //下方向にRayを飛ばして、指定したレイヤーのオブジェクトと接触しているかどうかを判別する
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.2f, GroundLayer);
        //ヒットしていない場合はnullが解される
        if (hit.collider != null)
        {
            ret = true;
        }
        return ret;
    }

    /// <summary>
    ///関数名：DamageBounce()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：被弾時の吹っ飛び処理
    /// </summary>
    void DamageBounce()
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
    ///関数名：DamageStiffnessFunction()<br />
    ///引数名：なし<br />
    ///戻り値：なし <br />
    ///用　途：被弾硬直時間処理
    /// </summary>
    void DamageStiffnessFunction()
    {
        if (RepeatCount == 0)
        {
            DamageBounce();
            
        }
        ++RepeatCount;
        //加速度をX軸のみN分の1に減少させる;
        Vector2 v = _rb.velocity;
        v.x = v.x / 1.1f;
        _rb.velocity = v;

        StiffnessTime -= Time.deltaTime;
        if (StiffnessTime <= 0)
        {
            //加速度をX軸のみ初期化
            Vector2 v2 = _rb.velocity;
            v2.x = 0.0f;
            _rb.velocity = v2;

            StiffnessTime = 0.0f;
            st = Status.Fine;
            //通常状態に戻す
            ResetStatus();
            //無敵時間分点滅させる
            Sprite.GetComponent<PlayerSprite>().UpdateFlicker(InvincibleTime);
        }
    }
    /// <summary>
    ///関数名：ElapsedInvincibleTime()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：無敵時間を経過させる処理
    /// </summary>
    void ElapsedInvincibleTime()
    {
        //無敵時間を経過させる
        if (InvincibleTime > 0)
        {
            InvincibleTime -= Time.deltaTime;
        }
        //無敵時間が0を下回ったら0にする
        if (InvincibleTime < 0)
        {
            InvincibleTime = 0;
        }
    }

    /// <summary>
    ///関数名：ChangeSlot()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：花の切り替え処理
    /// </summary>
    void ChangeSlot()
    {
        if (ParryCoolTime <= 0.0f)
        {
            if (InputGetAction("ChangeSlotR", "Down"))
            {
                SaveSelectSlot = SelectSlot;
                ++SelectSlot;
                SelectSlotLoop();
                if (SelectSlot == 1)
                {
                    //アニメーションを変更
                    AnimationSummary(0, 10);
                }
                else if (SelectSlot == 2)
                {
                    //アニメーションを変更
                    AnimationSummary(0, 12);
                }
                else if (SelectSlot == 3)
                {
                    //アニメーションを変更
                    AnimationSummary(0, 10);
                }
                /*/if(SelectSlot == 1)
                {
                    ChangeDandelion();
                }/*/
                /*/if (SlotHP[SelectSlot] <= 0)
                {
                    ++SelectSlot;
                    SelectSlotLoop();
                }
                if (SlotHP[SelectSlot] <= 0)
                {
                    ++SelectSlot;
                    SelectSlotLoop();
                }/*/
            }
            if (InputGetAction("ChangeSlotL", "Down"))
            {
                SaveSelectSlot = SelectSlot;
                --SelectSlot;
                SelectSlotLoop();
                if (SelectSlot == 1)
                {
                    //アニメーションを変更
                    AnimationSummary(0, 22);
                }
                else if (SelectSlot == 2)
                {
                    //アニメーションを変更
                    AnimationSummary(0, 10);
                }
                else if (SelectSlot == 3)
                {
                    //アニメーションを変更
                    AnimationSummary(0, 10);
                }
                /*/if (SlotHP[SelectSlot] <= 0)
                {
                    --SelectSlot;
                    SelectSlotLoop();
                }
                if (SlotHP[SelectSlot] <= 0)
                {
                    --SelectSlot;
                    SelectSlotLoop();
                }/*/
            }


            /*/HPが0になったら別のスロットに移行
            if (SlotHP[SelectSlot] <= 0)
            {
                ++SelectSlot;
                SelectSlotLoop();
                if (SlotHP[SelectSlot] <= 0)
                {
                    ++SelectSlot;
                    SelectSlotLoop();
                }
                if (SlotHP[SelectSlot] <= 0)
                {
                    ++SelectSlot;
                    SelectSlotLoop();
                }
            }/*/

            SelectSlotLoop();

            if (InputGetAction("ChangeSlotR", "Down") || InputGetAction("ChangeSlotL", "Down"))
            {
                Ast = ActionStatus.Parry;
                st = Status.NoControll;

                //アニメーションを変更
                AnimationSummary(SlotID[SaveSelectSlot], 10);
            }

        }

    }
    void Parry()
    {
        //ReposeAir();

        if (RepeatCount == 0)
        {
            
            

            //硬直時間を設定(全体フレーム)
            ActionIncapacityTime = 0.5f;
            //無敵時間を設定
            InvincibleTime = 0.5f;

            //Instantiate(ParryEffectPrefab, transform.position, Quaternion.identity);
        }

        

        if (RepeatCount == 10)
        {
            //アニメーションを変更
            AnimationSummary(SlotID[SelectSlot], 11);
        }
        
        ++RepeatCount;

            //    追加入力受付時間が0を下回ったら
            if (ActionIncapacityTime <= 0)
        {
            //パリィのクールタイムを設定
            ParryCoolTime = 2.0f;
            //特殊技のプログラムを終了し通常状態に戻す(※これがないと特殊技が終了されず動けなくなる)
            ResetStatus();
        }
        ActionIncapacityTime -= Time.deltaTime;
    }

    /*/void ChangeDandelion()
    {
        if (RepeatCount == 0)
        {
            //アニメーションを変更
            AnimationSummary(0, 12);
            //硬直時間を設定(全体フレーム)
            ActionIncapacityTime = 0.5f;
            //無敵時間を設定
            InvincibleTime = 0.5f;
            //パリィのクールタイムを設定
            ParryCoolTime = 2.0f;
            //特殊技のプログラムを終了し通常状態に戻す(※これがないと特殊技が終了されず動けなくなる)
            ResetStatus();
        }
    }/*/

    void TransitionGameOvere()
    {
        //全てのスロットのHPが0になったら死亡状態へ  没
        //if (SlotHP[1] <= 0 && SlotHP[2] <= 0 && SlotHP[3] <= 0)

        //HPが0になったら死亡状態へ
        if (PlayerHP <= 0)
        {
            ResetStatus();
            st = Status.Dead;
        }

    }

    /// <summary>
    ///関数名：SelectSlotLoop()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：SelectSlotのループ処理
    /// </summary>
    void SelectSlotLoop()
    {
        //ループさせる
        if (SelectSlot <= 0)
        {
            SelectSlot += 3;
        }
        if (SelectSlot >= 4)
        {
            SelectSlot -= 3;
        }
    }

    /// <summary>
    ///関数名：ActionProduce()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：各行動へ移行するプログラムを総括する関数
    /// </summary>
    void ActionProduce()
    {
        //ジャンプ
        if(InputGetAction("Jump", "Down") && IsGrounded())
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
            Ast = ActionStatus.SpecialAttack;
            st = Status.NoControll;
        }
    }

    /// <summary>
    ///関数名：NormalAttack()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：通常攻撃全体を動かす関数
    /// </summary>
    void NormalAttack(int ID)
    {
        switch (ID)
        {
            //-----------------------------------------------------------
            //
            //if (RepeatCount == 0)
            //{
            //    //硬直時間と追加入力受付時間を設定(二つの合計が全体フレーム)
            //    ActionIncapacityTime = 0.5f;  硬直時間
            //    AddInputInterval = 0.5f;　　　追加入力受付時間
            //
            //    (最初に変数などを初期化したい場合もここに)
            //}++RepeatCount;


            //                  ↓Nに好きな値を入力することでNフレーム経過後一回だけプログラムを動かせる
            //if (RepeatCount == N) 
            //{
            //    //例　攻撃判定のプレハブをインスタンス化
            //    Instantiate(プレイヤーの弾のPrefab, transform.position, Quaternion.identity);
            //}


            //    硬直時間が0を下回ったら
            //if (ActionIncapacityTime <= 0)
            //{
            //　　追加入力受付時間を進ませる
            //    AddInputInterval -= Time.deltaTime;
            //
            //        特定のボタンが押されたら追加の処理を行う(例一段目の攻撃から二段目に移る)
            //    if (Input.GetKey(KeyCode.???))
            //    {
            //        
            //    }
            //}


            //    追加入力受付時間が0を下回ったら
            //if (AddInputInterval <= 0)
            //{
            //    特殊技のプログラムを終了し通常状態に戻す(※これがないと特殊技が終了されず動けなくなる)
            //    ResetStatus();
            //}
            //ActionIncapacityTime -= Time.deltaTime;
            //
            //-----------------------------------------------------------

            //「バラ」
            case 1:
                {
                    Debug.Log("バラ通常攻撃");
                    AnimationSummary(SlotID[SelectSlot], 8);

                    //Ast = ActionStatus.NoAction;
                    //st = Status.Fine;
                    //this.tag = "PlayerAtk";

                    Instantiate(RoseAttack, new Vector3(transform.position.x,transform.position.y,0.0f), Quaternion.identity);
                    AtkSupport();
                }
                break;
            //「タンポポ」
            case 2:
                {
                    if (RepeatCount == 0)
                    {
                        Debug.Log("タンポポ通常攻撃");
                        AnimationSummary(SlotID[SelectSlot], 8);
                      
                        //硬直時間と追加入力受付時間を設定(二つの合計が全体フレーム)
                        ActionIncapacityTime = 0.3f; //硬直時間
                        AddInputInterval = 0.5f; //追加入力受付時間
                    }
                    ++RepeatCount;
                    //硬直時間が0を下回ったら
                    if (ActionIncapacityTime <= 0)
                    {
                        //追加入力受付時間を進ませる
                        AddInputInterval -= Time.deltaTime;

                        //特定のボタンが押されたら追加の処理を行う
                        if (InputGetAction("NormalAttack", "Down"))
                        {
                            //タンポポ攻撃を最初から
                            RepeatCount = 0;
                        }
                    }
                    //追加入力受付時間が0を下回ったら
                    if (AddInputInterval <= 0)
                    {
                        //特殊技のプログラムを終了し通常状態に戻す(※これがないと特殊技が終了されず動けなくなる)
                        ResetStatus();
                    }
                    ActionIncapacityTime -= Time.deltaTime;
                }
                break;
            //「アサガオ」
            case 4:
                {
                    if (RepeatCount == 0)
                    {
                        Debug.Log("アサガオ通常攻撃"); 

                        //硬直時間と追加入力受付時間を設定(二つの合計が全体フレーム)
                        ActionIncapacityTime = 0.3f; //硬直時間
                        AddInputInterval = 0.5f; //追加入力受付時間
                    }
                    ++RepeatCount;

                    MorningGloryAttack();

                    //硬直時間が0を下回ったら
                    if (ActionIncapacityTime <= 0)
                    {
                        //追加入力受付時間を進ませる
                        AddInputInterval -= Time.deltaTime;

                        //特定のボタンが押されたら追加の処理を行う
                        if (InputGetAction("NormalAttack", "Down"))
                        {
                            //アサガオ攻撃を最初から
                            RepeatCount = 0;
                        }
                    }
                    
                    //通常状態に戻す
                    ResetStatus();
                }
                break;
        }

        //没
        //switch (Ast)
        //{
        //    //「通常攻撃一段」
        //    case ActionStatus.NormalAttack1:
        //        {
        //            AttackMove();
        //            NormalAttack1();
        //        }
        //        break;
        //    //「通常攻撃二段」
        //    case ActionStatus.NormalAttack2:
        //        {
        //            AttackMove();
        //            NormalAttack2();
        //        }
        //        break;
        //    //「通常攻撃三段」
        //    case ActionStatus.NormalAttack3:
        //        {
        //            AttackMove();
        //            NormalAttack3();
        //        }
        //        break;
        //}
    }

    /*/
    /// <summary>
    ///関数名：NormalAttack1()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：通常攻撃一段目の関数
    /// </summary>
    void NormalAttack1()
    {
        //ReposeAir();
        if (RepeatCount == 0)
        {
            //アニメーションを通常攻撃1に
            AnimationSummary(SlotID[SelectSlot], 11);
            //攻撃判定のプレハブをインスタンス化
            Instantiate(AttackCollider1Prefab, transform.position, Quaternion.identity);
            //硬直時間と追加入力受付時間を設定(二つの合計が全体フレーム)
            ActionIncapacityTime = 0.4f;
            AddInputInterval = 0.5f;
            //フラグを初期化
            AddInputFlag = false;
        }
        ++RepeatCount;

        if (ActionIncapacityTime <= 0)
        {
            if (InputGetAction("NormalAttack", "Down"))
            {
                AddInputFlag = true;
                //繰り返し回数を初期化
                RepeatCount = 0;
                Ast = ActionStatus.NormalAttack2;
            }
            //ジャンプキャンセル処理
            if (InputGetAction("Jump", "Down"))
            {
                //通常状態に戻す
                Jump();
                ResetStatus();
            }
                AddInputInterval -= Time.deltaTime;
        }
        if (AddInputInterval <= 0)
        {
            //通常状態に戻す
            ResetStatus();
        }

        ActionIncapacityTime -= Time.deltaTime;
    }

    /// <summary>
    ///関数名：NormalAttack2()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：通常攻撃二段目の関数
    /// </summary>
    void NormalAttack2()
    {
        //ReposeAir();
        if (RepeatCount == 0)
        {
            //アニメーションを通常攻撃2に
            AnimationSummary(SlotID[SelectSlot], 12);
            //攻撃判定のプレハブをインスタンス化
            Instantiate(AttackCollider2Prefab, transform.position, Quaternion.identity);
            //硬直時間と追加入力受付時間を設定(二つの合計が全体フレーム)
            ActionIncapacityTime = 0.4f;
            AddInputInterval = 0.5f;
            //フラグを初期化
            AddInputFlag = false;
        }
        ++RepeatCount;

        if (ActionIncapacityTime <= 0)
        {
            if (InputGetAction("NormalAttack", "Down"))
            {
                AddInputFlag = true;
                //繰り返し回数を初期化
                RepeatCount = 0;
                Ast = ActionStatus.NormalAttack3;
            }
            //ジャンプキャンセル処理
            if (InputGetAction("Jump", "Down"))
            {
                //通常状態に戻す
                Jump();
                ResetStatus();
            }
            AddInputInterval -= Time.deltaTime;
        }
        if (AddInputInterval <= 0)
        {
            //通常状態に戻す
            ResetStatus();
        }

        ActionIncapacityTime -= Time.deltaTime;
    }

    /// <summary>
    ///関数名：NormalAttack3()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：通常攻撃三段目の関数
    /// </summary>
    void NormalAttack3()
    {
        //ReposeAir();
        if (RepeatCount == 0)
        {
            //アニメーションを通常攻撃3に
            AnimationSummary(SlotID[SelectSlot], 13);
            //攻撃判定のプレハブをインスタンス化
            Instantiate(AttackCollider3Prefab, transform.position, Quaternion.identity);
            //硬直時間と追加入力受付時間を設定(二つの合計が全体フレーム)
            ActionIncapacityTime = 0.5f;
            AddInputInterval = 0.5f;
            //フラグを初期化
            AddInputFlag = false;
        }
        ++RepeatCount;

        if (ActionIncapacityTime <= 0)
        {
            //ジャンプキャンセル処理
            if (InputGetAction("Jump", "Down"))
            {
                //通常状態に戻す
                Jump();
                ResetStatus();
            }
            AddInputInterval -= Time.deltaTime;
        }
        if (AddInputInterval <= 0)
        {
            //通常状態に戻す
            ResetStatus();
        }

        ActionIncapacityTime -= Time.deltaTime;
    }
    /*/

    /// <summary>
    ///関数名：SpecialAttack()<br />
    ///引数名：ID	<br />
    ///戻り値：なし <br />
    ///用　途：それぞれの特殊攻撃がまとめられている関数
    /// </summary>
    void SpecialAttack(int ID)
    {
        switch (ID)
        {
            //-----------------------------------------------------------
            //
            //if (RepeatCount == 0)
            //{
            //    //硬直時間と追加入力受付時間を設定(二つの合計が全体フレーム)
            //    ActionIncapacityTime = 0.5f;  硬直時間
            //    AddInputInterval = 0.5f;　　　追加入力受付時間
            //
            //    (最初に変数などを初期化したい場合もここに)
            //}++RepeatCount;


            //                  ↓Nに好きな値を入力することでNフレーム経過後一回だけプログラムを動かせる
            //if (RepeatCount == N) 
            //{
            //    //例　攻撃判定のプレハブをインスタンス化
            //    Instantiate(プレイヤーの弾のPrefab, transform.position, Quaternion.identity);
            //}


            //    硬直時間が0を下回ったら
            //if (ActionIncapacityTime <= 0)
            //{
            //　　追加入力受付時間を進ませる
            //    AddInputInterval -= Time.deltaTime;
            //
            //        特定のボタンが押されたら追加の処理を行う(例一段目の攻撃から二段目に移る)
            //    if (Input.GetKey(KeyCode.???))
            //    {
            //        
            //    }
            //}


            //    追加入力受付時間が0を下回ったら
            //if (AddInputInterval <= 0)
            //{
            //    特殊技のプログラムを終了し通常状態に戻す(※これがないと特殊技が終了されず動けなくなる)
            //    ResetStatus();
            //}
            //ActionIncapacityTime -= Time.deltaTime;
            //
            //-----------------------------------------------------------

            //「バラ」
            case 1:
                {
                    Debug.Log("バラ特殊攻撃");
                    Ast = ActionStatus.NoAction;
                    st = Status.Fine;
                }
                break;
            //「タンポポ」
            case 2:
                {
                    if (RepeatCount == 0)
                    {
                        if (CoolDownTimer > 0.0f)
                        {
                            Debug.Log("クールタイム中");
                            ResetStatus();
                            break;
                        }
                        Debug.Log("タンポポ特殊攻撃");
                        _rb.AddForce(Vector2.up * (JumpPower/2));

                        //5～9個の弾をインスタンス化
                        for (int i = 0; i < Random.Range(5, 9); ++i)
                        {
                            //タンポポの弾をインスタンス化
                            GameObject DandelionBullet = Instantiate(DandelionBulletPrefab, transform.position, Quaternion.identity);
                            if (DandelionBullet != null)
                            {
                                DandelionBullet.GetComponent<DandelionBullet>().UpdateWey(_sr.flipX);
                                DandelionBullet = null;
                            }
                        }

                        //アニメーションを特殊攻撃に
                        AnimationSummary(SlotID[SelectSlot], 20);
                        CoolDownTimer = 1.5f;

                        //硬直時間と追加入力受付時間を設定(二つの合計が全体フレーム)
                        ActionIncapacityTime = 0.3f; //硬直時間
                        AddInputInterval = 0.5f; //追加入力受付時間
                    }
                    ++RepeatCount;
                    //硬直時間が0を下回ったら
                    if (ActionIncapacityTime <= 0)
                    {
                        //追加入力受付時間を進ませる
                        AddInputInterval -= Time.deltaTime;

                        //特定のボタンが押されたら追加の処理を行う
                        if (InputGetAction("SpecialAttack", "Down"))
                        {
                            //タンポポ攻撃を最初から
                            RepeatCount = 0;
                        }
                    }
                    //追加入力受付時間が0を下回ったら
                    if (AddInputInterval <= 0)
                    {
                        //特殊技のプログラムを終了し通常状態に戻す(※これがないと特殊技が終了されず動けなくなる)
                        ResetStatus();
                    }
                    ActionIncapacityTime -= Time.deltaTime;
                    
                    
                }
                break;

            //「アサガオ」
            case 4:
                {
                    Debug.Log("アサガオ特殊攻撃");

                    //特殊攻撃中じゃなかったら処理
                    //if (_specialFlag)
                    //{

                    //硬直時間を設定(全体フレーム)
                    ActionIncapacityTime = 2.0f;
                    //無敵時間を設定
                    InvincibleTime = 2.0f;
                    //地面にRayが当たっていたら処理する
                    if (IsGrounded())
                        {
                            //特殊攻撃
                            SpecialAtk();
                        }
                    //}
                    //特殊攻撃中だったら処理
                    //else
                    //{

                    //タイマーを加算
                    _specialTimer += Time.deltaTime;
                        //一定時間たったら処理
                        if (_specialTimer >= SpecialTime)
                        {
                            //タイマーを初期化
                            _specialTimer = 0.0f;

                            //無敵状態を解除
                            gameObject.tag = "Player";
                        }
                    //}
                    //通常状態に戻す
                    ResetStatus();
                }
                break;
        }
    }

    //追加入力時間を格納する変数
    public float AddInputTime;

    //計測時間を格納する変数
    public float IntervalTime;

    public int ACnt;

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
            ACnt = 0;
        }
        if (IntervalTime <= 0.0f)
        {
            ResetStutas();
        }
        IntervalTime -= Time.deltaTime;
    }
    
    

   

    /// <summary>
    /// 関数名：MorningGloryAttack  <br />
    /// 引　数：無し  <br />
    /// 戻り値：無し  <br />
    /// 通常攻撃の関数
    /// </summary>
    void MorningGloryAttack()
    {
        //Xを押したら処理
        //if (Input.GetKeyDown(KeyCode.X))
        {
            for (int i = 0; i < 3; i++)
            {
                if (BulletSelect == i)
                {
                    //インスタンス化する場所
                    Vector3 _instanceBulletPos;
                    _instanceBulletPos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);

                    //インスタンス化
                    Instantiate(BulletObj[i], _instanceBulletPos, Quaternion.identity);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 関数名：SpecialAtk
    /// 引　数：無し
    /// 戻り値：無し
    /// アサガオの特殊攻撃の関数
    /// </summary>
    void SpecialAtk()
    {
        UncontrollableTimer = 1.5f;
        //無敵
        gameObject.tag = "Invincible";

        //衝撃波
        Instantiate(SpecialObj, transform.position, Quaternion.identity);
    }

    public void ResetStutas()
     {
           this.tag = "Player";
           ACnt = 0;
           st = Status.Fine;
        Ast = ActionStatus.NoAction;
     }

    /// <summary>
    ///関数名：DandelionFall()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：タンポポの浮遊処理
    /// </summary>
    void DandelionFall()
    {
        //タンポポ状態なら浮遊処理を行う
        if (SlotID[SelectSlot]==2)
        {
            //ジャンプが押された際空中であるなら
            if(InputGetAction("Jump", "Stay") && IsGrounded() == false)
            {
                if (_rb.velocity.y < -DandelionGravityMaxSpeed)
                {
                    //現在の加速を一時的に保存する
                    Vector2 v = _rb.velocity;
                    v.y = -DandelionGravityMaxSpeed;
                    _rb.velocity = v;
                }
            }
        } 
    }

    /// <summary>
    ///関数名：InputGetAction()<br />
    ///引数名：InputActionType(データ型string) Timing"Down","Stay","Up"(データ型string)<br />
    ///戻り値：bool <br />
    ///用　途：行動ごとに入力を総括する関数　<br />
    /// </summary>
    bool InputGetAction(string InputActionType, string Timing)
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
                        if (Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.DownArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Stay")
                    {
                        if (Input.GetKey(KeyCode.Joystick1Button1) || Input.GetKey(KeyCode.DownArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Up")
                    {
                        if (Input.GetKeyUp(KeyCode.Joystick1Button1) || Input.GetKeyUp(KeyCode.DownArrow))
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
                        if (Input.GetKeyDown(KeyCode.Joystick1Button4) || Input.GetKeyDown(KeyCode.RightArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Stay")
                    {
                        if (Input.GetKey(KeyCode.Joystick1Button4) || Input.GetKey(KeyCode.RightArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Up")
                    {
                        if (Input.GetKeyUp(KeyCode.Joystick1Button4) || Input.GetKeyUp(KeyCode.RightArrow))
                        { Signal = true; }
                    }
                }
                break;
            case "ChangeSlotL":
                {
                    if (Timing == "Down")
                    {
                        if (Input.GetKeyDown(KeyCode.Joystick1Button5) || Input.GetKeyDown(KeyCode.LeftArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Stay")
                    {
                        if (Input.GetKey(KeyCode.Joystick1Button5) || Input.GetKey(KeyCode.LeftArrow))
                        { Signal = true; }
                    }
                    if (Timing == "Up")
                    {
                        if (Input.GetKeyUp(KeyCode.Joystick1Button5) || Input.GetKeyUp(KeyCode.LeftArrow))
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
                        if (h == 1)
                        { Signal = true; }
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
                        if (h == -1)
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
        
        return Signal;
    }

    void AirAnimation()
    {
        Vector2 v = _rb.velocity;
        //上昇
        if (v.y > 5.0f)
        {
            AnimationSummary(SlotID[SelectSlot], 3);
        }
        else
        //下降
        if (v.y < -5.0f)
        {
            AnimationSummary(SlotID[SelectSlot], 5);
        }
        //滞空
        else
        {
            AnimationSummary(SlotID[SelectSlot], 4);
        }
    }

    /// <summary>
    ///関数名：AnimationSummary()<br />
    ///引数名：FlourID ActionType<br />
    ///戻り値：なし <br />
    ///用　途：アニメーションを総括する関数　<br />
    /// </summary>
    void AnimationSummary(int FlourID, int ActionType)
    {
        int PlayAnimation = 0;
        switch (FlourID)
        {
            
            case 0:
                {
                    PlayAnimation += 000 + ActionType;
                }
                break;
            //バラ
            case 1:
                {
                    PlayAnimation += 100 + ActionType;
                }
                break;
            case 2:
                {
                    PlayAnimation += 200 + ActionType;
                }
                break;
            //アサガオ
            case 3:
                {
                    PlayAnimation = +300 + ActionType;
                }
                break;
            case 4:
                {
                    PlayAnimation = +400 + ActionType;
                }
                break;
        }


        if (Sprite != null)
        {
            Sprite.GetComponent<PlayerSprite>().UpdateAnimation(PlayAnimation);
            Sprite.GetComponent<PlayerSprite>().UpdatePlayerWey(_sr.flipX);
        }

        _animator.SetInteger("Action", PlayAnimation);
    }

    /// <summary>
    ///関数名：ResetInterval()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：各時間間隔系列の変数を初期化(無敵、被弾時間以外)
    /// </summary>
    void ResetInterval()
    {
        //繰り返し回数を初期化
        RepeatCount = 0;
    }

    /// <summary>
    ///関数名：ResetStatus()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：移動やジャンプができる状態に戻す処理
    /// </summary>
    void ResetStatus()
    {
        //アニメーションを待機状態に
        AnimationSummary(SlotID[SelectSlot], 0);

        ResetInterval();
        Ast = ActionStatus.NoAction;
        st = Status.Fine;
        _rb.bodyType = RigidbodyType2D.Dynamic;
    }

    /// <summary>
    ///関数名：VibrationCamera()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：カメラを揺らす関数を呼び出す
    /// </summary>
    void VibrationCamera()
    {
        if (Camera != null)
        {
            Camera.GetComponent<NewCameraMove>().VibrationCamera(0.2f, 0.2f);
        }
    }

    /// <summary>
    ///関数名：GetFindObject()<br />
    ///引数名：探したいオブジェクトの名前	<br />
    ///戻り値：探したオブジェクト <br />
    ///用　途：名前からオブジェクトを探し取得の成功失敗を確認する
    /// </summary>
    Object GetFindObject(string ObjectName)
    {
        GameObject Object= GameObject.Find(ObjectName);
        if (Object == null)
        {
            Debug.Log(ObjectName+"は存在しません");
        }
        else
        {
            Debug.Log(ObjectName + "のオブジェクト取得に成功");
        }
        return Object;
    }

    /// <summary>
    ///関数名：OnCollisionEnter2D()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：当たり判定
    /// </summary>
    void OnCollisionStay2D(Collision2D collision)
    {
        //無敵時間でないなら
        if (InvincibleTime <= 0)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                
                //カメラを揺らす
                VibrationCamera();
                //スプライトを振動させる
                Sprite.GetComponent<PlayerSprite>().UpdateVibStatus(0.1f, 1.2f, 0.3f, 0.3f, 500.0f);

                //硬直時間を設定
                StiffnessTime = 0.5f;
                //無敵時間を設定
                InvincibleTime = 1.0f;
                //各時間間隔系列の変数を初期化
                ResetInterval();

                //停止フラグと時間を設定(ヒットストップ)
                StopFlag = true;
                StopTime = 0.1f;

                //HPを1減らす
                //--SlotHP[SelectSlot];
                PlayerHP -= 1;


                //接触対象の座標を取得
                Vector3 tmp = GameObject.Find(collision.gameObject.name).transform.position;

                Debug.Log("衝突相手の名前は、" + collision.gameObject.name);
                Debug.Log(tmp.x);

                HitDamagePosX = tmp.x;
                st = Status.DamageStiffness;
            }
            if (collision.gameObject.tag == "EnemyAttack")
            {
                //アニメーションを被弾状態に
                AnimationSummary(SlotID[SelectSlot], 2);
                //カメラを揺らす
                VibrationCamera();
                //スプライトを振動させる
                Sprite.GetComponent<PlayerSprite>().UpdateVibStatus(0.1f, 1.2f, 0.3f, 0.3f, 500.0f);

                //硬直時間を設定
                StiffnessTime = 0.5f;
                //無敵時間を設定
                InvincibleTime = 1.0f;
                //各時間間隔系列の変数を初期化
                ResetInterval();

                //停止フラグと時間を設定(ヒットストップ)
                StopFlag = true;
                StopTime = 0.1f;


                //HPを2減らす
                //SlotHP[SelectSlot]-=2;
                PlayerHP -= 2;


                //接触対象の座標を取得
                Vector3 tmp = GameObject.Find(collision.gameObject.name).transform.position;

                Debug.Log("衝突相手の名前は、" + collision.gameObject.name);
                Debug.Log(tmp.x);


                HitDamagePosX = tmp.x;
                st = Status.DamageStiffness;
            }
        }

    }
    void TestController()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            Debug.Log("Joystick1Button0");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            Debug.Log("Joystick1Button1");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button2))
        {
            Debug.Log("Joystick1Button2");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button3))
        {
            Debug.Log("Joystick1Button3");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button4))
        {
            Debug.Log("Joystick1Button4");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button5))
        {
            Debug.Log("Joystick1Button5");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button6))
        {
            Debug.Log("Joystick1Button6");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button7))
        {
            Debug.Log("Joystick1Button7");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button8))
        {
            Debug.Log("Joystick1Button8");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button9))
        {
            Debug.Log("Joystick1Button9");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button10))
        {
            Debug.Log("Joystick1Button10");
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button11))
        {
            Debug.Log("Joystick1Button11");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button12))
        {
            Debug.Log("Joystick1Button12");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button13))
        {
            Debug.Log("Joystick1Button13");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button14))
        {
            Debug.Log("Joystick1Button14");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button15))
        {
            Debug.Log("Joystick1Button15");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button16))
        {
            Debug.Log("Joystick1Button16");
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button17))
        {
            Debug.Log("Joystick1Button17");
        }
    }
    void CheckStatus()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            switch (st)
            {
                case Status.Fine:
                    { Debug.Log("通常時"); }
                    break;
                case Status.Dead:
                    { Debug.Log("死亡"); }
                    break;
                case Status.Respawn:
                    { Debug.Log("復帰"); }
                    break;
                case Status.Clear:
                    { Debug.Log("クリア"); }
                    break;
                case Status.NoControll:
                    { Debug.Log("操作不能"); }
                    break;
                case Status.DamageStiffness:
                    { Debug.Log("被弾硬直"); }
                    break;
            }
            switch (Ast)
            {
                //パリィ
                case ActionStatus.Parry:
                    {
                        //Parry();
                    }
                    break;
                //ジャンプ
                case ActionStatus.Jump:
                    { Debug.Log("ジャンプ"); }
                    break;
                //ノーアクション
                case ActionStatus.NoAction:
                    { Debug.Log("ノーアクション"); }
                    break;
            }

        }

    }
    //【デバッグ用関数】
    //Rayを表示
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, new Vector3(0, -1.2f, 0));
    }
}