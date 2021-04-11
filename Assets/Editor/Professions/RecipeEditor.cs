using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RecipeEditor : EditorWindow
{
    BaseRecipe currentRecipe;

    bool ingredientsFoldout = false;
    bool resultsFoldout = false;
    Vector2 scrollPos;

    [MenuItem("BattleFox/Recipe Editor")]
    public static void open()
    {
        EditorWindow.GetWindow<RecipeEditor>().Show();
    }

    private void OnGUI()
    {
        if (Selection.activeObject is BaseRecipe)
            currentRecipe = (BaseRecipe)Selection.activeObject;
        else
            currentRecipe = null;

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Open", GUILayout.ExpandWidth(false)))
        {
            AssetSelector selector = EditorWindow.GetWindow<AssetSelector>();
            AssetSelector.Open("t:BaseRecipe", (sel) =>
            {
                currentRecipe = (BaseRecipe)sel;
                Selection.activeObject = sel;
                Focus();

            }, (sel) => sel is BaseRecipe);
        }

        if (currentRecipe == null)
        {
            if (GUILayout.Button("Create", GUILayout.ExpandWidth(false)))
            {
                BaseRecipe prof = new BaseRecipe();
                AssetDatabase.CreateAsset(prof, "Assets/Scripts/Data/Professions/Assets/NewRecipe.asset");
                Selection.activeObject = prof;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            if (GUILayout.Button("Delete", GUILayout.ExpandWidth(false)))
            {
                if (EditorUtility.DisplayDialog("Deleting", "Are you sure you want to delete " + currentRecipe.name + "?", "I am"))
                {
                    if (Registry.assets.recipes.IsPresent(currentRecipe.UID))
                    {
                        if (EditorUtility.DisplayDialog("Recipe Table", "This recipe was registered in the Registry. Should i remove it?", "Yes", "No"))
                        {
                            Registry.assets.recipes.Remove(currentRecipe.UID);
                        }
                    }
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(currentRecipe));
                    currentRecipe = null;
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

                currentRecipe.Upload();
            }
            if (GUILayout.Button("Download", GUILayout.ExpandWidth(false)))
            {
                currentRecipe.Download();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = false;
            currentRecipe.UID = EditorGUILayout.TextField("UID:", currentRecipe.UID);
            GUI.enabled = true;
            if (GUILayout.Button("Generate UID", GUILayout.ExpandWidth(false)))
            {
                currentRecipe.GenerateUID();
            }
            EditorGUILayout.EndHorizontal();
            currentRecipe.MinimumNeededValue = EditorGUILayout.IntSlider("Minimum Needed Value:",currentRecipe.MinimumNeededValue, 0, 150);
            if (!Registry.assets.recipes.IsPresent(currentRecipe.UID))
            {
                EditorGUILayout.HelpBox("This item is not registered in the items table! Would you like to add it now?", MessageType.Warning);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Add It!", GUILayout.ExpandWidth(false)))
                {
                    RecipeTableEntry e = new RecipeTableEntry();
                    e.UID = currentRecipe.UID;
                    e.recipe = currentRecipe;
                    Registry.assets.recipes.entries.Add(e);
                    EditorUtility.SetDirty(Registry.assets.recipes);
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            ingredientsFoldout = EditorGUILayout.Foldout(ingredientsFoldout, "Ingredients");
            if (ingredientsFoldout)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                EditorGUILayout.BeginVertical();
                for(int i = 0; i < currentRecipe.Ingredients.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    currentRecipe.Ingredients[i].Item = (BaseItem)EditorGUILayout.ObjectField(currentRecipe.Ingredients[i].Item, typeof(BaseItem));
                    currentRecipe.Ingredients[i].Amount = EditorGUILayout.IntField(currentRecipe.Ingredients[i].Amount);
                    if (GUILayout.Button("Pick", GUILayout.Width(40)))
                    {
                        int index = i;
                        AssetSelector.Open("t:BaseItem", (sel) => {
                            currentRecipe.Ingredients[index].Item = (BaseItem)sel;
                        }, (sel) => sel is BaseItem);
                    }
                    if (GUILayout.Button("X", GUILayout.Width(30)))
                    {
                        currentRecipe.Ingredients.RemoveAt(i);
                        i--;
                        return;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Add", GUILayout.Width(50)))
                {
                    currentRecipe.Ingredients.Add(new BaseRecipe.RecipeItemData());
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.Space(10);
            resultsFoldout = EditorGUILayout.Foldout(resultsFoldout, "Results:");
            if (resultsFoldout)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                EditorGUILayout.BeginVertical();
                for (int i = 0; i < currentRecipe.Result.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    currentRecipe.Result[i].Item = (BaseItem)EditorGUILayout.ObjectField(currentRecipe.Result[i].Item, typeof(BaseItem));
                    currentRecipe.Result[i].Amount = EditorGUILayout.IntField(currentRecipe.Result[i].Amount);
                    if (GUILayout.Button("Pick", GUILayout.Width(40)))
                    {
                        int index = i;
                        AssetSelector.Open("t:BaseItem", (sel) => {
                            currentRecipe.Result[index].Item = (BaseItem)sel;
                        }, (sel) => sel is BaseItem);
                    }
                    if (GUILayout.Button("X", GUILayout.Width(30)))
                    {
                        currentRecipe.Result.RemoveAt(i);
                        i--;
                        return;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Add", GUILayout.Width(50)))
                {
                    currentRecipe.Result.Add(new BaseRecipe.RecipeItemData());
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            if (currentRecipe != null)
                EditorUtility.SetDirty(currentRecipe);
            EditorUtility.SetDirty(Registry.assets.recipes);
        }
    }
}
