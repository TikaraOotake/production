#pragma once

#include <string>
#include <iostream>

using namespace std;

// カーソルの位置を指定する
void moveCursorTo(int x, int y) {
	cout << "\033[" << y << ";" << x << "H" << flush;
}
