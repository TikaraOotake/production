using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FairyInstance : MonoBehaviour
{
    private bool _enableFairy;  //

    [SerializeField]
    private int InstanceTextNum; //テキストの番号

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
    };                          //

    private GameObject _player; //Player
    [SerializeField]
    private GameObject Fairy;   //生成したい妖精

    private GameObject instanceFairy;   //
    private TextInstance _instances;

    void Start()
    {
        //初期化
        _enableFairy = false;
    }

    void Update()
    {
        //Playerを見つける
        FindPlayer();

        if (_enableFairy && instanceFairy == null)
        {
            //妖精を生成
            instanceFairy = Instantiate(
                Fairy,
                new Vector3(
                _player.transform.position.x + instancePos.x,
                _player.transform.position.y + instancePos.y,
                _player.transform.position.z),
                Quaternion.identity,
                _player.transform) as GameObject;

            foreach (Transform child in instanceFairy.transform)
            {
                if (child.name == "吹き出し")
                {
                    //生成した妖精の子オブジェクトのスクリプト
                    _instances = child.GetComponent<TextInstance>();
                    _instances.SetTextNum(InstanceTextNum);
                }
            }

                    _enableFairy = false;
        }
    }

    /// <summary>
    /// 関数名：FindPlayer    <br />
    /// 引　数：無し    <br />
    /// 戻り値：無し  <br />
    /// Playerを見つける関数
    /// </summary>
    private void FindPlayer()
    {
        //Playerを取得
        for (int i = 0; i < _playerTags.Length; i++)
        {
            //PlayerがHierarchyにいたら取得
            if (GameObject.Find(_playerTags[i]) != null)
            {
                _player = GameObject.Find(_playerTags[i]);
                break;
            }
        }
        //Playerがいなかったら自身を取得
        if (_player == null)
        {
            _player = gameObject;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _enableFairy = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Playerのすべての子オブジェクトをチェック
            foreach (Transform child in _player.transform)
            {
                if (child.name == "Fairy(Clone)")
                {
                    Destroy(child.gameObject);
                    break;
                }
            }
        }

    }
}
