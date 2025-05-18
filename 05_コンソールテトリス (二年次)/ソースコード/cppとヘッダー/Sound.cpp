#include "Sound.h"
#include <future> 

Sound::Sound() : isPlaying(false)
{

}
Sound::~Sound()
{
    stop();  // �f�X�g���N�^�ŃX���b�h���~
}
// BGM�̍Đ��J�n
void Sound::start() {
    if (isPlaying) return;
    isPlaying = true;

    // �����x�^�C�}�[��ݒ�
    timeBeginPeriod(1);

    playThread = std::thread(&Sound::playMusic, this);
}

// BGM�̍Đ���~
void Sound::stop() {
    if (!isPlaying) return;
    isPlaying = false;
    if (playThread.joinable()) {
        playThread.join();
    }

    // �����x�^�C�}�[�����Z�b�g
    timeEndPeriod(1);
}

// ���ʉ��̍Đ�
void Sound::PlaySoundEffect(int _SoundID) {
    if (_SoundID == 1) {  // ���C��������
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
        if (i > sampleCount - sampleRate * 0.1) {  // �t�F�[�h�A�E�g����
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

    // �Đ�������ҋ@
    while (!(waveHeader.dwFlags & WHDR_DONE)) {
        Sleep(1);
    }

    waveOutUnprepareHeader(hWaveOut, &waveHeader, sizeof(WAVEHDR));
    waveOutClose(hWaveOut);
    delete[] buffer;
}
// �����f�B�ƃx�[�X���������čĐ�����֐�
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
        double combinedSample = (sampleMelody + sampleBass) / 2.0;  // �g�`���������ĉ��ʂ𒲐�
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

    // �Đ�������ҋ@
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
    int duration = 200;  // �e�����̒����i�~���b�j
    int length = sizeof(melody) / sizeof(melody[0]);

    while (isPlaying) {
        for (int i = 0; i < length; i++) {
            if (!isPlaying) return;

            std::thread melodyThread([&]() {
                if (melody[i] != 0) {
                    playNote(melody[i] * 2, duration);
                }
                else {
                    std::this_thread::sleep_for(std::chrono::milliseconds(duration)); // �����x�ȑҋ@
                }
                });

            std::thread bassThread([&]() {
                if (bass[i] != 0) {
                    playNote(bass[i] * 1, duration);
                }
                else {
                    std::this_thread::sleep_for(std::chrono::milliseconds(duration)); // �����x�ȑҋ@
                }
                });

            melodyThread.join();
            bassThread.join();
        }
    }
}


// ���C���������̌��ʉ����Đ�����
void Sound::playLineClearSound() {
    std::async(std::launch::async, [this]() {
        const double PI = 3.14159265358979323846;
        int frequencies[] = { 1300, 1100, 900 };  // ���߂̎��g��
        int durations[] = { 30, 30, 40 };         // �e���̒����i�~���b�j
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

            // �Đ�������ҋ@
            while (!(waveHeader.dwFlags & WHDR_DONE)) {
                Sleep(1);
            }

            waveOutUnprepareHeader(hWaveOut, &waveHeader, sizeof(WAVEHDR));
            waveOutClose(hWaveOut);
            delete[] buffer;
        }
        });
}