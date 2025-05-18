using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallFloor : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private PlatformEffector2D _platformEffector;
    public int cnt = 0;         //当たった回数の記録用
    public float FallTimer = 0; //落ちるまでの時間

    // インスペクターからスプライトを受け取っておく
    [SerializeField]
    private Sprite NewSprite;   //変更後の画像
    [SerializeField]
    private Sprite NewSprite2;  //元に戻った時の画像

    // Start is called before the first frame update
    void Start()
    {
        //コンポーネントを取得しておく
        _platformEffector = GetComponent<PlatformEffector2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //当たった回数で判定
        if(cnt >= 1)
        {
            //タイマーを起動して指定時間を超えたら
            FallTimer += Time.deltaTime;
            if(FallTimer >= 2.0f)
            {
                //足場の判定を消して落ちるように
                _platformEffector.rotationalOffset = 180f;
                spriteRenderer.sprite = NewSprite;
            }

            //さらに指定時間を超えたら
            if(FallTimer >= 4.0f)
            {
                //足場を元に戻して、タイマーや当たった回数の記録をリセットする
                _platformEffector.rotationalOffset = 0f;
                spriteRenderer.sprite = NewSprite2;
                cnt = 0;
                FallTimer = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       // 衝突したオブジェクト名がPlayerなら
        if (collision.gameObject.name == "Player")
        {
            //当たった回数を増やす
            ++cnt;
        }
    }

    /*/private void OnTriggerExit2D(Collider2D collision)
    {
        //--cnt;
    }/*/
}
