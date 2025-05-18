using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stagescene : MonoBehaviour
{
    public float ClearTimer = 0.0f;
    private bool TreeHit = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (TreeHit == true)
        {
            ClearTimer += Time.deltaTime;
            if (ClearTimer >= 3.0f)
            {
                SceneManager.LoadScene("MainStage");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //
        if (collision.gameObject.tag == "TutorialBoss")
        {
            TreeHit = true;
        }
    }
}