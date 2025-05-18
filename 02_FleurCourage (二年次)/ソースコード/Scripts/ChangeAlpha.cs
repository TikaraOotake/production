using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAlpha : MonoBehaviour
{
    private bool _alphaControlFlag;     //Alpha値を制御する
    private bool _fixedAlphaFlag;       //Alpha値を固定するフラグ

    private float _alpha;               //Alpha値

    [SerializeField,Tooltip("自身のCanvasGroup")]
    private CanvasGroup Cg;             //Alpha値を変える為のコンポーネント
    // Start is called before the first frame update
    void Start()
    {
        //初期化
        _alpha = 0.0f;

        _alphaControlFlag = false;
        _fixedAlphaFlag = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_fixedAlphaFlag == false)
        {
            //Alpha値を制御
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

            //Alpha値を変える
            Cg.alpha = _alpha;
        }
        //Alpha値を固定する
        if (Input.GetKeyDown(KeyCode.A))
        {
            _fixedAlphaFlag = true;
            Cg.alpha = 1.0f;
        }
    }
}
