using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallFloor : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private PlatformEffector2D _platformEffector;
    public int cnt = 0;         //���������񐔂̋L�^�p
    public float FallTimer = 0; //������܂ł̎���

    // �C���X�y�N�^�[����X�v���C�g���󂯎���Ă���
    [SerializeField]
    private Sprite NewSprite;   //�ύX��̉摜
    [SerializeField]
    private Sprite NewSprite2;  //���ɖ߂������̉摜

    // Start is called before the first frame update
    void Start()
    {
        //�R���|�[�l���g���擾���Ă���
        _platformEffector = GetComponent<PlatformEffector2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //���������񐔂Ŕ���
        if(cnt >= 1)
        {
            //�^�C�}�[���N�����Ďw�莞�Ԃ𒴂�����
            FallTimer += Time.deltaTime;
            if(FallTimer >= 2.0f)
            {
                //����̔���������ė�����悤��
                _platformEffector.rotationalOffset = 180f;
                spriteRenderer.sprite = NewSprite;
            }

            //����Ɏw�莞�Ԃ𒴂�����
            if(FallTimer >= 4.0f)
            {
                //��������ɖ߂��āA�^�C�}�[�ⓖ�������񐔂̋L�^�����Z�b�g����
                _platformEffector.rotationalOffset = 0f;
                spriteRenderer.sprite = NewSprite2;
                cnt = 0;
                FallTimer = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       // �Փ˂����I�u�W�F�N�g����Player�Ȃ�
        if (collision.gameObject.name == "Player")
        {
            //���������񐔂𑝂₷
            ++cnt;
        }
    }

    /*/private void OnTriggerExit2D(Collider2D collision)
    {
        //--cnt;
    }/*/
}
