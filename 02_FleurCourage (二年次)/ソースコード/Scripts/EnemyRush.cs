using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRush : MonoBehaviour
{
    [SerializeField]
    private int RushCnt = 0;

    public GameObject Enemy1;
    public GameObject Enemy2;
    public GameObject Enemy3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(RushCnt == 1) 
        {
            Instantiate(Enemy1, new Vector3(transform.position.x + 18.0f, transform.position.y, 0.0f), Quaternion.identity);
            Instantiate(Enemy1, new Vector3(transform.position.x + 13.0f, transform.position.y, 0.0f), Quaternion.identity);
            RushCnt++;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            {
                RushCnt++;
            }
        }
    }
}
