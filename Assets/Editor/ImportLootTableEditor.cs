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
            foreach (FeatureDropData d in selected.drops)
            {
                if (d.item != null)
                    EditorGUILayout.LabelField("- Item: " + d.item.Name + " - Weight: " + d.weight.ToString());
                else
                    EditorGUILayout.LabelField("- Item: Empty");
            }

            if (GUILayout.Button("Import"))
            {
                onImport(selected.drops);
                EditorWindow.GetWindow<FeatureEditor>().Focus();
                this.Close();
            }


        }
    }
}
