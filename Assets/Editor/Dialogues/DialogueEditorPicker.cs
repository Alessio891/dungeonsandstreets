using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogueEditorPicker : EditorWindow
{
    string filter = "";
    Vector2 scrollPos;
    public static System.Action<DialogueAsset> onSelect;
    private void OnGUI()
    {
        filter = EditorGUILayout.TextField("Filter:", filter);
        GUILayout.Space(10);
        string[] assets = AssetDatabase.FindAssets("t:DialogueAsset");

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        foreach (string s in assets)
        {
            string path = AssetDatabase.GUIDToAssetPath(s);
            DialogueAsset i = AssetDatabase.LoadAssetAtPath<DialogueAsset>(path);
            if (!string.IsNullOrEmpty(filter) && !i.DialogueTitle.ToLower().Contains(filter.ToLower()))
            {
                continue;
            }

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
                else
                {
                    Selection.activeObject = i;
                    EditorWindow.GetWindow<DialogueEditor>().Focus();
                }
                this.Close();
            }
            EditorGUILayout.EndHorizontal();
            Rect r = GUILayoutUtility.GetLastRect();
            EditorGUILayout.LabelField(i.DialogueText);

            EditorGUILayout.EndVertical();
            GUILayout.Space(2);


        }

        EditorGUILayout.EndScrollView();
    }
}
