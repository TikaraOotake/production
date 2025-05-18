using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public float Timer = 0;�@�@�@//����o���̎��Ԍv���̂��߂̃^�C�}�[
    public float pointX = 0.0f;�@//����o���̂����W�𒲐�����ϐ�
    public float pointY = 0.0f;  //����o���̂����W�𒲐�����ϐ�
    public GameObject Platform;  //�o��������I�u�W�F�N�g
    public int cnt = 0;          //���ꂪ�o�������񐔂��J�E���g����ϐ�

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //�^�C�}�[���X�^�[�g������
        Timer += Time.deltaTime;

        //�^�C�}�[���w�莞�ԂɂȂ����瑫����o��������
        if(Timer >= 1 && cnt == 0)
        {
            Instantiate(Platform, new Vector3(pointX, pointY,0.0f), Quaternion.identity);
            ++cnt;
        }

        if(Timer >= 2 && cnt == 1)
        {
            Instantiate(Platform, new Vector3(pointX + 3, pointY - 1, 0.0f), Quaternion.identity);
            ++cnt;
        }

        if(Timer >= 3 && cnt == 2)
        {
            Instantiate(Platform, new Vector3(pointX + 6, pointY , 0.0f), Quaternion.identity);
            ++cnt;
        }

        //�^�C�}�[���w�莞�Ԃ𒴂�����^�C�}�[�ƃJ�E���g�����ɖ߂�
        if(Timer >= 5 && cnt >= 3)
        {
            Timer = 0;
            cnt = 0;
        }
    }
}
