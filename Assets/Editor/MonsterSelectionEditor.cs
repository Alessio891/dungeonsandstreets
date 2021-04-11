using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MonsterSelectionEditor : EditorWindow {

    public static System.Action<BaseMonster> onSelect;
    public static System.Action<BaseMonster> onBaseMonsterSelect;
    Vector2 scrollPos;

    void OnGUI()
    {
        string[] assets = AssetDatabase.FindAssets("t:BaseMonster");
        //Debug.Log("assets size " + assets.Length);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);        
        foreach (string s in assets)
        {
            //Debug.Log("Found " + s);
            string path = AssetDatabase.GUIDToAssetPath(s);
            BaseMonster i = AssetDatabase.LoadAssetAtPath<BaseMonster>(path);

            
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            string[] split = path.Split('/');
            string name = split[split.Length - 1].Replace(".asset", "");
            EditorGUILayout.LabelField(name + ":");
            if (GUILayout.Button("Load", GUILayout.Height(25)))
            {
                if (onSelect != null)
                {
                    onSelect(i);
                }
                this.Close();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            
        }
        EditorGUILayout.EndScrollView();
    }
}
