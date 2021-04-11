using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopEntry
{
	public string Item;
	public int minPrice;
	public int maxPrice;
	public int minAmount;
	public int maxAmount;
	public float weight;


	public void Deserialize(Dictionary<string, object> data)
	{
		Item = data ["Item"].ToString ();
		int.TryParse (data ["minPrice"].ToString (), out minPrice);
		int.TryParse (data ["maxPrice"].ToString (), out maxPrice);
		int.TryParse (data ["minAmount"].ToString (), out minAmount);
		int.TryParse (data ["maxAmount"].ToString (), out maxAmount);
		float.TryParse (data ["weight"].ToString (), out weight);
	}
	public Dictionary<string, object> Serialize()
	{
		Dictionary<string, object> retVal = new Dictionary<string, object> ();

		retVal.Add ("Item", Item);
		retVal.Add ("minPrice", minPrice);
		retVal.Add ("maxPrice", maxPrice);
		retVal.Add ("minAmount", minAmount);
		retVal.Add ("maxAmount", maxAmount);
		retVal.Add ("weight", weight);
		return retVal;
	}

}

public class BaseShop : MapPOI {
	
	public List<ShopEntry> entries;
	public int maxItems;

	public override Dictionary<string, object> Serialize ()
	{
        Dictionary<string, object> retVal = base.Serialize();
		
		List<Dictionary<string, object>> serializedEntries = new List<Dictionary<string, object>> ();
		foreach (ShopEntry e in entries) {
			serializedEntries.Add (e.Serialize());
		}
		retVal.Add ("possibleItems", serializedEntries);
		retVal.Add ("maxItems", maxItems);
		return retVal;
	}

	public override void Deserialize (Dictionary<string, object> serialized)
	{
        base.Deserialize(serialized);
		List<object> data = (List<object>)serialized ["possibleItems"];
		entries = new List<ShopEntry> ();
		foreach (object o in data) {
			Dictionary<string, object> item = (Dictionary<string, object>)o;
			ShopEntry e = new ShopEntry ();
			e.Deserialize (item);
			entries.Add (e);
		}		
		int.TryParse (serialized ["maxItems"].ToString(), out maxItems);
	}

	[ContextMenu("Upload Shop")]
	public override void Upload()
	{
		Dictionary<string, object> data = new Dictionary<string, object> ();
		data.Add ("UID", UID);
		data.Add ("data", MiniJSON.Json.Serialize(Serialize ()));

		Bridge.POST (Bridge.url + "Shop/UploadShop", data, (r) => {
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

	[ContextMenu("Generate UID")]
	public void GenerateUID()
	{
		UID = System.Guid.NewGuid ().ToString ();
	}


	[ContextMenu("Download Shop")]
	public void Download()
	{
		Bridge.GET (Bridge.url + "Shop/DownloadShop?uid=" + UID, (r) => {
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
}
