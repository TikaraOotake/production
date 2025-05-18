using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    private bool _timerFlag;                        //�^�C�}�[���J�E���g�_�E��������t���O

    [SerializeField]
    private int _changeNum;

    [SerializeField, Tooltip("Scene��؂�ւ��鎞��")]
    private float ChangeTime;                       //Scene��؂�ւ��鎞��
    private float _firstChangeTime;                 //Scene��؂�ւ��鎞�Ԃ̏����l

    // Start is called before the first frame update
    void Start()
    {
        //������
        _timerFlag = false;
        _firstChangeTime = ChangeTime;
    }

    // Update is called once per frame
    void Update()
    {
        //�^�C�}�[���J�E���g�_�E��������
        if (_timerFlag)
        {
            ChangeTime -= Time.deltaTime;
        }
        //�l���󂯎���Đ؂�ւ���H
        //�X�C�b�`�P�[�X���g��

        switch(GetChangeNum())
        {
            case 0:
                //�^�C�g������X�e�[�W��
                TitleToStage1();
                break;

            case 1:
                TitleToPrologue();
                break;

            case 2:
                //�v�����[�O����X�e�[�W��
                PrologueToStage1();
                break;

            case 3:
                //�X�e�[�W����Q�[���I�[�o�[��
                Stage1ToGameOver();
                break;

            case 4:
                //�X�e�[�W����Q�[���N���A��
                Stage1TogameClear();
                break;

            case 5:
                //�Q�[���N���A���Q�[���I�[�o�[����^�C�g����
                GameOverAndGameClearTotitle();
                break;
        }     
    }
    /// <summary>
    /// �֐����FSetChangeNum
    /// ���@���Fint
    /// �߂�l�F����
    /// scene��؂�ւ���l��ݒ�
    /// </summary>
    public void SetChangeNum(int _changenum)
        {
            _changeNum = _changenum;
        }

    /// <summary>
    /// �֐����FGetChangeNum
    /// ���@���F����
    /// �߂�l�Fint
    /// scene��؂�ւ���l���擾
    /// </summary>
    int GetChangeNum()
    {
        return _changeNum;
    }

    /// <summary>
    /// �֐����FTitleToStage1
    /// ���@���F����
    /// �߂�l�F����
    /// �^�C�g������X�e�[�W�P��Scene��؂�ւ���
    /// </summary>
    void TitleToStage1()
    {
        //Title����Stage1�ɐ؂�ւ�
        if (SceneManager.GetActiveScene().name == "Title")
        {
            if (Input.GetKeyDown(KeyCode.A)||(Input.GetKeyDown(KeyCode.Joystick1Button0)))
            {
                _timerFlag = true;
            }
            if (ChangeTime < 0.0f)
            {
                _timerFlag = false;
                ChangeTime = _firstChangeTime;
                SceneManager.LoadScene("TutorialStage");
            }
        }
    }

    void TitleToPrologue()
    {
        if (SceneManager.GetActiveScene().name == "Title")
        {
            SceneManager.LoadScene("Prologue");
        }
    }
    void PrologueToStage1()
    {
        //Prologue����Stage1�ɐ؂�ւ�
        if (/*�I������ &&*/SceneManager.GetActiveScene().name == "Prologue")
        {
            SceneManager.LoadScene("Stage1");
        }
    }
    void Stage1ToGameOver()
    {
        //Stage1����GameOver�ɐ؂�ւ�
        if (/*�Q�[���I�[�o�[���� &&*/ SceneManager.GetActiveScene().name == "Stage1")
        {
            SceneManager.LoadScene("GameOver");
        }
    }
    void Stage1TogameClear()
    {
        //Stage1����GameClear�ɐ؂�ւ�
        if (/*�Q�[���N���A���� &&*/ SceneManager.GetActiveScene().name == "Stage1")
        {
            SceneManager.LoadScene("GameClear");
        }
    }
    void GameOverAndGameClearTotitle()
    {
        //GameOver�܂���GameClear����Title�ɐ؂�ւ�
        if (SceneManager.GetActiveScene().name == "GameOver" || SceneManager.GetActiveScene().name == "GameClear")
        {
            SceneManager.LoadScene("Title");
        }
    }
}
