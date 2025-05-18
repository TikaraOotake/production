using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantTreeBlossom : MonoBehaviour
{
    public GameObject GiantTree;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "SpMorninggloryAtk")
        {
            Instantiate(GiantTree, new Vector3(transform.position.x, transform.position.y, 0.0f), Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
