using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThroughFloor : MonoBehaviour
{
    public float Timer = 0; //�����߂����߂̃^�C�}�[
    private PlatformEffector2D _platformEffector;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Awake()
    {
        //�R���|�[�l���g���擾
        _platformEffector = GetComponent<PlatformEffector2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //�^�C�}�[�Ŗ߂�܂ł̎��Ԍv��
        Timer += Time.deltaTime;

        //S�L�[�������ꂽ�Ƃ�����Ă��锻��������Ă��蔲����悤��
        if (Input.GetKey(KeyCode.S))
            _platformEffector.rotationalOffset = 180f;

        //�w�肵�����ԂɂȂ����画�����ɖ߂�
        if (Timer >= 0.3)
        {
            _platformEffector.rotationalOffset = 0f;
            Timer = 0;
        }
    }
}
