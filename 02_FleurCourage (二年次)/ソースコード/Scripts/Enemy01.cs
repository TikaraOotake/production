using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01 : MonoBehaviour
{
    public GameObject SpritePrefab;//Spriteを表示するオブジェクトのプレハブ
    public GameObject Sprite;//生成したスプライトオブジェクトを保存する

    public GameObject EnemyGroundPrefab;//EnemyGroundオブジェクトのプレハブ
    public GameObject EnemyGround;//生成したEnemyGroundオブジェクトを保存する

    public bool GroundOnFlag;//上にプレイヤーがいるかどうかのフラグ

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
        Move,//移動
        Attack,//攻撃
        Jump,//ジャンプ
    }
    ActionStatus Ast;

    // Start is called before the first frame update
    void Start()
    {
        //EnemyGroundオブジェクトをインスタンス化
        if (EnemyGroundPrefab != null)
        {
            EnemyGround = Instantiate(EnemyGroundPrefab, transform.position, Quaternion.identity);
            EnemyGround.GetComponent<Enemy01Ground>().UpdateParentObject(this.gameObject);
        }
        //Spriteを表示するオブジェクトをインスタンス化
        if (SpritePrefab != null)
        {
            GameObject childObject = Instantiate(SpritePrefab, transform);
            Sprite = childObject;
            Sprite.GetComponent<Enemy01Split>().UpdatePlayerObject(this.gameObject);
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        //自身のオブジェクトを透明に
        GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 0);

        switch (st)
        {
            //「通常」
            case Status.Fine:
                {
                    if(GroundOnFlag)
                    {
                        //スプライトを振動させる
                        if (Sprite != null)
                        { Sprite.GetComponent<Enemy01Split>().UpdateVibStatus(0.1f, 1.2f, 0.1f, 0.0f, 300.0f); }
                        Debug.Log("aaaadefeg");
                    }
                }
                break;
        }
        switch (st)
        {
            //「死亡」
            case Status.Dead:
                {

                }
                break;
        }
    }

    /// <summary>
    ///関数名：UpdateObject()<br />
    ///引数名：プレイヤーのオブジェクト<br />
    ///戻り値：なし <br />
    ///用　途：プレイヤーのオブジェクトを取得する　<br />
    /// </summary>
    public void UpdateGroundOnFlag(bool Flag)
    {
        GroundOnFlag = Flag;
    }
}
