using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddleBoss : MonoBehaviour
{
    //中ボスの体力
    public int Hp;

    public int HalfHp;

    //移動切り替え用の変数
    public int num;

    //プレイヤーの攻撃で吹っ飛ぶ反発力を入れる変数
    public int RepulsiveForce;

    //移動の速さ
    public float MoveSpeed;

    //次の行動までの待機時間
    public float WaiTime;

    //次の行動までの時間計測
    public float WaiTimer;

    //フェードアウトするまでの時間
    public float FadeTime;

    //フェードアウトする時間計測
    public float FadeTimer;

    //位置情報格納用の変数
    public Vector2 pos;

    //弾のプレハブを入れる変数
    public GameObject BulletPrefab;

    //爆発のプレハブを入れる変数
    public GameObject ExPrefab;

    //地面として判断するオブジェクトのレイヤー
    public LayerMask groundLayer;

    //中ボスの行動状態
    public enum MBoss_Status
    {
        //通常の時
        Fine, 
        //HPが半分以下の時
        Half,
        //HPがなくなった時
        Dead,
    }
    //MBoss_Stutasを変数Stで管理する。
    MBoss_Status St;

    public enum Atk_Stutas
	{
        //移動時
        Move,
        //攻撃０１の時
        Atk1,
        //攻撃０２の時
        Atk2,
        //大技攻撃の時
        SpAtk,
	}
    Atk_Stutas A_St;

    //SpriteRendererを格納する変数
    SpriteRenderer Sr;

    //RigidBody2Dを格納する変数
    Rigidbody2D Rb;

    // 初速度
    Vector3 initialVelocity;

    // Start is called before the first frame update
    void Start()
    {
        //変数の初期化
        WaiTimer = 0.0f;
        FadeTimer = 0.0f;
        num = 1;
        HalfHp = Hp / 2;

        
        // 発射角度（ここでは45度）
        float angle = 90f;
        // 初速度（ここでは10m/s）
        float speed = 10f;
        //水平方向の速度、垂直方向の速度
        //オブジェクトに対して、重力と放物運動による力を加えています。
        float vx = speed * Mathf.Cos(angle * Mathf.Deg2Rad);
        float vy = speed * Mathf.Sin(angle * Mathf.Deg2Rad);
        initialVelocity = new Vector3(vx, vy, 0f);

        //状態の初期化
        St = MBoss_Status.Fine;

        //Componentの取得
        Sr = GetComponent<SpriteRenderer>();
        Rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //中ボスの行動パターン
        switch (St)
        {
            //通常時
            case MBoss_Status.Fine:
                {

                    //移動処理
                    MBMove();

                    if (pos.x > 6.5f)
					{
                        //攻撃処理
                        MBAtk1();
                    }

                    break;
                }

            //Hpが半分以下の時
            case MBoss_Status.Half:
                {
                    //移動処理
                    MBMove();
                    break;
                }

            //Hpがなくなった時
            case MBoss_Status.Dead:
                {
                    //フェードアウトの時間計測
                    FadeTimer += Time.deltaTime;

                    //FadeTimeを超えるまでに徐々に透明にしていく
                    if (FadeTimer < FadeTime)
                    {
                        //α値を(FadeTimer / FadeTime)の分、差し引いた値にする。
                        //alpha : 変更後のα値の値
                        float alpha = 1.0f - FadeTimer / FadeTime;
                        //色情報の取得及びα値の変更
                        Color color = Sr.color;
                        color.a = alpha;
                        Sr.color = color;
                    }
                    //(FadeTimer > FadeTime)になった時、Dead()を発動する。
                    else
                    {
                        //爆発のアニメーションプレハブをインスタンス化
                        Instantiate(ExPrefab, this.transform.position, Quaternion.identity);
                        Dead();
                    }
                    break;
                }
        }
    }

    //移動の関数
    public void MBMove()
    {
        //位置の取得
        pos = transform.position;

        //★マイナスをかけることで逆方向に移動する → 変数numで行う
        //transform.right = ワールド空間のローカル（transform）x軸　・・・赤軸(MoveTool)
        //移動 =（ローカルx軸 * 経過時間 * 速度 * 変数num〈左右切り替え用〉）
        transform.Translate(transform.right * Time.deltaTime * MoveSpeed * num);

        //pos.xが6.5fより大きい位置にいた場合、左に移動転換させる(右 → 左)
        if (pos.x > 6.5f)
        {
            num = -1;
            Sr.flipX = false;
        }
        //pos.xが-6.5fより小さい位置にいた場合、右に移動転換させる(左 → 右)
        else if (pos.x < -6.5f)
        {
            num = 1;
            Sr.flipX = true;
        }
        
    }


    //通常攻撃１の関数
    public void MBAtk1()
    {
        if (num == 1)
		{
            
            
            Vector3 gravity = new Vector3(0f, 9.8f, 0f);  // 重力
            Rb.AddForce(gravity, ForceMode2D.Force);
            Rb.AddForce(initialVelocity, ForceMode2D.Impulse);
        }
        else if (num == -1)
		{
            Instantiate(BulletPrefab, new Vector3(pos.x + 1.0f, pos.y, transform.position.z), transform.rotation);
        } 
    }

    //通常攻撃２の関数
    public void MBAtk2()
    {

    }

    //特殊攻撃の関数
    public void SpAtk()
    {

    }

    //死亡した時の関数
    public void Dead()
    {
        //自身を破棄する。
        Destroy(gameObject);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("接触判定を行う");
        //プレイヤーから攻撃を受けた際のダメージ判定
        if (collision.gameObject.tag == "PlayerAtk")
        {
            Hp--;
            if (num == -1)
            {
                Rb.AddForce((this.transform.up + this.transform.right) * RepulsiveForce);
            }
            else
            {
                Rb.AddForce((this.transform.up + this.transform.right) * RepulsiveForce);
            }
            Debug.Log("接触した");

            if (Hp == HalfHp)
            {
                St = MBoss_Status.Half;
            }
            else if (Hp <= 0)
            {
                Hp = 0;
                St = MBoss_Status.Dead;
            }
        }
    }
    
}
