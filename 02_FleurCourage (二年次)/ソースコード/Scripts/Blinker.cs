using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinker : MonoBehaviour
{
    // �_�ł�����Ώ�
    [SerializeField] private SpriteRenderer _target;

    // �_�Ŏ���[s]
    [SerializeField] private float _cycle = 1;

    private bool _isBlinking;
    private float _defaultAlpha;
    private double _time;

    /// <summary>
    /// �֐���:BeginBlink <br />
	/// ������:�Ȃ�       <br />
	/// �߂�l:�Ȃ�       <br />
	/// �_�ł��J�n����֐�
    /// </summary>
    public void BeginBlink()
    {
        // �_�Œ��͉������Ȃ�
        if (_isBlinking) return;

        //�_�ł̔����true��
        _isBlinking = true;

        // ���Ԃ��J�n���_�ɖ߂�
        _time = 0;
    }

    /// <summary>
	/// �֐���:EndBlink   <br />
	/// ������:�Ȃ�       <br />
	/// �߂�l:�Ȃ�       <br />
    /// �_�ł��I������
    /// </summary>
    public void EndBlink()
    {
        //�_�ł̔����false��
        _isBlinking = false;

        // ������Ԃ̃A���t�@�l�ɖ߂�
        SetAlpha(_defaultAlpha);
    }
    void Start()
    {
        _defaultAlpha = _target.color.a;
    }

    // Update is called once per frame
    void Update()
    {

        // �_�ł��Ă��Ȃ��Ƃ��͉������Ȃ�
        if (!_isBlinking) return;

        // �����������o�߂�����
        _time += Time.deltaTime;

        // ����cycle�ŌJ��Ԃ��l�̎擾
        // 0�`cycle�͈̔͂̒l��������
        var repeatValue = Mathf.Repeat((float)_time, _cycle);

        // ��������time�ɂ����閾�ŏ�Ԃ𔽉f
        // Image�̃A���t�@�l��ύX���Ă���
        SetAlpha(repeatValue >= _cycle * 0.5f ? 1 : 0);
    }

    /// <summary>
	/// �֐���:SetAlpha <br />
	/// ������:alpha    <br />
	/// �߂�l:�Ȃ�     <br />
    /// Image�̃A���t�@�l��ύX���郁�\�b�h
    /// </summary>
    private void SetAlpha(float alpha)
    {
        var color = _target.color;
        color.a = alpha;
        _target.color = color;
    }
}
