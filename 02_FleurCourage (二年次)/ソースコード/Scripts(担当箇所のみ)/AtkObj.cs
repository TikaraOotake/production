using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AtkObj : MonoBehaviour
{
    public float cnt = 0.0f;
    public float DestroyTimer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cnt += Time.deltaTime;

        if (cnt >= DestroyTimer)
        {
            Destroy(this.gameObject);
        }
    }
}
