using UnityEngine;

public class SpecialBullet : MonoBehaviour
{
	[SerializeField, Tooltip("Destory‚·‚é‚Ü‚Å‚ÌŠÔ")]
	private float DestoryTime;          //Destory‚·‚é‚Ü‚Å‚ÌŠÔ

	private GameObject _playerObj;       //Player‚ÌObject
	Player _player;                  //Player‚ª‚Á‚Ä‚¢‚éScript

	[SerializeField, Tooltip("ˆÚ“®‘¬“x")]
	private float MoveSpeed;			//“ÁêUŒ‚‚Ì’e‚Ì‘¬“x

	// Start is called before the first frame update
	void Start()
	{
		//Player‚ÌScript‚ğæ“¾
		_playerObj = GameObject.Find("Player");
		_player = _playerObj.GetComponent<Player>();
	}

	// Update is called once per frame
	void Update()
	{		
		//ˆÚ“®
		transform.position = new Vector2(transform.position.x + MoveSpeed, transform.position.y);

		//n•bŒã‚Éíœ
		Destroy(gameObject, DestoryTime);
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		//Enemy‚É“–‚½‚Á‚½‚çPlayer‚ÌHP‚ğ1‰ñ•œ
		if (collision.gameObject.tag == "Enemy")
		{
			//HP‚ª6–¢–‚¾‚Á‚½‚çHP‚ğ‰ñ•œ
			if (_player.HP < 6)
            {
				_player.HP++;
			}			
		}
	}
}
