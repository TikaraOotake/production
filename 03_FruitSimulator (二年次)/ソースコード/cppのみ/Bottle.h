#pragma once

#include "GameObject.h"



class Bottle :public GameObject
{
public:
	Bottle();
	~Bottle();

	void Init() override;
	void Update() override;

	void OnOverlapped(ICollision* _pCollision) override;
private:

};

