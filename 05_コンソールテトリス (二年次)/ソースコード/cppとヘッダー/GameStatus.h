#pragma once



class GameStatus
{
public:
	GameStatus();
	~GameStatus();

	void ResetStatus();//‚Ù‚Ú‚·‚×‚Ä‚Ì•Ï”‚ğ‰Šú‰»

	int GetNextMino(int _Num);
	int GetHoldMino();
	void RotateNextMino();//‡‚ğ‰ñ‚·
	int SwapHoldMino(int _DropMino);//—‰º’†‚Ìƒ~ƒm‚Æ•Û’†‚Ìƒ~ƒm‚ğ“ü‚ê‘Ö‚¦‚é

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


