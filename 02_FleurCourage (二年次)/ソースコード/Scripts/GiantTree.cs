using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantTree : MonoBehaviour
{
    public float Timer = 0.0f;
    public GameObject GiantTree2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        if(Timer >= 1.0f)
        {
            Instantiate(GiantTree2, new Vector3(transform.position.x -0.2f, transform.position.y +8.7f, 0.0f), Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
