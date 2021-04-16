using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class ItemEditor : EditorWindow {
    public BaseItem currentItem;
    public string[] categories = new string[4] { "consumable", "weapon", "armor", "special" };
    Vector2 scrollPos;
    int catIndex = 0;
    int typeSelected = 0;

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

                EditorInputDialog.Show((name) => {
                    this.Focus();
                    BaseItem item = null;
                    switch(typeSelected)
                    {
                        case 0:
                            item = new BaseConsumable();
                            item.category = "Consumable";
                            break;
                        case 1:
                            item = new BaseWeapon();
                            item.category = "Weapon";
                            break;
                        case 2:
                            item = new BaseArmor();
                            item.category = "Armor";
                            break;
                        case 3:
                            item = new BaseItem();
                            item.category = "Special";
                            break;
                    }
                    item.Name = name;
                    item.ItemID = name.Replace(" ", "_");
                    AssetDatabase.CreateAsset(item, "Assets/Resources/Data/Items/"+name+".asset");
                    Selection.activeObject = item;
                }, "NewItem", () => this.Focus() );
            }
            typeSelected = EditorGUILayout.Popup(typeSelected, new string[] { "Consumable", "Weapon", "Armor", "Special" });
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            if (GUILayout.Button("Delete", GUILayout.ExpandWidth(false)))
            {
                if (EditorUtility.DisplayDialog("Deleting", "Are you sure you want to delete " + currentItem.Name + "?", "I am"))
                {
                    if (Registry.assets.items.IsPresent(currentItem.ItemID))
                    {
                        if (EditorUtility.DisplayDialog("Item Table", "This item was registered in the Registry. Should i remove it?", "Yes", "No"))
                        {
                            Registry.assets.items.Remove(currentItem.ItemID);
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
            
            EditorGUILayout.BeginHorizontal();
            currentItem.ItemID = EditorGUILayout.TextField("UID:", currentItem.ItemID);
            
            if (GUILayout.Button("Generate UID", GUILayout.ExpandWidth(false)))
            {
                currentItem.GenerateUID();
            }
            EditorGUILayout.EndHorizontal();
            if (!Registry.assets.items.IsPresent(currentItem.ItemID))
            {
                EditorGUILayout.HelpBox("This item is not registered in the items table! Would you like to add it now?", MessageType.Warning);
                if (GUILayout.Button("Add It!", GUILayout.ExpandWidth(false)))
                {
                    ItemsTableEntry e = new ItemsTableEntry();
                    e.ID = currentItem.ItemID;
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
            if (currentItem is BaseConsumable)
            {
                (currentItem as BaseConsumable).logic = EditorGUILayout.TextField("Logic:", (currentItem as BaseConsumable).logic);
                (currentItem as BaseConsumable).UsableInCombat = EditorGUILayout.Toggle("Usable in Combat:", (currentItem as BaseConsumable).UsableInCombat);
            }
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
            currentItem.category = EditorGUILayout.TextField("Category:", currentItem.category);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
          
            if (currentItem is BaseWeapon)
            {
                BaseWeapon w = (BaseWeapon)currentItem;
                //EditorGUILayout.BeginHorizontal("box");
                w.Damage = EditorGUILayout.IntField("Weapon Damage:", w.Damage);
                w.DamageType = EditorGUILayout.TextField("Damage Type:", w.DamageType);             
                //EditorGUILayout.EndHorizontal();
                w.HitChance = EditorGUILayout.DoubleField("Hit Chance:", w.HitChance);
                w.CritChance = EditorGUILayout.DoubleField("Crit chance:", w.CritChance);
                w.StatUsed = EditorGUILayout.TextField("Stat:", w.StatUsed);                              
            }
            else if (currentItem is BaseArmor)
            {
                BaseArmor a = (BaseArmor)currentItem;
                a.Defense = EditorGUILayout.IntField("Defense:", a.Defense);
                a.Slot = EditorGUILayout.TextField("Slot:", a.Slot);
            }
            else if (currentItem is BaseConsumable)
            {
                BaseConsumable c = (BaseConsumable)currentItem;
                c.statsToModify = EditorGUILayout.TextField("Stats:", c.statsToModify);
                c.amountToModify = EditorGUILayout.IntField("Amount:", c.amountToModify);
            }

            if (currentItem is BaseEquippable)
            {
                BaseEquippable e = (BaseEquippable)currentItem;
                e.Slot = EditorGUILayout.TextField("Slot:", e.Slot);
                EditorGUILayout.BeginVertical("box");
                for(int i = 0; i < e.ItemMods.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal("box");
                   
                    for (int j = 0; j < e.ItemMods[i].data.Count; j++)
                    {                        
                        EditorGUILayout.BeginHorizontal(); 
                        EditorGUILayout.LabelField(e.ItemMods[i].data[j].key, GUILayout.Width(60));
                        e.ItemMods[i].data[j].value = EditorGUILayout.TextField(e.ItemMods[i].data[j].value.ToString(), GUILayout.Width(70));
                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            e.ItemMods[i].data.RemoveAt(j);
                            return;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    if (GUILayout.Button("Add", GUILayout.Width(40)))
                    {
                        int _i = i;
                        BaseEquippable equip = e; 
                        EditorInputDialog.Show((s) => { 

                            equip.ItemMods[_i].data.Add(new ItemModDataValue() { key = s, value = "new_value" });
                        }, "mod_key", () => this.Focus());
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Add Static Mod", GUILayout.Width(150)))
                {
                    e.ItemMods.Add(new ItemModData());
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
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
            c.ItemID = currentItem.ItemID;
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
            c.ItemID = currentItem.ItemID;
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
            c.ItemID = currentItem.ItemID;
            c.Value = currentItem.Value;
            c.sprite = currentItem.sprite;
            c.Description = currentItem.Description;
            AssetDatabase.CreateAsset(c, path);
            Selection.activeObject = c;
        }
    }
}
