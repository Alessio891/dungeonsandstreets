using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FeatureEditor : EditorWindow {

    public FeatureAsset current;
    public bool Locked = false;

    Vector2 scrollPos;
    public List<bool> toggled = new List<bool>();
    List<int> indexToRemove = new List<int>();

    float sumOfWeights = 0;

    [MenuItem("BattleFox/Feature editor")]
    public static void open()
    {
        EditorWindow.GetWindow<FeatureEditor>().Show();
    }

    void OnGUI()
    {
        if (!Locked)
        {

            if (Selection.activeObject != null && Selection.activeObject.GetType() == typeof(FeatureAsset))
                current = (FeatureAsset)Selection.activeObject;
            else
                current = null;
        }
        if (current == null)
        {
            Locked = false;                        
        }

        //EditorGUILayout.LabelField("Selected " + current.FeatureName);
        drawMenus();
        if (current == null)
            return;
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Upload", GUILayout.ExpandWidth(false)))
        {
           current.Upload();
        }
        if (GUILayout.Button("Download", GUILayout.ExpandWidth(false)))
        {
           current.Download();
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        if (!Registry.assets.features.isPresent(current.UID))
        {
            EditorGUILayout.HelpBox("This feature is not present in the registry. Would you like to add it?", MessageType.Warning);
            if (GUILayout.Button("Add It", GUILayout.ExpandWidth(false)))
            {
                Feature f = new Feature();
                f.key = current.UID;
                f.feature = current;
                Registry.assets.features.features.Add(f);
                EditorUtility.SetDirty(Registry.assets.features);
            }
        }

        drawCurrent();
        EditorUtility.SetDirty(current); 

    }

    void drawMenus()
    {
        EditorGUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Create", GUILayout.ExpandWidth(false)))
        {
            FeatureAsset newF = new FeatureAsset();
            newF.possibleItems = new List<FeatureDropData>();
            AssetDatabase.CreateAsset(newF, "Assets/Scripts/Data/Features/newFeature.asset");
            Selection.activeObject = newF;            
        }
        if (GUILayout.Button("Open", GUILayout.ExpandWidth(false))) {
            EditorWindow.GetWindow<FeatureLoadingWindow>().Show();
        }
        if (GUILayout.Button("Save", GUILayout.ExpandWidth(false))) { }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    void drawCurrent()
    {
        EditorGUILayout.BeginHorizontal();
        if (Locked)
        {
            if (GUILayout.Button("Unlock"))
                Locked = false;
        }
        else
        {
            if (GUILayout.Button("Lock"))
                Locked = true;
        }
        if (GUILayout.Button("Select"))
        {
            Selection.activeObject = current;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();
        current.UID = EditorGUILayout.TextField("Feature UID:", current.UID);
        if (GUILayout.Button("Generate", GUILayout.ExpandWidth(false)))
        {
            current.GenerateUID();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        current.FeatureName = EditorGUILayout.TextField("Feature Name:", current.FeatureName);        
        EditorGUILayout.EndHorizontal();
        current.FeatureType = EditorGUILayout.TextField("Feature Type:", current.FeatureType);
        current.OnMapFeature = (BasicFeature)EditorGUILayout.ObjectField("Map Feature:", current.OnMapFeature, typeof(BasicFeature));
        current.Rarity = EditorGUILayout.Slider("Weight", current.Rarity, -1, 500);
        if (current.isNode)
        {
            string[] split = current.extraData.Split(':');
            if (split.Length <= 1)
            {
                current.extraData += ":0";
                split = current.extraData.Split(':');
            }
            EditorGUILayout.BeginHorizontal();
            BaseProfession prof = Registry.assets.professions[split[0]];
            prof = (BaseProfession)EditorGUILayout.ObjectField(prof, typeof(BaseProfession));
            split[1] = EditorGUILayout.IntField(int.Parse(split[1])).ToString();
           
            current.extraData = ( (prof != null) ? prof.UID : "" ) + ":" + split[1];
            EditorGUILayout.EndHorizontal();

        }
        else
            current.extraData = EditorGUILayout.TextField("Extra Data:", current.extraData);
        EditorGUILayout.EndVertical();
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal("box");
        current.minGoldDrop = EditorGUILayout.IntField("Min Gold Drop:", current.minGoldDrop);
        current.maxGoldDrop = EditorGUILayout.IntField("Max Gold Drop:", current.maxGoldDrop);
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal("box");
        current.minItemDrops = EditorGUILayout.IntField("Min Items Drop:", current.minItemDrops);
        current.maxItemDrops = EditorGUILayout.IntField("Max Items Drop:", current.maxItemDrops);
        EditorGUILayout.EndHorizontal();
        current.isNode = EditorGUILayout.Toggle("Is Node", current.isNode);
        drawBiomeDataEntry("open_world_grass");
        drawBiomeDataEntry("open_world_sea");
        drawBiomeDataEntry("forest");
        drawBiomeDataEntry("village");
        drawBiomeDataEntry("town");
        drawItemsDrop();
    }
    void drawBiomeDataEntry(string key)
    {
        if (!current.BiomesData.ContainsKey(key))
            current.BiomesData.Add(key, 0.0);
        current.BiomesData[key] = EditorGUILayout.DoubleField(key + ":", current.BiomesData[key]);
    }
    void drawItemsDrop()
    {
        EditorGUILayout.BeginVertical("box");
        GUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Droppable Items");
        if (GUILayout.Button("Add Item", GUILayout.ExpandWidth(false)))
        {
            current.possibleItems.Add(new FeatureDropData());
        }
        if (GUILayout.Button("Import Loot Table", GUILayout.ExpandWidth(false)))
        {
            ImportLootTableEditor.get((r) => {
                foreach (FeatureDropData d in r)
                {
                    bool present = false;
                    foreach (FeatureDropData d2 in current.possibleItems)
                    {
                        if (d2.item == null)
                            continue;
                        if (d2.item.UID == d.item.UID)
                        {
                            present = true;
                            break;
                        }
                    }
                    if (present)
                        continue;
                    current.possibleItems.Add(d);
                }
            });
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);
        EditorGUI.indentLevel++;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        indexToRemove.Clear();
        int index = 0;
        sumOfWeights = 0;
        foreach (FeatureDropData d in current.possibleItems)
        {
            sumOfWeights += d.weight;         
        }
        foreach (FeatureDropData d in current.possibleItems)
        {
            drawItem(d, index);
            index++;
        }
        EditorGUILayout.EndScrollView();
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
        foreach (int i in indexToRemove)
            current.possibleItems.RemoveAt(i);
    }

    void drawItem(FeatureDropData data, int index)
    {
        // count = 0
        // index = 0

        if (index > toggled.Count - 1)
        {
            for (int i = toggled.Count - 1; i < index+1; i++)
            {
                toggled.Add(false);
            }
        }
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();
        float perc = data.weight / sumOfWeights;
        perc *= 100;
        float truncated = (float)(System.Math.Truncate((double)perc * 100.0) / 100.0);

        toggled[index] = EditorGUILayout.Foldout(toggled[index], (data.item != null) ? data.item.Name + " ["+truncated.ToString()+"%]": "Empty");
        if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
        {
            indexToRemove.Add(index);
        }
        EditorGUILayout.EndHorizontal();

        if (toggled[index])
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            data.item = (BaseItem)EditorGUILayout.ObjectField("Item:", data.item, typeof(BaseItem));
            if (GUILayout.Button("Pick", GUILayout.ExpandWidth(false)))
            {
                ItemSelectionEditor.onSelect = (item) => {
                    data.item = item;
                    ItemSelectionEditor.onSelect = null;              
                };
                EditorWindow.GetWindow<ItemSelectionEditor>().Show();
            }
            EditorGUILayout.EndHorizontal();
            if (data.item != null)
            {
                data.weight = EditorGUILayout.FloatField("Weight:", data.weight);
                EditorGUILayout.LabelField("Item Description: " + data.item.Description);                
            }
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.EndVertical();
    }
}
