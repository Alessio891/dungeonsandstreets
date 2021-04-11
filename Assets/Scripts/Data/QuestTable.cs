using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class QuestTableEntry
{
    public string UID;
    public BaseQuest quest;
}

public class QuestTable : ScriptableObject
{
    public List<QuestTableEntry> entries;
    public BaseQuest this[string key] { get { return entries.Where<QuestTableEntry>(e => e.UID == key).FirstOrDefault<QuestTableEntry>().quest; } }
    public void Remove(string id)
    {
        for(int i = 0; i < entries.Count; i++)
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
        foreach(QuestTableEntry e in entries)
        {
            if (e.UID == id)
                return true;
        }
        return false;
    }
}