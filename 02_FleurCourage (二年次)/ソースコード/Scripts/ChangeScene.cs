using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    private bool _timerFlag;                        //タイマーをカウントダウンさせるフラグ

    [SerializeField]
    private int _changeNum;

    [SerializeField, Tooltip("Sceneを切り替える時間")]
    private float ChangeTime;                       //Sceneを切り替える時間
    private float _firstChangeTime;                 //Sceneを切り替える時間の初期値

    // Start is called before the first frame update
    void Start()
    {
        //初期化
        _timerFlag = false;
        _firstChangeTime = ChangeTime;
    }

    // Update is called once per frame
    void Update()
    {
        //タイマーをカウントダウンさせる
        if (_timerFlag)
        {
            ChangeTime -= Time.deltaTime;
        }
        //値を受け取って切り替える？
        //スイッチケースを使う

        switch(GetChangeNum())
        {
            case 0:
                //タイトルからステージへ
                TitleToStage1();
                break;

            case 1:
                TitleToPrologue();
                break;

            case 2:
                //プロローグからステージへ
                PrologueToStage1();
                break;

            case 3:
                //ステージからゲームオーバーへ
                Stage1ToGameOver();
                break;

            case 4:
                //ステージからゲームクリアへ
                Stage1TogameClear();
                break;

            case 5:
                //ゲームクリアかゲームオーバーからタイトルへ
                GameOverAndGameClearTotitle();
                break;
        }     
    }
    /// <summary>
    /// 関数名：SetChangeNum
    /// 引　数：int
    /// 戻り値：無し
    /// sceneを切り替える値を設定
    /// </summary>
    public void SetChangeNum(int _changenum)
        {
            _changeNum = _changenum;
        }

    /// <summary>
    /// 関数名：GetChangeNum
    /// 引　数：無し
    /// 戻り値：int
    /// sceneを切り替える値を取得
    /// </summary>
    int GetChangeNum()
    {
        return _changeNum;
    }

    /// <summary>
    /// 関数名：TitleToStage1
    /// 引　数：無し
    /// 戻り値：無し
    /// タイトルからステージ１へSceneを切り替える
    /// </summary>
    void TitleToStage1()
    {
        //TitleからStage1に切り替え
        if (SceneManager.GetActiveScene().name == "Title")
        {
            if (Input.GetKeyDown(KeyCode.A)||(Input.GetKeyDown(KeyCode.Joystick1Button0)))
            {
                _timerFlag = true;
            }
            if (ChangeTime < 0.0f)
            {
                _timerFlag = false;
                ChangeTime = _firstChangeTime;
                SceneManager.LoadScene("TutorialStage");
            }
        }
    }

    void TitleToPrologue()
    {
        if (SceneManager.GetActiveScene().name == "Title")
        {
            SceneManager.LoadScene("Prologue");
        }
    }
    void PrologueToStage1()
    {
        //PrologueからStage1に切り替え
        if (/*終了条件 &&*/SceneManager.GetActiveScene().name == "Prologue")
        {
            SceneManager.LoadScene("Stage1");
        }
    }
    void Stage1ToGameOver()
    {
        //Stage1からGameOverに切り替え
        if (/*ゲームオーバー条件 &&*/ SceneManager.GetActiveScene().name == "Stage1")
        {
            SceneManager.LoadScene("GameOver");
        }
    }
    void Stage1TogameClear()
    {
        //Stage1からGameClearに切り替え
        if (/*ゲームクリア条件 &&*/ SceneManager.GetActiveScene().name == "Stage1")
        {
            SceneManager.LoadScene("GameClear");
        }
    }
    void GameOverAndGameClearTotitle()
    {
        //GameOverまたはGameClearからTitleに切り替え
        if (SceneManager.GetActiveScene().name == "GameOver" || SceneManager.GetActiveScene().name == "GameClear")
        {
            SceneManager.LoadScene("Title");
        }
    }
}
