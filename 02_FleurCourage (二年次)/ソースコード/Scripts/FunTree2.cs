using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunTree2 : MonoBehaviour
{
    public float Timer = 0.0f;
    public GameObject FunTree3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        if(Timer >= 1.5)
        {
            Instantiate(FunTree3, new Vector3(transform.position.x, transform.position.y, 0.0f), Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
