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
        //VSyncCount ‚ğ Dont Sync ‚É•ÏX
        QualitySettings.vSyncCount = 0;
        // FPS‚ğ60‚ÉŒÅ’è
        Application.targetFrameRate = 60;
    }
}
