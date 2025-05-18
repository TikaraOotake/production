#include "Input.h"
#include <windows.h>


Input::Input()
{
}

Input::~Input()
{
}


// キーが押されているか確認する関数
bool Input::GetKey(int key) {
    return (GetAsyncKeyState(key) & 0x8000) != 0;
}
bool Input::GetKeyDown(int key) {
    // 現在のキーの状態を取得
    bool isCurrentlyPressed = (GetAsyncKeyState(key) & 0x8000) != 0;

    // 前回のキー状態を取得（初回は false とみなす）
    bool wasPreviouslyPressed = previousKeyStates[key];

    // 押した瞬間かどうかを判定
    bool isPressedOnce = isCurrentlyPressed && !wasPreviouslyPressed;

    // 現在の状態を記録しておく
    previousKeyStates[key] = isCurrentlyPressed;

    return isPressedOnce;
}

bool Input::GetKeyKeep(int key, int count) {
    // 現在のキーの状態を取得
    bool isCurrentlyPressed = (GetAsyncKeyState(key) & 0x8000) != 0;

    if (isCurrentlyPressed) {
        // 押されている場合はカウントを増やす
        keyPressCount[key]++;
        // カウントが閾値を超えたら true を返す
        if (keyPressCount[key] >= count) {
            return true;
        }
    }
    else {
        // 押されていない場合はカウントをリセット
        keyPressCount[key] = 0;
    }

    return false;
}

