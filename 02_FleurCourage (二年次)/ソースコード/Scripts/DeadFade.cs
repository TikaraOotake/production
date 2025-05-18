using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeadFade : MonoBehaviour
{
    GameObject playerRose;
    PlayerRoseControl fade;
    // Start is called before the first frame update
    void Start()
    {
        playerRose = GameObject.Find("PlayerRose");
        fade = playerRose.GetComponent<PlayerRoseControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fade.dead == true)
        {
            StartCoroutine("Color_FadeIn");
        }
    }

    IEnumerator Color_FadeIn()
    {
        //画面をフェードアウトさせるコールチン

        //色を変えるゲームオブジェクトからImageコンポーネントを取得
        Image fade = GetComponent<Image>();

        //フェード元の色を設定（黒 透明）
        fade.color = new Color((0.0f / 255.0f), (0.0f / 255.0f), (0.0f / 255.0f), (0.0f / 255.0f));

        //フェードインにかかる時間（秒）
        const float fade_time = 0.7f;

        //ループ回数（0はエラー）
        const int loop_count = 10;

        //ウェイト時間算出
        float wait_time = fade_time / loop_count;

        //色の間隔を算出
        float alpha_interval = 255.0f / loop_count;

        //色を徐々に変えるループ
        for (float alpha = 0.0f; alpha <= 255.0f; alpha += alpha_interval)
        {
            //待ち時間
            yield return new WaitForSeconds(wait_time);

            //Alpha値を少しずつ上げる
            Color new_color = fade.color;
            new_color.a = alpha / 255.0f;
            fade.color = new_color;
        }
    }
}
