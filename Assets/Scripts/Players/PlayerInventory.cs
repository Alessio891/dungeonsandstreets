using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string instanceID;
    public string UID;
    public string Name;
    public int Damage;
    public int HitChance;
    public int CritChance;
    public int currentRefine;
    public int amount;
    public string category;
    public string data;
    public BaseItem itemInstance;
}

public class PlayerInventory : MonoBehaviour {

	public Dictionary<BaseItem, int> inventory;
    public List<ItemData> inventoryData;

	public static PlayerInventory instance;

	public event System.Action<BaseItem, int> OnItemAdded;
	public event System.Action<BaseItem, int> OnItemRemoved;

	void Awake() { instance = this; }
	// Use this for initialization
	void Start () {
        PlayerServerSync.instance.OnInventoryUpdate += Deserialize;
    }

    public int GetQuantity(string itemId)
    {
        foreach (ItemData d in inventoryData)
        {
            if (d.UID == itemId)
                return d.amount;
        }
        return 0;
    }

    /// <summary>
    /// Returns an item given the base item ID (not the instanced id)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ItemData GetItemByID(string id)
    {
        foreach (ItemData d in inventoryData)
        {
            if (d.UID == id)
                return d;
        }
        return null;
    }

    public ItemData GetItem(string UID)
    {
        foreach (ItemData d in inventoryData)
        {
            if (d.instanceID == UID)
            {
                return d;
            }
        }
        return null;
    }

    public bool HasItem(string itemId, int amount = 1)
    {
        foreach (ItemData d in inventoryData)
        {
            if (d.UID == itemId && d.amount >= amount)
                return true;
        }
        return false;
    }

	// Update is called once per frame
	void Update () {
		
	}

	#region IServerSerializable implementation

	public Dictionary<string, object> Serialize ()
	{
		Dictionary<string, object> retVal = new Dictionary<string, object> ();

		return retVal;
	}

    public void Deserialize(List<Dictionary<string, object>> data)
    {
        Debug.Log("Parsing items...");
        inventoryData = new List<ItemData>();
        foreach (Dictionary<string, object> _o in data)
        {
            Dictionary<string, object> o = (Dictionary<string, object>)_o["data"];

            BaseItem item = null;
            if (o["Category"].ToString().ToLower() == "weapon")
            {
                item = new BaseWeapon();
                (item as BaseWeapon).Deserialize(o);
            } else if (o["Category"].ToString().ToLower() == "armor")
            {
                item = new BaseArmor();
                (item as BaseArmor).Deserialize(o);
            } else if (o["Category"].ToString().ToLower() == "consumable")
            {
                item = new BaseConsumable();
                (item as BaseConsumable).Deserialize(o);
            }

            BaseItem templateItem = Resources.Load<BaseItem>(Registry.assets.items[o["ItemID"].ToString()]);
            item.sprite = templateItem.sprite;

            ItemData d = new ItemData();
            d.itemInstance = item;
            d.Name = o["Name"].ToString();
            d.UID = _o["itemUID"].ToString();
            d.instanceID = o["UID"].ToString();
            //Debug.Log("Instance id:" + d.instanceID);
            if (o["Category"].ToString() == "weapon")
            {
                int.TryParse(o["Damage"].ToString(), out d.Damage);
                int.TryParse(o["HitChance"].ToString(), out d.HitChance);
                int.TryParse(o["CritChance"].ToString(), out d.CritChance);                
            }
            int.TryParse(_o["amount"].ToString(), out d.amount);
            Debug.Log("Parsed item " + d.Name);
            inventoryData.Add(d);
        }

        Resources.UnloadUnusedAssets();
    }

	

	#endregion
}
