using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fairy : MonoBehaviour
{
    [SerializeField]
    private float MoveSpeed;    //�d���̈ړ����x

    [SerializeField]
    private float threshold;    //���W�ɓ��B�Ƃ݂Ȃ��͈�

    [Serializable]
    struct TargetPos
    {
        public float x;
        public float y;
    }
    [SerializeField]
    TargetPos targetPos;        //�ړ�������W

    private Vector2 _firstMyPos;    //�����ʒu

    private string[] _playerTags = {
        "PlayerRose",
        "PlayerRose(Clone)",
        "PlayerDandelion(Clone)",
        "PlayerMorningglory(Clone)",
        "PlayerRose - Copy(Clone)"
    };                              //Player�������Ă���tag

    [SerializeField]
    GameObject SpeechBubble;        //�����o���̃I�u�W�F�N�g

    GameObject _player;             //Player�̃I�u�W�F�N�g

    [SerializeField]
    private Rigidbody2D Rb;         //���g��RigidBody2D

    void Start()
    {
        //�������W
        _firstMyPos = transform.position;
    }

    void Update()
    {
        FairyMove();
        FairySpeak();
    }

    /// <summary>
    /// �֐����FFairyMove   <br />
    /// ���@���F����  <br />
    /// �߂�l�F����  <br />
    /// �d���̈ړ�
    /// </summary>
    void FairyMove()
    {
        FindPlayer();

        transform.position = Vector3.MoveTowards(
            transform.position,
            new Vector2(_player.transform.position.x + targetPos.x, _player.transform.position.y + targetPos.y),
            MoveSpeed * Time.deltaTime);

    }
    /// <summary>
    /// �֐����FFairySpeak   <br />
    /// ���@���F����  <br />
    /// �߂�l�F����  <br />
    /// �d���̒�����e
    /// </summary>
    void FairySpeak()
    {
        Vector3 TargetPos = new Vector2(_player.transform.position.x + targetPos.x, _player.transform.position.y + targetPos.y);

        //�w�肳�ꂽ���W�̋߂��ɗ����琁���o����\��
        if (Vector2.Distance(transform.position,TargetPos) <= threshold)
        {
            SpeechBubble.SetActive(true);
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
}
