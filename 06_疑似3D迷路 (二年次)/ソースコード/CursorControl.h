#pragma once

#include <string>
#include <iostream>

using namespace std;

// �J�[�\���̈ʒu���w�肷��
void moveCursorTo(int x, int y) {
	cout << "\033[" << y << ";" << x << "H" << flush;
}
