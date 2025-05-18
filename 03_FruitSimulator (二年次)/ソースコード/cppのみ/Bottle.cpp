#include"Bottle.h"

#include "CollisionMng.h"
#include "GameMng.h"

Bottle::Bottle()
{
	Init();
}

Bottle::~Bottle()
{
}

void Bottle::Init()
{
	m_TextureFilePath = "Images/bottle.png";
	
	initImage(
		m_TextureFilePath,
		64.0f * 10.0f,
		64.0f * 10.0f
	);

	m_Shape.SetSize(Vector2f(32.0f * 10.0f, 32.0f * 10.0f));

	//コリジョンを登録
	CollisionMng::GetInstance()->Register(this);
}
void Bottle::Update()
{

}

void Bottle::OnOverlapped(ICollision* _pCollision)
{

}