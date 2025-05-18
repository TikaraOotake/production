#pragma once

#include "Shape.h"
#include "ICollision.h"

#include <list>



//描画を行うのに必要なもの
#include"Fwk/Graphics/Texture.h"
#include"Fwk/Graphics/Sprite.h"
using namespace Lib::Graphics;

//ベクターを使うのに必要な物
#include "Lib/Math/Vector.h"
using namespace Lib::Math;

struct CollisionForces
{
	Vector2f RepulsionVec;
	Vector2f AddjustPos;
	float FricRotVelocity;
};

class GameObject : public ICollision
{
public:
	GameObject();
	~GameObject();

	virtual void Init();//初期化処理
	virtual void Update();//更新処理
	virtual void Render();//描画更新処理
	virtual void Term();//削除処理

	//アクセサ
	void SetMass(float _Mass);
	float GetMass();

	void SetVelocity(Vector2f _Velocity);
	void SetVelocity(float _x, float _y);
	Vector2f GetVelocity();

	void SetAccel(Vector2f _Accel);
	Vector2f GetAccel();

	void SetGravity(Vector2f _Gravity);
	Vector2f GetGravity();

	void SetRotVelocity(float _RotVelocity);
	float GetRotVelocity();

	void SetRotAccel(float _RotAccel);
	float GetRotAccel();

	void SetPosition(Vector2f _Position);
	void SetPosition(float _x, float _y);
	Vector2f GetPosition();

	void SetAngle(float  _Angle);
	float GetAngle();

	void SetRestitution(float _Restitution);
	float GetRestitution();

	void SetFriction(float _Friction);
	float GetFriction();

	void SetRadius(float _Radius);

	void SetDisplayPriority(float _DisplayPriority);
	float GetDisplayPriority();
	void SetTextureFilePath(string _FilePath);
	void SetSpriteSize(Vector2f _Size);
	void SetSpriteSize(float _SizeX, float _SizeY);
	void SetSpritePos(Vector2f _Pos);
	void SetSpritePos(float _x, float _y);

	float GetRadius();
	string GetShapeType();
	void SetSize(Vector2f _Size);
	void SetSize(float _SizeX, float _SizeY);
	Vector2f GetSize();

	bool IsActive() const;
	void SetActive(bool isActive);

	void SetGameTag(string _GameTag);
	string GetGameTag();

	//ICollisionの実装
	virtual const Shape* GetShape()const;
	virtual void OnOverlapped(ICollision* _pCollision);
	virtual GameObject* GetGameObject();

	void SetOnDestroyFunction(function<void(GameObject*) >& _func);

protected:
	//見た目を初期化する
	void initImage(
		const string& texFilePath,
		float width,
		float height
	);

	//衝突判定用の形状
	Shape m_Shape;

	virtual void UpdateSpritePosition();
protected:
	//プロパティ
	float m_Mass;//質量
	Vector2f m_Velocity;//速度
	Vector2f m_Accel;//加速度
	Vector2f m_Gravity;//重力加速度

	float m_RotVelocity;//回転速度
	float m_RotAccel;//回転加速度

	Vector2f m_Position;//座標
	Vector2f m_Position_old[10];
	float m_Angle;//角度

	float m_Restitution;//反発係数
	float m_Friction;//摩擦係数

	enum BodyType
	{
		Static,    // 動かない
		Kinematic, // 自由に動くが力は影響しない
		Dynamic    // 動き、力の影響を受ける
	};

	BodyType m_BodyType;

	string m_GameTag;//ゲームタグ
	float m_DisplayPriority;//表示優先度

	Texture m_Texture;
	Sprite m_Sprite;

	//m_Positionを原点としたときのスプライトの表示座標
	Vector2f m_Offset;
	Vector2f m_SpritePos;

	float m_SpriteAttractRate;//スプライトを引き寄せる割合
	string m_TextureFilePath;//スプライトのファイルパス

	bool m_IsActive;

	std::list<CollisionForces> m_CollisionForcesList;//衝突の際に計算された力を記録していくリスト

	function<void(GameObject*)> m_onDestroy;
	//メソッド

	void UpdatePhysics();
	void UpdateGravity();
	void UpdateSpriteAttractRate();//引き寄せ率を計算

	void Translate(Vector2f _v);
	void Translate(float _x,float _y);

	void OnActiveChanged(bool _isActive);

	void CalculateRestitution(GameObject* pGameObject);

	Vector2f ExtractVector(Vector2f _Velocity, Vector2f _NormalVector);//任意の法線ベクトルと水平な量を抽出する
	float calculateNormalForce(Vector2f _Vel1, Vector2f _Vel2, float _Mass1, float _Mass2, Vector2f _Normal);//垂直抗力を求める
	
	Vector2f perpendicularToNormal(Vector2f _Vel, Vector2f _Normal);// 移動ベクトルを法線ベクトルに垂直なベクトルに変換する関数
	// 法線ベクトルが真上(0, 1)を向くように速度ベクトルを回転させる関数
	Vector2f RotateVertically(Vector2f _Vel, Vector2f _Normal);
	Vector2f ReverseRotateVertically(Vector2f _Vel, Vector2f _Normal);
};

