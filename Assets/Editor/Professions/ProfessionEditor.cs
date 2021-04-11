using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ProfessionEditor : EditorWindow
{
    BaseProfession currentProfession;

    bool recipesFoldout = false;

    [MenuItem("BattleFox/Profession Editor")]
    public static void open()
    {
        EditorWindow.GetWindow<ProfessionEditor>().Show();
    }

    private void OnGUI()
    {
        if (Selection.activeObject is BaseProfession)
            currentProfession = (BaseProfession)Selection.activeObject;
        else
            currentProfession = null;

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Open", GUILayout.ExpandWidth(false)))
        {
            AssetSelector selector = EditorWindow.GetWindow<AssetSelector>();
            AssetSelector.Open("t:BaseProfession", (sel) => {
                currentProfession = (BaseProfession)sel;
                Selection.activeObject = sel;
                Focus();
                
            }, (sel) => sel is BaseProfession);
        }

        if (currentProfession == null)
        {
            if (GUILayout.Button("Create", GUILayout.ExpandWidth(false)))
            {
                BaseProfession prof = new BaseProfession();
                AssetDatabase.CreateAsset(prof, "Assets/Scripts/Data/Professions/Assets/NewProfession.asset");
                Selection.activeObject = prof;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            if (GUILayout.Button("Delete", GUILayout.ExpandWidth(false)))
            {
                if (EditorUtility.DisplayDialog("Deleting", "Are you sure you want to delete " + currentProfession.Name + "?", "I am"))
                {
                    if (Registry.assets.professions.IsPresent(currentProfession.UID))
                    {
                        if (EditorUtility.DisplayDialog("Profession Table", "This profession was registered in the Registry. Should i remove it?", "Yes", "No"))
                        {
                            Registry.assets.professions.Remove(currentProfession.UID);
                        }
                    }
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(currentProfession));
                    currentProfession = null;
                    return;
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();


            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Upload", GUILayout.ExpandWidth(false)))
            {

                currentProfession.Upload();
            }
            if (GUILayout.Button("Download", GUILayout.ExpandWidth(false)))
            {
                currentProfession.Download();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = false;
            currentProfession.UID = EditorGUILayout.TextField("UID:", currentProfession.UID);
            GUI.enabled = true;
            if (GUILayout.Button("Generate UID", GUILayout.ExpandWidth(false)))
            {
                currentProfession.GenerateUID();
            }
            EditorGUILayout.EndHorizontal();
            if (!Registry.assets.professions.IsPresent(currentProfession.UID))
            {
                EditorGUILayout.HelpBox("This item is not registered in the items table! Would you like to add it now?", MessageType.Warning);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Add It!", GUILayout.ExpandWidth(false)))
                {
                    ProfessionTableEntry e = new ProfessionTableEntry();
                    e.UID = currentProfession.UID;
                    e.profession = currentProfession;
                    Registry.assets.professions.entries.Add(e);
                    EditorUtility.SetDirty(Registry.assets.professions);
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            currentProfession.Name = EditorGUILayout.TextField("Name:", currentProfession.Name);
            currentProfession.type = (BaseProfession.Type)EditorGUILayout.EnumPopup("Type:", currentProfession.type);
            currentProfession.Icon = (Sprite)EditorGUILayout.ObjectField("Icon:", currentProfession.Icon, typeof(Sprite));
            if (currentProfession.type == BaseProfession.Type.Crafting)
            {
                recipesFoldout = EditorGUILayout.Foldout(recipesFoldout, "Recipes");
                if (recipesFoldout)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    EditorGUILayout.BeginVertical();
                    for (int i = 0; i < currentProfession.Recipes.Count; i++)
                    {
                        BaseRecipe r = currentProfession.Recipes[i];
                        EditorGUILayout.BeginHorizontal();
                        currentProfession.Recipes[i] = (BaseRecipe)EditorGUILayout.ObjectField(currentProfession.Recipes[i], typeof(BaseRecipe));
                        if (GUILayout.Button("Pick", GUILayout.Width(50)))
                        {
                            int index = i;
                            AssetSelector.Open("t:BaseRecipe", (sel) => {
                                currentProfession.Recipes[index] = (BaseRecipe)sel;
                            }, (sel) => sel is BaseRecipe);
                        }
                        if (GUILayout.Button("Edit", GUILayout.Width(50)))
                        {
                            Selection.activeObject = r;
                            EditorWindow.GetWindow<RecipeEditor>().Show();
                            EditorWindow.GetWindow<RecipeEditor>().Focus();
                        }
                        if (GUILayout.Button("X", GUILayout.Width(30)))
                        {
                            currentProfession.Recipes.RemoveAt(i);
                            i--;
                            return;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space(5);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Add Recipe", GUILayout.Width(90)))
                    {
                        currentProfession.Recipes.Add(new BaseRecipe());
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }
                if (currentProfession != null)
                    EditorUtility.SetDirty(currentProfession);
                EditorUtility.SetDirty(Registry.assets.professions);
            }
        }
    }
}
