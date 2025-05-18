#include "Input.h"
#include <windows.h>


Input::Input()
{
}

Input::~Input()
{
}


// �L�[��������Ă��邩�m�F����֐�
bool Input::GetKey(int key) {
    return (GetAsyncKeyState(key) & 0x8000) != 0;
}
bool Input::GetKeyDown(int key) {
    // ���݂̃L�[�̏�Ԃ��擾
    bool isCurrentlyPressed = (GetAsyncKeyState(key) & 0x8000) != 0;

    // �O��̃L�[��Ԃ��擾�i����� false �Ƃ݂Ȃ��j
    bool wasPreviouslyPressed = previousKeyStates[key];

    // �������u�Ԃ��ǂ����𔻒�
    bool isPressedOnce = isCurrentlyPressed && !wasPreviouslyPressed;

    // ���݂̏�Ԃ��L�^���Ă���
    previousKeyStates[key] = isCurrentlyPressed;

    return isPressedOnce;
}

bool Input::GetKeyKeep(int key, int count) {
    // ���݂̃L�[�̏�Ԃ��擾
    bool isCurrentlyPressed = (GetAsyncKeyState(key) & 0x8000) != 0;

    if (isCurrentlyPressed) {
        // ������Ă���ꍇ�̓J�E���g�𑝂₷
        keyPressCount[key]++;
        // �J�E���g��臒l�𒴂����� true ��Ԃ�
        if (keyPressCount[key] >= count) {
            return true;
        }
    }
    else {
        // ������Ă��Ȃ��ꍇ�̓J�E���g�����Z�b�g
        keyPressCount[key] = 0;
    }

    return false;
}

