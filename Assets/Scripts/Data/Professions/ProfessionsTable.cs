using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ProfessionTableEntry
{
    public string UID;
    public BaseProfession profession;
}

public class ProfessionsTable : ScriptableObject
{
    public List<ProfessionTableEntry> entries;
    public BaseProfession this[string key] {
        get {
            ProfessionTableEntry entry = entries.Where<ProfessionTableEntry>(e => e.UID == key).FirstOrDefault<ProfessionTableEntry>();
            if (entry != null)
                return entry.profession;
            else
                return null;
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
        foreach (ProfessionTableEntry e in entries)
        {
            if (e.UID == id)
                return true;
        }
        return false;
    }
}
