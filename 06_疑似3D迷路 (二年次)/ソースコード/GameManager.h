#pragma once

#include <string>
using namespace std;

class GameManager
{
public:
	GameManager();
	~GameManager();
	//メソッド
	void UpdateGame();
private:
	//メソッド

	//プロパティ
	string SceneName;

	int i;
};


