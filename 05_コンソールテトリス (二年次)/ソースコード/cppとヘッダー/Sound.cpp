#include "Sound.h"
#include <future> 

Sound::Sound() : isPlaying(false)
{

}
Sound::~Sound()
{
    stop();  // デストラクタでスレッドを停止
}
// BGMの再生開始
void Sound::start() {
    if (isPlaying) return;
    isPlaying = true;

    // 高精度タイマーを設定
    timeBeginPeriod(1);

    playThread = std::thread(&Sound::playMusic, this);
}

// BGMの再生停止
void Sound::stop() {
    if (!isPlaying) return;
    isPlaying = false;
    if (playThread.joinable()) {
        playThread.join();
    }

    // 高精度タイマーをリセット
    timeEndPeriod(1);
}

// 効果音の再生
void Sound::PlaySoundEffect(int _SoundID) {
    if (_SoundID == 1) {  // ライン消去音
        playLineClearSound();
    }
}
void Sound::playNote(int frequency, int duration) {
    const double PI = 3.14159265358979323846;
    double volume = 0.1;
    const int sampleCount = sampleRate * duration / 1000;
    short* buffer = new short[sampleCount];
    double angleIncrement = 2.0 * PI * frequency / sampleRate;
    double fadeFactor = 1.0;

    for (int i = 0; i < sampleCount; i++) {
        buffer[i] = static_cast<short>(volume * 32767 * sin(angleIncrement * i) * fadeFactor);
        if (i > sampleCount - sampleRate * 0.1) {  // フェードアウト処理
            fadeFactor *= 0.99;
        }
    }

    WAVEFORMATEX waveFormat = { 0 };
    waveFormat.wFormatTag = WAVE_FORMAT_PCM;
    waveFormat.nChannels = 1;
    waveFormat.nSamplesPerSec = sampleRate;
    waveFormat.wBitsPerSample = 16;
    waveFormat.nBlockAlign = waveFormat.nChannels * waveFormat.wBitsPerSample / 8;
    waveFormat.nAvgBytesPerSec = waveFormat.nSamplesPerSec * waveFormat.nBlockAlign;

    HWAVEOUT hWaveOut;
    waveOutOpen(&hWaveOut, WAVE_MAPPER, &waveFormat, 0, 0, CALLBACK_NULL);

    WAVEHDR waveHeader = { 0 };
    waveHeader.lpData = reinterpret_cast<LPSTR>(buffer);
    waveHeader.dwBufferLength = sampleCount * sizeof(short);
    waveOutPrepareHeader(hWaveOut, &waveHeader, sizeof(WAVEHDR));
    waveOutWrite(hWaveOut, &waveHeader, sizeof(WAVEHDR));

    // 再生完了を待機
    while (!(waveHeader.dwFlags & WHDR_DONE)) {
        Sleep(1);
    }

    waveOutUnprepareHeader(hWaveOut, &waveHeader, sizeof(WAVEHDR));
    waveOutClose(hWaveOut);
    delete[] buffer;
}
// メロディとベースを合成して再生する関数
void Sound::playNote(int freqMelody, int freqBass, int duration) {
    const double PI = 3.14159265358979323846;
    const int sampleCount = sampleRate * duration / 1000;
    double volume = 0.1;
    short* buffer = new short[sampleCount];
    double angleIncrementMelody = 2.0 * PI * freqMelody / sampleRate;
    double angleIncrementBass = 2.0 * PI * freqBass / sampleRate;

    for (int i = 0; i < sampleCount; i++) {
        double sampleMelody = (freqMelody == 0) ? 0 : sin(angleIncrementMelody * i);
        double sampleBass = (freqBass == 0) ? 0 : sin(angleIncrementBass * i);
        double combinedSample = (sampleMelody + sampleBass) / 2.0;  // 波形を合成して音量を調整
        buffer[i] = static_cast<short>(volume * 32767 * combinedSample);
    }

    WAVEFORMATEX waveFormat = { 0 };
    waveFormat.wFormatTag = WAVE_FORMAT_PCM;
    waveFormat.nChannels = 1;
    waveFormat.nSamplesPerSec = sampleRate;
    waveFormat.wBitsPerSample = 16;
    waveFormat.nBlockAlign = waveFormat.nChannels * waveFormat.wBitsPerSample / 8;
    waveFormat.nAvgBytesPerSec = waveFormat.nSamplesPerSec * waveFormat.nBlockAlign;

    HWAVEOUT hWaveOut;
    waveOutOpen(&hWaveOut, WAVE_MAPPER, &waveFormat, 0, 0, CALLBACK_NULL);

    WAVEHDR waveHeader = { 0 };
    waveHeader.lpData = reinterpret_cast<LPSTR>(buffer);
    waveHeader.dwBufferLength = sampleCount * sizeof(short);
    waveOutPrepareHeader(hWaveOut, &waveHeader, sizeof(WAVEHDR));
    waveOutWrite(hWaveOut, &waveHeader, sizeof(WAVEHDR));

    // 再生完了を待機
    Sleep(sampleCount * 1000 / sampleRate);
    waveOutUnprepareHeader(hWaveOut, &waveHeader, sizeof(WAVEHDR));
    waveOutClose(hWaveOut);

    delete[] buffer;
}

