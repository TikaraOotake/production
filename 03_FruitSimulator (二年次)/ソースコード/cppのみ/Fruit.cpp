#include"Fruit.h"

#include "CollisionMng.h"
#include "GameMng.h"

#include "SquareGround.h"
#include "TextureFilePathLibrary.h"

#include <random>

//デバッグログを扱うため
#include<Fwk/Framework.h>

Fruit::Fruit() :
	m_Level(0),
	m_StaticFlag(true),
	m_IsLanding(false)
{
}

Fruit::~Fruit()
{
}
void Fruit::Init()//初期化処理
{
	/*initImage(
		"Images/temp/coin.png",
		64.0f*1.33f,
		64.0f
	);
	*/

	TextureFilePathLibrary Library;
	m_TextureFilePath = Library.GetTextureFilePath(m_Level,"Fruit");
	
	m_Position_old[0] = m_Position;

	m_GameTag = "Fruit";

	//テスト用
	m_Friction = 0.1f;
	m_Restitution = 0.7f;

	//コリジョンを登録
	CollisionMng::GetInstance()->Register(this);
}
void Fruit::Update()
{
	m_StaticFlag = false;
	if (m_StaticFlag)
	{
		m_Velocity = Vector2f(0.0f, 0.0f);
		m_RotVelocity = 0.0f;
		m_StaticFlag = false;
		return;
	}


	////------------------------------親元の処理をとりあえずコピー

	m_Position_old[8] = m_Position_old[7];
	m_Position_old[7] = m_Position_old[6];
	m_Position_old[6] = m_Position_old[5];
	m_Position_old[5] = m_Position_old[4];
	m_Position_old[4] = m_Position_old[3];
	m_Position_old[3] = m_Position_old[2];
	m_Position_old[2] = m_Position_old[1];
	m_Position_old[1] = m_Position_old[0];
	m_Position_old[0] = m_Position;

	UpdatePhysics();//物理挙動の更新
	m_Shape.SetPosition(m_Position);//当たり判定の座標更新

	UpdateSpritePosition();//スプライトの座標更新
	UpdateSpriteAttractRate();//スプライト引き寄せ率更新
}

void Fruit::OnOverlapped(ICollision* _pCollision)
{
	GameObject* pGameObject = dynamic_cast<GameObject*>(_pCollision);
	if (pGameObject)
	{
		Fruit* pFruit = dynamic_cast<Fruit*>(_pCollision);
		
		//当たった対象がフルーツか
		if (pFruit)
		{
			//同じレベルか？
			if (m_Level == pFruit->GetLevel())
			{
				GameMng* pGameMng = GameMng::GetInstance();

				Vector2f GeneratePos = m_Position;
				GeneratePos += (pFruit->GetPosition() - m_Position) / 2.0f;

				bool CheckDelete = false;

				//自身と相手を削除
				CheckDelete = pGameMng->DestroyGameObject(pFruit) && pGameMng->DestroyGameObject(this);

				//削除に成功した方のクラスで処理を続行
				if (CheckDelete)
				{
					//スコア加算
					pGameMng->AddScore(m_Level + 1);

					//フルーツの合成
					if (m_Level < 10)
					{
						pGameMng->GenerateFruit(GeneratePos.x, GeneratePos.y, m_Level + 1);
					}
				}

				return;
			}
		}

		SquareGround* pSquareGround = dynamic_cast<SquareGround*>(_pCollision);

		//フルーツか四角地形にぶつかったら
		if (pFruit || pSquareGround)
		{
			m_IsLanding = true;
		}
		else
		{
			m_IsLanding = false;
		}

		//衝突による反発力の計算
		CalculateRestitution(pGameObject);
	}
}

//アクセサ
void Fruit::SetLevel(int _Level) { m_Level = _Level; }
int Fruit::GetLevel() { return m_Level; }
bool Fruit::GetIsLanding() { return m_IsLanding; }

