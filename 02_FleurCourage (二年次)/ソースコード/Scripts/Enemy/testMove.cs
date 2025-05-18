using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testMove : MonoBehaviour
{
    public void Moves(float speed)
    {
        transform.position = new Vector3(transform.position.x + speed, transform.position.y, transform.position.z);
    }

}