void Sound::playMusic() {
    int melody[] = {
            E_HIGH, 0, B, C_HIGH, D_HIGH, 0, C_HIGH, B,
            A, 0, A, C_HIGH, E_HIGH, 0, D_HIGH, C_HIGH,
            B, 0, 0, C_HIGH, D_HIGH, 0, E_HIGH, 0,
            C_HIGH, 0, A, 0, A, 0, 0, 0,

            0, D_HIGH, 0, F_HIGH, A_HIGH, 0, G_HIGH, F_HIGH,
            E_HIGH, 0,0,C_HIGH, E_HIGH, 0, D_HIGH, C_HIGH,
            B, 0, B, C_HIGH, D_HIGH, 0, E_HIGH, 0,
            C_HIGH, 0, A, 0, A, 0, 0, 0,

            E_HIGH / 2, 0, B / 2, C_HIGH / 2, D_HIGH / 2, 0, C_HIGH / 2, B / 2, A / 2, 0, A / 2, C_HIGH / 2, E_HIGH / 2, 0,
            D_HIGH / 2, C_HIGH / 2, B / 2, 0, 0, C_HIGH / 2, D_HIGH / 2, 0, E_HIGH / 2, 0, C_HIGH / 2, 0, A / 2, 0,
            A / 2, 0, 0, 0, 0,

            D_HIGH / 2, 0, F_HIGH / 2, A_HIGH / 2, 0, G_HIGH / 2, F_HIGH / 2, E_HIGH / 2, 0,0,
            C_HIGH / 2, E_HIGH / 2, 0, D_HIGH / 2, C_HIGH / 2, B / 2, 0, B / 2, C_HIGH / 2, D_HIGH / 2, 0, E_HIGH / 2,
            0, C_HIGH / 2, 0, A / 2, 0, A / 2, 0, 0, 0,

            E_HIGH,0,0,0,C_HIGH,0,0,0,D_HIGH,0,0,0,B,0,0,0,
            C_HIGH,0,0,0,A,0,0,0,415,0,0,0,B,0,0,0,

            E_HIGH,0,0,0,C_HIGH,0,0,0,D_HIGH,0,0,0,B,0,0,0,
            C_HIGH,0,D_HIGH,0,A_HIGH,0,0,0,830,0,0,0,0,0,0,0,
    };

    int bass[] = {
    E / 2, E, E / 2, E, E / 2, E, E / 2, E ,
    A / 2, A, A / 2, A, A / 2, A, A / 2, A ,
    415 / 2, 415, 415 / 2, 415, E / 2, E, E / 2, E ,
    A / 2, A, A / 2, A, A / 2, A, B / 2, C / 2 ,

    D, D / 2, D, D / 2, D, D / 2, A / 2, F / 2,
    C / 2, C, C / 2, C, C / 2, C, G / 2, G,
    B / 2, B, B / 2, B, E / 2, E, 415 / 2, 415,
    A / 2, E, A / 2, E, A / 2, E, A / 2, 0,

    E / 2, E, E / 2, E, E / 2, E, E / 2, E ,
    A / 2, A, A / 2, A, A / 2, A, A / 2, A ,
    415 / 2, 415, 415 / 2, 415, E / 2, E, E / 2, E ,
    A / 2, A, A / 2, A, A / 2, A, B / 2, C / 2 ,

    D, D / 2, D, D / 2, D, D / 2, A / 2, F / 2,
    C / 2, C, C / 2, C, C / 2, C, G / 2, G,
    B / 2, B, B / 2, B, E / 2, E, 415 / 2, 415,
    A / 2, E, A / 2, E, A / 2, E, A / 2, 0,

    E / 2, E, E / 2, E, E / 2, E, E / 2, E ,
    A / 2, A, A / 2, A, A / 2, A, A / 2, A ,
    415 / 2, 415, 415 / 2, 415, E / 2, E, E / 2, E ,
    A / 2, A, A / 2, A, A / 2, A, B / 2, C / 2 ,

    D, D / 2, D, D / 2, D, D / 2, A / 2, F / 2,
    C / 2, C, C / 2, C, C / 2, C, G / 2, G,
    B / 2, B, B / 2, B, E / 2, E, 415 / 2, 415,
    A / 2, E, A / 2, E, A / 2, E, A / 2, 0,
    };
    int duration = 200;  // 各音符の長さ（ミリ秒）
    int length = sizeof(melody) / sizeof(melody[0]);

    while (isPlaying) {
        for (int i = 0; i < length; i++) {
            if (!isPlaying) return;

            std::thread melodyThread([&]() {
                if (melody[i] != 0) {
                    playNote(melody[i] * 2, duration);
                }
                else {
                    std::this_thread::sleep_for(std::chrono::milliseconds(duration)); // 高精度な待機
                }
                });

            std::thread bassThread([&]() {
                if (bass[i] != 0) {
                    playNote(bass[i] * 1, duration);
                }
                else {
                    std::this_thread::sleep_for(std::chrono::milliseconds(duration)); // 高精度な待機
                }
                });

            melodyThread.join();
            bassThread.join();
        }
    }
}


