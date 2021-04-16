using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class MonsterEditor : EditorWindow {

    
    public BaseMonster monster;
    public BaseEnemy enemy;

    [MenuItem("BattleFox/Monster editor")]
    public static void open()
    {
        EditorWindow.GetWindow<MonsterEditor>().Show();
    }

    void OnGUI()
    {


        drawMenu();
        if (monster == null)
        {
            return;
        }



        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Monster Data");
     
            
           
        EditorGUILayout.BeginHorizontal();
        GUI.enabled = false;
        monster = (BaseMonster)EditorGUILayout.ObjectField("Monster Asset:", monster, typeof(BaseMonster));
        GUI.enabled = true;
       
        EditorGUILayout.EndHorizontal();
      

        monster.Name = EditorGUILayout.TextField("Monster:", monster.Name);
        monster.Type = (MonsterType)EditorGUILayout.EnumPopup("Type:",monster.Type);
        monster.MaxHp = EditorGUILayout.IntField("MaxHp:", monster.MaxHp);
        monster.Str = EditorGUILayout.IntField("Str:", monster.Str);
        monster.Dex = EditorGUILayout.IntField("Dex:", monster.Dex);
        monster.Int = EditorGUILayout.IntField("Int:", monster.Int);
        monster.BaseDamage = EditorGUILayout.IntField("Damage:", monster.BaseDamage);
        monster.Level = EditorGUILayout.IntField("Level:", monster.Level);
        EditorGUILayout.LabelField("Gold Drop (Min\\Max)");
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        monster.minGold = EditorGUILayout.IntField(monster.minGold, GUILayout.Width(70));
        monster.maxGold = EditorGUILayout.IntField(monster.maxGold, GUILayout.Width(70));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        monster.XP = EditorGUILayout.IntField("XP Awarded:", monster.XP);

        drawBiomeDataEntry("open_world_grass");
        drawBiomeDataEntry("open_world_sea");
        drawBiomeDataEntry("forest");
        drawBiomeDataEntry("village");
        drawBiomeDataEntry("town");



        EditorGUILayout.BeginVertical("box");
        if (monster.elementalData == null)
            monster.elementalData = new List<ElementalData>();
        if (GUILayout.Button("Add Elemental Data"))
        {
            monster.elementalData.Add(new ElementalData());
        }
                
        for (int i = 0; i < monster.elementalData.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            monster.elementalData[i].ElementName = EditorGUILayout.TextField("Element:", monster.elementalData[i].ElementName, GUILayout.ExpandWidth(false));
            monster.elementalData[i].amount = EditorGUILayout.IntField("Amount (%):", monster.elementalData[i].amount, GUILayout.ExpandWidth(false));
            if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
            {
                monster.elementalData.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
        //Registry.assets.monsters.r
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Loot");
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("Type", GUILayout.Width(100));
        EditorGUILayout.LabelField("Chance", GUILayout.Width(100));
        EditorGUILayout.LabelField("Amount", GUILayout.Width(100));
        EditorGUILayout.LabelField("", GUILayout.Width(60));
        EditorGUILayout.LabelField("", GUILayout.Width(50));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        for (int i = 0; i < monster.Drops.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            monster.Drops[i].Type = EditorGUILayout.TextField(monster.Drops[i].Type, GUILayout.Width(100));
            monster.Drops[i].Chance = EditorGUILayout.FloatField(monster.Drops[i].Chance, GUILayout.Width(100));
            monster.Drops[i].Amount = EditorGUILayout.IntField(monster.Drops[i].Amount, GUILayout.Width(100));
            
            if (GUILayout.Button("Pick", GUILayout.Width(60)))
            {
                FeatureDropData data = monster.Drops[i];
                ItemSelectionEditor.onSelect += (item) => {
                    data.Type = item.ItemID;
                    Focus();
                };
                EditorWindow.GetWindow<ItemSelectionEditor>().Show();
            }
            if (GUILayout.Button("X", GUILayout.Width(50)))
            {
                monster.Drops.RemoveAt(i);
                return;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add Drop", GUILayout.Width(80)))
        {
            monster.Drops.Add(new FeatureDropData() { Type = "", Amount = 1, Chance = 1.0f });
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        
        if (monster != null)
        {
            if (GUILayout.Button("Upload"))
            {
              
                 monster.Upload();
                
            }
        }
        EditorGUILayout.EndVertical();
       
        if (monster == null)
            enemy = null;
        else        
            enemy = Resources.Load<BaseEnemy>(monster.ModelPath);

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Combat Data");
        enemy = (BaseEnemy)EditorGUILayout.ObjectField("Combat Model:", enemy, typeof(BaseEnemy));
        EditorGUILayout.EndVertical();

        if (monster != null)
        {
            monster.ModelPath = AssetDatabase.GetAssetPath(enemy).Replace("Assets/Resources/", "").Replace(".prefab", "");
        }
       
        if (monster != null)
            EditorUtility.SetDirty(monster);
        if (enemy != null)
            EditorUtility.SetDirty(enemy);
        EditorUtility.SetDirty(Registry.assets.features);
        EditorUtility.SetDirty(Registry.assets.monsters);
    }

    void drawBiomeDataEntry(string key)
    {
        if (!monster.BiomesData.ContainsKey(key))
            monster.BiomesData.Add(key, 0.0);
        monster.BiomesData[key] = EditorGUILayout.DoubleField(key + ":", monster.BiomesData[key]);
    }

    void drawMenu() {
        EditorGUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Open", GUILayout.ExpandWidth(false)))
        {
            MonsterSelectionEditor.onSelect = (m) =>
            {
                monster = m;
                EditorWindow.GetWindow<MonsterEditor>().Focus();
                MonsterSelectionEditor.onSelect = null;
            };
            EditorWindow.GetWindow<MonsterSelectionEditor>().Show();
        }
        if (GUILayout.Button("Create", GUILayout.ExpandWidth(false)))
        {
            BaseMonster f = new BaseMonster();
            f.Name= "NewMonster";
            
            AssetDatabase.CreateAsset(f, "Assets/Scripts/Data/Monsters/NewMonster.asset");
            monster = f;
        }
        if (monster != null)
        {
            if (GUILayout.Button("Delete", GUILayout.ExpandWidth(false)))
            { }
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }


}
