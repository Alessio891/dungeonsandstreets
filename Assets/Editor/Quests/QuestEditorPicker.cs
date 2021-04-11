using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class QuestEditorPicker : EditorWindow
{
    string filter = "";
    Vector2 scrollPos;
    public static System.Action<BaseQuest> onSelect;
    private void OnGUI()
    {
        filter = EditorGUILayout.TextField("Filter:", filter);
        GUILayout.Space(10);
        string[] assets = AssetDatabase.FindAssets("t:BaseQuest");

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        foreach (string s in assets)
        {
            string path = AssetDatabase.GUIDToAssetPath(s);
            BaseQuest i = AssetDatabase.LoadAssetAtPath<BaseQuest>(path);
            if (!string.IsNullOrEmpty(filter) && !i.QuestTitle.ToLower().Contains(filter.ToLower()))
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
                    EditorWindow.GetWindow<QuestEditor>().Focus();
                }
                this.Close();
            }
            EditorGUILayout.EndHorizontal();
            Rect r = GUILayoutUtility.GetLastRect();
            EditorGUILayout.LabelField(i.QuestDescription);

            EditorGUILayout.EndVertical();
            GUILayout.Space(2);
            

        }

        EditorGUILayout.EndScrollView();
    }
}
