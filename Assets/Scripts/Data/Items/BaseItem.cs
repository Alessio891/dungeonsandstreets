using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class BaseItem : ScriptableObject, IServerSerializable{

    public enum Quality
    {
        Trash = 0,
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

	public string UID;
	public string Name; 
	public string Description;
	public Sprite sprite;
	public string category;
	//public float Rarity;
    public Quality ItemQuality;

	public int Value;

    public bool CanStack = false;

    public bool UsableInCombat = true;

    public string logic;

    public virtual void Consume()
    {
        
        Dictionary<string, object> values = new Dictionary<string, object>();
        values.Add("userName", PlayerPrefs.GetString("user"));
        values.Add("itemId", this.UID);
        Bridge.POST(Bridge.url + "ConsumeItem", values, (r) => {
            Debug.Log("[CONSUME ITEM] " + r);
            //UIInventory.instance.Refresh();
            ServerResponse resp = new ServerResponse(r);
            if (resp.status == ServerResponse.ResultType.Success)
            {
                UIInventory.instance.refresh = true;
                UIInventory.instance.OnInventoryUpdate(resp.GetIncomingList());
            }
        });
    }

    public virtual void OnUse ()
	{

        if (CombatManager.instance != null && !UsableInCombat)
        {
            PopupManager.ShowPopup("Error", "You can't use this item in combat.", (s, p) => p.Close());
            return;
        }

        Dictionary<string, object> values = new Dictionary<string, object>();
        values.Add("user", PlayerPrefs.GetString("user"));
        values.Add("itemUid", UIInventory.instance.currentEntry.itemData["UID"]);

        Bridge.POST(Bridge.url + "Items/UseItem", values, (r) => {
            ServerResponse response = new ServerResponse(r);
            Debug.Log("[UsedItem] " + r);
            if (response.status != ServerResponse.ResultType.Success)
            {
                PopupManager.ShowPopup("Connection error", "Something went wrong, the item couldn't be used. Try again.", (s, p) => p.Close());
            }
            else
            {
                if (response.message == "Item used")
                {
                    if (CombatManager.instance != null)
                    {
                        Dictionary<string, object> v = new Dictionary<string, object>();
                        v.Add("combatId", CombatManager.instance.combatId);
                        v.Add("user", PlayerPrefs.GetString("user"));
                        v.Add("itemId", this.UID);
                        Bridge.POST(Bridge.url + "Combat/UsedItem", v, (re) =>
                        {
                            Debug.Log("[USED ITEM RESPONSE] " + re);
                            ServerResponse resp = new ServerResponse(re);
                            if (resp.status == ServerResponse.ResultType.Success)
                            {

                                CombatManager.instance.combatData.Deserialize(resp.GetIncomingDictionary());
                                CombatManager.instance.CanAct = false;
                                UIInventory.instance.Hide();

                            }
                        });
                    }
                    if (UIInventory.instance.group.alpha > 0)
                        UIInventory.instance.Refresh();
                }
                else
                {
                    PopupManager.ShowPopup("Error", "You can't perform this action at this moment.", (s, p) => p.Close());
                }
            }
        });


	}


	public virtual Dictionary<string, object> Serialize ()
	{
		Dictionary<string, object> retVal = new Dictionary<string, object> ();
		retVal.Add ("UID", UID);
		retVal.Add ("Name", Name);
		retVal.Add ("Category", category);
		//retVal.Add ("Rarity", Rarity.ToString(CultureInfo.InvariantCulture));
		retVal.Add ("Value", Value);
        retVal.Add("CanStack", CanStack);
        retVal.Add("logic", logic);
        retVal.Add("UsableInCombat", UsableInCombat);
        retVal.Add("Quality", (int)ItemQuality);
		return retVal;
	}
	public virtual void Deserialize (Dictionary<string, object> serialized)
	{
		UID = serialized ["UID"].ToString ();
		Name = serialized ["Name"].ToString ();
		category = serialized ["Category"].ToString ();
       // Rarity = float.Parse(serialized["Rarity"].ToString(), CultureInfo.InvariantCulture);		
		int.TryParse (serialized ["Value"].ToString (), out Value);
        CanStack = (bool)serialized["CanStack"];
        logic = serialized["logic"].ToString();
        UsableInCombat = (bool)serialized["UsableInCombat"];
        ItemQuality = (Quality)int.Parse(serialized["Quality"].ToString());
	}

    public virtual ItemData GetItemData()
    {
        ItemData d = new ItemData();
        return d;
    }

	[ContextMenu("Generate UID")]
	public void GenerateUID()
	{
		UID = System.Guid.NewGuid ().ToString ();
	}

	[ContextMenu("Upload Item")]
	public void Upload()
	{
		Bridge.UpdateItemOnServer (this, (r) => {
			Debug.Log(r);
			ServerResponse resp = new ServerResponse(r);
			if (resp.status == ServerResponse.ResultType.Error)
			{
				Debug.Log("ERROR FROM SERVER: " + resp.message);
			} else
			{
				Debug.Log("SUCCESS: " + resp.message);
			}
		});
	}

	[ContextMenu("Download Item")]
	public void Download()
	{
		Bridge.DownloadItemFromServer (this, (r) => {
			Debug.Log(r);
			ServerResponse resp = new ServerResponse(r);
			if (resp.status == ServerResponse.ResultType.Error)
			{
				Debug.Log("Error from server: " + resp.message);
			} else
			{
				Debug.Log("Success.");
				this.Deserialize(resp.GetIncomingDictionary());
			}
		});
	}

	#if UNITY_EDITOR
	[ContextMenu("Add to Registry")]
	public void addToRegistry()
	{
		ItemsTableEntry e = new ItemsTableEntry();
		e.ID = UID;
		e.path = UnityEditor.AssetDatabase.GetAssetPath (this).Replace ("Assets/Resources/", "").Replace(".asset", "");
		Registry.assets.items.items.Add (e);
	}
	#endif
}
