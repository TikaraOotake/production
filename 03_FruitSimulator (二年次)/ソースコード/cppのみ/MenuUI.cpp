#include"MenuUI.h"

#include<Fwk/Framework.h>

MenuUI::MenuUI():
	m_ChoiceNum(0),
	m_Mode(0)
{
	Init();
}

MenuUI::~MenuUI()
{
}


void MenuUI::Init()
{
	m_ChoiceNum = 0;
}
void MenuUI::Update()
{
	if (m_Mode == 1)
	{
		if (Input_I->IsKeyDown(VK_SPACE))
		{
			if (m_ChoiceNum == 0)
			{

			}
			if (m_ChoiceNum == 1)
			{

			}
		}
		if (Input_I->IsKeyDown('W'))
		{
			--m_ChoiceNum;
		}
		if (Input_I->IsKeyDown('S'))
		{
			++m_ChoiceNum;
		}

		AdjustChoice(2);
	}
	if (m_Mode == 2)
	{
		if (Input_I->IsKeyDown(VK_SPACE))
		{
			if (m_ChoiceNum == 0)//コンティニュー
			{

			}
			if (m_ChoiceNum == 1)
			{

			}
			if (m_ChoiceNum == 2)
			{

			}
		}
		if (Input_I->IsKeyDown('W'))
		{
			--m_ChoiceNum;
		}
		if (Input_I->IsKeyDown('S'))
		{
			++m_ChoiceNum;
		}
		
		AdjustChoice(3);
	}
}
void MenuUI::Render()
{

}
void MenuUI::Term()
{

}

void MenuUI::AdjustChoice(int _SelectRange)
{
	while (m_ChoiceNum < 0)
	{
		m_ChoiceNum += _SelectRange;
	}
	while (m_ChoiceNum > _SelectRange - 1)
	{
		m_ChoiceNum -= _SelectRange;
	}
}