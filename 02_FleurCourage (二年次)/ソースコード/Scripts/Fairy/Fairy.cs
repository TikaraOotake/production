using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fairy : MonoBehaviour
{
    [SerializeField]
    private float MoveSpeed;    //妖精の移動速度

    [SerializeField]
    private float threshold;    //座標に到達とみなす範囲

    [Serializable]
    struct TargetPos
    {
        public float x;
        public float y;
    }
    [SerializeField]
    TargetPos targetPos;        //移動する座標

    private Vector2 _firstMyPos;    //初期位置

    private string[] _playerTags = {
        "PlayerRose",
        "PlayerRose(Clone)",
        "PlayerDandelion(Clone)",
        "PlayerMorningglory(Clone)",
        "PlayerRose - Copy(Clone)"
    };                              //Playerが持っているtag

    [SerializeField]
    GameObject SpeechBubble;        //吹き出しのオブジェクト

    GameObject _player;             //Playerのオブジェクト

    [SerializeField]
    private Rigidbody2D Rb;         //自身のRigidBody2D

    void Start()
    {
        //初期座標
        _firstMyPos = transform.position;
    }

    void Update()
    {
        FairyMove();
        FairySpeak();
    }

    /// <summary>
    /// 関数名：FairyMove   <br />
    /// 引　数：無し  <br />
    /// 戻り値：無し  <br />
    /// 妖精の移動
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
    /// 関数名：FairySpeak   <br />
    /// 引　数：無し  <br />
    /// 戻り値：無し  <br />
    /// 妖精の喋る内容
    /// </summary>
    void FairySpeak()
    {
        Vector3 TargetPos = new Vector2(_player.transform.position.x + targetPos.x, _player.transform.position.y + targetPos.y);

        //指定された座標の近くに来たら吹き出しを表示
        if (Vector2.Distance(transform.position,TargetPos) <= threshold)
        {
            SpeechBubble.SetActive(true);
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
}
