using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ItemsTableEntry
{
	public string ID;
	public string path;
}

public class ItemsTable : ScriptableObject {
	public List<ItemsTableEntry> items;
	public string this [string id] { get {
			
			foreach (ItemsTableEntry e in items) {
				if (e != null && e.ID == id) {
					return e.path;
				}
			}
			return "DefaultItem";
		} }

	[ContextMenu("Clear")]
	public void Clear()
	{
		items.Clear ();
	}
	[ContextMenu("Upload All")]
	public void UploadAll()
	{
		foreach (ItemsTableEntry e in items) {
			BaseItem i = Resources.Load<BaseItem>(e.path);
			i.Upload ();
		}
	}

    public bool IsPresent(string uid)
    {
        return this[uid] != "DefaultItem";
    }

    public void Remove(string uid)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ID == uid)
            {
                items.RemoveAt(i);
                break;
            }
        }
    }

	[ContextMenu("Refresh ID")]
	public void RefreshId()
	{
		foreach (ItemsTableEntry e in items) {
			BaseItem i = Resources.Load<BaseItem>(e.path);
			e.ID = i.UID;
		}
	}

}
