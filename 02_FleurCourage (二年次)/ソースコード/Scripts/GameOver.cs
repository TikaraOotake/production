using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    private float timer = 0.0f;

    //RoadGameOver roadGameOver;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timer <= 0.5f)
        {
            timer += Time.deltaTime;
        }

        if (timer >= 0.5f)
        {
            if (ChangeSceneNum.num == 0)
            {
                if (Input.GetKeyDown(KeyCode.A) || (Input.GetKeyDown(KeyCode.Joystick1Button0)))
                {
                    SceneManager.LoadScene("TutorialStage");
                }
            }
            else if (ChangeSceneNum.num == 1)
            {
                if (Input.GetKeyDown(KeyCode.A) || (Input.GetKeyDown(KeyCode.Joystick1Button0)))
                {
                    SceneManager.LoadScene("MainStage");
                }
            }
            if (Input.GetKeyDown(KeyCode.B) || (Input.GetKeyDown(KeyCode.Joystick1Button1)))
            {
                SceneManager.LoadScene("Title");
            }
        }
    }
}
