using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ImportLootTableEditor : EditorWindow {

    public static ImportLootTableEditor w;
    public static System.Action<List<FeatureDropData>> onImport;

    public LootTable selected;

    public static void get(System.Action<List<FeatureDropData>> OnImport)
    {
        if (w == null)
            w = EditorWindow.GetWindow<ImportLootTableEditor>();

        w.Show();

        onImport = OnImport;
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Importing Loot Table");
        selected = (LootTable)EditorGUILayout.ObjectField("Loot Table:", selected, typeof(LootTable));
        if (selected != null)
        {
            
        }
    }
}
