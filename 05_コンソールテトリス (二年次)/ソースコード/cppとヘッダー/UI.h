#pragma once

//�O���錾
class Input;
class GameManager;
class ScreenDrawer;

class UI
{
public:
	UI();
	~UI();

	void GameoverSelecte();
	void GameTitleSelecte();
	void SetGameManager(GameManager* _pGameManager);
	void SetScreenDrawer(ScreenDrawer* _pScreenDrawer);
	int GetSelectNum();
private:

	int m_SelectNum;//���Ԃ̑I�����ڂ�I��ł��邩�L�^

	void AddjustSelectNum(int _ChoicesNum);

	GameManager* m_pGameManager;
	ScreenDrawer* m_pScreenDrawer;

	Input* input;//���͎󂯕t���p
};


