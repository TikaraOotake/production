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
        //VSyncCount �� Dont Sync �ɕύX
        QualitySettings.vSyncCount = 0;
        // FPS��60�ɌŒ�
        Application.targetFrameRate = 60;
    }
}
