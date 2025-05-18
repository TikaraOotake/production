
#include<Fwk/Framework.h>
#include "Dropper.h"
#include "GameMng.h"
#include "Fruit.h"
#include "CollisionMng.h"

#include "TextureFilePathLibrary.h"

#include <iostream>
using namespace std;

Dropper::Dropper():
	m_HoldFruitLevel(0),
	m_ReloadTimer(0.0f),
	m_ReloadTime(0.0f),
	m_pGameMng(nullptr),
	m_pFallingFruit(nullptr),
	m_NextFruitLevel(0),
	m_pNextFruitSpriteBoard(nullptr)
{
	Init();
}

Dropper::~Dropper()
{
}

void Dropper::Init()
{
	TextureFilePathLibrary Library;

	m_TextureFilePath = Library.GetTextureFilePath(0, "Character");

	initImage(
		m_TextureFilePath,
		64.0f * 3.0f,
		64.0f * 2.0f
	);

	m_Offset = Vector2f(0.0f, 32.0f);

	SetPosition(0.0f,300.0f);

	m_ReloadTime = 1.0f;

	m_pGameMng = GameMng::GetInstance();

	m_Shape.SetShapeType("NoShape");

	m_GameTag = "Dropper";


	//フルーツを決める
	// 0から4の範囲でランダムな値を生成
	m_HoldFruitLevel = std::rand() % 5;
	m_NextFruitLevel = std::rand() % 5;

	
	m_FruitTexture.Load(Library.GetTextureFilePath(m_HoldFruitLevel, "Fruit"));

	m_FruitSprite.Init();
	m_FruitSprite.SetTexture(m_FruitTexture);

	float Diameter = 64.0f + 64 * m_HoldFruitLevel * 0.25f;
	m_FruitSprite.SetSize(Diameter, Diameter);

	//SpriteBoardの生成
	m_pNextFrameSpriteBoard = new GameObject;
	m_pNextFrameSpriteBoard->SetPosition(400.0f, 250.0f);
	m_pNextFrameSpriteBoard->SetDisplayPriority(10.0f);
	m_pNextFrameSpriteBoard->SetActive(false);
	m_pNextFrameSpriteBoard->SetTextureFilePath(Library.GetTextureFilePath(1, "UI"));
	m_pNextFrameSpriteBoard->SetSpriteSize(192.0f, 192.0f);
	//オブジェクト登録
	GameMng::GetInstance()->SetGameObjectList(m_pNextFrameSpriteBoard);

	//SpriteBoardの生成
	m_pNextFruitSpriteBoard = new GameObject;
	m_pNextFruitSpriteBoard->SetPosition(400.0f, 250.0f);
	m_pNextFruitSpriteBoard->SetDisplayPriority(11.0f);
	m_pNextFruitSpriteBoard->SetActive(false);
	m_pNextFruitSpriteBoard->SetTextureFilePath(Library.GetTextureFilePath(m_NextFruitLevel, "Fruit"));
	m_pNextFruitSpriteBoard->SetSpriteSize(64.0f, 64.0f);
	//オブジェクト登録
	GameMng::GetInstance()->SetGameObjectList(m_pNextFruitSpriteBoard);
}

