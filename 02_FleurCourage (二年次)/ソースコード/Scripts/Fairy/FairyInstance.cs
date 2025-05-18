using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FairyInstance : MonoBehaviour
{
    private bool _enableFairy;  //

    [SerializeField]
    private int InstanceTextNum; //�e�L�X�g�̔ԍ�

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
    };                          //

    private GameObject _player; //Player
    [SerializeField]
    private GameObject Fairy;   //�����������d��

    private GameObject instanceFairy;   //
    private TextInstance _instances;

    void Start()
    {
        //������
        _enableFairy = false;
    }

    void Update()
    {
        //Player��������
        FindPlayer();

        if (_enableFairy && instanceFairy == null)
        {
            //�d���𐶐�
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
                if (child.name == "�����o��")
                {
                    //���������d���̎q�I�u�W�F�N�g�̃X�N���v�g
                    _instances = child.GetComponent<TextInstance>();
                    _instances.SetTextNum(InstanceTextNum);
                }
            }

                    _enableFairy = false;
        }
    }

    /// <summary>
    /// �֐����FFindPlayer    <br />
    /// ���@���F����    <br />
    /// �߂�l�F����  <br />
    /// Player��������֐�
    /// </summary>
    private void FindPlayer()
    {
        //Player���擾
        for (int i = 0; i < _playerTags.Length; i++)
        {
            //Player��Hierarchy�ɂ�����擾
            if (GameObject.Find(_playerTags[i]) != null)
            {
                _player = GameObject.Find(_playerTags[i]);
                break;
            }
        }
        //Player�����Ȃ������玩�g���擾
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
            // Player�̂��ׂĂ̎q�I�u�W�F�N�g���`�F�b�N
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
