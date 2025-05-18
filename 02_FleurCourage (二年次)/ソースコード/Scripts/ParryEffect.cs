using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryEffect : MonoBehaviour
{
    public float DestroyTime;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (DestroyTime <= 0)
        {
            Destroy(this.gameObject);
        }
        DestroyTime -= Time.deltaTime;
    }
}
