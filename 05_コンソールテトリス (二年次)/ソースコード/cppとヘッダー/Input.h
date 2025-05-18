#pragma once

#include <unordered_map>


class Input
{
public:
	Input();
	~Input();

    bool GetKeyDown(int key);
    bool GetKey(int key);
    bool GetKeyKeep(int key, int count);
private:
    // キーの状態を記録するマップ
    std::unordered_map<int, bool> previousKeyStates;
    // キーのカウントを保持するマップ
    std::unordered_map<int, int> keyPressCount;
};
