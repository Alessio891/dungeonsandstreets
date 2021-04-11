using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class AssetSelector : EditorWindow {


    public System.Action<ScriptableObject> OnSelect;
    public System.Func<ScriptableObject, bool> Condition;
    public string type;
    
    Vector2 scrollPos;
    //public Data data;
    public static void Open(string type, System.Action<ScriptableObject> select, System.Func<ScriptableObject, bool> cond = null)
    {
        AssetSelector w = EditorWindow.GetWindow<AssetSelector>();
        w.OnSelect = select;
        w.type = type;
        w.Condition = cond;
        w.Show();
    }

    void OnGUI()
    {
        string[] assets = AssetDatabase.FindAssets(type);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        foreach (string s in assets)
        {
            string path = AssetDatabase.GUIDToAssetPath(s);
            ScriptableObject i = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

            if (Condition != null)
            {
                if (!Condition(i))
                    continue;
            }

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            string[] split = path.Split('/');
            string name = split[split.Length - 1].Replace(".asset", "");
            EditorGUILayout.LabelField(name);
            if (GUILayout.Button("Load", GUILayout.Height(25)))
            {
                if (OnSelect != null)
                {
                    OnSelect(i);
                }
                Close();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();

    }
}
