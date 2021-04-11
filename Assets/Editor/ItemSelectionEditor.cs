using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ItemSelectionEditor : EditorWindow {

    string cat = "all";
    Vector2 scrollPos;

    public static System.Action<BaseItem> onSelect;
    string filter = "";
    void OnGUI()
    {

        EditorGUILayout.BeginHorizontal();
        Color oldC = GUI.color;

        if (cat == "all")
            GUI.color = Color.green;
        if (GUILayout.Button("All", GUILayout.ExpandWidth(false)))
        {
            cat = "all";
        }
        GUI.color = oldC;
        if (cat == "weapon")
            GUI.color = Color.green;        
        if (GUILayout.Button("Weapons", GUILayout.ExpandWidth(false)))
        {
            cat = "weapon";
        }
        GUI.color = oldC;
        if (cat == "armor")
            GUI.color = Color.green;
        if (GUILayout.Button("Armor", GUILayout.ExpandWidth(false)))
        {
            cat = "armor";
        }
        GUI.color = oldC;
        if (cat == "consumable")
            GUI.color = Color.green;
        if (GUILayout.Button("Consumable", GUILayout.ExpandWidth(false)))
        {
            cat = "consumable";
        }
        GUI.color = oldC;
        if (cat == "special")
            GUI.color = Color.green;
        if (GUILayout.Button("Special", GUILayout.ExpandWidth(false)))
        {
            cat = "special";
        }
        GUI.color = oldC;
        EditorGUILayout.EndHorizontal();
        filter = EditorGUILayout.TextField("Filter:", filter);
        GUILayout.Space(10);
        string[] assets = AssetDatabase.FindAssets("t:BaseItem");

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        foreach (string s in assets)
        {
            string path = AssetDatabase.GUIDToAssetPath(s);
            BaseItem i = AssetDatabase.LoadAssetAtPath<BaseItem>(path);
            if (!string.IsNullOrEmpty(filter) && !i.Name.ToLower().Contains(filter.ToLower()))
            {
                continue;
            }
            if (cat == "all" || cat == i.category)
            {
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
                        EditorWindow.GetWindow<ItemEditor>().Focus();
                    }
                    this.Close();
                }
                EditorGUILayout.EndHorizontal();
                Rect r = GUILayoutUtility.GetLastRect();
                EditorGUILayout.LabelField(i.Description);
                r.y += 5;
                r.x = 250;
                r.width = 30;
                r.height = 30;
                EditorGUI.DrawTextureTransparent(r, i.sprite.texture);
                EditorGUILayout.EndVertical();
                GUILayout.Space(2);
            }
            
        }

        EditorGUILayout.EndScrollView();
    }
}
