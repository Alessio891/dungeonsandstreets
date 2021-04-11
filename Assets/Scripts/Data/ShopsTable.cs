using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[System.Serializable]
public class ShopTableEntry
{
	public string UID;
	public BaseShop shop;
}

public class ShopsTable : ScriptableObject {
	public List<ShopTableEntry> entries;
	public BaseShop this [string key] { get { return entries.Where<ShopTableEntry> (e => e.UID == key).FirstOrDefault<ShopTableEntry> ().shop; } }
}
