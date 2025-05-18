using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollision : MonoBehaviour
{
    float deleteTime;

    // Start is called before the first frame update
    void Start()
    {
        deleteTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        deleteTime+=Time.deltaTime;
        if(deleteTime>=0.5f)
        {
            Destroy(this.gameObject);
        }
        transform.localScale *= 1.04f;
    }
}
