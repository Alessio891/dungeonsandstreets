using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FeatureLoadingWindow : EditorWindow {


    Vector2 scrollPos;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void OnGUI () {
        string[] assets = AssetDatabase.FindAssets("t:FeatureAsset");

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        foreach (string s in assets)
        {
            string path = AssetDatabase.GUIDToAssetPath(s);
            FeatureAsset i = AssetDatabase.LoadAssetAtPath<FeatureAsset>(path);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            string[] split = path.Split('/');
            string name = split[split.Length - 1].Replace(".asset", "");
            EditorGUILayout.LabelField(name + ":");
            if (GUILayout.Button("Load", GUILayout.Height(25)))
            {
                Selection.activeObject = i;
                EditorWindow.GetWindow<FeatureEditor>().Focus();
                this.Close();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            GUILayout.Space(2);
            
        }

        EditorGUILayout.EndScrollView();
    }
}
