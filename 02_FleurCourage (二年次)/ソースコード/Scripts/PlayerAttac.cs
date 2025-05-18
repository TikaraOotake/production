using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttac : MonoBehaviour
{
    public int RepeatCounter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        ++RepeatCounter;
        if (RepeatCounter == 2)
        {
            Destroy(this.gameObject);
        }
    }
}
