using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBOSSInstance : MonoBehaviour
{
    [System.NonSerialized]
    public bool IsInstance;

    [System.Serializable]
    struct InstancePos
    {
        public float x;

        public float y;
    }
    [SerializeField]
    InstancePos instancePos;    //生成したい座標

    private string[] _playerTags = {
        "PlayerRose",
        "PlayerRose(Clone)",
        "PlayerDandelion(Clone)",
        "PlayerMorningglory(Clone)",
        "PlayerRose - Copy(Clone)"
    };

    [SerializeField]
    private GameObject Player;

    [SerializeField]
    private GameObject TutorialBOSS;

    PlayerBase playerBase;

    void Start()
    {
        IsInstance = false;
    }

    void Update()
    {

        FindPlayer();
        playerBase = Player.GetComponent<PlayerBase>();

        if (IsInstance)
        {
            TutorialBOSS.SetActive(true);

            IsInstance = false;
        }
    }

    /// <summary>
    /// 関数名：FindPlayer    <br />
    /// 引　数：無し    <br />
    /// 戻り値：無し  <br />
    /// Playerを見つける関数
    /// </summary>
    void FindPlayer()
    {
        //Playerを取得
        for (int i = 0; i < _playerTags.Length; i++)
        {
            //PlayerがHierarchyにいたら取得
            if (GameObject.Find(_playerTags[i]) != null)
            {
                Player = GameObject.Find(_playerTags[i]);
                break;
            }
        }
        //Playerがいなかったら自身を取得
        if (Player == null)
        {
            Player = gameObject;
        }
    }

    public void SetIsInstance(bool ret)
    {
        IsInstance = ret;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            IsInstance = true;
            //Playerの動きを止める
            playerBase.SetMoveFlag(false);
        }
    }
}
