using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockPlatform : MonoBehaviour
{
    public float Timer = 0; //���ꂪ������܂ł̎��Ԃ��v������^�C�}�[

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //�C���X�^���X���ꂽ���Ƀ^�C�}�[�N��
        Timer += Time.deltaTime;

        //�w�莞�Ԃ𒴂����瑫�����������
        if(Timer >= 2.0f)
        {
            Destroy(gameObject);
        }
    }
}
