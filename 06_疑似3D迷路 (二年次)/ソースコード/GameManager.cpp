
#include "GameManager.h"

GameManager::GameManager() :SceneName("Title")
{
}

GameManager::~GameManager()
{
}
void GameManager::UpdateGame()
{
	//タイトル
	if (SceneName == "Title")
	{

	}
	else
		//　描画設定
		if (SceneName == "ScreenSetup")
		{

		}
		else
			//迷路
			if (SceneName == "MazeGame")
			{
			}
}