using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFloor : MonoBehaviour
{
    [SerializeField] 
    private GameObject[] waypoints; �@�@�@�@//�E�F�C�|�C���g�̐ݒ�
    private int currentWaypointIndex = 0;   //�E�F�C�|�C���g�̐�

    [SerializeField]
    private float speed = 2f;  //�ړ����x�̕ϐ�

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // ���݂̏��̈ʒu���ړI�n�ɔ��ɋ߂��ꍇ
        if (Vector2.Distance(waypoints[currentWaypointIndex].transform.position, transform.position) < .1f)
        {
            // �ړI�n�����̃|�C���g�ɃZ�b�g����
            currentWaypointIndex++;

            // �Ō�܂ōs������A��ԍŏ��̃|�C���g��ړI�n�Ƃ���
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
            }
        }

        // ���݂̏��̈ʒu����A�ړI�n�̈ʒu�܂ňړ�����
        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypointIndex].transform.position, Time.deltaTime * speed);

    }
}
