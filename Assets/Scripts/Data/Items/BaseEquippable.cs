using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ItemModDataValue
{
    public string key;
    public string value;
}

[System.Serializable]
public class ItemModData
{    
    public List<ItemModDataValue> data = new List<ItemModDataValue>();

    public ItemModData()
    {
        data.Add(new ItemModDataValue() { key = "mod", value = "stat_mod" });
    }

    public string this[string key]
    {
        get
        {
            foreach (ItemModDataValue v in data)
                if (v.key == key)
                    return v.value;
            return null;
        }
    }
}

public class BaseEquippable : BaseItem
{
    public bool IsUnique = false;
    public List<ItemModData> ItemMods = new List<ItemModData>();
    public string Slot = "head";

    public override void OnUse()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("user", PlayerPrefs.GetString("user"));
        data.Add("uid", UIInventory.instance.currentEntry.itemData["UID"]);

        Bridge.POST(Bridge.url + "Player/EquipItem", data, (r) => {
        ServerResponse response = new ServerResponse(r);
        Debug.Log("[UsedItem] " + r);
            if (response.status != ServerResponse.ResultType.Success)
            {
                PopupManager.ShowPopup("Connection error", "Something went wrong, the item couldn't be used. Try again.", (s, p) => p.Close());
            }
            else
            {
                Debug.Log("Equipped item");
            }
        });
    }

    public override Dictionary<string, object> Serialize()
    {
        Dictionary<string, object> retVal = base.Serialize();
        retVal.Add("IsUnique", IsUnique);
        

        List<Dictionary<string, object>> mods = new List<Dictionary<string, object>>();

        foreach(ItemModData m in ItemMods)
        {
            Dictionary<string, object> modData = new Dictionary<string, object>();
            foreach(ItemModDataValue value in m.data)
            {
                modData.Add(value.key, value.value);
            }
            mods.Add(modData);
        }
        retVal.Add("Mods", mods);
        retVal.Add("Slot", Slot);
        return retVal;
    }

    public override void Deserialize(Dictionary<string, object> serialized)
    {
        base.Deserialize(serialized);
        IsUnique = (bool)serialized["IsUnique"];
        Slot = serialized["Slot"].ToString();
        ItemMods = new List<ItemModData>();
        List<object> mods = (List<object>)serialized["Mods"];
        foreach(Dictionary<string, object> m in mods)
        {
            ItemModData d = new ItemModData();
            d.data = new List<ItemModDataValue>();
            foreach(KeyValuePair<string, object> pair in m)
            {
                ItemModDataValue v = new ItemModDataValue();
                v.key = pair.Key;
                v.value = pair.Value.ToString();
                d.data.Add(v);
            }
            ItemMods.Add(d);
        }
    }
}
