using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoseNormalAttackCol : MonoBehaviour
{
    [SerializeField]
    private float _destroyTimer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _destroyTimer -= Time.deltaTime;
        if (_destroyTimer <= 0.0f)
        {
            Destroy(gameObject);
        }
    }
}
