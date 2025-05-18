using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HPUI : MonoBehaviour
{
    public GameObject GetHP;
    public int HPobj;
    public PlayerManager GetHPobj;
    public bool Startfrg = true;
    public float HPposition;




    // Start is called before the first frame update
    void Start()
    {
        GetHPobj = GameObject.Find("Main Camera").GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        HPobj = GetHPobj.PlayerHP;
        if (Startfrg == true)
        {
            for (int i = 0; i < HPobj; i++)
            {
                Instantiate(GetHP, new Vector3(-3.5f + HPposition * i, 3.5f, 0.0f), Quaternion.identity);
            }
            Startfrg = false;
        }
    }
}
