using UnityEngine;

public class DandelionBulletenemy : MonoBehaviour
{
   private GameObject _parent;      //親オブジェクト

    private Enemy _enemy;           //親オブジェクトのEnemyスクリプト

    void Start()
    {
        //親オブジェクトを取得
        _parent = transform.parent.gameObject;
        _enemy = _parent.GetComponent<Enemy>();
    }

    void Update()
    {
        //Timerがリセットされたら実行
        if (_enemy.FlyTimer <= 0.1f) 
        {
            Destroy(gameObject);
        }
    }
}
