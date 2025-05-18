#pragma once

#include <iostream>
#include <windows.h>
#include <chrono>
#include "Vector2D.h"



using namespace std::chrono;
using namespace std;

// �O���錾��ǉ�
class Mino;
class GridField;
class ScreenDrawer;
class Sound;
class GameStatus;
class UI;

class GameManager
{
public:
	GameManager();
	~GameManager();

	void playGame();
    void ResetMino();

    void Event_DeleteLine();
    void Event_Dead();
    void Event_SwapMino();
    void Event_LandMino();
    void Event_Gameover();
    void Event_GameTitle();
    void Event_GameStart();
    void Event_GameEnd();
private:
    int m_GameSceneID;//0�^�C�g���@1�V���O���v���C�@2�Q�[���I�[�o�[�@-1�I��

    GridField* m_pGridField;
    Mino* m_pMino;
    ScreenDrawer* m_pScreenDrawer;
    Sound* m_pSound;
    GameStatus* m_pGameStatus;
    UI* m_pUI;

    void WaitForNextFrame(time_point<high_resolution_clock>& previousTime);//�t���[���Ԃ̎��Ԑ���

    const double FRAME_TIME = 1.0 / 30.0; // 20 FPS (1�t���[�� = 0.05�b);
};
