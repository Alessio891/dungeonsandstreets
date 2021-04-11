using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class RecipeTableEntry
{
    public string UID;
    public BaseRecipe recipe;
}


public class RecipesTable : ScriptableObject
{
    public List<RecipeTableEntry> entries;
    public BaseRecipe this[string key] { get { return entries.Where<RecipeTableEntry>(e => e.UID == key).FirstOrDefault<RecipeTableEntry>().recipe; } }
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
        foreach (RecipeTableEntry e in entries)
        {
            if (e.UID == id)
                return true;
        }
        return false;
    }
}
