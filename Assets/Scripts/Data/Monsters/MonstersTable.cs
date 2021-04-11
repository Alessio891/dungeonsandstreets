using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterTableEntry
{
    public string key;
    public BaseMonster monster;
}

public class MonstersTable : ScriptableObject {
    public List<MonsterTableEntry> monsters = new List<MonsterTableEntry>();

    public BaseMonster this[string key] { get
        {
            foreach (MonsterTableEntry e in monsters)
            {
                if (e.key == key)
                    return e.monster;
            }
            return null;
        }
    }
}
