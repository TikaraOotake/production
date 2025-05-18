using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject Player; //現在のプレイヤーのオブジェクト

    public GameObject PlayerRosePrefab;//バラのプレハブ
    public GameObject PlayerDandelionPrefab;//タンポポのプレハブ
    public GameObject PlayerMorninggloryPrefab;//アサガオのプレハブ

    public GameObject HeartUI_0;//HeartのUI
    public GameObject HeartUI_1;//HeartのUI
    public GameObject HeartUI_2;//HeartのUI

    public GameObject HPtextObject;

    public GameObject UI_EP_Bar;
    public GameObject UI_DressIcon;


    public int EP_Max;//EPの最大値
    public int EP_Remain;//EPの残量

    public float CT_Max;//CoolTimerの最大値
    public float CT_Remain;//CoolTimerの残量

    //スロット毎に花のIDを保存する
    public int[] SlotID = new int[4];
　　
    public int SelectSlot;//何番目のスロットを使っているかの変数

    public int PlayerHP;//プレイヤーのHP
    public int PlayerHP_Old;//プレイヤーの古いHP
    public Vector2 PlayerPos;//プレイヤーの座標

    public Vector2 ReturnPos;//復帰座標を記録

    public float ScreenOutRange;//画面外の距離
    public bool AddjustStayScreenF;//画面内にいるように調整するかのフラグ

    // Start is called before the first frame update
    void Start()
    {
        //チェックポイントが設定されていたらそこに飛ばす
        //if(0 != this.gameObject.GetComponent<GameData>().GetCheckPointNum()) 
        {
            //Player.transform.position = this.gameObject.GetComponent<GameData>().GetReturnPos();
        }

        Player = this.gameObject.GetComponent<NewCameraMove>().GetPlayerObject();

    }

    // Update is called once per frame
    void Update()
    {
        //テスト
        if(Input.GetKeyDown(KeyCode.B))
        {
            this.gameObject.GetComponent<GameData>().SetCheckPointNum(1);
            this.gameObject.GetComponent<GameData>().SetReturnPos(Player.transform.position);
        }


        //PlayerのHPに変化があった場合HP_UIの表示を切り替える
        if(PlayerHP_Old != PlayerHP)
        {
            if (HeartUI_0 != null) { HeartUI_0.GetComponent<Heart_UI>().UpdateHP(PlayerHP); }
            if (HeartUI_1 != null) { HeartUI_1.GetComponent<Heart_UI>().UpdateHP(PlayerHP); }
            if (HeartUI_2 != null) { HeartUI_2.GetComponent<Heart_UI>().UpdateHP(PlayerHP); }

            //古いHP情報を更新
            PlayerHP_Old = PlayerHP;
        }


        //EPが上限を超えたら戻す
        if (EP_Remain > EP_Max) { EP_Remain = EP_Max; }

        //EPの表示を更新
        if (UI_EP_Bar != null) { UI_EP_Bar.GetComponent<UI_EP_Bar>().EP_Update(EP_Remain, EP_Max); }

        //クールタイマーの表示を更新
        if (UI_DressIcon != null) { UI_DressIcon.GetComponent<UI_DressIcon>().CoolTimer_Update(CT_Remain, CT_Max); }

        //設定されてない場合は初期化
        if (ScreenOutRange == 0.0f)
        { ScreenOutRange = 11.0f; }

        //毎フレーム更新
        if (AddjustStayScreenF)
        { AddjustSteyPlayer(); }

    }

    /// <summary>
    ///関数名：PlayerObjectIndex<br />
    ///引数名：ID<br />
    ///戻り値：IDに対応するプレハブ <br />
    ///用　途：渡されたIDから検索しオブジェクトを返す　<br />
    /// </summary>
    GameObject PlayerObjectIndex(int ID)
    {
        GameObject Object = null;

        switch (ID)
        {
            //「バラ」
            case 1:
                {
                    Object = PlayerRosePrefab;
                }
                break;
            //「タンポポ」
            case 2:
                {
                    Object = PlayerDandelionPrefab;
                }
                break;
            //「アサガオ」
            case 3:
                {
                    Object = PlayerMorninggloryPrefab;
                }
                break;
        }

        return Object;
    }

    /// <summary>
    ///関数名：ChangePlayer()<br />
    ///引数名：Vector2(座標),bool(スロットずらす方向), int(プレイヤーのHP) ,bool (プレイヤーの向き),GameObject(削除するプレイヤー)<br />
    ///戻り値：なし <br />
    ///用　途：プレイヤーを別の形態に切り替える　<br />
    /// </summary>
    public void ChangePlayer(Vector2 GetPos,bool SlotShiftSign, int GetHP ,bool GetWay,GameObject DeletePlayerObject)
    {
        if (DeletePlayerObject != null)
        {
            //プレイヤーを削除
            Destroy(DeletePlayerObject);
            Debug.Log("プレイヤーの削除に成功");
        }
        else
        {
            Debug.Log("プレイヤーの削除に失敗");
        }

            //選択しているスロットをずらす
            if (SlotShiftSign)
        {
            SelectSlot++;
        }
        else
        {
            SelectSlot--;
        }
        //枠を超えたらループさせる
        if (SelectSlot < 1)
        {
            SelectSlot += 3;
        }
        if (SelectSlot > 3)
        {
            SelectSlot -= 3;
        }

        //次にインスタンス化するプレイヤーのプレハブを保存する
        GameObject InstanceObject = PlayerObjectIndex(SlotID[SelectSlot]);

        //新たなプレイヤーをインスタンス化し各ステータスを引き継ぐ
        if (InstanceObject != null)
        {
            GameObject GeneratedObject = Instantiate(InstanceObject, new Vector2(GetPos.x, GetPos.y), Quaternion.identity);

            //現在のプレイヤーオブジェクトを記録
            Player = GeneratedObject;

            this.gameObject.GetComponent<NewCameraMove>().SetPlayerObject(GeneratedObject);
            
            Debug.Log("新たなプレイヤーをインスタンス化");
            Debug.Log("プレイヤーから受け取った方向を開示" + GetWay);
            Debug.Log(SlotID[SelectSlot]);

            //HPtextObject.GetComponent<HPtext>().UpdateFlowerID(SlotID[SelectSlot]);

            //ドレスアイコンを更新
            if (UI_DressIcon != null) { UI_DressIcon.GetComponent<UI_DressIcon>().DressID_Update(SlotID[SelectSlot]); }

            //IDごとにscriptを持ってくる
            switch (SlotID[SelectSlot])
            {
                //「バラ」
                case 1:
                    {
                        GeneratedObject.GetComponent<PlayerRoseControl>().InheritStatus(GetHP, GetWay);
                        Debug.Log("ステータスの引継ぎに成功");
                    }
                    break;
                //「タンポポ」
                case 2:
                    {
                        GeneratedObject.GetComponent<PlayerDandelionControl>().InheritStatus(GetHP, GetWay);
                        Debug.Log("ステータスの引継ぎに成功");
                    }
                    break;
                //「アサガオ」
                case 3:
                    {
                        GeneratedObject.GetComponent<PlayerMorninggloryControl>().InheritStatus(GetHP, GetWay);
                        Debug.Log("ステータスの引継ぎに成功");
                    }
                    break;
            }
        }
        else
        {
            Debug.Log("次にインスタンス化するオブジェクトがありません");
        }
    }

    public void GetPlayerStatus(Vector2 GetPos,int GetHP)
    {

    }
    public void  UpdatePlayerHP( int GetHP)
    {
        PlayerHP = GetHP;
    }
    /// <summary>
    ///関数名：GetPlayerObject()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：PlayerのObjectを取得
    /// </summary>
    public GameObject GetPlayerObject()
    {
        return Player;
    }
    public Vector2 GetPlayerPos()
    {
        return Player.transform.position;
    }
    public int GetEP()
    {
        return EP_Remain;
    }
    public void SetEP(int ep)
    {
        EP_Remain = ep;
    }
    public void AddEP(int Add)
    {
        EP_Remain += Add;
    }
    public bool EP_CheckLimit()
    {
        if (EP_Remain >= EP_Max)
        { return true; }
        else
        { return false; }
    }

    public void SetReturnPos(Vector2 Pos)
    {
        ReturnPos = Pos;
    }

    public Vector2 GetReturnPos()
    {
        return ReturnPos;
    }

    public void SetCoolTimer(float  timer ,float max)
    {
        CT_Remain = timer;
        CT_Max = max;
    }

    /// <summary>
    ///関数名：AddjustSteyPlayer()<br />
    ///引数名：なし	<br />
    ///戻り値：なし <br />
    ///用　途：Playerを画面内にいるように調整
    /// </summary>
    private void AddjustSteyPlayer()
    {
        if (Player != null)
        {
            if (Player.transform.position.x > this.transform.position.x + ScreenOutRange)
            {
                Debug.Log("右にはみ出ています");
                Player.transform.Translate((this.transform.position.x + ScreenOutRange - Player.transform.position.x), 0.0f, 0.0f);
            }
            if (Player.transform.position.x < this.transform.position.x - ScreenOutRange)
            {
                Debug.Log("左にはみ出ています");
                Player.transform.Translate((this.transform.position.x - ScreenOutRange - Player.transform.position.x), 0.0f, 0.0f);
            }
        }
    }

    public void OnTriggerStey2D(Collider2D collision)
    {
        //衝突対象がプレイヤー
        {
            if (collision.gameObject.tag == "Player")
            {
                Debug.Log("プレイヤーが画面内にいます");
                AddjustStayScreenF = true;
            }
        }

    }

    public void OnCollisionStey2D(Collider2D collision)
    {
        //衝突対象がプレイヤー
        {
            if (collision.gameObject.tag == "Player")
            {
                Debug.Log("プレイヤーが画面内にいます");
                AddjustStayScreenF = true;
            }
        }

    }
}
