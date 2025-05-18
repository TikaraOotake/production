using UnityEngine;

public class EnemyExplosion : MonoBehaviour
{
    //自身のアニメーション
    [SerializeField]
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //アニメーションが終わったらDestroyする
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
		{
            Destroy(gameObject);
		}
    }
}
