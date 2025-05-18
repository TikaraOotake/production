#pragma once

#include "GameObject.h"



class SquareGround :public GameObject
{
public:
	SquareGround();
	~SquareGround();

	void Init() override;
	void Update() override;

	void OnOverlapped(ICollision* _pCollision) override;
private:

};

