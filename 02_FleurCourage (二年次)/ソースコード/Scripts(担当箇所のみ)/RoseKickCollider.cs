using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoseKickCollider : MonoBehaviour
{
    public GameObject Player;//�v���C���[�̃I�u�W�F�N�g��ۑ�
    public GameObject PlayerAttackPrefab;//�U������̃v���n�u

    private int GroundTouchCounter;//�n�`�ƐڐG�����񐔂��L�^

    void Start()
    {
        transform.position = new Vector2(Player.transform.position.x, Player.transform.position.y);
    }

    void Update()
    {
        transform.position = new Vector2(Player.transform.position.x, Player.transform.position.y);

        //5F��葽���ڐG���Ă�����L�b�N���I��
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
            Debug.Log("�U������𐶐����܂�");
            GameObject Attack = Instantiate(PlayerAttackPrefab, transform.position, Quaternion.identity);
        }
    }

    void OnTriggerStay2D(Collider2D Collider)
    {
        if (Collider.gameObject.tag == "EnemyAttack" || Collider.gameObject.tag == "Enemy" || Collider.gameObject.tag == "lnvincible")
        {
            Debug.Log("�ڐG�����m");
            if (PlayerAttackPrefab != null)
            {
                GameObject Attack = Instantiate(PlayerAttackPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Debug.Log("�U������̐����Ɏ��s");
            }

            Player.GetComponent<PlayerRoseControl>().UpdateKickCancelFlag(true);
        }

        if (Collider.gameObject.tag == "Ground")
        {
            ++GroundTouchCounter;
        }
    }
}
