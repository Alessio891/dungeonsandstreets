using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class POITableEntry
{
    public string UID;
    public MapPOI poi;
}

public class POITable : ScriptableObject
{
    public List<POITableEntry> entries;
    public MapPOI this[string key] { get {
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    return entries.Where<POITableEntry>(e => e.UID == key).FirstOrDefault<POITableEntry>().poi;
                }
                else return null;
            } catch
            {
                return null;
            }
        } }
}