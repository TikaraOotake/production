#include"TextureFilePathLibrary.h"

TextureFilePathLibrary::TextureFilePathLibrary()
{
}

TextureFilePathLibrary::~TextureFilePathLibrary()
{
}

string TextureFilePathLibrary::GetTextureFilePath(int _Index, string _Type)
{
	string FilePath = "Images/temp/FFFFFF.png";
	if (_Type == "Temp")
	{
		if (_Index == 0) { FilePath = "Images/temp/FFFFFF.png"; }
		if (_Index == 1) { FilePath = "Images/temp/FF0000.png"; }
	}
	if (_Type == "Fruit")
	{
		if (_Index == 0) { FilePath = "Images/FruitSimulator/fruit/00_cherry.png"; }
		if (_Index == 1) { FilePath = "Images/FruitSimulator/fruit/01_strawberry.png"; }
		if (_Index == 2) { FilePath = "Images/FruitSimulator/fruit/02_grapes.png"; }
		if (_Index == 3) { FilePath = "Images/FruitSimulator/fruit/03_orange.png"; }
		if (_Index == 4) { FilePath = "Images/FruitSimulator/fruit/04_persimmon.png"; }
		if (_Index == 5) { FilePath = "Images/FruitSimulator/fruit/05_apple.png"; }
		if (_Index == 6) { FilePath = "Images/FruitSimulator/fruit/06_pear.png"; }
		if (_Index == 7) { FilePath = "Images/FruitSimulator/fruit/07_peach.png"; }
		if (_Index == 8) { FilePath = "Images/FruitSimulator/fruit/08_pineapple.png"; }
		if (_Index == 9) { FilePath = "Images/FruitSimulator/fruit/09_melon.png"; }
		if (_Index == 10) { FilePath = "Images/FruitSimulator/fruit/10_watermelon.png"; }
	}
	if (_Type == "Background")
	{
		if (_Index == 3) { FilePath = "Images/FruitSimulator/BackGround_Table3.jpg"; }
		if (_Index == 4) { FilePath = "Images/FruitSimulator/Bottle01.png"; }
		if (_Index == 5) { FilePath = "Images/FruitSimulator/Bottle02.png"; }
		if (_Index == 6) { FilePath = "Images/FruitSimulator/Bottle03.png"; }
		if (_Index == 7) { FilePath = "Images/FruitSimulator/Bottle04.png"; }
		if (_Index == 8) { FilePath = "Images/FruitSimulator/Bottle05.png"; }
		if (_Index == 9) { FilePath = "Images/FruitSimulator/Bottle_Long.png"; }
	}
	if (_Type == "Character")
	{
		if (_Index == 0) { FilePath = "Images/FruitSimulator/Cloud.png"; }
	}
	if (_Type == "UI")
	{
		if (_Index == 0) { FilePath = "Images/FruitSimulator/NextUI_Dish.png"; }
		if (_Index == 1) { FilePath = "Images/FruitSimulator/NextUI_Dish_01.png"; }
		if (_Index == 2) { FilePath = "Images/FruitSimulator/GameOver.png"; }
		if (_Index == 3) { FilePath = "Images/FruitSimulator/GameOver_Fruit.png"; }
		if (_Index == 4) { FilePath = "Images/FruitSimulator/GameOver_Wood.png"; }
		if (_Index == 5) { FilePath = "Images/FruitSimulator/TitleLogo01.png"; }
	}

	return FilePath;
}