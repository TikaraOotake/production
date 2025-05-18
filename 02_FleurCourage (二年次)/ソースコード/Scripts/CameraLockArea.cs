using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLockArea : MonoBehaviour
{
    public GameObject Camera;//カメラ
    public float ExitPlayerTimer;//プレイヤーが退出
    public bool EntryPlayerFlag;//プレイヤーが入出したかのフラグ

    public bool NotAutoUnlock;//自動解除しないかのFlag
    public bool AutoAddjust;//侵入した際自動的にプレイヤーを画面内に居させるか
    public float AddjustRange;//調整する範囲を指定

    public Vector2 CameraMovement;

    public bool CameraScrollFlag; //スクロールを実行するフラグ

    public float CameraScrollSpeed; //スクロール速度
    public int CameraScrollSequence;//カメラスクロールの段階指定
    public GameObject[] CameraScrollPoint;//スクロールの目標座標
    public float[] CameraScrollSequence_Speed; //シーケンス毎に速度を指定

    public Vector2 StartPos;//初期座標
    public float StartSpeed;//初期速度

    // Start is called before the first frame update
    void Start()
    {
        if (null == Camera)
        {
            Camera = GameObject.Find("Main Camera");
            if (Camera != null)
            { Debug.Log("カメラオブジェクトの取得に成功"); }
            else
            { Debug.Log("カメラオブジェクトの取得に失敗"); }
        }

        EntryPlayerFlag = false;

        //速度を変更
        //要素数が設定されているかチェック
        if (CameraScrollSequence_Speed.Length != 0 && CameraScrollSequence_Speed.Length != CameraScrollSequence)
        {
            //0の場合は無視する
            if (CameraScrollSequence_Speed[CameraScrollSequence] != 0)
            {
                CameraScrollSpeed = CameraScrollSequence_Speed[CameraScrollSequence];
            }
        }

        StartPos = this.transform.position;
        StartSpeed = CameraScrollSpeed;
        AddjustRange = 10.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (EntryPlayerFlag == false)
        {
            ExitPlayerTimer += Time.deltaTime;
        }

        //プレイヤーが退出してから0.3秒後なら
        if (ExitPlayerTimer >= 0.3f && ExitPlayerTimer <= 0.4f)
        {
            Camera.GetComponent<NewCameraMove>().CameraUnlock();
        }

        //現在の座標を渡し続ける
        if (EntryPlayerFlag == true)
        {
            Camera.GetComponent<NewCameraMove>().CameraLock(transform.position.x, transform.position.y);
        }


        //要素数が0以外のとき&要素数にシーケンスが達してない時に実行
        if (CameraScrollPoint.Length != 0 && CameraScrollPoint.Length != CameraScrollSequence)
        {
            //スクロールする方向を計算し移動する
            if (EntryPlayerFlag && CameraScrollFlag)
            {
                if (CameraScrollPoint[CameraScrollSequence] != null)
                {
                    float vecX = CameraScrollPoint[CameraScrollSequence].transform.position.x - this.transform.position.x;
                    float vecY = CameraScrollPoint[CameraScrollSequence].transform.position.y - this.transform.position.y;

                    float Range = Mathf.Sqrt(vecX * vecX + vecY * vecY);

                    CameraMovement.x = (vecX / Range) * CameraScrollSpeed;
                    CameraMovement.y = (vecY / Range) * CameraScrollSpeed;
                }

                //有効な数値のみ
                if (CameraMovement.x <= 0|| CameraMovement.x >= 0 || CameraMovement.y <= 0 || CameraMovement.y >= 0)
                {
                    transform.Translate(CameraMovement.x, CameraMovement.y, 0.0f);
                }


                //誤差を修正
                if (this.transform.position.x - 0.3f < CameraScrollPoint[CameraScrollSequence].transform.position.x &&
                    this.transform.position.x + 0.3f > CameraScrollPoint[CameraScrollSequence].transform.position.x &&
                    this.transform.position.y - 0.3f < CameraScrollPoint[CameraScrollSequence].transform.position.y &&
                    this.transform.position.y + 0.3f > CameraScrollPoint[CameraScrollSequence].transform.position.y)
                {
                    this.transform.position
                        = new Vector2(CameraScrollPoint[CameraScrollSequence].transform.position.x,
                        CameraScrollPoint[CameraScrollSequence].transform.position.y);

                    //シーケンスを移行する
                    ++CameraScrollSequence;

                    //速度を変更
                    //要素数が設定されているかチェック
                    if (CameraScrollSequence_Speed.Length != 0 && CameraScrollSequence_Speed.Length != CameraScrollSequence)
                    {
                        //0の場合は無視する
                        if (CameraScrollSequence_Speed[CameraScrollSequence] != 0)
                        {
                            CameraScrollSpeed = CameraScrollSequence_Speed[CameraScrollSequence];
                        }
                    }
                }
            }
        }

        int i = CameraScrollPoint.Length;
    }
    void OnTriggerEnter2D(Collider2D Collider)
    {
        if (Collider.gameObject.tag == "Player")
        {
            EntryPlayerFlag = true;
            ExitPlayerTimer = 0.0f;

            if(AutoAddjust)
            { Camera.GetComponent<PlayerManager>().AddjustStayScreenF = true;
                Camera.GetComponent<PlayerManager>().ScreenOutRange = AddjustRange;
            }

            Debug.Log("接触を検知");
            Camera.GetComponent<NewCameraMove>().CameraLock(transform.position.x, transform.position.y);
        }
    }
    void OnTriggerExit2D(Collider2D Collider)
    {
        if (Collider.gameObject.tag == "Player")
        {
            if (NotAutoUnlock == false)
            {
                EntryPlayerFlag = false;

                Debug.Log("対象が離れました");

            }

        }
    }

    public void SetCameraScrollFlag(bool Flag)
    {
        CameraScrollFlag = Flag;
    }

    public void ResetSequence()
    {
        this.transform.position = StartPos;
        CameraScrollSequence = 0;
        CameraScrollSpeed = StartSpeed;
        CameraScrollFlag = false;
    }
}