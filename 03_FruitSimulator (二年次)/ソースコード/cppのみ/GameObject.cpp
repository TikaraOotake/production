#include "GameObject.h"

#include "CollisionMng.h"
#include <iostream>
#include <algorithm>
#include <random>

using namespace std;

//デバッグログを扱うため
#include<Fwk/Framework.h>

GameObject::GameObject():
	m_Mass(1.0f),
	m_Velocity({0.0f,0.0f}),
	m_Accel({ 0.0f,0.0f }),
	m_Gravity({ 0.0f,-0.98f/2.0f }),
	m_RotVelocity(0.0f),
	m_RotAccel(0.0f),
	m_Position({ 0.0f,0.0f }),
	m_Angle(0.0f),
	m_Restitution(0.0f),//反発係数
	m_Friction(0.0f),
	m_GameTag("Noname"),
	m_DisplayPriority(0.0f),
	m_Texture(),
	m_Sprite(),
	m_Offset({ 0.0f,0.0f }),
	m_SpriteAttractRate(1.0f),
	m_IsActive(true),
	m_BodyType(Dynamic)
{
	Init();

	m_Position_old[0] = m_Position;
	m_Position_old[1] = m_Position;
	m_Position_old[2] = m_Position;
	m_Position_old[3] = m_Position;
	m_Position_old[4] = m_Position;
	m_Position_old[5] = m_Position;
	m_Position_old[6] = m_Position;
	m_Position_old[7] = m_Position;
}

GameObject::~GameObject()
{
	Term();
}

