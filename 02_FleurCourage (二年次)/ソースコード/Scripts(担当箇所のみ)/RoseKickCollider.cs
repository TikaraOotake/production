using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoseKickCollider : MonoBehaviour
{
    public GameObject Player;//プレイヤーのオブジェクトを保存
    public GameObject PlayerAttackPrefab;//攻撃判定のプレハブ

    private int GroundTouchCounter;//地形と接触した回数を記録

    void Start()
    {
        transform.position = new Vector2(Player.transform.position.x, Player.transform.position.y);
    }

    void Update()
    {
        transform.position = new Vector2(Player.transform.position.x, Player.transform.position.y);

        //5Fより多く接触していたらキックを終了
        if (5 < GroundTouchCounter)
        {
            Player.GetComponent<PlayerRoseControl>().UpdateKickCancelFlag(true);
        }
    }

    public void GetPlayerObject(GameObject Object)
    {
        Player = Object;
    }

    public void GeneratePlayerAttack()
    {
        if (PlayerAttackPrefab != null)
        {
            Debug.Log("攻撃判定を生成します");
            GameObject Attack = Instantiate(PlayerAttackPrefab, transform.position, Quaternion.identity);
        }
    }

    void OnTriggerStay2D(Collider2D Collider)
    {
        if (Collider.gameObject.tag == "EnemyAttack" || Collider.gameObject.tag == "Enemy" || Collider.gameObject.tag == "lnvincible")
        {
            Debug.Log("接触を検知");
            if (PlayerAttackPrefab != null)
            {
                GameObject Attack = Instantiate(PlayerAttackPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Debug.Log("攻撃判定の生成に失敗");
            }

            Player.GetComponent<PlayerRoseControl>().UpdateKickCancelFlag(true);
        }

        if (Collider.gameObject.tag == "Ground")
        {
            ++GroundTouchCounter;
        }
    }
}
