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
	//入力を受け付ける
    if (input->GetKeyDown('S') || (input->GetKeyDown(VK_NUMPAD2)))//下
    {
        ++m_SelectNum;
    }
    if (input->GetKeyDown('W') || (input->GetKeyDown(VK_NUMPAD8)))//下
    {
        --m_SelectNum;
    }
    if (input->GetKeyDown(VK_SPACE) || (input->GetKeyDown(VK_NUMPAD0)) || (input->GetKeyDown(VK_RETURN)))//決定
    {
        if (m_SelectNum == 0)
        {
            //コンティニュー
            if (m_pGameManager != nullptr)
            {
                m_pGameManager->Event_GameStart();
            }
        }
        if (m_SelectNum == 1)
        {
            //タイトル画面
            if (m_pGameManager != nullptr)
            {
                m_pGameManager->Event_GameTitle();
            }
        }
        if (m_SelectNum == 2)
        {
            //ゲームエンド(プログラムを終了)
            if (m_pGameManager != nullptr)
            {
                m_pGameManager->Event_GameEnd();
            }
        }
    }

    //入力値を補正
    AddjustSelectNum(3);//選択肢の数は三個
}
void UI::GameTitleSelecte()
{
    //入力を受け付ける
    if (input->GetKeyDown('S') || (input->GetKeyDown(VK_NUMPAD2)))//下
    {
        ++m_SelectNum;
    }
    if (input->GetKeyDown('W') || (input->GetKeyDown(VK_NUMPAD8)))//下
    {
        --m_SelectNum;
    }
    if (input->GetKeyDown(VK_SPACE) || (input->GetKeyDown(VK_NUMPAD0)) || (input->GetKeyDown(VK_RETURN)))//決定
    {
        if (m_SelectNum == 0)
        {
            //ゲームスタート
            if (m_pGameManager != nullptr)
            {
                m_pGameManager->Event_GameStart();
            }
        }
        if (m_SelectNum == 1)//ゲームエンド
        {
            //ゲームエンド(プログラムを終了)
            if (m_pGameManager != nullptr)
            {
                m_pGameManager->Event_GameEnd();
            }
        }
    }

    //入力値を補正
    AddjustSelectNum(2);//選択肢の数は二個
}
void UI::AddjustSelectNum(int _ChoicesNum)
{
    //入力値を補正
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