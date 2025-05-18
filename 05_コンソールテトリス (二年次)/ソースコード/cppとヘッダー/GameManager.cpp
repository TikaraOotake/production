#include "GameManager.h"
#include "Mino.h" 
#include "GridField.h"
#include "ScreenDrawer.h"
#include "Sound.h"
#include "GameStatus.h"
#include "UI.h"

GameManager::GameManager() :
    m_GameSceneID(0),//����͑�inGame��
    m_pGridField(nullptr),
    m_pMino(nullptr),
    m_pScreenDrawer(nullptr),
    m_pSound(nullptr),
    m_pGameStatus(nullptr),
    m_pUI(nullptr)
{
}

GameManager::~GameManager()
{
    //�������
    if (m_pGridField != nullptr)    { delete m_pGridField;       m_pGridField = nullptr; }
    if (m_pMino != nullptr )        { delete  m_pMino;           m_pMino = nullptr; }
    if (m_pScreenDrawer != nullptr) { delete m_pScreenDrawer;    m_pScreenDrawer = nullptr; }
    if (m_pSound != nullptr)        { delete m_pSound;           m_pSound = nullptr; }
    if (m_pGameStatus != nullptr)   { delete m_pGameStatus;      m_pGameStatus = nullptr; }
    if (m_pUI != nullptr)           { delete m_pUI;              m_pUI = nullptr; }
}
void GameManager::playGame()
{
    // �����̃V�[�h�����݂̎����ŏ�����
    std::srand(std::time(0));

    //�Q�[���X�e�[�^�X���쐬
    m_pGameStatus = new GameStatus;

    //�O���b�h�t�B�[���h���쐬
    m_pGridField = new GridField;
    m_pGridField->SetGameManager(this);

    //�~�m���쐬
    m_pMino = new Mino;
    m_pMino->SetGameManager(this);//�Q�[���}�l�[�W���[�Z�b�g
    m_pMino->SetGridField(m_pGridField);//�O���b�h�t�B�[���h�Z�b�g
    m_pMino->Reset(m_pGameStatus->GetNextMino(0), m_pGameStatus->GetLevel());//�~�m�ƃ��x�����w��
    m_pGameStatus->RotateNextMino();//�����l�߂Ă���

    //UI���쐬
    m_pUI = new UI;
    m_pUI->SetGameManager(this);//�Q�[���}�l�[�W���[�Z�b�g

    //��ʕ`����쐬
    m_pScreenDrawer = new ScreenDrawer;
    m_pScreenDrawer->enableAnsiEscape();//Windows�ł����������삳���邽�߂̂��܂��Ȃ��H

    //�������쐬
    m_pSound = new Sound;


    // ���ԊǗ�
    auto previousTime = high_resolution_clock::now();
    do {
        if (m_GameSceneID == 0)//�^�C�g��
        {
            m_pUI->GameTitleSelecte();//�I����ʂ𓮂���

            //�`��X�V
            m_pScreenDrawer->SetScreenData_Initialize();//�L�����o�X���܂������
            m_pScreenDrawer->SetScreenData_GameTitle();
            m_pScreenDrawer->SetScreenData_GameTitleSelect(m_pUI);//�I����ʂ�\��
            m_pScreenDrawer->DrawScreen();
        }
        else

            if (m_GameSceneID == 1)//�C���Q�[��
            {
                //�Q�[���X�e�[�^�X�X�V
                m_pGameStatus->Update_GameStats();
                //�~�m�X�V
                m_pMino->Update_Mino();

                m_pScreenDrawer->SetScreenData_Initialize();//�L�����o�X���܂������
                //�\�����������̂��w��
                m_pScreenDrawer->SetScreenData_GameStatus(m_pGameStatus);
                m_pScreenDrawer->SetScreenData_NextMino(m_pGameStatus);
                m_pScreenDrawer->SetScreenData_HoldMino(m_pGameStatus);
                m_pScreenDrawer->SetScreenData_Field(m_pGridField, m_pMino);//

                //�`��X�V
                m_pScreenDrawer->DrawScreen();

                if ((GetAsyncKeyState('T') & 0x8000) != 0)//�e�X�g�p
                {
                    //SE���Đ�
                    m_pSound->PlaySoundEffect(1);
                }

                //���S���Ă��邩�m�F
                if (m_pGameStatus->GetDeadFlag())
                {
                    //�Q�[���I�[�o�[�V�[��
                    m_GameSceneID = 2;
                    //BGM���I��
                    m_pSound->stop();

                    //�t�F�[�h�A�E�g����
                    m_pScreenDrawer->SetScreenData_Fadeout();
                }
            }

            else

                if (m_GameSceneID == 2)//�Q�[���I�[�o�[
                {
                    m_pUI->GameoverSelecte();//�I����ʂ𓮂���

                    //�`��X�V
                    m_pScreenDrawer->SetScreenData_GameOver();
                    m_pScreenDrawer->SetScreenData_GameOverSelect(m_pUI);//�I����ʂ�\��
                    m_pScreenDrawer->DrawScreen();
                }

        // �t���[���^�C������
        WaitForNextFrame(previousTime);
    } while (m_GameSceneID != -1);
}
void GameManager::ResetMino()
{
    //�K�v�ȕϐ����擾
    int Level = m_pGameStatus->GetLevel();
    int MinoID = m_pGameStatus->GetNextMino(0);

    m_pMino->Reset(MinoID, Level);//�~�m���w��
    m_pGameStatus->RotateNextMino();//�����l�߂Ă���

    //�`��X�V
    m_pScreenDrawer->SetScreenData_NextMino(m_pGameStatus);
    m_pScreenDrawer->DrawField(m_pGridField, m_pMino);
}
void GameManager::Event_DeleteLine()
{
    //�폜�������Z
    int Lines = m_pGameStatus->GetLines() + 1;
    m_pGameStatus->SetLines(Lines);
    //SE���Đ�
    m_pSound->PlaySoundEffect(1);
}
void GameManager::Event_Dead()
{
    //�X�e�[�^�X�����S��
    m_pGameStatus->SetDeadFlag(true);
}
void GameManager::Event_SwapMino()
{
    //�K�v�ȕϐ����擾
    int Level = m_pGameStatus->GetLevel();
    int DropMino = m_pMino->GetShapeID();
    int HoldMino = 0;

    //�~�m�����ւ���
    HoldMino = m_pGameStatus->SwapHoldMino(DropMino);
    m_pMino->Reset(HoldMino, Level);

    //��ʕ`����X�V
    m_pScreenDrawer->SetScreenData_NextMino(m_pGameStatus);//���̃~�m���X�V
    m_pScreenDrawer->DrawScreen();//�`��X�V
}
void GameManager::Event_LandMino()
{
    //�K�v�ȕϐ����擾
    int Level = m_pGameStatus->GetLevel();      //���݂̃��x�����擾
    int MinoID = m_pGameStatus->GetNextMino(0); //���ɏo������~�m���擾
    int ScoreTake = m_pGameStatus->GetScore();  //�Q�[���X�e�[�^�X�������Ă���X�R�A���擾
    int ScoreAdd = m_pMino->GetScore();         //�~�m�������Ă����X�R�A���擾


    int DeleteLineCounter = 0;//�����ꂽ��̐����L�^
    //�ő�4�i����C�ɏ����邱�Ƃ�����̂�4��J��Ԃ�
    for (int i = 0; i < 4; ++i)
    {
        //���C���폜
        bool Result = m_pGridField->CheckDeleteLine();
        if (Result == true) { ++DeleteLineCounter; }//�����ꂽ�����Z
    }

    //�����ꂽ��ɉ����ăX�R�A�v�Z
    if (DeleteLineCounter == 1) { ScoreAdd += 100; }//�V���O��
    if (DeleteLineCounter == 2) { ScoreAdd += 300; }//�_�u��
    if (DeleteLineCounter == 3) { ScoreAdd += 500; }//�g���v��
    if (DeleteLineCounter == 4) { ScoreAdd += 800; }//�e�g���X
    //�X�R�A���Z
    m_pGameStatus->SetScore(ScoreAdd + ScoreTake);

    //�~�m�����Z�b�g
    m_pMino->Reset(MinoID, Level);//�~�m���w��
    m_pGameStatus->RotateNextMino();//�����l�߂Ă���
}
void GameManager::Event_Gameover()
{

}
void GameManager::WaitForNextFrame(time_point<high_resolution_clock>& previousTime) {
    auto currentTime = high_resolution_clock::now();
    double deltaTime = duration<double>(currentTime - previousTime).count();

    // �t���[���������Ԃ��v�����A�ڕW�t���[�����Ԃ��ێ����邽�߂ɃX���[�v
    double frameElapsed = duration<double>(high_resolution_clock::now() - currentTime).count();
    double sleepTime = FRAME_TIME - frameElapsed;
    if (sleepTime > 0) {
        Sleep(static_cast<DWORD>(sleepTime * 1000)); // �b���~���b�ɕϊ�
    }

    // ���̃��[�v�p�Ɏ��Ԃ��X�V
    previousTime = high_resolution_clock::now();
}
void GameManager::Event_GameTitle()
{
    //�Q�[���V�[�����Z�b�g
    m_GameSceneID = 0;
}
void GameManager::Event_GameStart()
{
    //�Q�[���X�e�[�^�X������
    m_pGameStatus->ResetStatus();

    //�O���b�h�t�B�[���h������
    m_pGridField->ResetField();

    //���y���Đ�
    m_pSound->start();

    //�~�m���Z�b�g
    m_pMino->Reset(m_pGameStatus->GetNextMino(0), m_pGameStatus->GetLevel());//�~�m�ƃ��x�����w��
    m_pGameStatus->RotateNextMino();//�����l�߂Ă���

    //�Q�[���V�[�����Z�b�g
    m_GameSceneID = 1;
}
void GameManager::Event_GameEnd()
{
    //�Q�[�����I������
    m_GameSceneID = -1;
}
