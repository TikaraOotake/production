#if defined(DEBUG) || defined(_DEBUG)
#define _CRTDBG_MAP_ALLOC
#include <crtdbg.h>
#endif

#include <Windows.h>
#include <Fwk/Framework.h>
#include <Lib/Util/Log.h>
#include <Fwk/Graphics/Font.h>

#include "App.h"
#include "AppDef.h"



LRESULT WndProc(HWND hwnd, UINT msg, WPARAM wp, LPARAM lp)
{
	switch (msg)
	{
	case WM_DESTROY:
	{
		PostQuitMessage(0);
	}
	break;
	default:
		break;
	}

	return DefWindowProc(hwnd, msg, wp, lp);
}
#ifdef _DEBUG
int wmain(int argc, wchar_t** argv, wchar_t** envp)
#else
int WINAPI WinMain(HINSTANCE,HINSTANCE,LPSTR,int)
#endif
{
#if defined(DEBUG) || defined(_DEBUG)
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif

	Fwk::Framework::GenerateInstance();

	Fwk::FrameworkDesc desc = {};
	desc.width = WINDOW_WIDTH;
	desc.height = WINDOW_HEIGHT;
	desc.pClassName = WINDOW_CLASS_NAME;
	desc.pWndTitle = WINDOW_TITLE;
	desc.WndProc = WndProc;
	desc.AssetRootDir = ASSET_FOLDER_NAME;
	desc.AssetBufferSize = ASSET_BUFFER_SIZE;
	desc.RenderRequestBufferSize = RENDER_REQUEST_MAX;
	desc.SpriteFontFile = SPRITE_FONT;
	desc.VtxShaderFile = VTX_SHADER;
	desc.PixShaderFile = PIX_SHADER;
	desc.TargetFPS = TARGET_FRAMERATE;
	desc.DeltaTimeMsMax = DELTA_TIME_MS_MAX;
	desc.DescriptorSize_Resouce = DESCRIPTOR_SIZE_RES;
	desc.DescriptorSize_Sampler = DESCRIPTOR_SIZE_SMP;
	desc.CollisionBufferSize = COLLISION_BUFFER_SIZE;
	
 	//フレームワークの初期化
	if (!Framework_I->Init(desc)) {
		return false;
	}

	//終了時にappのデストラクタが先に走る様にしておく為に{}しておく
	{
		App app;
		app.Init();

		MSG msg = {};
		while (msg.message != WM_QUIT)
		{
			if (PeekMessage(&msg, nullptr, 0, 0, PM_REMOVE) == TRUE) {
				TranslateMessage(&msg);
				DispatchMessage(&msg);
			}

			Framework_I->Update();
			app.Update();
			//更新処理が終わった後にコリジョンを処理する
			Framework_I->GetCollisionManager()->ProcCollisions();

			app.Render();
			Framework_I->Render();

			Framework_I->FrameSync();
		}

		//GPUの処理が終わっていることを確認
		RenderManager_I->SyncGpuProc();

		//appの終了処理
		app.Term();
	}

	Framework_I->Term();
	Fwk::Framework::ReleaseInstance();

	return 0;
}