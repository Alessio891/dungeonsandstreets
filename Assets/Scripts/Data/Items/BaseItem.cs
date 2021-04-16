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

	public string ItemID;
	public string Name; 
	public string Description;
	public Sprite sprite;
	public string category;
	//public float Rarity;
    public Quality ItemQuality;
   

	public int Value;

    public bool CanStack = false;

    

    public virtual void OnUse ()
	{
	}


	public virtual Dictionary<string, object> Serialize ()
	{
		Dictionary<string, object> retVal = new Dictionary<string, object> ();
		retVal.Add ("ItemID", ItemID);
		retVal.Add ("Name", Name);
		retVal.Add ("Category", category);
		//retVal.Add ("Rarity", Rarity.ToString(CultureInfo.InvariantCulture));
		retVal.Add ("Value", Value);
        retVal.Add("CanStack", CanStack);
        retVal.Add("Quality", ItemQuality.ToString());
		return retVal;
	}
	public virtual void Deserialize (Dictionary<string, object> serialized)
	{
		ItemID = serialized ["ItemID"].ToString ();
		Name = serialized ["Name"].ToString ();
		category = serialized ["Category"].ToString ();
       // Rarity = float.Parse(serialized["Rarity"].ToString(), CultureInfo.InvariantCulture);		
		int.TryParse (serialized ["Value"].ToString (), out Value);
        CanStack = (bool)serialized["CanStack"];        
        ItemQuality = (Quality)System.Enum.Parse(typeof(Quality), serialized["Quality"].ToString(), true);    
	}

    public virtual ItemData GetItemData()
    {
        ItemData d = new ItemData();
        return d;
    }

	[ContextMenu("Generate UID")]
	public void GenerateUID()
	{
		ItemID = System.Guid.NewGuid ().ToString ();
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
		e.ID = ItemID;
		e.path = UnityEditor.AssetDatabase.GetAssetPath (this).Replace ("Assets/Resources/", "").Replace(".asset", "");
		Registry.assets.items.items.Add (e);
	}
	#endif
}
