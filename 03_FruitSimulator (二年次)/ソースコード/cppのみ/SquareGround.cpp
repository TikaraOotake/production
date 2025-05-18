#include"SquareGround.h"

#include "CollisionMng.h"
#include "GameMng.h"

SquareGround::SquareGround()
{
	Init();
}

SquareGround::~SquareGround()
{
}

void SquareGround::Init()
{
	m_GameTag = "SquareGround";
	m_TextureFilePath = "Images/temp/FFFFFF.png";
	
	initImage(
		m_TextureFilePath,
		64.0f * 10.0f,
		64.0f
	);

	//m_Shape.SetSize(Vector2f(64.0f * 10.0f, 64.0f ));
	m_Shape.SetShapeType("Square");

	//コリジョンを登録
	CollisionMng::GetInstance()->Register(this);
}
void SquareGround::Update()
{
	m_Shape.SetPosition(m_Position);//当たり判定の座標更新
}

void SquareGround::OnOverlapped(ICollision* _pCollision)
{

}