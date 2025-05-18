// ItemSubsystem.cpp
#include "ItemSubsystem.h"

void UItemSubsystem::Initialize(FSubsystemCollectionBase& Collection)
{
    Super::Initialize(Collection);
    // �������R�[�h
    // 
    
    // ItemFlag�̏������i�T�C�Y�⏉���l�̐ݒ�Ȃǁj
    ItemFlag.SetNum(10); // �Ⴆ�΁A10�̃A�C�e���t���O������
    for (bool& Flag : ItemFlag)
    {
        Flag = false; // ������Ԃł͂��ׂ�false
    }
}
bool UItemSubsystem::GetItemFlag(int _Index)
{
    bool result = false;
    //�v�f�ԍ����z������`�F�b�N
    if (_Index >= 0 && _Index < 10)
    {
        result = ItemFlag[_Index];
    }
    return result;
}
void UItemSubsystem::SetItemFlag(int _Index, bool _Flag)
{
    //�v�f�ԍ����z������`�F�b�N
    if (_Index >= 0 && _Index < 10)
    {
        ItemFlag[_Index]= _Flag;
    }
}