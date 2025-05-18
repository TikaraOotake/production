using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAlpha : MonoBehaviour
{
    private bool _alphaControlFlag;     //Alpha�l�𐧌䂷��
    private bool _fixedAlphaFlag;       //Alpha�l���Œ肷��t���O

    private float _alpha;               //Alpha�l

    [SerializeField,Tooltip("���g��CanvasGroup")]
    private CanvasGroup Cg;             //Alpha�l��ς���ׂ̃R���|�[�l���g
    // Start is called before the first frame update
    void Start()
    {
        //������
        _alpha = 0.0f;

        _alphaControlFlag = false;
        _fixedAlphaFlag = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_fixedAlphaFlag == false)
        {
            //Alpha�l�𐧌�
            if (_alphaControlFlag == false)
            {
                _alpha += Time.deltaTime;
                if (_alpha >= 0.5f)
                {
                    _alphaControlFlag = true;
                }
            }
            else
            {
                _alpha -= Time.deltaTime;
                if (_alpha <= 0.0f)
                {
                    _alphaControlFlag = false;
                }
            }

            //Alpha�l��ς���
            Cg.alpha = _alpha;
        }
        //Alpha�l���Œ肷��
        if (Input.GetKeyDown(KeyCode.A))
        {
            _fixedAlphaFlag = true;
            Cg.alpha = 1.0f;
        }
    }
}
