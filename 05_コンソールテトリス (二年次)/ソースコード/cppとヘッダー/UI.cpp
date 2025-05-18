#include "UI.h"

#include <windows.h>
#include "Input.h"
#include "GameManager.h"
#include "ScreenDrawer.h"

UI::UI():
	m_SelectNum(0),
    m_pGameManager(nullptr),
    m_pScreenDrawer(nullptr),
    input(new Input)
{
}

UI::~UI()
{
    if (input != nullptr) { delete input; input = nullptr; }
}
void UI::GameoverSelecte()
{
	//���͂��󂯕t����
    if (input->GetKeyDown('S') || (input->GetKeyDown(VK_NUMPAD2)))//��
    {
        ++m_SelectNum;
    }
    if (input->GetKeyDown('W') || (input->GetKeyDown(VK_NUMPAD8)))//��
    {
        --m_SelectNum;
    }
    if (input->GetKeyDown(VK_SPACE) || (input->GetKeyDown(VK_NUMPAD0)) || (input->GetKeyDown(VK_RETURN)))//����
    {
        if (m_SelectNum == 0)
        {
            //�R���e�B�j���[
            if (m_pGameManager != nullptr)
            {
                m_pGameManager->Event_GameStart();
            }
        }
        if (m_SelectNum == 1)
        {
            //�^�C�g�����
            if (m_pGameManager != nullptr)
            {
                m_pGameManager->Event_GameTitle();
            }
        }
        if (m_SelectNum == 2)
        {
            //�Q�[���G���h(�v���O�������I��)
            if (m_pGameManager != nullptr)
            {
                m_pGameManager->Event_GameEnd();
            }
        }
    }

    //���͒l��␳
    AddjustSelectNum(3);//�I�����̐��͎O��
}
void UI::GameTitleSelecte()
{
    //���͂��󂯕t����
    if (input->GetKeyDown('S') || (input->GetKeyDown(VK_NUMPAD2)))//��
    {
        ++m_SelectNum;
    }
    if (input->GetKeyDown('W') || (input->GetKeyDown(VK_NUMPAD8)))//��
    {
        --m_SelectNum;
    }
    if (input->GetKeyDown(VK_SPACE) || (input->GetKeyDown(VK_NUMPAD0)) || (input->GetKeyDown(VK_RETURN)))//����
    {
        if (m_SelectNum == 0)
        {
            //�Q�[���X�^�[�g
            if (m_pGameManager != nullptr)
            {
                m_pGameManager->Event_GameStart();
            }
        }
        if (m_SelectNum == 1)//�Q�[���G���h
        {
            //�Q�[���G���h(�v���O�������I��)
            if (m_pGameManager != nullptr)
            {
                m_pGameManager->Event_GameEnd();
            }
        }
    }

    //���͒l��␳
    AddjustSelectNum(2);//�I�����̐��͓��
}
void UI::AddjustSelectNum(int _ChoicesNum)
{
    //���͒l��␳
    while (m_SelectNum < 0)
    {
        m_SelectNum += _ChoicesNum;
    }
    while (m_SelectNum >= _ChoicesNum)
    {
        m_SelectNum -= _ChoicesNum;
    }
}
void UI::SetGameManager(GameManager* _pGameManager)
{
    m_pGameManager = _pGameManager;
}
void UI::SetScreenDrawer(ScreenDrawer* _pScreenDrawer)
{
    m_pScreenDrawer = _pScreenDrawer;
}
int UI::GetSelectNum()
{
    return m_SelectNum;
}