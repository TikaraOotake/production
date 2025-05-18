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
    InstancePos instancePos;    //�������������W

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

    public void SetIsInstance(bool ret)
    {
        IsInstance = ret;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            IsInstance = true;
            //Player�̓������~�߂�
            playerBase.SetMoveFlag(false);
        }
    }
}
