using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveArea : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "PlayerAtk")
        {
            Destroy(collision.gameObject);
        }
    }
}
