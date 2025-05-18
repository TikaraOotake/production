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
    /// �֐����FFindPlayer    <br />
    /// ���@���F����    <br />
    /// �߂�l�F����  <br />
    /// Player��������֐�
    /// </summary>
    void FindPlayer()
    {
        //Player���擾
        for (int i = 0; i < _playerTags.Length; i++)
        {
            //Player��Hierarchy�ɂ�����擾
            if (GameObject.Find(_playerTags[i]) != null)
            {
                Player = GameObject.Find(_playerTags[i]);
                break;
            }
        }
        //Player�����Ȃ������玩�g���擾
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
