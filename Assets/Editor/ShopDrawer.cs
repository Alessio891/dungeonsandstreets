using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapPOI), true)]
public class ShopDrawer : Editor {
	bool listToggle = true;
    bool questListToggle = true;

	public List<bool> elementToggles = new List<bool>();
    public List<bool> tagByText = new List<bool>();

	public override void OnInspectorGUI()
	{
        MapPOI poi = (MapPOI)target;
        poi.UID = EditorGUILayout.TextField ("UID:", poi.UID);
        poi.POIName = EditorGUILayout.TextField ("Name:", poi.POIName);
        poi.POIText = EditorGUILayout.TextField("POI Text:", poi.POIText);        
        poi.NpcAvatar = (Sprite)EditorGUILayout.ObjectField(poi.NpcAvatar, typeof(Sprite));        
        if (poi is BaseShop)
        {
            BaseShop shop = (BaseShop)target;

            shop.maxItems = EditorGUILayout.IntField("Max Items: ", shop.maxItems);
            //listToggle = EditorGUILayout.BeginToggleGroup ("[Possible Items]", listToggle);
            listToggle = EditorGUILayout.Foldout(listToggle, "[Possible Items]");
            if (listToggle)
            {
                int count = 0;
                List<int> toBeRemoved = new List<int>();
                foreach (ShopEntry entry in shop.entries)
                {
                    EditorGUILayout.BeginVertical("box");
                    if (elementToggles.Count <= count)
                    {
                        for (int i = 0; i < count + 1; i++)
                        {
                            elementToggles.Add(false); 
                        }
                    }
                    BaseItem loaded = Resources.Load<BaseItem>(Registry.assets.items[entry.Item]);
                    elementToggles[count] = EditorGUILayout.Foldout(elementToggles[count], (loaded != null) ? loaded.Name : "Empty");
                    if (elementToggles[count])
                    {
                        BaseItem lastLoaded = loaded;

                        loaded = (BaseItem)EditorGUILayout.ObjectField(loaded, typeof(BaseItem));

                        if (loaded != lastLoaded)
                        {
                            entry.Item = loaded.UID;

                            //Debug.Log ("Switching to " + table.items [i].path);
                            EditorUtility.SetDirty(shop);
                        }

                        //EditorGUILayout.BeginHorizontal ();
                        entry.minAmount = EditorGUILayout.IntField("Min Amount:", entry.minAmount);
                        entry.maxAmount = EditorGUILayout.IntField("Max Amount:", entry.maxAmount);
                        entry.minPrice = EditorGUILayout.IntField("Min Price:", entry.minPrice);
                        entry.maxPrice = EditorGUILayout.IntField("Max Price:", entry.maxPrice);
                        entry.weight = EditorGUILayout.FloatField("Weight:", entry.weight);
                        if (GUILayout.Button("Remove"))
                        {
                            toBeRemoved.Add(count);
                        }
                        //EditorGUILayout.EndHorizontal ();
                    }
                    count++;
                    EditorGUILayout.EndVertical();

                }
                foreach (int i in toBeRemoved)
                    shop.entries.RemoveAt(i);
            }

        }
        else
        {
            poi.Dialogue = (DialogueAsset)EditorGUILayout.ObjectField("Dialogue:", poi.Dialogue, typeof(DialogueAsset));
            questListToggle = EditorGUILayout.Foldout(questListToggle, "Quests");
            if (questListToggle)
            {
                for (int i = 0; i < poi.PossibleQuests.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    poi.PossibleQuests[i] = (BaseQuest)EditorGUILayout.ObjectField(poi.PossibleQuests[i], typeof(BaseQuest));
                    if (GUILayout.Button("X", GUILayout.Width(40)))
                    {
                        poi.PossibleQuests.RemoveAt(i);
                        i--;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("Add Quest", GUILayout.Width(80)))
                {
                    poi.PossibleQuests.Add(null);
                }
            }
        }
        poi.prefab = (MapPOIComponent)EditorGUILayout.ObjectField(poi.prefab, typeof(MapPOIComponent));

        int _ind = MapTags.IndexOf(poi.Type);
        if (_ind == -1)
            _ind = 0;

        poi.Type = MapTags.GetTypeList()[EditorGUILayout.Popup("POI Type:",MapTags.IndexOf(poi.Type), MapTags.GetTypeList().ToArray())];

        for(int i=0; i < poi.Categories.Count; i++)
        {
            List<string> catList = MapTags.GetCategoryList(poi.Type);
            int cat_ind = catList.IndexOf(poi.Categories[i]);
            if (cat_ind < 0)
                cat_ind = 0;
            GUILayout.BeginHorizontal();
            poi.Categories[i] = catList[EditorGUILayout.Popup(cat_ind, catList.ToArray())];
            if (GUILayout.Button("X", GUILayout.Width(30)))
            {
                poi.Categories.RemoveAt(i);
                i--;
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add Category", GUILayout.Width(130)))
        {
            poi.Categories.Add(MapTags.GetCategoryList(poi.Type)[0]);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        /*
        for (int i = 0; i < poi.spawnTags.Count; i++) {
            if (tagByText.Count-1 < i)
            {
                tagByText.Add(false);
            }
			EditorGUILayout.BeginHorizontal ();
            //GUILayout.FlexibleSpace();
            if (!tagByText[i])
            {
                int ind = MapTags.PossibleTags.IndexOf(poi.spawnTags[i]);
                if (ind < 0)
                {
                    ind = 0;
                    Debug.LogError("[EditorError]: Tag " + poi.spawnTags[i] + " non trovato. Sostituisco con default.");
                    poi.spawnTags[i] = MapTags.PossibleTags[0];
                }
                poi.spawnTags[i] = MapTags.PossibleTags[EditorGUILayout.Popup(ind, MapTags.PossibleTags.ToArray())];
            }
            else
                poi.spawnTags[i] = EditorGUILayout.TextField(poi.spawnTags[i]);
            if (!tagByText[i] && GUILayout.Button("Manual"))
            {
                tagByText[i] = true;
            } else if (tagByText[i] && GUILayout.Button("List"))
            {
                tagByText[i] = false;
            }
            
			if (GUILayout.Button ("X", GUILayout.ExpandWidth(false))) {
                poi.spawnTags.RemoveAt(i);
				i--;
			}
            //GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal ();
		}
		if (GUILayout.Button ("Add Tag")) {
            poi.spawnTags.Add (MapTags.PossibleTags[0]);
		}

    */

        EditorGUILayout.EndFadeGroup ();
	}
}