// ライン消去時の効果音を再生する
void Sound::playLineClearSound() {
    std::async(std::launch::async, [this]() {
        const double PI = 3.14159265358979323846;
        int frequencies[] = { 1300, 1100, 900 };  // 高めの周波数
        int durations[] = { 30, 30, 40 };         // 各音の長さ（ミリ秒）
        double volume = 0.1;

        for (int j = 0; j < 3; j++) {
            int sampleCount = sampleRate * durations[j] / 1000;
            short* buffer = new short[sampleCount];

            double angleIncrement = 2.0 * PI * frequencies[j] / sampleRate;
            for (int i = 0; i < sampleCount; i++) {
                buffer[i] = static_cast<short>(volume * 32767 * sin(angleIncrement * i));
            }

            WAVEFORMATEX waveFormat = { 0 };
            waveFormat.wFormatTag = WAVE_FORMAT_PCM;
            waveFormat.nChannels = 1;
            waveFormat.nSamplesPerSec = sampleRate;
            waveFormat.wBitsPerSample = 16;
            waveFormat.nBlockAlign = waveFormat.nChannels * waveFormat.wBitsPerSample / 8;
            waveFormat.nAvgBytesPerSec = waveFormat.nSamplesPerSec * waveFormat.nBlockAlign;

            HWAVEOUT hWaveOut;
            waveOutOpen(&hWaveOut, WAVE_MAPPER, &waveFormat, 0, 0, CALLBACK_NULL);

            WAVEHDR waveHeader = { 0 };
            waveHeader.lpData = reinterpret_cast<LPSTR>(buffer);
            waveHeader.dwBufferLength = sampleCount * sizeof(short);
            waveOutPrepareHeader(hWaveOut, &waveHeader, sizeof(WAVEHDR));
            waveOutWrite(hWaveOut, &waveHeader, sizeof(WAVEHDR));

            // 再生完了を待機
            while (!(waveHeader.dwFlags & WHDR_DONE)) {
                Sleep(1);
            }

            waveOutUnprepareHeader(hWaveOut, &waveHeader, sizeof(WAVEHDR));
            waveOutClose(hWaveOut);
            delete[] buffer;
        }
        });
}