void Dropper::Update()
{
	float MoveSpeed = 5.0f;

	//タイマーを進める
	if (m_pGameMng != nullptr)
	{
		if (m_ReloadTimer > 0.0f)
		{
			m_ReloadTimer -= m_pGameMng->GetDeltaTime();
		}
		else if (m_ReloadTimer < 0.0f)
		{
			m_ReloadTimer = 0.0f;
			//新しいファイルパスを設定
			TextureFilePathLibrary Library;
			if (GameMng::GetInstance()->CheckExistObjectList(m_pNextFruitSpriteBoard))
			{
				m_pNextFruitSpriteBoard->SetTextureFilePath(Library.GetTextureFilePath(m_NextFruitLevel, "Fruit"));
				m_pNextFruitSpriteBoard->SetSpriteSize(64.0f, 64.0f);
			}
		}
	}

	//落としたフルーツが接地したかチェック
	if (m_pFallingFruit != nullptr)
	{
		Fruit* pFruit = nullptr;
		if (GameMng::GetInstance()->CheckExistObjectList(m_pFallingFruit))
		{
			pFruit = dynamic_cast<Fruit*>(m_pFallingFruit);
		}
		else
		{
			m_pFallingFruit = nullptr;
			//新しいファイルパスを設定
			TextureFilePathLibrary Library;
			if (GameMng::GetInstance()->CheckExistObjectList(m_pNextFruitSpriteBoard))
			{
				m_pNextFruitSpriteBoard->SetTextureFilePath(Library.GetTextureFilePath(m_NextFruitLevel, "Fruit"));
				m_pNextFruitSpriteBoard->SetSpriteSize(64.0f, 64.0f);
			}
		}
		

		//フルーツか
		if (pFruit)
		{
			if (pFruit->GetIsLanding())
			{
				m_pFallingFruit = nullptr;
				//新しいファイルパスを設定
				TextureFilePathLibrary Library;
				if (GameMng::GetInstance()->CheckExistObjectList(m_pNextFruitSpriteBoard))
				{
					m_pNextFruitSpriteBoard->SetTextureFilePath(Library.GetTextureFilePath(m_NextFruitLevel, "Fruit"));
					m_pNextFruitSpriteBoard->SetSpriteSize(64.0f, 64.0f);
				}
				else
				{
					m_pNextFruitSpriteBoard = nullptr;
				}
			}
		}
	}
	
	

	if (Input_I->IsKeyPressed('A'))
	{
		m_Position.x -= MoveSpeed;
	}
	if (Input_I->IsKeyPressed('D'))
	{
		m_Position.x += MoveSpeed;
	}

	if (Input_I->IsKeyDown(VK_SPACE))
	{
		if (m_ReloadTimer <= 0.0f || m_pFallingFruit == nullptr)
		{
			if (m_pGameMng != nullptr)
			{
				m_pFallingFruit = m_pGameMng->GenerateFruit(m_Position.x, m_Position.y, m_HoldFruitLevel);
			}

			//タイマーを設定
			m_ReloadTimer = m_ReloadTime;

			//フルーツを決める
			// 0から4の範囲でランダムな値を生成
			m_HoldFruitLevel = m_NextFruitLevel;
			m_NextFruitLevel = std::rand() % 5;

			DebugLog("m_HoldFruitLevel:" + to_string(m_HoldFruitLevel));
			DebugLog("m_NextFruitLevel:" + to_string(m_NextFruitLevel));

			//新しいファイルパスを設定
			TextureFilePathLibrary Library;
			m_FruitTexture.Load(Library.GetTextureFilePath(m_HoldFruitLevel, "Fruit"));//

			m_FruitSprite.Init();
			m_FruitSprite.SetTexture(m_FruitTexture);

			float Diameter = 64.0f + 64 * m_HoldFruitLevel * 0.25f;
			m_FruitSprite.SetSize(Diameter, Diameter);
		}
	}

	//位置調整
	float OverLine = 64.0f * 3.0f;
	if (m_Position.x > OverLine)
	{
		m_Position.x = -OverLine;
	}
	if (m_Position.x < -OverLine)
	{
		m_Position.x = OverLine;
	}

	UpdateSpritePosition();
}
void Dropper::Render()
{
	m_Sprite.Draw();
	if (m_ReloadTimer <= 0.0f || m_pFallingFruit == nullptr)
	{
		m_FruitSprite.Draw();
	}
}
void Dropper::Term()
{
	m_IsActive = false;

	m_Texture.Unload();//テクスチャの後片付け
	m_FruitTexture.Unload();
	m_Sprite.Term();
	m_FruitSprite.Term();

	//コリジョンを登録解除
	CollisionMng::GetInstance()->Unregister(this);

	if (m_pNextFruitSpriteBoard != nullptr)
	{
		//削除依頼
		GameMng::GetInstance()->DestroyGameObject(m_pNextFruitSpriteBoard);
		m_pNextFruitSpriteBoard = nullptr;
	}
	if (m_pNextFrameSpriteBoard != nullptr)
	{
		//削除依頼
		GameMng::GetInstance()->DestroyGameObject(m_pNextFrameSpriteBoard);
		m_pNextFrameSpriteBoard = nullptr;
	}
}

void Dropper::UpdateSpritePosition()
{
	m_SpritePos = m_Position;

	m_Sprite.SetPosition(
		m_SpritePos.x + m_Offset.x,
		m_SpritePos.y + m_Offset.y
	);

	m_FruitSprite.SetPosition(
		m_SpritePos.x,
		m_SpritePos.y
	);
	

	//回転速度が異常だったら補正
	if (m_RotVelocity < -1000.0f)
	{
		m_RotVelocity = -1000.0f;
	}
	if (m_RotVelocity > 1000.0f)
	{
		m_RotVelocity = 1000.0f;
	}
	m_Angle += m_RotVelocity;
	//0～360を超えてたら補正
	while (m_Angle < 0.0f)
	{
		m_Angle += 360.0f;
	}
	while (m_Angle >= 360.0f)
	{
		m_Angle -= 360.0f;
	}
	if (m_Angle >= 0.0f && m_Angle <= 360.0f)
	{
		m_Sprite.SetRotation(m_Angle);
	}
}
int Dropper::GetNextFruitLevel() { return  m_NextFruitLevel; }

void Dropper::SetFallingFruit(GameObject* _pGameObject) { m_pFallingFruit = _pGameObject; }
GameObject* Dropper::GetFallingFruit() { return m_pFallingFruit; }