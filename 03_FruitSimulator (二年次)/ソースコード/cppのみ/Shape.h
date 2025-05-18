#pragma once//インクルードガード（多重読み込みを防ぐ）

#include <string>
using namespace std;

//ベクターを使うのに必要な物
#include "Lib/Math/Vector.h"
using namespace Lib::Math;

//Shape(形状)クラス
//今回はこのクラスが円の形状を表すことにする

class Shape
{
public:
	Shape();
	~Shape();

	void SetPosition(const Vector2f& _pos);
	void SetRadius(float _radius);
	float GetRadius();

	void SetSize(Vector2f _Size);
	Vector2f GetSize();

	void SetShapeType(string _Type);
	string GetShapeType();
	//与えられたShapeと衝突しているかを判定する
	bool IsOverlapped(const Shape* _pShape)const;

private:
	Vector2f m_position;
	float    m_radius;

	Vector2f m_Size;

	string m_ShapeType;
};