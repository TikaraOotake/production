using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackObj : MonoBehaviour
{
    public float DestroyTimer = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DestroyTimer += Time.deltaTime;
        if(DestroyTimer >= 0.2f)
        {
            Destroy(gameObject);
        }
    }
}
