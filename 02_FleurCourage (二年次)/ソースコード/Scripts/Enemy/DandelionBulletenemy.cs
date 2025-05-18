using UnityEngine;

public class DandelionBulletenemy : MonoBehaviour
{
   private GameObject _parent;      //�e�I�u�W�F�N�g

    private Enemy _enemy;           //�e�I�u�W�F�N�g��Enemy�X�N���v�g

    void Start()
    {
        //�e�I�u�W�F�N�g���擾
        _parent = transform.parent.gameObject;
        _enemy = _parent.GetComponent<Enemy>();
    }

    void Update()
    {
        //Timer�����Z�b�g���ꂽ����s
        if (_enemy.FlyTimer <= 0.1f) 
        {
            Destroy(gameObject);
        }
    }
}
