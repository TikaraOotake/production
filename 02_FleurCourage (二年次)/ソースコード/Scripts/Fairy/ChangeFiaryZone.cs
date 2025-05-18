using UnityEngine;

public class ChangeFiaryZone : MonoBehaviour
{
    [SerializeField]
    private bool PlayerDandelion;   //

    [SerializeField]
    private bool OpenFan;           //

    [SerializeField]
    private bool OpenTree;          //

    private bool _isPlayerHit;      //

    private string[] _playerTags = {
        "PlayerRose",
        "PlayerRose(Clone)",
        "PlayerDandelion(Clone)",
        "PlayerMorningglory(Clone)",
        "PlayerRose - Copy(Clone)"
    };      //

    [SerializeField]
    private GameObject ChangeTarget;    //

    [SerializeField]
    private GameObject Player;          //

    private GameObject Fan;             //

    private GameObject GiantTree;       //

    void Start()
    {
        _isPlayerHit = false;
    }

    void Update()
    {
        if (_isPlayerHit)
        {
            if (PlayerDandelion)
            {
                FindPlayer();

                if (Player.name == "PlayerDandelion(Clone)")
                {
                    ChangeTarget.SetActive(true);

                    Destroy(gameObject);
                }
            }
            if (OpenFan)
            {
                Fan = GameObject.Find("Fan(Clone)");
                if (Fan != null)
                {
                    ChangeTarget.SetActive(true);

                    Destroy(gameObject);
                }
            }

            if (OpenTree)
            {
                GiantTree = GameObject.Find("GiantTree(Clone)");
                if (GiantTree != null)
                {
                    //ChangeTarget.SetActive(true);

                    Destroy(gameObject);
                }
            }

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _isPlayerHit = true;
        }
    }
}
