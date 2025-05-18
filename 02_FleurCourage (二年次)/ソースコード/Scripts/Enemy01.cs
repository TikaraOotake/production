using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01 : MonoBehaviour
{
    public GameObject SpritePrefab;//Sprite��\������I�u�W�F�N�g�̃v���n�u
    public GameObject Sprite;//���������X�v���C�g�I�u�W�F�N�g��ۑ�����

    public GameObject EnemyGroundPrefab;//EnemyGround�I�u�W�F�N�g�̃v���n�u
    public GameObject EnemyGround;//��������EnemyGround�I�u�W�F�N�g��ۑ�����

    public bool GroundOnFlag;//��Ƀv���C���[�����邩�ǂ����̃t���O

    enum Status
    {
        Fine, //�ʏ�
        Dead, //���S
        Respawn, //���A
        Clear, //�N���A
        NoControll,//����s�\
        DamageStiffness,//��e�d��
    }
    Status st;

    enum ActionStatus
    {
        NoAction,//�������Ă��Ȃ�
        Move,//�ړ�
        Attack,//�U��
        Jump,//�W�����v
    }
    ActionStatus Ast;

    // Start is called before the first frame update
    void Start()
    {
        //EnemyGround�I�u�W�F�N�g���C���X�^���X��
        if (EnemyGroundPrefab != null)
        {
            EnemyGround = Instantiate(EnemyGroundPrefab, transform.position, Quaternion.identity);
            EnemyGround.GetComponent<Enemy01Ground>().UpdateParentObject(this.gameObject);
        }
        //Sprite��\������I�u�W�F�N�g���C���X�^���X��
        if (SpritePrefab != null)
        {
            GameObject childObject = Instantiate(SpritePrefab, transform);
            Sprite = childObject;
            Sprite.GetComponent<Enemy01Split>().UpdatePlayerObject(this.gameObject);
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        //���g�̃I�u�W�F�N�g�𓧖���
        GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 0);

        switch (st)
        {
            //�u�ʏ�v
            case Status.Fine:
                {
                    if(GroundOnFlag)
                    {
                        //�X�v���C�g��U��������
                        if (Sprite != null)
                        { Sprite.GetComponent<Enemy01Split>().UpdateVibStatus(0.1f, 1.2f, 0.1f, 0.0f, 300.0f); }
                        Debug.Log("aaaadefeg");
                    }
                }
                break;
        }
        switch (st)
        {
            //�u���S�v
            case Status.Dead:
                {

                }
                break;
        }
    }

    /// <summary>
    ///�֐����FUpdateObject()<br />
    ///�������F�v���C���[�̃I�u�W�F�N�g<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�v���C���[�̃I�u�W�F�N�g���擾����@<br />
    /// </summary>
    public void UpdateGroundOnFlag(bool Flag)
    {
        GroundOnFlag = Flag;
    }
}
