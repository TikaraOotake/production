#pragma once



class GameStatus
{
public:
	GameStatus();
	~GameStatus();

	void ResetStatus();//�قڂ��ׂĂ̕ϐ���������

	int GetNextMino(int _Num);
	int GetHoldMino();
	void RotateNextMino();//������
	int SwapHoldMino(int _DropMino);//�������̃~�m�ƕێ����̃~�m�����ւ���

	void SetDeadFlag(bool _Flag);
	bool GetDeadFlag();

	void SetLevel(int _Level);
	int GetLevel();

	void SetLines(int _Lines);
	int GetLines();

	void SetScore(int _Score);
	int GetScore();

	void Update_GameStats();

private:
	int m_Level;
	int m_EXP;
	int m_Score;
	int m_NextMino[6];
	int m_HoldMino;
	int m_Lines;
	bool m_DeadFlag;

	int MinoLottery();
	bool m_UseMino[7];
};


