using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextInstance : MonoBehaviour
{
    private bool _isInstance;

    private int _textNum;

    [SerializeField]
    private GameObject[] _textObj;

    void Start()
    {
        _isInstance = false;
    }

    void Update()
    {
        if (_isInstance == false)
        {
            Instantiate(
                _textObj[_textNum],
                transform.position, Quaternion.identity,
                transform.parent);
            _isInstance = true;
        }
    }

    public void SetTextNum(int num)
    {
        _textNum = num;
    }
}
