#pragma once

//描画を行うのに必要なもの
#include"Fwk/Graphics/Texture.h"
#include"Fwk/Graphics/Sprite.h"
using namespace Lib::Graphics;

//ベクターを使うのに必要な物
#include "Lib/Math/Vector.h"
using namespace Lib::Math;

class MenuUI
{
public:
	MenuUI();
	~MenuUI();

	void Init();
	void Update();
	void Render();
	void Term();

private:
	int m_ChoiceNum;//入力の段階を記録

	int m_Mode;//0:非表示 1:タイトル 2:ゲームオーバー

	void AdjustChoice(int _SelectRange);
};

