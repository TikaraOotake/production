#include"CollisionMng.h"
#include "AppDef.h"
#include "ICollision.h"
#include "Shape.h"
#include "Fwk/Framework.h"

//クラス唯一のインスタンスのアドレスを保持する
CollisionMng* CollisionMng::g_instance = nullptr;

//クラス唯一のインスタンスの生成
void CollisionMng::GenerateInstance() {
	if (g_instance != nullptr) { return; }
	g_instance = new CollisionMng();
}
//クラス唯一のインスタンスの破棄
void CollisionMng::DestroyInstance() {
	if (g_instance != nullptr) { delete g_instance; g_instance = nullptr; } 
}
//クラス唯一のインスタンスを取得する
CollisionMng* CollisionMng::GetInstance() {
	return g_instance;
}

CollisionMng::CollisionMng()
	:m_pCollisionsVector()
{
	m_pCollisionsVector.push_back(nullptr);
	Init();
}

CollisionMng::~CollisionMng() {
	Term();
}

void CollisionMng::Init() {
}
void CollisionMng::Process() {
	if (m_pCollisionsVector.size() <= 1)
	{
		//要素が1つ以下であれば終了
		return;
	}
	for (size_t i = 0; i < m_pCollisionsVector.size() - 1; ++i) {
		if (m_pCollisionsVector[i] == nullptr) {
			continue;
		}

		ICollision* pColA = m_pCollisionsVector[i];
		const Shape* pShapeA = pColA->GetShape();

		for (size_t n = i + 1; n < m_pCollisionsVector.size(); ++n) {

			if (m_pCollisionsVector[n] == nullptr) {
				continue;
			}

			ICollision* pColB = m_pCollisionsVector[n];
			const Shape* pShapeB = pColB->GetShape();

			// AとBが当たっているか
			if (pShapeA->IsOverlapped(pShapeB)) {
				pColA->OnOverlapped(pColB);
				pColB->OnOverlapped(pColA);
			}
		}
	}
}
void CollisionMng::Term() {
	Init();
}
//登録関数
void CollisionMng::Register(ICollision* pCollision) {
	if (pCollision == nullptr) {
		DebugLog("キマリは通さない");
		return;
	}

	//重複をチェック
	for (ICollision* ptr : m_pCollisionsVector)
	{
		if (ptr == pCollision)
		{
			//すでに登録されているコリジョンであれば関数を抜ける
			return;
		}
	}

	//インデックス追加
	m_pCollisionsVector.push_back(pCollision);
	DebugLog("要素いれておくで");
}

//登録解除
void CollisionMng::Unregister(ICollision* pCollision) {
	if (pCollision == nullptr) {
		return;
	}
	if (m_pCollisionsVector.size() == 0) {
		// 要素が1つもなかったら終了
		return;
	}

	// ベクター内で一致するポインタを削除する
	for (auto it = m_pCollisionsVector.begin(); it != m_pCollisionsVector.end();) {
		ICollision* ptr = *it;
		if (ptr == pCollision) {
			// 一致したポインタを削除し、次のイテレータを取得
			it = m_pCollisionsVector.erase(it);
			break;
		}
		else {
			++it;  // 一致しなかった場合は次に進む
		}
	}
}