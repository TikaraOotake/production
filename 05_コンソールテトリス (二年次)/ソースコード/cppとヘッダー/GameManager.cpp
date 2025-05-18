#include "GameManager.h"
#include "Mino.h" 
#include "GridField.h"
#include "ScreenDrawer.h"
#include "Sound.h"
#include "GameStatus.h"
#include "UI.h"

GameManager::GameManager() :
    m_GameSceneID(0),//現状は即inGameへ
    m_pGridField(nullptr),
    m_pMino(nullptr),
    m_pScreenDrawer(nullptr),
    m_pSound(nullptr),
    m_pGameStatus(nullptr),
    m_pUI(nullptr)
{
}

GameManager::~GameManager()
{
    //解放処理
    if (m_pGridField != nullptr)    { delete m_pGridField;       m_pGridField = nullptr; }
    if (m_pMino != nullptr )        { delete  m_pMino;           m_pMino = nullptr; }
    if (m_pScreenDrawer != nullptr) { delete m_pScreenDrawer;    m_pScreenDrawer = nullptr; }
    if (m_pSound != nullptr)        { delete m_pSound;           m_pSound = nullptr; }
    if (m_pGameStatus != nullptr)   { delete m_pGameStatus;      m_pGameStatus = nullptr; }
    if (m_pUI != nullptr)           { delete m_pUI;              m_pUI = nullptr; }
}
void GameManager::playGame()
{
    // 乱数のシードを現在の時刻で初期化
    std::srand(std::time(0));

    //ゲームステータスを作成
    m_pGameStatus = new GameStatus;

    //グリッドフィールドを作成
    m_pGridField = new GridField;
    m_pGridField->SetGameManager(this);

    //ミノを作成
    m_pMino = new Mino;
    m_pMino->SetGameManager(this);//ゲームマネージャーセット
    m_pMino->SetGridField(m_pGridField);//グリッドフィールドセット
    m_pMino->Reset(m_pGameStatus->GetNextMino(0), m_pGameStatus->GetLevel());//ミノとレベルを指定
    m_pGameStatus->RotateNextMino();//順を詰めておく

    //UIを作成
    m_pUI = new UI;
    m_pUI->SetGameManager(this);//ゲームマネージャーセット

    //画面描画を作成
    m_pScreenDrawer = new ScreenDrawer;
    m_pScreenDrawer->enableAnsiEscape();//Windowsでも正しく動作させるためのおまじない？

    //音響を作成
    m_pSound = new Sound;


    // 時間管理
    auto previousTime = high_resolution_clock::now();
    do {
        if (m_GameSceneID == 0)//タイトル
        {
            m_pUI->GameTitleSelecte();//選択画面を動かす

            //描画更新
            m_pScreenDrawer->SetScreenData_Initialize();//キャンバスをまっさらに
            m_pScreenDrawer->SetScreenData_GameTitle();
            m_pScreenDrawer->SetScreenData_GameTitleSelect(m_pUI);//選択画面を表示
            m_pScreenDrawer->DrawScreen();
        }
        else

            if (m_GameSceneID == 1)//インゲーム
            {
                //ゲームステータス更新
                m_pGameStatus->Update_GameStats();
                //ミノ更新
                m_pMino->Update_Mino();

                m_pScreenDrawer->SetScreenData_Initialize();//キャンバスをまっさらに
                //表示したいものを指定
                m_pScreenDrawer->SetScreenData_GameStatus(m_pGameStatus);
                m_pScreenDrawer->SetScreenData_NextMino(m_pGameStatus);
                m_pScreenDrawer->SetScreenData_HoldMino(m_pGameStatus);
                m_pScreenDrawer->SetScreenData_Field(m_pGridField, m_pMino);//

                //描画更新
                m_pScreenDrawer->DrawScreen();

                if ((GetAsyncKeyState('T') & 0x8000) != 0)//テスト用
                {
                    //SEを再生
                    m_pSound->PlaySoundEffect(1);
                }

                //死亡しているか確認
                if (m_pGameStatus->GetDeadFlag())
                {
                    //ゲームオーバーシーン
                    m_GameSceneID = 2;
                    //BGMを終了
                    m_pSound->stop();

                    //フェードアウトする
                    m_pScreenDrawer->SetScreenData_Fadeout();
                }
            }

            else

                if (m_GameSceneID == 2)//ゲームオーバー
                {
                    m_pUI->GameoverSelecte();//選択画面を動かす

                    //描画更新
                    m_pScreenDrawer->SetScreenData_GameOver();
                    m_pScreenDrawer->SetScreenData_GameOverSelect(m_pUI);//選択画面を表示
                    m_pScreenDrawer->DrawScreen();
                }

        // フレームタイム調整
        WaitForNextFrame(previousTime);
    } while (m_GameSceneID != -1);
}
void GameManager::ResetMino()
{
    //必要な変数を取得
    int Level = m_pGameStatus->GetLevel();
    int MinoID = m_pGameStatus->GetNextMino(0);

    m_pMino->Reset(MinoID, Level);//ミノを指定
    m_pGameStatus->RotateNextMino();//順を詰めておく

    //描画更新
    m_pScreenDrawer->SetScreenData_NextMino(m_pGameStatus);
    m_pScreenDrawer->DrawField(m_pGridField, m_pMino);
}
void GameManager::Event_DeleteLine()
{
    //削除数を加算
    int Lines = m_pGameStatus->GetLines() + 1;
    m_pGameStatus->SetLines(Lines);
    //SEを再生
    m_pSound->PlaySoundEffect(1);
}
void GameManager::Event_Dead()
{
    //ステータスを死亡に
    m_pGameStatus->SetDeadFlag(true);
}
void GameManager::Event_SwapMino()
{
    //必要な変数を取得
    int Level = m_pGameStatus->GetLevel();
    int DropMino = m_pMino->GetShapeID();
    int HoldMino = 0;

    //ミノを入れ替える
    HoldMino = m_pGameStatus->SwapHoldMino(DropMino);
    m_pMino->Reset(HoldMino, Level);

    //画面描画を更新
    m_pScreenDrawer->SetScreenData_NextMino(m_pGameStatus);//次のミノを更新
    m_pScreenDrawer->DrawScreen();//描画更新
}
void GameManager::Event_LandMino()
{
    //必要な変数を取得
    int Level = m_pGameStatus->GetLevel();      //現在のレベルを取得
    int MinoID = m_pGameStatus->GetNextMino(0); //次に出現するミノを取得
    int ScoreTake = m_pGameStatus->GetScore();  //ゲームステータスが持っているスコアを取得
    int ScoreAdd = m_pMino->GetScore();         //ミノが持っていたスコアを取得


    int DeleteLineCounter = 0;//消された列の数を記録
    //最大4段が一気に消えることがあるので4回繰り返し
    for (int i = 0; i < 4; ++i)
    {
        //ライン削除
        bool Result = m_pGridField->CheckDeleteLine();
        if (Result == true) { ++DeleteLineCounter; }//消された分加算
    }

    //消された列に応じてスコア計算
    if (DeleteLineCounter == 1) { ScoreAdd += 100; }//シングル
    if (DeleteLineCounter == 2) { ScoreAdd += 300; }//ダブル
    if (DeleteLineCounter == 3) { ScoreAdd += 500; }//トリプル
    if (DeleteLineCounter == 4) { ScoreAdd += 800; }//テトリス
    //スコア加算
    m_pGameStatus->SetScore(ScoreAdd + ScoreTake);

    //ミノをリセット
    m_pMino->Reset(MinoID, Level);//ミノを指定
    m_pGameStatus->RotateNextMino();//順を詰めておく
}
void GameManager::Event_Gameover()
{

}
void GameManager::WaitForNextFrame(time_point<high_resolution_clock>& previousTime) {
    auto currentTime = high_resolution_clock::now();
    double deltaTime = duration<double>(currentTime - previousTime).count();

    // フレーム処理時間を計測し、目標フレーム時間を維持するためにスリープ
    double frameElapsed = duration<double>(high_resolution_clock::now() - currentTime).count();
    double sleepTime = FRAME_TIME - frameElapsed;
    if (sleepTime > 0) {
        Sleep(static_cast<DWORD>(sleepTime * 1000)); // 秒→ミリ秒に変換
    }

    // 次のループ用に時間を更新
    previousTime = high_resolution_clock::now();
}
void GameManager::Event_GameTitle()
{
    //ゲームシーンをセット
    m_GameSceneID = 0;
}
void GameManager::Event_GameStart()
{
    //ゲームステータス初期化
    m_pGameStatus->ResetStatus();

    //グリッドフィールド初期化
    m_pGridField->ResetField();

    //音楽を再生
    m_pSound->start();

    //ミノをセット
    m_pMino->Reset(m_pGameStatus->GetNextMino(0), m_pGameStatus->GetLevel());//ミノとレベルを指定
    m_pGameStatus->RotateNextMino();//順を詰めておく

    //ゲームシーンをセット
    m_GameSceneID = 1;
}
void GameManager::Event_GameEnd()
{
    //ゲームを終了する
    m_GameSceneID = -1;
}
