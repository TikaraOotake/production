#include"Shape.h"
#include<Fwk/Framework.h>

#include <iostream>
#include <fstream>

#include <conio.h>

Shape::Shape() :
	m_position({ 0.0f,0.0f }),
	m_radius(0.0f),
	m_Size({0.0f,0.0f}),
	m_ShapeType("Circle")
{
	;
}

Shape::~Shape() { ; }

void Shape::SetPosition(const Vector2f& _pos) {
	m_position = _pos;
}
void Shape::SetRadius(float _radius) {
	m_radius = _radius;
}
float Shape::GetRadius() {
	return m_radius;
}
void Shape::SetSize(Vector2f _Size)
{
	m_Size = _Size;
}
Vector2f Shape::GetSize()
{
	return m_Size;
}
void Shape::SetShapeType(string _Type){
	m_ShapeType = _Type;
}
string Shape::GetShapeType()
{
	return m_ShapeType;
}
//与えられたShapeと衝突しているかを判定する
bool Shape::IsOverlapped(const Shape* _pShape)const {
	if (m_ShapeType == "Circle" && _pShape->m_ShapeType == "Circle")
	{
		float a = this->m_position.y - _pShape->m_position.y;
		float b = this->m_position.x - _pShape->m_position.x;

		//ここで三平方の定理を使って斜辺Cの長さを求める
		float c = sqrtf(a * a + b * b);
		//Cが２つの円の半径を足した長さより短ければ重なっている
		if (c < (this->m_radius + _pShape->m_radius)) {
			return true;
		}
	}

	if (m_ShapeType == "Circle" && _pShape->m_ShapeType == "OuterBox")
	{
		if (m_position.y < _pShape->m_Size.y + _pShape->m_position.y + m_radius)
		{
			return true;
		}
		//左壁
		if (m_position.x < -_pShape->m_Size.x + _pShape->m_position.x + m_radius)
		{
			return true;
		}
		//右壁
		if (m_position.x > _pShape->m_Size.x + _pShape->m_position.x - m_radius)
		{
			return true;
		}
	}

	//丸to四角or四角to丸
	if ((m_ShapeType == "Circle" && _pShape->m_ShapeType == "Square") || (m_ShapeType == "Square" && _pShape->m_ShapeType == "Circle"))
	{
		//円形情報を取得
		float CirclePosX = 0.0f;
		float CirclePosY = 0.0f;
		float CircleRad = 0.0f;//半径

		//四角形情報を取得
		float SquarePosX = 0.0f;
		float SquarePosY = 0.0f;
		float SquareSizeX = 0.0f;//横幅
		float SquareSizeY = 0.0f;//縦幅

		if (m_ShapeType == "Circle" && _pShape->m_ShapeType == "Square")
		{
			//円形情報を取得
			CirclePosX = this->m_position.x;
			CirclePosY = this->m_position.y;
			CircleRad = this->m_radius;//半径

			//四角形情報を取得
			SquarePosX = _pShape->m_position.x;
			SquarePosY = _pShape->m_position.y;
			SquareSizeX = _pShape->m_Size.x;//横幅
			SquareSizeY = _pShape->m_Size.y;//縦幅
		}
		if (m_ShapeType == "Square" && _pShape->m_ShapeType == "Circle")
		{
			//円形情報を取得
			CirclePosX = _pShape->m_position.x;
			CirclePosY = _pShape->m_position.y;
			CircleRad = _pShape->m_radius;//半径

			//四角形情報を取得
			SquarePosX = this->m_position.x;
			SquarePosY = this->m_position.y;
			SquareSizeX = this->m_Size.x;//横幅
			SquareSizeY = this->m_Size.y;//縦幅
		}

		/*cout << "円情報" << endl << "X" << CirclePosX << "Y" << CirclePosY << "Rad" << CircleRad << endl;
		cout << "四角情報" << endl << "X" << SquarePosX << "Y" << SquarePosY << "SizeX" << SquareSizeX << "SizeY" << SquareSizeY << endl;

		cout << "計算結果" << (SquarePosX - (SquareSizeX / 2.0f)) << " " << (SquarePosX + (SquareSizeX / 2.0f)) << endl;
		*/


		bool Xaxis = false;
		bool Yaxis = false;

		bool IsCollision = false;

		//横長
		if ((((SquarePosX - (SquareSizeX / 2.0f) - CircleRad) <= CirclePosX) && ((SquarePosX + (SquareSizeX / 2.0f) + CircleRad) >= CirclePosX))
			&& (((SquarePosY - (SquareSizeY / 2.0f)) <= CirclePosY) && (SquarePosY + (SquareSizeY / 2.0f) >= CirclePosY)))
		{
			IsCollision = true;
		}

		//縦長
		if (((SquarePosX - (SquareSizeX / 2.0f) <= CirclePosX) && (SquarePosX + (SquareSizeX / 2.0f) >= CirclePosX))
			&& (((SquarePosY - (SquareSizeY / 2.0f) - CircleRad) <= CirclePosY) && ((SquarePosY + (SquareSizeY / 2.0f) + CircleRad) >= CirclePosY)))
		{
			IsCollision = true;
		}

		float X = 0.0f;
		float Y = 0.0f;

		//右上
		X = SquarePosX + (SquareSizeX / 2.0f) - CirclePosX;
		Y = SquarePosY + (SquareSizeY / 2.0f) - CirclePosY;
		//斜辺
		if (sqrtf(X * X + Y * Y) <= CircleRad)
		{
			IsCollision = true;
		}

		//右下
		X = SquarePosX + (SquareSizeX / 2.0f) - CirclePosX;
		Y = SquarePosY - (SquareSizeY / 2.0f) - CirclePosY;
		//斜辺
		if (sqrtf(X * X + Y * Y) <= CircleRad)
		{
			IsCollision = true;
		}

		//左上
		X = SquarePosX - (SquareSizeX / 2.0f) - CirclePosX;
		Y = SquarePosY + (SquareSizeY / 2.0f) - CirclePosY;
		//斜辺
		if (sqrtf(X * X + Y * Y) <= CircleRad)
		{
			IsCollision = true;
		}

		//左下
		X = SquarePosX - (SquareSizeX / 2.0f) - CirclePosX;
		Y = SquarePosY - (SquareSizeY / 2.0f) - CirclePosY;
		//斜辺
		if (sqrtf(X * X + Y * Y) <= CircleRad)
		{
			IsCollision = true;
		}

		if (IsCollision)
		{
			return true;
		}

	}

	return false;
}