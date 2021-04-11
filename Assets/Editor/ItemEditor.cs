using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ItemEditor : EditorWindow {
    public BaseItem currentItem;
    public string[] categories = new string[4] { "consumable", "weapon", "armor", "special" };
    Vector2 scrollPos;
    int catIndex = 0;

    [MenuItem("BattleFox/Item Editor")]
    public static void open()
    {
        EditorWindow.GetWindow<ItemEditor>().Show();
    }

    void OnGUI()
    {

        if (Selection.activeObject is BaseItem)
            currentItem = (BaseItem)Selection.activeObject;
        else
            currentItem = null;

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Open", GUILayout.ExpandWidth(false)))
        {
            EditorWindow.GetWindow<ItemSelectionEditor>().Show();
        }

        if (currentItem == null)
        {
            if (GUILayout.Button("Create", GUILayout.ExpandWidth(false)))
            {
                BaseItem item = new BaseItem();
                AssetDatabase.CreateAsset(item, "Assets/Resources/Data/Items/NewItem.asset");
                Selection.activeObject = item;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            if (GUILayout.Button("Delete", GUILayout.ExpandWidth(false)))
            {
                if (EditorUtility.DisplayDialog("Deleting", "Are you sure you want to delete " + currentItem.Name + "?", "I am"))
                {
                    if (Registry.assets.items.IsPresent(currentItem.UID))
                    {
                        if (EditorUtility.DisplayDialog("Item Table", "This item was registered in the Registry. Should i remove it?", "Yes", "No"))
                        {
                            Registry.assets.items.Remove(currentItem.UID);
                        }
                    }
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(currentItem));
                    currentItem = null;
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
               
              currentItem.Upload();
            }
            if (GUILayout.Button("Download", GUILayout.ExpandWidth(false)))
            {
                if (!EditorApplication.isPlaying)
                {
                    if (EditorUtility.DisplayDialog("Not in play!", "You must be in Play Mode to download an item. Activate play mode?", "Do it!"))
                    {
                        EditorApplication.isPlaying = true;
                    }
                }
                else
                    currentItem.Download();                                
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUI.enabled = false;
            EditorGUILayout.BeginHorizontal();
            currentItem.UID = EditorGUILayout.TextField("UID:", currentItem.UID);
            GUI.enabled = true;
            if (GUILayout.Button("Generate UID", GUILayout.ExpandWidth(false)))
            {
                currentItem.GenerateUID();
            }
            EditorGUILayout.EndHorizontal();
            if (!Registry.assets.items.IsPresent(currentItem.UID))
            {
                EditorGUILayout.HelpBox("This item is not registered in the items table! Would you like to add it now?", MessageType.Warning);
                if (GUILayout.Button("Add It!", GUILayout.ExpandWidth(false)))
                {
                    ItemsTableEntry e = new ItemsTableEntry();
                    e.ID = currentItem.UID;
                    e.path = AssetDatabase.GetAssetPath(currentItem).Replace("Assets/Resources/", "").Replace(".asset", "");
                    Registry.assets.items.items.Add(e);
                    EditorUtility.SetDirty(Registry.assets.items);
                }
            }
            currentItem.Name = EditorGUILayout.TextField("Name:", currentItem.Name);
            currentItem.name = currentItem.Name;
            currentItem.ItemQuality = (BaseItem.Quality)EditorGUILayout.EnumPopup("Quality:", currentItem.ItemQuality);
            currentItem.Description = EditorGUILayout.TextField("Description:", currentItem.Description);
            currentItem.Value = EditorGUILayout.IntField("Value:", currentItem.Value);
            currentItem.CanStack = EditorGUILayout.Toggle("Can Stack", currentItem.CanStack);
            currentItem.logic = EditorGUILayout.TextField("Logic:", currentItem.logic);
            currentItem.UsableInCombat = EditorGUILayout.Toggle("Usable in Combat:", currentItem.UsableInCombat);
            if (currentItem.Value < 0)
                currentItem.Value = 0;
            currentItem.sprite = (Sprite)EditorGUILayout.ObjectField("Graphic:", currentItem.sprite, typeof(Sprite));
            for(int i = 0; i < categories.Length; i++)
            {
                if (categories[i] == currentItem.category)
                {
                    catIndex = i;
                    break;
                }
                
            }
            int oldCatIndex = catIndex;
            catIndex = EditorGUILayout.Popup("Category:", catIndex, categories);
            currentItem.category = categories[catIndex];
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            if (catIndex != oldCatIndex)
            {
                RecreateAsset(categories[catIndex]);
            }
            if (currentItem.category == "weapon" && currentItem.GetType() == typeof(BaseWeapon))
            {
                BaseWeapon w = (BaseWeapon)currentItem;
                //EditorGUILayout.BeginHorizontal("box");
                w.Damage = EditorGUILayout.IntField("Weapon Damage:", w.Damage);
                w.DamageType = EditorGUILayout.TextField("Damage Type:", w.DamageType);             
                //EditorGUILayout.EndHorizontal();
                w.HitChance = EditorGUILayout.DoubleField("Hit Chance:", w.HitChance);
                w.CritChance = EditorGUILayout.DoubleField("Crit chance:", w.CritChance);
                w.StatUsed = EditorGUILayout.TextField("Stat:", w.StatUsed);
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Refine Items Needed");
                if (GUILayout.Button("Add"))
                {
                    RefineItemEntry refineEntry = new RefineItemEntry();
                    refineEntry.key = "";
                    refineEntry.amount = 1;
                    w.refineItemsNeeded.Add(refineEntry);
                }
                for (int j = 0; j < w.refineItemsNeeded.Count; j++)
                {
                    EditorGUILayout.BeginVertical("box");
                    RefineItemEntry refined = w.refineItemsNeeded[j];
                    BaseItem i = Resources.Load<BaseItem>(Registry.assets.items[refined.key]);
                    i = (BaseItem)EditorGUILayout.ObjectField("Item:", i, typeof(BaseItem));
                    if (i != null)
                        w.refineItemsNeeded[j].key = i.UID;                    
                    w.refineItemsNeeded[j].amount = EditorGUILayout.IntField("Amount:", w.refineItemsNeeded[j].amount);
                    if (GUILayout.Button("Remove"))
                    {
                        w.refineItemsNeeded.RemoveAt(j);
                        j--;
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.LabelField("Growth");

                if (GUILayout.Button("Add"))
                {
                    GrowthData d = new GrowthData();
                    d.growth = new List<int>();
                    for (int i = 0; i < 10; i++) d.growth.Add(0);
                    d.stat = "stat";
                    w.refineGrowth.Add(d);
                }

                for(int j= 0; j < w.refineGrowth.Count; j++)
                {
                    GrowthData d = w.refineGrowth[j];
                    EditorGUILayout.BeginVertical("box");
                    w.refineGrowth[j].stat = EditorGUILayout.TextField("Stat:", w.refineGrowth[j].stat);
                    EditorGUILayout.BeginHorizontal();
                    for(int i = 0; i < d.growth.Count; i++)
                    {
                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(30));
                        w.refineGrowth[j].growth[i] = EditorGUILayout.IntField(d.growth[i], GUILayout.Width(30));
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();
                    if (GUILayout.Button("Remove"))
                    {
                        w.refineGrowth.RemoveAt(j);
                        j--;
                    }
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Sockets");
                if (GUILayout.Button("Add"))
                {
                    w.sockets.Add("");
                }
                for (int i = 0; i < w.sockets.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    w.sockets[i] = EditorGUILayout.TextField("Socket Type:", w.sockets[i]);
                    if (GUILayout.Button("Remove"))
                    {
                        w.sockets.RemoveAt(i);
                        i--;
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
            }
            else if (currentItem.category == "consumable" && currentItem.GetType() == typeof(BaseConsumable))
            {
                BaseConsumable c = (BaseConsumable)currentItem;
                c.statsToModify = EditorGUILayout.TextField("Stats:", c.statsToModify);
                c.amountToModify = EditorGUILayout.IntField("Amount:", c.amountToModify);
            }
            //currentItem.sprite.texture
            EditorGUILayout.EndScrollView();
        }

        if (currentItem != null)
            EditorUtility.SetDirty(currentItem);
        EditorUtility.SetDirty(Registry.assets.items);
    }

    void RecreateAsset(string category)
    {
        string path = AssetDatabase.GetAssetPath(currentItem);
        if (category == "consumable")
        {
            BaseConsumable c = new BaseConsumable();
            c.Name = currentItem.Name;
            c.name = c.Name;
            c.UID = currentItem.UID;
            c.Value = currentItem.Value;
            c.sprite = currentItem.sprite;
            c.Description = currentItem.Description;
            AssetDatabase.CreateAsset(c, path);
            Selection.activeObject = c;
        }
        else if (category == "weapon")
        {
            BaseWeapon c = new BaseWeapon();
            c.Name = currentItem.Name;
            c.name = c.Name;
            c.UID = currentItem.UID;
            c.Value = currentItem.Value;
            c.sprite = currentItem.sprite;
            c.Description = currentItem.Description;
            AssetDatabase.CreateAsset(c, path);
            Selection.activeObject = c;
        }
        else if (category == "armor")
        {
            BaseArmor c = new BaseArmor();
            c.Name = currentItem.Name;
            c.name = c.Name;
            c.UID = currentItem.UID;
            c.Value = currentItem.Value;
            c.sprite = currentItem.sprite;
            c.Description = currentItem.Description;
            AssetDatabase.CreateAsset(c, path);
            Selection.activeObject = c;
        }
    }
}
