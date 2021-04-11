using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BattleFoxSimulator : EditorWindow {

    public string currentSection = "Stats";

    int playerStr = 1;
    int playerDex = 1;
    int playerInt = 1;
    int playerToughness = 1;
    int playerLuck = 1;
    int playerMaxHp = 1;
    int playerLevel = 1;

    int enemyStr = 1;
    int enemyDex = 1;
    int enemyHP = 1;
    int enemyLevel = 1;

    BaseWeapon rightHand;

    [MenuItem("BattleFox/Simulator")]
    public static void open()
    {
        EditorWindow.GetWindow<BattleFoxSimulator>().Show();
    }
    
    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();

        Color c = GUI.color;
        if (currentSection == "Stats")
            GUI.color = Color.green;
        if (GUILayout.Button("Stats", GUILayout.ExpandWidth(false)))
        {
            currentSection = "Stats";
        }
        GUI.color = c;
        if (currentSection == "Combat")
            GUI.color = Color.green;
        if (GUILayout.Button("Combat", GUILayout.ExpandWidth(false)))
        {
            currentSection = "Combat";
        }

        GUI.color = c;
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        //if (currentSection == "Stats")
            DrawStats();
        //else if (currentSection == "Combat")
            DrawCombat();

    }

    void DrawStats() {
        GUILayout.Space(10);
        playerMaxHp = 10 + (playerToughness / 2);
        GUI.enabled = false;
        EditorGUILayout.IntField("Max HP:", playerMaxHp);
        GUI.enabled = true;
        playerLevel = EditorGUILayout.IntField("Level:", playerLevel);
        GUILayout.Space(5);
        playerStr = EditorGUILayout.IntField("Str:", playerStr);
        playerDex = EditorGUILayout.IntField("Dex:", playerDex);
        playerInt = EditorGUILayout.IntField("Int:", playerInt);
        playerToughness = EditorGUILayout.IntField("Toughness:", playerToughness);
        playerLuck = EditorGUILayout.IntField("Luck:", playerLuck);

        GUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();              
        rightHand = (BaseWeapon) EditorGUILayout.ObjectField("Right Hand:", rightHand, typeof(BaseWeapon));
        if (GUILayout.Button("Pick", GUILayout.ExpandWidth(false)))
        {
            ItemSelectionEditor.onSelect = (i) => {
                if (i.GetType() == typeof(BaseWeapon))
                {
                    rightHand = (BaseWeapon)i;
                    this.Focus();
                    ItemSelectionEditor.onSelect = null;               
                }
            };
            EditorWindow.GetWindow<ItemSelectionEditor>().Show();
        }
        EditorGUILayout.EndHorizontal();
    }

    int calculateDamage(string stat, int weaponDamage)
    {
        float statPoints = 0;
        if (stat == "Str")
            statPoints = playerStr * 0.5f;
        else if (stat == "Dex")
            statPoints = playerDex * 0.5f;

        return (int)(statPoints + (weaponDamage * 0.8f));
    }

    void DrawCombat() {
        string weapon = "Unarmed";
        int damage = 1;
        double hitChance = 85;
        double critChance = 10;
        string statUsed = "Str";
        if (rightHand != null)
        {
            weapon = rightHand.Name;
            damage = rightHand.Damage;
            hitChance = rightHand.HitChance;
            critChance = rightHand.CritChance;
            statUsed = rightHand.StatUsed;
        }
        GUI.enabled = false;
        EditorGUILayout.TextField("Weapon Name:", weapon);
        EditorGUILayout.IntField("Weapon Damage:", damage);
        EditorGUILayout.DoubleField("Hit Chance:", hitChance);
        EditorGUILayout.DoubleField("Crit Chance:", critChance);
        GUI.enabled = true;

        enemyLevel = EditorGUILayout.IntField("Enemy Level:", enemyLevel);
        enemyStr = EditorGUILayout.IntField("Enemy Str:", enemyStr);
        enemyDex = EditorGUILayout.IntField("Enemy Dex:", enemyDex);
        enemyHP = EditorGUILayout.IntField("Enemy MaxHp:", enemyHP);

        EditorGUILayout.LabelField("On average, this weapon will deal " + calculateDamage(statUsed, damage).ToString() + " points of damage");

        // hit -> playerLevel-mobLevel

        double hit = hitChance + (playerLevel - enemyLevel) + ( (playerDex - enemyDex)/2);

        EditorGUILayout.LabelField("You will have a " + hit.ToString() + "% chance to hit.");

        if (playerLevel == 0)
            playerLevel = 1;
        double crit = critChance + (playerLuck / (2 * playerLevel));

        EditorGUILayout.LabelField("This weapon will crit " + crit.ToString() + "% of the times.");
    }

}
