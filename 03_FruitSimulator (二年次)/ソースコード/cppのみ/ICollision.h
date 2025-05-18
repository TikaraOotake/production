#pragma once//インクルードガード（多重読み込みを防ぐ）

//コリジョンインターフェース

//Shapeクラスの前方宣言
class Shape;
class GameObject;

class ICollision
{
public:
	virtual const Shape* GetShape()const = 0;
	virtual void OnOverlapped(ICollision* pCollision) = 0;
	virtual GameObject* GetGameObject() = 0;
};

