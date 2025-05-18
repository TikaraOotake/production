using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Clearscene : MonoBehaviour
{
    private float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= 0.5f)
        {
            timer += Time.deltaTime;
        }

        if (timer >= 0.5f)
        {
            if (Input.GetKeyDown(KeyCode.A) || (Input.GetKeyDown(KeyCode.Joystick1Button0)))
            {
                SceneManager.LoadScene("Title");
            }
        }
    }
}
