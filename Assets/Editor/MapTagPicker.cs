using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapTagPicker : EditorWindow
{
    public static System.Action<string> OnPick;

    public static string type = "";
    public static string category = "";
    public static string oldTag = "";

    public static void Show(string _type, string _cat, string _oldTag)
    {
        type = _type;
        category = _cat;
        oldTag = _oldTag;
        Debug.Log("Showing with " + type + ", " + category + ", " + oldTag);
        EditorWindow.GetWindow<MapTagPicker>().Show();
    }

    string filter = "";
    bool doFilter = false;
    static int currentLetter = 0;
    string[] Letters = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "j", "k","l", "i", "m", "n", "o", "p", "q", "r", "s", "v", "t", "w", "x", "y", "z" };
    Vector2 scrollPos;

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        filter = EditorGUILayout.TextField(filter);
        if (GUILayout.Button((doFilter) ? "Stop" : "Search", GUILayout.Width(60)))
        {
            doFilter = !doFilter;
        }
        EditorGUILayout.EndHorizontal();
        currentLetter = EditorGUILayout.Popup(currentLetter, Letters);

        if (!doFilter)
        {
            int columnIndex = 0;
            foreach(string s in MapTags.PossibleTags)
            {
               
                if (s.ToLower().StartsWith(Letters[currentLetter]))
                {
                    if (columnIndex == 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                    }
                    if (GUILayout.Button(new GUIContent(s, s)))
                    {
                        List<string> tags = MapTags.GetType(type).GetCategory(category).Tags;
                        int tagIndex = tags.IndexOf(oldTag);
                        Debug.Log("Setting " + s + " to " + type + ", " + category + ", " + oldTag + "-- TagIndex is " + tagIndex);
                        foreach (string ts in tags)
                        {
                            Debug.Log("Tag: " + ts);
                        }
                        MapTags.GetType(type).GetCategory(category).Tags[tagIndex] = s;
                        this.Close();
                    }
                    columnIndex++;
                }
                if (columnIndex == 5)
                {
                    EditorGUILayout.EndHorizontal();
                    columnIndex = 0;
                }
            }
            
        } else
        {
            int columnIndex = 0;
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            foreach (string s in MapTags.PossibleTags)
            {

                if (s.ToLower().Contains(filter))
                {
                    if (columnIndex == 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                    }
                    if (GUILayout.Button(new GUIContent(s, s)))
                    {

                        List<string> tags = MapTags.GetType(type).GetCategory(category).Tags;
                        int tagIndex = tags.IndexOf(oldTag);
                        Debug.Log("Setting " + s + " to " + type + ", " + category + ", " + oldTag + "-- TagIndex is " + tagIndex);
                        foreach(string ts in tags)
                        {
                            Debug.Log("Tag: " + ts);
                        }
                        MapTags.GetType(type).GetCategory(category).Tags[tagIndex] = s;
                        this.Close();
                    }
                    columnIndex++;
                }
                if (columnIndex == 5)
                {
                    EditorGUILayout.EndHorizontal();
                    columnIndex = 0;
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }

}
