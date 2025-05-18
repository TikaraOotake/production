using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLockArea : MonoBehaviour
{
    public GameObject Camera;//�J����
    public float ExitPlayerTimer;//�v���C���[���ޏo
    public bool EntryPlayerFlag;//�v���C���[�����o�������̃t���O

    public bool NotAutoUnlock;//�����������Ȃ�����Flag
    public bool AutoAddjust;//�N�������ێ����I�Ƀv���C���[����ʓ��ɋ������邩
    public float AddjustRange;//��������͈͂��w��

    public Vector2 CameraMovement;

    public bool CameraScrollFlag; //�X�N���[�������s����t���O

    public float CameraScrollSpeed; //�X�N���[�����x
    public int CameraScrollSequence;//�J�����X�N���[���̒i�K�w��
    public GameObject[] CameraScrollPoint;//�X�N���[���̖ڕW���W
    public float[] CameraScrollSequence_Speed; //�V�[�P���X���ɑ��x���w��

    public Vector2 StartPos;//�������W
    public float StartSpeed;//�������x

    // Start is called before the first frame update
    void Start()
    {
        if (null == Camera)
        {
            Camera = GameObject.Find("Main Camera");
            if (Camera != null)
            { Debug.Log("�J�����I�u�W�F�N�g�̎擾�ɐ���"); }
            else
            { Debug.Log("�J�����I�u�W�F�N�g�̎擾�Ɏ��s"); }
        }

        EntryPlayerFlag = false;

        //���x��ύX
        //�v�f�����ݒ肳��Ă��邩�`�F�b�N
        if (CameraScrollSequence_Speed.Length != 0 && CameraScrollSequence_Speed.Length != CameraScrollSequence)
        {
            //0�̏ꍇ�͖�������
            if (CameraScrollSequence_Speed[CameraScrollSequence] != 0)
            {
                CameraScrollSpeed = CameraScrollSequence_Speed[CameraScrollSequence];
            }
        }

        StartPos = this.transform.position;
        StartSpeed = CameraScrollSpeed;
        AddjustRange = 10.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (EntryPlayerFlag == false)
        {
            ExitPlayerTimer += Time.deltaTime;
        }

        //�v���C���[���ޏo���Ă���0.3�b��Ȃ�
        if (ExitPlayerTimer >= 0.3f && ExitPlayerTimer <= 0.4f)
        {
            Camera.GetComponent<NewCameraMove>().CameraUnlock();
        }

        //���݂̍��W��n��������
        if (EntryPlayerFlag == true)
        {
            Camera.GetComponent<NewCameraMove>().CameraLock(transform.position.x, transform.position.y);
        }


        //�v�f����0�ȊO�̂Ƃ�&�v�f���ɃV�[�P���X���B���ĂȂ����Ɏ��s
        if (CameraScrollPoint.Length != 0 && CameraScrollPoint.Length != CameraScrollSequence)
        {
            //�X�N���[������������v�Z���ړ�����
            if (EntryPlayerFlag && CameraScrollFlag)
            {
                if (CameraScrollPoint[CameraScrollSequence] != null)
                {
                    float vecX = CameraScrollPoint[CameraScrollSequence].transform.position.x - this.transform.position.x;
                    float vecY = CameraScrollPoint[CameraScrollSequence].transform.position.y - this.transform.position.y;

                    float Range = Mathf.Sqrt(vecX * vecX + vecY * vecY);

                    CameraMovement.x = (vecX / Range) * CameraScrollSpeed;
                    CameraMovement.y = (vecY / Range) * CameraScrollSpeed;
                }

                //�L���Ȑ��l�̂�
                if (CameraMovement.x <= 0|| CameraMovement.x >= 0 || CameraMovement.y <= 0 || CameraMovement.y >= 0)
                {
                    transform.Translate(CameraMovement.x, CameraMovement.y, 0.0f);
                }


                //�덷���C��
                if (this.transform.position.x - 0.3f < CameraScrollPoint[CameraScrollSequence].transform.position.x &&
                    this.transform.position.x + 0.3f > CameraScrollPoint[CameraScrollSequence].transform.position.x &&
                    this.transform.position.y - 0.3f < CameraScrollPoint[CameraScrollSequence].transform.position.y &&
                    this.transform.position.y + 0.3f > CameraScrollPoint[CameraScrollSequence].transform.position.y)
                {
                    this.transform.position
                        = new Vector2(CameraScrollPoint[CameraScrollSequence].transform.position.x,
                        CameraScrollPoint[CameraScrollSequence].transform.position.y);

                    //�V�[�P���X���ڍs����
                    ++CameraScrollSequence;

                    //���x��ύX
                    //�v�f�����ݒ肳��Ă��邩�`�F�b�N
                    if (CameraScrollSequence_Speed.Length != 0 && CameraScrollSequence_Speed.Length != CameraScrollSequence)
                    {
                        //0�̏ꍇ�͖�������
                        if (CameraScrollSequence_Speed[CameraScrollSequence] != 0)
                        {
                            CameraScrollSpeed = CameraScrollSequence_Speed[CameraScrollSequence];
                        }
                    }
                }
            }
        }

        int i = CameraScrollPoint.Length;
    }
    void OnTriggerEnter2D(Collider2D Collider)
    {
        if (Collider.gameObject.tag == "Player")
        {
            EntryPlayerFlag = true;
            ExitPlayerTimer = 0.0f;

            if(AutoAddjust)
            { Camera.GetComponent<PlayerManager>().AddjustStayScreenF = true;
                Camera.GetComponent<PlayerManager>().ScreenOutRange = AddjustRange;
            }

            Debug.Log("�ڐG�����m");
            Camera.GetComponent<NewCameraMove>().CameraLock(transform.position.x, transform.position.y);
        }
    }
    void OnTriggerExit2D(Collider2D Collider)
    {
        if (Collider.gameObject.tag == "Player")
        {
            if (NotAutoUnlock == false)
            {
                EntryPlayerFlag = false;

                Debug.Log("�Ώۂ�����܂���");

            }

        }
    }

    public void SetCameraScrollFlag(bool Flag)
    {
        CameraScrollFlag = Flag;
    }

    public void ResetSequence()
    {
        this.transform.position = StartPos;
        CameraScrollSequence = 0;
        CameraScrollSpeed = StartSpeed;
        CameraScrollFlag = false;
    }
}