using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueTable : ScriptableObject
{
    public List<DialogueTableEntry> entries;
    public DialogueAsset this[string key] {
        get {
            try
            {
                return entries.Where<DialogueTableEntry>(e => e.UID == key).FirstOrDefault<DialogueTableEntry>().quest;
            } catch
            {
                return null;
            }
        } }
    public void Remove(string id)
    {
        for (int i = 0; i < entries.Count; i++)
        {
            if (entries[i].UID == id)
            {
                entries.RemoveAt(i);
                break;
            }
        }
    }
    public bool IsPresent(string id)
    {
        foreach (DialogueTableEntry e in entries)
        {
            if (e.UID == id)
                return true;
        }
        return false;
    }
}

[System.Serializable]
public class DialogueTableEntry
{
    public string UID;
    public DialogueAsset quest;
}
