#pragma once

//前方宣言
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

	int m_SelectNum;//何番の選択項目を選んでいるか記録

	void AddjustSelectNum(int _ChoicesNum);

	GameManager* m_pGameManager;
	ScreenDrawer* m_pScreenDrawer;

	Input* input;//入力受け付け用
};