void GameObject::Init()//初期化処理
{
	/*initImage(
		"Images/temp/coin.png",
		64.0f*1.33f,
		64.0f
	);
	*/

	m_TextureFilePath = "Images/temp/Bullet/EnemyBullet01.png";

	initImage(
		m_TextureFilePath,
		64.0f,
		64.0f
	);
	

	m_Shape.SetRadius(28.0f);

	//テスト用
	m_Friction = 0.5f;
	m_Restitution = 0.9f;

	//コリジョンを登録
	CollisionMng::GetInstance()->Register(this);
}
void GameObject::Update()//更新処理
{
	m_Position_old[8] = m_Position_old[7];
	m_Position_old[7] = m_Position_old[6];
	m_Position_old[6] = m_Position_old[5];
	m_Position_old[5] = m_Position_old[4];
	m_Position_old[4] = m_Position_old[3];
	m_Position_old[3] = m_Position_old[2];
	m_Position_old[2] = m_Position_old[1];
	m_Position_old[1] = m_Position_old[0];
	m_Position_old[0] = m_Position;

	//最初に反発後のベクトルを適用しておく
	Vector2f RepulsionVec(0.0f, 0.0f);
	Vector2f AddjustPos(0.0f, 0.0f);
	float FricRotVelocity(0.0f);

	// リストが空かどうかをチェック
	if (m_CollisionForcesList.empty()) {
		//std::cout << "リストが空です。" << std::endl;
		// 空の場合は処理を終了
	}
	else
	{
		// ランダム数生成器の準備
		std::random_device rd;             // 乱数生成器の初期化
		std::mt19937 gen(rd());            // メルセンヌ・ツイスタ法による擬似乱数生成器
		std::uniform_int_distribution<> dis(0.0, m_CollisionForcesList.size() - 1.0);  // 乱数の範囲（リストのインデックス範囲）

		// ランダムなインデックスを生成
		int randomIndex = dis(gen);

		// ランダムなインデックスの要素を取得するためにイテレータを使う
		std::list<CollisionForces>::iterator it = m_CollisionForcesList.begin();
		std::advance(it, randomIndex);  // イテレータをランダムインデックスに移動

		//選んだ要素から値を取得
		RepulsionVec = it->RepulsionVec;
		FricRotVelocity = it->FricRotVelocity;

		//補正値を全て加算
		for (std::list<CollisionForces>::iterator it = m_CollisionForcesList.begin(); it != m_CollisionForcesList.end(); ++it) {
			AddjustPos += it->AddjustPos;
		}
	}

	m_CollisionForcesList.clear();//計算済なので要素を削除



	float Rate = 0.7f;

	//代入
	Vector2f ZeroVec(0.0f, 0.0f);
	if (RepulsionVec != ZeroVec)
	{
		m_Velocity = RepulsionVec;
		m_RotVelocity += FricRotVelocity;
		m_RotVelocity *= 0.99f;//謎加速を抑える
	}
	if (AddjustPos != ZeroVec)
	{
		m_Position += AddjustPos * Rate;
	}




	//テスト処理
	//床
	if (m_Position.y < -300.0f + m_Shape.GetRadius())
	{
		float PeripheralSpeed = (m_RotVelocity * 3.1415926535f / 180.0f * m_Shape.GetRadius()) + m_Velocity.x;//接点の速度を計算
		float AddNum = PeripheralSpeed * m_Friction;
		//代入
		m_RotVelocity -= AddNum;
		m_Velocity.x -= AddNum;

		m_Velocity.y *= -m_Restitution;
		m_Position.y += (-m_Position.y + -300.0f + m_Shape.GetRadius()) * Rate;
	}
	//左壁
	if (m_Position.x < -400.0f + m_Shape.GetRadius())
	{
		m_Velocity.x *= -m_Restitution;
		m_Position.x += (-m_Position.x + -400.0f + m_Shape.GetRadius()) * Rate;
	}
	//右壁
	if (m_Position.x > 400.0f - m_Shape.GetRadius())
	{

		m_Velocity.x *= -m_Restitution;
		m_Position.x += (-m_Position.x + 400.0f - m_Shape.GetRadius()) * Rate;
	}

	UpdatePhysics();//物理挙動の更新
	m_Shape.SetPosition(m_Position);//当たり判定の座標更新

	UpdateSpritePosition();//スプライトの座標更新
	UpdateSpriteAttractRate();//スプライト引き寄せ率更新
}
void GameObject::Render()//描画更新処理
{
	m_Sprite.SetPosition(
		m_Position.x + m_Offset.x,
		m_Position.y + m_Offset.y
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
	m_Sprite.Draw();
}
void GameObject::Term()//削除処理
{
	m_IsActive = false;

	m_Texture.Unload();//テクスチャの後片付け
	m_Sprite.Term();

	//コリジョンを登録解除
	CollisionMng::GetInstance()->Unregister(this);
}

void GameObject::initImage(
	const string& texFilePath,
	float width,
	float height
) {
	m_Texture.Load(texFilePath);

	m_Sprite.Init();
	m_Sprite.SetTexture(m_Texture);
	m_Sprite.SetSize(width, height);
}

void GameObject::UpdateSpritePosition()
{
	/*m_SpritePos.x += (m_Position.x - m_SpritePos.x) * m_SpriteAttractRate;
	m_SpritePos.y += (m_Position.y - m_SpritePos.y) * m_SpriteAttractRate;

	m_Sprite.SetPosition(
		m_SpritePos.x + m_Offset.x,
		m_SpritePos.y + m_Offset.y
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
	*/
	

}
void GameObject::UpdatePhysics()
{
	//加速度から速度を計算
	m_Velocity += m_Accel;
	UpdateGravity();

	//移動
	Translate(m_Velocity);
}
void GameObject::UpdateGravity()
{
	m_Velocity += m_Gravity;
}
void GameObject::UpdateSpriteAttractRate()
{
	//移動量を計算
	Vector2f tempV =
		m_Position - m_Position_old[0] +
		m_Position_old[0] - m_Position_old[1] +
		m_Position_old[1] - m_Position_old[2] +
		m_Position_old[2] - m_Position_old[3] +
		m_Position_old[3] - m_Position_old[4] +
		m_Position_old[4] - m_Position_old[5] +
		m_Position_old[5] - m_Position_old[6] +
		m_Position_old[6] - m_Position_old[7] +
		m_Position_old[7] - m_Position_old[8];

	float MoveAmount = tempV.GetLength();
	if (MoveAmount < 0.0f)
	{
		MoveAmount = 0.0f;
	}

	//移動量をもとにRateを計算
	m_SpriteAttractRate = (MoveAmount * m_Shape.GetRadius() * 0.0008f) + 0.01f;

	//あふれたら補正
	if (m_SpriteAttractRate > 1.0f)
	{
		m_SpriteAttractRate = 1.0f;
	}
	if (m_SpriteAttractRate <= 0.0f)
	{
		m_SpriteAttractRate = 0.0f;
	}
}

void GameObject::Translate(Vector2f _v)
{
	m_Position.x += _v.x;
	m_Position.y += _v.y;
	UpdateSpritePosition();
}
void GameObject::Translate(float _x, float _y)
{
	Vector2f v = { _x,_y };
	Translate(v);
}
void GameObject::OnActiveChanged(bool _isActive)
{
	if (_isActive) {
		//アクティブ状態になったときはコリジョンMNGに自分を登録
		CollisionMng::GetInstance()->Register(this);
	}
	else {
		//非アクティブ状態になったときは
		//コリジョンMNGへの登録を解除
		CollisionMng::GetInstance()->Unregister(this);
	}
}
void GameObject::CalculateRestitution(GameObject* pGameObject)
{
	//円&円
	if (m_Shape.GetShapeType() == "Circle" && pGameObject->GetShapeType() == "Circle")
	{
		//使う移動ベクトルと座標を取得
		Vector2f Vec1 = m_Velocity;
		Vector2f Vec2 = pGameObject->GetVelocity();
		Vector2f Pos1 = m_Position;
		Vector2f Pos2 = pGameObject->GetPosition();

		//半径の取得
		float Radius1 = m_Shape.GetRadius();
		float Radius2 = pGameObject->GetRadius();
		//回転速度を取得
		float RotVel1 = m_RotVelocity;
		float RotVel2 = pGameObject->GetRotVelocity();

		//弾性を定義 (影響を少し抑える)
		//値が小さいほうを使用する
		float e = pGameObject->GetRestitution();
		if (m_Restitution < e)
		{
			e = m_Restitution;
		}

		if (e < 0.0f)
		{
			e = 0.0f;
		}
		if (e > 1.0f)
		{
			e = 1.0f;
		}

		//摩擦係数を定義
		float f =  pGameObject->GetFriction();
		if (m_Friction < f)
		{
			f = m_Friction;
		}
		if (f < 0.0f)
		{
			f = 0.0f;
		}
		if (f > 1.0f)
		{
			f = 1.0f;
		}

		//質量比を定義
		float MassRatio = 1.0f;
		float MassTotal = m_Mass + pGameObject->GetMass();
		if (MassTotal != 0) {
			MassRatio = 2.0f * (1.0f - (m_Mass / MassTotal));
		}

		//法線ベクトルの計算
		Vector2f n = Pos2 - Pos1;
		Vector2f n_hat = n.GetNormalized();

		//垂直抗力を定義
		float NormalForce = calculateNormalForce(Vec1,Vec2, m_Mass,pGameObject->GetMass(),n_hat);

		//補正値を計算 (過剰補正を防ぐ)
		float Length = sqrtf(n.x * n.x + n.y * n.y);
		Vector2f AddjustPos;
		float OverLength = (Radius1 + Radius2) - Length;
		if (OverLength > 0) { // 重なりが発生しているときのみ補正
			AddjustPos = -1.0f * n_hat * (OverLength * 0.5f);
		}
		else {
			AddjustPos = Vector2f(0, 0);
		}

		//反発の計算 (影響を抑える)
		Vector2f v1_parallel = n_hat * Vector2f::Dot(Vec1, n_hat);
		Vector2f v2_parallel = n_hat * Vector2f::Dot(Vec2, n_hat);

		Vector2f v1_perpendicular = Vec1 - v1_parallel;
		Vector2f v2_perpendicular = Vec2 - v2_parallel;

		Vec1 = v1_perpendicular - e * (v1_parallel - v2_parallel) * MassRatio * 0.5f;
		Vec2 = v2_perpendicular + e * (v1_parallel - v2_parallel) * (2.0f - MassRatio) * 0.5f;


		//反発後のベクトルを一時的に接点法線に合わせて上を向かせる
		Vec1 = RotateVertically(Vec1, n_hat);
		Vec2 = RotateVertically(Vec2, n_hat * -1.0f);

		//------------------------------------------------------摩擦と回転の計算
		// 回転エネルギー保存のための調整
		float rotationTransfer = (RotVel1 - RotVel2) * MassRatio;
		RotVel1 += rotationTransfer;
		RotVel2 -= rotationTransfer;

		//計算には「実際」にどれだけ移動かの距離を使う(座標補正による静止状態も移動していると認識してしまうため)
		Vector2f RealVel = m_Position - m_Position_old[0];
		RealVel = RotateVertically(RealVel, n_hat);

		//float PeripheralSpeed = (m_RotVelocity * 3.1415926535f / 180.0f * m_Shape.GetRadius()) + m_Velocity.x;//接点の速度を計算
		//float AddNum = PeripheralSpeed * m_Friction;
		//代入
		//m_RotVelocity -= AddNum;
		//m_Velocity.x -= AddNum;



		//接点速度差を求める (影響を抑える)
		float PeripheralSpeed1 = ( -1* RotVel1 * (3.1415926535f / 180.0f) * Radius1) + RealVel.x;//自身の接点速度(外円の速度+水平成分の移動速度)
		float PeripheralSpeed2 = ( -1* RotVel2 * (3.1415926535f / 180.0f )* Radius2) + Vec2.x;//相手の接点速度(外円の速度+水平成分の移動速度)
		float relativeSpeed = (PeripheralSpeed2 - PeripheralSpeed1);

		//速度差の割合
		float SpeedRatio;
		if (relativeSpeed == 0)
		{
			SpeedRatio = 0.5f;
		}
		else
		{
			SpeedRatio = std::abs(PeripheralSpeed1) / std::abs(relativeSpeed);
		}

		// 摩擦力を適用（摩擦係数が強すぎないように調整）
		float frictionForce = std::abs(relativeSpeed) * f * 0.05f;
		if (std::abs(frictionForce) < 0.01f) frictionForce = 0.0f;  // 小さい摩擦力を無視

		float AddNum = relativeSpeed * SpeedRatio * f * 0.5f;//* frictionForce ;

		//少しエネルギーを損失させる
		if (AddNum > 0.1f)
		{
			AddNum += -0.1f;
		}
		else
		if (AddNum < -0.1f)
		{
			AddNum += 0.1f;
		}

		//代入
		float Rot = 0.0f;
		Rot -= (AddNum / Radius1) * (180.0f / 3.1415926535f);
		Vec1.x += AddNum;

		//------------------------------------------------------

		//元に戻す
		Vec1 = ReverseRotateVertically(Vec1, n_hat);
		Vec2 = ReverseRotateVertically(Vec2, n_hat * -1.0f);


		//衝突法線に対して水平な成分を定義
		//Vector2f HorizontalVec1 = ExtractVector(Vec1, n_hat);
		//Vector2f HorizontalVec2 = ExtractVector(Vec2, n_hat * -1.0f);
		//Vector2f TotalHorizontalVec = HorizontalVec2 - HorizontalVec1;


		



		CollisionForces TempForces;
		TempForces.RepulsionVec = Vec1;
		TempForces.AddjustPos = AddjustPos*0.9f;
		TempForces.FricRotVelocity = Rot;
		//TempForces.FricRotVelocity = 0.0f;

		m_Velocity = Vec1;
		m_Position += AddjustPos * 0.9f;
		m_RotVelocity += Rot * 0.9f;

		//m_CollisionForcesList.push_back(TempForces);
	}
	
	if (m_Shape.GetShapeType() == "Circle" && pGameObject->GetShapeType() == "Square")
	{
		//どの面に接触しているか判断する
		//自分と当たった対象の座標を取得
		Vector2f thisPos = GetPosition();
		Vector2f BlockPos = pGameObject->GetPosition();

		//縦横どちらからぶつかったか計算する
		float X = thisPos.x - BlockPos.x * pGameObject->GetSize().x;
		float Y = thisPos.y - BlockPos.y * pGameObject->GetSize().y;

		//cout << "計算結果" << X << " " << Y << endl;

		float Rate = 0.9f;

		//Xの方が大きい場合左右からYのほうが大きい場合上下から衝突した
		if (abs(X) > abs(Y))//左右の衝突
		{
			//DebugLog("左右から衝突！");


			if (X < 0)//左側の辺
			{
				float PeripheralSpeed = (-m_RotVelocity * 3.1415926535f / 180.0f * m_Shape.GetRadius()) + m_Velocity.y;//接点の速度を計算
				float AddNum = PeripheralSpeed * m_Friction;

				m_Velocity.x *= -m_Restitution;
				m_Position.x += (-m_Position.x + BlockPos.x - (pGameObject->GetSize().x * 0.5f) - m_Shape.GetRadius()) * Rate;
			}
			if (X > 0)//右側の辺
			{
				float PeripheralSpeed = (m_RotVelocity * 3.1415926535f / 180.0f * m_Shape.GetRadius()) + m_Velocity.y;//接点の速度を計算
				float AddNum = PeripheralSpeed * m_Friction;

				m_Velocity.x *= -m_Restitution;
				m_Position.x += (-m_Position.x + BlockPos.x + (pGameObject->GetSize().x * 0.5f) + m_Shape.GetRadius()) * Rate;
			}
		}
		if (abs(X) < abs(Y))//上下の衝突
		{
			//DebugLog("上下から衝突！");
			if (Y < 0)//下側の辺
			{

			}
			if (Y > 0)//上側の辺
			{


				float PeripheralSpeed = (m_RotVelocity * 3.1415926535f / 180.0f * m_Shape.GetRadius()) + m_Velocity.x;//接点の速度を計算
				float AddNum = PeripheralSpeed * m_Friction;
				//代入
				m_RotVelocity -= AddNum;
				m_Velocity.x -= AddNum;

				m_Velocity.y *= -m_Restitution;
				m_Position.y += (-m_Position.y + BlockPos.y + (pGameObject->GetSize().y * 0.5f) + m_Shape.GetRadius()) * Rate;

				//DebugLog("上から衝突！");
			}
		}
	}
}
Vector2f GameObject::ExtractVector(Vector2f _Velocity, Vector2f _NormalVector)//移動ベクトル　法線ベクトル
{
	// 内積を計算
	float dotProduct = _Velocity.x * _NormalVector.x + _Velocity.y * _NormalVector.y;
	float normSquared = _NormalVector.x * _NormalVector.x + _NormalVector.y * _NormalVector.y;

	// 法線成分を計算
	Vector2f normalComponent;
	normalComponent.x = (dotProduct / normSquared) * _NormalVector.x;
	normalComponent.y = (dotProduct / normSquared) * _NormalVector.y;

	// 水平成分を計算
	Vector2f horizontalComponent;
	horizontalComponent.x = _Velocity.x - normalComponent.x;
	horizontalComponent.y = _Velocity.y - normalComponent.y;

	return horizontalComponent;
}
float GameObject::calculateNormalForce(Vector2f _Vel1,Vector2f _Vel2,float _Mass1,float _Mass2,Vector2f _Normal)
{
	//相対速度の法線成分を計算
	Vector2f relativeVelocity = _Vel2 - _Vel1;
	float relativeNormalVelocity = relativeVelocity.x * _Normal.x + relativeVelocity.y * _Normal.y;

	//衝突後の速度変化量を計算
	if (relativeNormalVelocity > 0) {
		// 衝突しない場合
		return 0.0f;
	}

	float deltaV = (2 * _Mass1 * _Mass2 * relativeNormalVelocity) / (_Mass1 + _Mass2);

	//垂直抗力を計算
	return deltaV;
}
// 移動ベクトルを法線ベクトルに垂直なベクトルに変換する関数
Vector2f GameObject::perpendicularToNormal(Vector2f _Vel, Vector2f _Normal) {
	//正規化
	_Normal = _Normal.GetNormalized();
	//法線ベクトルへの投影成分を計算
	float dotProduct = _Vel.x * _Normal.x + _Vel.y * _Normal.y;  // 移動ベクトルと法線ベクトルの内積
	Vector2f projection = _Normal * (dotProduct / (_Normal.x * _Normal.x + _Normal.y * _Normal.y));

	//法線ベクトルの投影成分を移動ベクトルから引くことで、垂直なベクトルを得る
	Vector2f perpendicularMovement = _Vel - projection;

	return perpendicularMovement;
}
Vector2f GameObject::RotateVertically(Vector2f _Vel, Vector2f _Normal) {
	// 法線ベクトルを正規化
	_Normal = _Normal.GetNormalized();

	// (0, 1)と法線ベクトルの間の角度を計算 (atan2で角度を求める)
	float angle = std::atan2(_Normal.x, _Normal.y);

	// 速度ベクトルを回転させる
	float cos_theta = std::cos(angle);
	float sin_theta = std::sin(angle);
	Vector2f vec(_Vel.x * cos_theta - _Vel.y * sin_theta, _Vel.x * sin_theta + _Vel.y * cos_theta);

	return vec;
}
Vector2f GameObject::ReverseRotateVertically(Vector2f _Vel, Vector2f _Normal) {
	// 法線ベクトルを正規化
	_Normal = _Normal.GetNormalized();

	// (0, 1)と法線ベクトルの間の角度を計算 (atan2で角度を求める)
	float angle = std::atan2(_Normal.x, _Normal.y);

	// 速度ベクトルを回転させる
	float cos_theta = std::cos(-angle);
	float sin_theta = std::sin(-angle);
	Vector2f vec(_Vel.x * cos_theta - _Vel.y * sin_theta, _Vel.x * sin_theta + _Vel.y * cos_theta);

	return vec;
}

void GameObject::SetOnDestroyFunction(function<void(GameObject*) >& _func)
{
	m_onDestroy = _func;
}

//アクセサ
void GameObject::SetMass(float _Mass)
{
	m_Mass = _Mass;
}
float GameObject::GetMass()
{
	return m_Mass;
}

void GameObject::SetVelocity(Vector2f _Velocity)
{
	m_Velocity = _Velocity;
}
void GameObject::SetVelocity(float _x,float _y)
{
	Vector2f v(_x, _y);
	SetVelocity(v);
}
Vector2f GameObject::GetVelocity()
{
	return  m_Velocity;
}

void GameObject::SetAccel(Vector2f _Accel)
{
	m_Accel = _Accel;
}
Vector2f GameObject::GetAccel()
{
	return m_Accel;
}

void GameObject::SetGravity(Vector2f _Gravity)
{
	m_Gravity = _Gravity;
}
Vector2f GameObject::GetGravity()
{
	return m_Gravity;
}

void GameObject::SetRotVelocity(float _RotVelocity)
{
	m_RotVelocity = _RotVelocity;
}
float GameObject::GetRotVelocity()
{
	return m_RotVelocity;
}

void GameObject::SetRotAccel(float _RotAccel)
{
	m_RotAccel = _RotAccel;
}
float GameObject::GetRotAccel()
{
	return m_RotAccel;
}

void GameObject::SetPosition(Vector2f _Position)
{
	m_Position = _Position;
}
void GameObject::SetPosition(float _x, float _y)
{
	Vector2f v{ _x,_y };
	SetPosition(v);
}
Vector2f GameObject::GetPosition()
{
	return m_Position;
}

void GameObject::SetAngle(float _Angle)
{
	m_Angle = _Angle;
}
float GameObject::GetAngle()
{
	return m_Angle;
}

void GameObject::SetRestitution(float _Restitution)
{
	m_Restitution = _Restitution;
}
float GameObject::GetRestitution()
{
	return m_Restitution;
}

void GameObject::SetFriction(float _Friction)
{
	m_Friction = _Friction;
}
float GameObject::GetFriction()
{
	return m_Friction;
}

float GameObject::GetRadius()
{
	return m_Shape.GetRadius();
}
string GameObject::GetShapeType()
{
	return m_Shape.GetShapeType();
}
void GameObject::SetSize(Vector2f _Size)
{
	m_Shape.SetSize(_Size);
}
void GameObject::SetSize(float _SizeX,float _SizeY)
{
	Vector2f Size(_SizeX, _SizeY);
	m_Shape.SetSize(Size);
}
Vector2f GameObject::GetSize()
{
	return m_Shape.GetSize();
}

void GameObject::SetRadius(float _Radius)
{
	m_Shape.SetRadius(_Radius);
}
void GameObject::SetDisplayPriority(float _DisplayPriority)
{
	m_DisplayPriority = _DisplayPriority;
}
float GameObject::GetDisplayPriority()
{
	return m_DisplayPriority;
}
void GameObject::SetTextureFilePath(string _FilePath)
{
	m_TextureFilePath = _FilePath;
}
void GameObject::SetSpriteSize(Vector2f _Size)
{
	initImage(
		m_TextureFilePath,
		_Size.x,
		_Size.y
	);
}
void GameObject::SetSpriteSize(float _SizeX, float _SizeY)
{
	Vector2f Size(_SizeX, _SizeY);
	SetSpriteSize(Size);
}
void GameObject::SetSpritePos(Vector2f _Pos)
{
	m_Sprite.SetPosition(_Pos);
}
void GameObject::SetSpritePos(float _x,float _y)
{
	Vector2f _Pos(_x, _y);
	SetSpritePos(_Pos);
}

bool GameObject::IsActive() const {
	return m_IsActive;
}
void GameObject::SetActive(bool isActive) {
	if (m_IsActive == isActive)
	{
		return;
	}
	m_IsActive = isActive;
	OnActiveChanged(m_IsActive);
}

void GameObject::SetGameTag(string _GameTag)
{
	m_GameTag = _GameTag;
}
string GameObject::GetGameTag()
{
	return m_GameTag;
}

//ICollisionの実装
const Shape* GameObject::GetShape()const {
	return &m_Shape;
}
void GameObject::OnOverlapped(ICollision* _pCollision) {
	GameObject* pGameObject = dynamic_cast<GameObject*>(_pCollision);
	if (pGameObject)
	{
		//衝突による反発力の計算
		CalculateRestitution(pGameObject);
	}
}
GameObject* GameObject::GetGameObject() {
	return this;
}