using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapTagsEditor : EditorWindow
{
   
    MapTags.Type current = null;
    List<bool> toggles = new List<bool>();    
    public GUISkin skin;
    bool addingData = false;
    string newDataString = "";
    bool addingCat = false;
    string newCatString = "";
    Vector2 scrollPos;

    [MenuItem("BattleFox/Map Tags Editor")]
    static void show()
    {
        EditorWindow.GetWindow<MapTagsEditor>().Show();
    }

    private void OnGUI()
    {
        if (skin == null)
        {
            skin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/Editor/MapTagsEditor.guiskin");
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load Data"))
        {
            if (MapTags.Data != null)
            {
                if (EditorUtility.DisplayDialog("Attenzione", "Stai per caricare dei dati. Se hai apportato modifiche verranno annullate. Sei sicuro di voler continuare?", "Si", "No"))
                {
                    MapTags.LoadData();
                    current = null;
                }
            }
            else
            {
                MapTags.LoadData();
                current = null;
            }
        }
        if (GUILayout.Button("Save Data"))
        {
            if (EditorUtility.DisplayDialog("Attenzione", "Stai per salvare i dati. Se non hai effettuato un backup i vecchi dati verranno persi. Sei sicuro?", "Si", "No"))
            {
                Save();
            }
        }
        if (GUILayout.Button("Upload Data"))
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("data", MapTags.GetStringData());
            Bridge.POST(Bridge.url + "UploadMapTags", data, (r) => {
                ServerResponse resp = new ServerResponse(r);
                Debug.Log(resp.message);
            });
        }
        if (MapTags.Data == null)
            return;
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        if (current == null)
        {            
            current = MapTags.Data[0];
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        foreach (MapTags.Type t in MapTags.Data)
        {
         
            if (GUILayout.Button(t.Name, (current.Name == t.Name) ? skin.FindStyle("selected") : skin.FindStyle("not_selected")))
            {
                current = t;
            }
           
        }
        if (!addingData && GUILayout.Button("+", GUILayout.Width(30)))
        {
            addingData = true;
        }
        if (addingData)
        {            
            EditorGUILayout.BeginVertical();
            newDataString = EditorGUILayout.TextField(newDataString, skin.textField, GUILayout.Width(100));
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                MapTags.Data.Add(new MapTags.Type() { Name = newDataString, Categories = new List<MapTags.Category>() });
                addingData = false;
            }
            if (GUILayout.Button("Cancel", GUILayout.Width(60)))
            {
                addingData = false;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(15);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int i = 0; i < current.Categories.Count; i++)
        {
            if (toggles.Count-1 < i)
            {
                toggles.Add(false);
            }
            
            MapTags.Category c = current.Categories[i];
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(5);
            toggles[i] = EditorGUILayout.Foldout(toggles[i], c.Name);
            if (GUILayout.Button("X", GUILayout.Width(30)))
            {
                if (EditorUtility.DisplayDialog("Attenzione", "Stai per rimuovere la categoria " + c.Name + ". Sei sicuro?", "Si", "No"))
                {
                    //MapTags.GetType(current.Name).Categories.RemoveAt(i);
                    current.Categories.RemoveAt(i);
                    i--;
                }
            }
            GUILayout.Space(5);
            EditorGUILayout.EndHorizontal();
            if (toggles[i])
            {
                for(int j = 0; j < c.Tags.Count; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15);
                    int ind = MapTags.PossibleTags.IndexOf(c.Tags[j]);
                    if (ind < 0)
                    {
                        ind = 0;                            
                        c.Tags[j] = MapTags.PossibleTags[0];
                    }
                    c.Tags[j] = MapTags.PossibleTags[EditorGUILayout.Popup(ind, MapTags.PossibleTags.ToArray())];
                    if (GUILayout.Button("P", GUILayout.Width(30)))
                    {
                        string t = c.Tags[j];
                        MapTagPicker.Show(current.Name, c.Name, t);
                    }
                    if (GUILayout.Button("X", GUILayout.Width(30)))
                    {
                        if (EditorUtility.DisplayDialog("Attenzione", "Stai per rimuovere il tag " + c.Tags[j] + ". Sei sicuro?", "Si", "No"))
                        {
                            c.Tags.RemoveAt(j);
                            j--;
                        }
                    }
                    GUILayout.Space(5);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Add Tag"))
                {
                    c.Tags.Add(MapTags.PossibleTags[0]);
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
        GUILayout.Space(15);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (!addingCat)
        {
            if (GUILayout.Button("Add Category"))
            {
                addingCat = true;
            }
            if (GUILayout.Button("Remove this Type"))
            {
                if (EditorUtility.DisplayDialog("Attenzione", "Stai per rimuovere il tipo " + current.Name + ". Sei sicuro?", "Si", "No"))
                {
                    MapTags.Data.RemoveAt(MapTags.Data.IndexOf(current));
                    current = MapTags.Data[0];
                }
            }
        } else
        {
            newCatString = EditorGUILayout.TextField(newCatString);
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                MapTags.GetType(current.Name).Categories.Add(new MapTags.Category() { Name = newCatString, Tags = new List<string>() });
                current = MapTags.GetType(current.Name);
                //current.Categories.Add(new MapTags.Category() { Name = newCatString, Tags = new List<string>() });
                addingCat = false;
            } 
            if (GUILayout.Button("Cancel", GUILayout.Width(60)))
            {
                addingCat = false;
            }
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    public void Save()
    {
        MapTags.SaveData();       
    }
}
