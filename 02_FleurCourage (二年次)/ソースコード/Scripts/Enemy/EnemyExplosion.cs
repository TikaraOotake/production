using UnityEngine;

public class EnemyExplosion : MonoBehaviour
{
    //���g�̃A�j���[�V����
    [SerializeField]
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //�A�j���[�V�������I�������Destroy����
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
		{
            Destroy(gameObject);
		}
    }
}
