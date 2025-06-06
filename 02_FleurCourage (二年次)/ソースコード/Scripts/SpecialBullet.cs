using UnityEngine;

public class SpecialBullet : MonoBehaviour
{
	[SerializeField, Tooltip("Destoryするまでの時間")]
	private float DestoryTime;          //Destoryするまでの時間

	private GameObject _playerObj;       //PlayerのObject
	Player _player;                  //Playerが持っているScript

	[SerializeField, Tooltip("移動速度")]
	private float MoveSpeed;			//特殊攻撃の弾の速度

	// Start is called before the first frame update
	void Start()
	{
		//PlayerのScriptを取得
		_playerObj = GameObject.Find("Player");
		_player = _playerObj.GetComponent<Player>();
	}

	// Update is called once per frame
	void Update()
	{		
		//移動
		transform.position = new Vector2(transform.position.x + MoveSpeed, transform.position.y);

		//n秒後に削除
		Destroy(gameObject, DestoryTime);
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		//Enemyに当たったらPlayerのHPを1回復
		if (collision.gameObject.tag == "Enemy")
		{
			//HPが6未満だったらHPを回復
			if (_player.HP < 6)
            {
				_player.HP++;
			}			
		}
	}
}
