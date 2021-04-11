using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPoiDungeon : MonoBehaviour
{
    public Image difficultyPrefab;
    public Color easyColor;
    public Color normalColor;
    public Color hardColor;
    public Color emptyColor;

    public HorizontalLayoutGroup DifficultyLayout;

    public Image DungeonImage;
    public Text DungeonName;
    public Text Waves;
    public Text MobTypes;
}
