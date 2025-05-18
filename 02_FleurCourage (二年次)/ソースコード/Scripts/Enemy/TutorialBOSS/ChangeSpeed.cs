using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpeed : MonoBehaviour
{
    private bool _isWithinRange;

    private readonly float Threshold = 0.1f;

    [SerializeField]
    private float changeSpeed;

    [SerializeField]
    private GameObject TutorialBOSS;

    TutorialBOSS _tutorialBOSS;

    void Start()
    {
        _isWithinRange = false;

        _tutorialBOSS = TutorialBOSS.GetComponent<TutorialBOSS>();
    }

    void Update()
    {
        _isWithinRange = Vector2.Distance(transform.position, TutorialBOSS.transform.position) <= Threshold;
        if (_isWithinRange)
        {
            _tutorialBOSS.SetMoveSpeed(changeSpeed);
        }
    }
}
