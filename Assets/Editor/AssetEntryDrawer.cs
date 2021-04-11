using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemsTable))]
public class AssetEntryDrawer : Editor {
	Vector2 scrollPos;
	public override void OnInspectorGUI ()
	{
		ItemsTable table = (ItemsTable)target;


		if (GUILayout.Button ("New")) {
			table.items.Add (null);
			EditorUtility.SetDirty (table);
		}

		scrollPos = EditorGUILayout.BeginScrollView (scrollPos);

		for (int i = table.items.Count-1; i >= 0; i--) {
			EditorGUILayout.LabelField ("-----------------");
			if (table.items [i] == null) {
				table.items [i] = new ItemsTableEntry ();
			}
//			Debug.Log ("Trying to load " + table.items [i].path);
			BaseItem loaded = Resources.Load<BaseItem> (table.items [i].path);
			BaseItem lastLoaded = loaded;

			if (loaded != null) {
				EditorGUILayout.LabelField (loaded.UID);
			}
			EditorGUILayout.BeginHorizontal ();

             
			loaded = (BaseItem)EditorGUILayout.ObjectField (loaded, typeof(BaseItem));

			if (loaded != lastLoaded) {
				table.items [i].path = AssetDatabase.GetAssetPath (loaded).Replace ("Assets/Resources/", "").Replace(".asset", "");
				table.items [i].ID = loaded.UID;
				for (int j = 0; j < table.items.Count; j++) {
					//if (e.ID == loaded.UID && 
					ItemsTableEntry e = table.items [j];
					if (e.ID == loaded.UID && j != i) {
						Debug.LogError("[Table] Duplicated id at " + i + " and " + j );
					}
				}
				//Debug.Log ("Switching to " + table.items [i].path);
				EditorUtility.SetDirty (table);
			}				

			if (GUILayout.Button ("X", GUILayout.Width (40))) {
				table.items.RemoveAt (i);
				//i++;
				EditorUtility.SetDirty (table);
			}

			EditorGUILayout.EndHorizontal ();
		//	EditorGUILayout.LabelField ("-----------------");
		}


		EditorGUILayout.EndScrollView ();
	}
}
