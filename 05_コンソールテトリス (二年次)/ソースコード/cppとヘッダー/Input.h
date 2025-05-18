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
    // �L�[�̏�Ԃ��L�^����}�b�v
    std::unordered_map<int, bool> previousKeyStates;
    // �L�[�̃J�E���g��ێ�����}�b�v
    std::unordered_map<int, int> keyPressCount;
};
