using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSetting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void Awake()
    {
        //VSyncCount を Dont Sync に変更
        QualitySettings.vSyncCount = 0;
        // FPSを60に固定
        Application.targetFrameRate = 60;
    }
}
