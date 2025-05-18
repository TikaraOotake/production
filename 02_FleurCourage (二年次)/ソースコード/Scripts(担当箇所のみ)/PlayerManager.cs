using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject Player; //���݂̃v���C���[�̃I�u�W�F�N�g

    public GameObject PlayerRosePrefab;//�o���̃v���n�u
    public GameObject PlayerDandelionPrefab;//�^���|�|�̃v���n�u
    public GameObject PlayerMorninggloryPrefab;//�A�T�K�I�̃v���n�u

    public GameObject HeartUI_0;//Heart��UI
    public GameObject HeartUI_1;//Heart��UI
    public GameObject HeartUI_2;//Heart��UI

    public GameObject HPtextObject;

    public GameObject UI_EP_Bar;
    public GameObject UI_DressIcon;


    public int EP_Max;//EP�̍ő�l
    public int EP_Remain;//EP�̎c��

    public float CT_Max;//CoolTimer�̍ő�l
    public float CT_Remain;//CoolTimer�̎c��

    //�X���b�g���ɉԂ�ID��ۑ�����
    public int[] SlotID = new int[4];
�@�@
    public int SelectSlot;//���Ԗڂ̃X���b�g���g���Ă��邩�̕ϐ�

    public int PlayerHP;//�v���C���[��HP
    public int PlayerHP_Old;//�v���C���[�̌Â�HP
    public Vector2 PlayerPos;//�v���C���[�̍��W

    public Vector2 ReturnPos;//���A���W���L�^

    public float ScreenOutRange;//��ʊO�̋���
    public bool AddjustStayScreenF;//��ʓ��ɂ���悤�ɒ������邩�̃t���O

    // Start is called before the first frame update
    void Start()
    {
        //�`�F�b�N�|�C���g���ݒ肳��Ă����炻���ɔ�΂�
        //if(0 != this.gameObject.GetComponent<GameData>().GetCheckPointNum()) 
        {
            //Player.transform.position = this.gameObject.GetComponent<GameData>().GetReturnPos();
        }

        Player = this.gameObject.GetComponent<NewCameraMove>().GetPlayerObject();

    }

    // Update is called once per frame
    void Update()
    {
        //�e�X�g
        if(Input.GetKeyDown(KeyCode.B))
        {
            this.gameObject.GetComponent<GameData>().SetCheckPointNum(1);
            this.gameObject.GetComponent<GameData>().SetReturnPos(Player.transform.position);
        }


        //Player��HP�ɕω����������ꍇHP_UI�̕\����؂�ւ���
        if(PlayerHP_Old != PlayerHP)
        {
            if (HeartUI_0 != null) { HeartUI_0.GetComponent<Heart_UI>().UpdateHP(PlayerHP); }
            if (HeartUI_1 != null) { HeartUI_1.GetComponent<Heart_UI>().UpdateHP(PlayerHP); }
            if (HeartUI_2 != null) { HeartUI_2.GetComponent<Heart_UI>().UpdateHP(PlayerHP); }

            //�Â�HP�����X�V
            PlayerHP_Old = PlayerHP;
        }


        //EP������𒴂�����߂�
        if (EP_Remain > EP_Max) { EP_Remain = EP_Max; }

        //EP�̕\�����X�V
        if (UI_EP_Bar != null) { UI_EP_Bar.GetComponent<UI_EP_Bar>().EP_Update(EP_Remain, EP_Max); }

        //�N�[���^�C�}�[�̕\�����X�V
        if (UI_DressIcon != null) { UI_DressIcon.GetComponent<UI_DressIcon>().CoolTimer_Update(CT_Remain, CT_Max); }

        //�ݒ肳��ĂȂ��ꍇ�͏�����
        if (ScreenOutRange == 0.0f)
        { ScreenOutRange = 11.0f; }

        //���t���[���X�V
        if (AddjustStayScreenF)
        { AddjustSteyPlayer(); }

    }

    /// <summary>
    ///�֐����FPlayerObjectIndex<br />
    ///�������FID<br />
    ///�߂�l�FID�ɑΉ�����v���n�u <br />
    ///�p�@�r�F�n���ꂽID���猟�����I�u�W�F�N�g��Ԃ��@<br />
    /// </summary>
    GameObject PlayerObjectIndex(int ID)
    {
        GameObject Object = null;

        switch (ID)
        {
            //�u�o���v
            case 1:
                {
                    Object = PlayerRosePrefab;
                }
                break;
            //�u�^���|�|�v
            case 2:
                {
                    Object = PlayerDandelionPrefab;
                }
                break;
            //�u�A�T�K�I�v
            case 3:
                {
                    Object = PlayerMorninggloryPrefab;
                }
                break;
        }

        return Object;
    }

    /// <summary>
    ///�֐����FChangePlayer()<br />
    ///�������FVector2(���W),bool(�X���b�g���炷����), int(�v���C���[��HP) ,bool (�v���C���[�̌���),GameObject(�폜����v���C���[)<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�F�v���C���[��ʂ̌`�Ԃɐ؂�ւ���@<br />
    /// </summary>
    public void ChangePlayer(Vector2 GetPos,bool SlotShiftSign, int GetHP ,bool GetWay,GameObject DeletePlayerObject)
    {
        if (DeletePlayerObject != null)
        {
            //�v���C���[���폜
            Destroy(DeletePlayerObject);
            Debug.Log("�v���C���[�̍폜�ɐ���");
        }
        else
        {
            Debug.Log("�v���C���[�̍폜�Ɏ��s");
        }

            //�I�����Ă���X���b�g�����炷
            if (SlotShiftSign)
        {
            SelectSlot++;
        }
        else
        {
            SelectSlot--;
        }
        //�g�𒴂����烋�[�v������
        if (SelectSlot < 1)
        {
            SelectSlot += 3;
        }
        if (SelectSlot > 3)
        {
            SelectSlot -= 3;
        }

        //���ɃC���X�^���X������v���C���[�̃v���n�u��ۑ�����
        GameObject InstanceObject = PlayerObjectIndex(SlotID[SelectSlot]);

        //�V���ȃv���C���[���C���X�^���X�����e�X�e�[�^�X�������p��
        if (InstanceObject != null)
        {
            GameObject GeneratedObject = Instantiate(InstanceObject, new Vector2(GetPos.x, GetPos.y), Quaternion.identity);

            //���݂̃v���C���[�I�u�W�F�N�g���L�^
            Player = GeneratedObject;

            this.gameObject.GetComponent<NewCameraMove>().SetPlayerObject(GeneratedObject);
            
            Debug.Log("�V���ȃv���C���[���C���X�^���X��");
            Debug.Log("�v���C���[����󂯎�����������J��" + GetWay);
            Debug.Log(SlotID[SelectSlot]);

            //HPtextObject.GetComponent<HPtext>().UpdateFlowerID(SlotID[SelectSlot]);

            //�h���X�A�C�R�����X�V
            if (UI_DressIcon != null) { UI_DressIcon.GetComponent<UI_DressIcon>().DressID_Update(SlotID[SelectSlot]); }

            //ID���Ƃ�script�������Ă���
            switch (SlotID[SelectSlot])
            {
                //�u�o���v
                case 1:
                    {
                        GeneratedObject.GetComponent<PlayerRoseControl>().InheritStatus(GetHP, GetWay);
                        Debug.Log("�X�e�[�^�X�̈��p���ɐ���");
                    }
                    break;
                //�u�^���|�|�v
                case 2:
                    {
                        GeneratedObject.GetComponent<PlayerDandelionControl>().InheritStatus(GetHP, GetWay);
                        Debug.Log("�X�e�[�^�X�̈��p���ɐ���");
                    }
                    break;
                //�u�A�T�K�I�v
                case 3:
                    {
                        GeneratedObject.GetComponent<PlayerMorninggloryControl>().InheritStatus(GetHP, GetWay);
                        Debug.Log("�X�e�[�^�X�̈��p���ɐ���");
                    }
                    break;
            }
        }
        else
        {
            Debug.Log("���ɃC���X�^���X������I�u�W�F�N�g������܂���");
        }
    }

    public void GetPlayerStatus(Vector2 GetPos,int GetHP)
    {

    }
    public void  UpdatePlayerHP( int GetHP)
    {
        PlayerHP = GetHP;
    }
    /// <summary>
    ///�֐����FGetPlayerObject()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�FPlayer��Object���擾
    /// </summary>
    public GameObject GetPlayerObject()
    {
        return Player;
    }
    public Vector2 GetPlayerPos()
    {
        return Player.transform.position;
    }
    public int GetEP()
    {
        return EP_Remain;
    }
    public void SetEP(int ep)
    {
        EP_Remain = ep;
    }
    public void AddEP(int Add)
    {
        EP_Remain += Add;
    }
    public bool EP_CheckLimit()
    {
        if (EP_Remain >= EP_Max)
        { return true; }
        else
        { return false; }
    }

    public void SetReturnPos(Vector2 Pos)
    {
        ReturnPos = Pos;
    }

    public Vector2 GetReturnPos()
    {
        return ReturnPos;
    }

    public void SetCoolTimer(float  timer ,float max)
    {
        CT_Remain = timer;
        CT_Max = max;
    }

    /// <summary>
    ///�֐����FAddjustSteyPlayer()<br />
    ///�������F�Ȃ�	<br />
    ///�߂�l�F�Ȃ� <br />
    ///�p�@�r�FPlayer����ʓ��ɂ���悤�ɒ���
    /// </summary>
    private void AddjustSteyPlayer()
    {
        if (Player != null)
        {
            if (Player.transform.position.x > this.transform.position.x + ScreenOutRange)
            {
                Debug.Log("�E�ɂ͂ݏo�Ă��܂�");
                Player.transform.Translate((this.transform.position.x + ScreenOutRange - Player.transform.position.x), 0.0f, 0.0f);
            }
            if (Player.transform.position.x < this.transform.position.x - ScreenOutRange)
            {
                Debug.Log("���ɂ͂ݏo�Ă��܂�");
                Player.transform.Translate((this.transform.position.x - ScreenOutRange - Player.transform.position.x), 0.0f, 0.0f);
            }
        }
    }

    public void OnTriggerStey2D(Collider2D collision)
    {
        //�ՓˑΏۂ��v���C���[
        {
            if (collision.gameObject.tag == "Player")
            {
                Debug.Log("�v���C���[����ʓ��ɂ��܂�");
                AddjustStayScreenF = true;
            }
        }

    }

    public void OnCollisionStey2D(Collider2D collision)
    {
        //�ՓˑΏۂ��v���C���[
        {
            if (collision.gameObject.tag == "Player")
            {
                Debug.Log("�v���C���[����ʓ��ɂ��܂�");
                AddjustStayScreenF = true;
            }
        }

    }
}
