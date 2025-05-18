using UnityEngine;

public class SpecialBullet : MonoBehaviour
{
	[SerializeField, Tooltip("Destory����܂ł̎���")]
	private float DestoryTime;          //Destory����܂ł̎���

	private GameObject _playerObj;       //Player��Object
	Player _player;                  //Player�������Ă���Script

	[SerializeField, Tooltip("�ړ����x")]
	private float MoveSpeed;			//����U���̒e�̑��x

	// Start is called before the first frame update
	void Start()
	{
		//Player��Script���擾
		_playerObj = GameObject.Find("Player");
		_player = _playerObj.GetComponent<Player>();
	}

	// Update is called once per frame
	void Update()
	{		
		//�ړ�
		transform.position = new Vector2(transform.position.x + MoveSpeed, transform.position.y);

		//n�b��ɍ폜
		Destroy(gameObject, DestoryTime);
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		//Enemy�ɓ���������Player��HP��1��
		if (collision.gameObject.tag == "Enemy")
		{
			//HP��6������������HP����
			if (_player.HP < 6)
            {
				_player.HP++;
			}			
		}
	}
}
