#include <iostream>
#include <conio.h>
#include <string>
#include <windows.h>
#include <thread>
#include <chrono>

#include "Vector2D.h"
#include "GameManager.h"
#include "MapManager.h"
#include "Player.h"
#include "ViewManager.h"
#include "CursorControl.h"

using namespace std;

int main()
{
	// コンソールの文字コードをUTF-8に設定
	//SetConsoleOutputCP(CP_UTF8);
	// 入力もUTF-8に設定（必要に応じて）
	//SetConsoleCP(CP_UTF8);

	// コンソールのクリア（最初の一回だけ）
	cout << "\033[2J" << flush;

	//セットアップ
	MapManager map;
	map.GenerateMapData();
	//map.ShowMapDateTest();

	Player player;
	player.SetMapMNG(&map);

	ViewManager view;
	view.SetPlayer(&player);

	//画面クリア
	system("cls");

	//描画のセットアップ
	view.SettingDraw();

	do {
		moveCursorTo(1, 1);  // 左上にカーソルを移動

		//無理やり表示します
		for (int y = 0; y < 15; ++y)
		{
			for (int x = 0; x < 15; ++x)
			{
				int id=map.GetMapData(x,y);
				if (player.GetPlayerPos().x == x && player.GetPlayerPos().y == y)
				{
					string Way = player.GetPlayerWay();
					view.DrawGridIndex(Way);

				}
				else
					if (id == 0)
					{
						view.DrawGridIndex(0);
					}
					else
						if (id == 1)
						{
							view.DrawGridIndex(1);
						}
						else
							if (id == 2)
							{
								view.DrawGridIndex(2);
							}
							else
								if (id == 3)
								{
									view.DrawGridIndex(3);
								}
			}
			cout << endl;
		}
		//一人称視点を描画
		view.DrawFPV();

		player.UpdatePlayer();

		//扉と接触したかチェック
		if (map.GetMapData(player.GetPlayerPos()) == 2)
		{
			//クリアしたため新たに迷路を生成
			map.GenerateMapData();
			player.RandomTeleportPos();

			//画面クリア
			system("cls");

			//演出
			view.DrawFakeLoading();
		}

		//一気に描画
		cout << flush;

		//画面クリア
		//system("cls");

	} while (true);
}


/*
int main() {

	

	while (true) {
		// カーソルを移動してカウントを出力
		moveCursorTo(1, 1);  // 左上にカーソルを移動
		cout << "カウント: "  << flush;

	}

	return 0;
}
*/