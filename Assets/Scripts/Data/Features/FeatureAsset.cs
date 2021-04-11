using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[System.Serializable]
public class FeatureDropData
{
	public BaseItem item;
	public float weight;

	public Dictionary<string, object> Serialize()
	{
		Dictionary<string, object> retVal = new Dictionary<string, object> ();

		retVal.Add (item.UID, weight);

		return retVal;
	}
}

[System.Serializable]
public class FeatureAsset : ScriptableObject, IServerSerializable {
	public string UID;
    public string FeatureName;
	public string FeatureType;
	public string extraData;
    public bool isCraftingStation = false;
    public bool isEncounter = false;
    public bool isNode = false;

	[Tooltip("0 = Common, Quality Range = 0 - 0.2\n\t// 1 = Uncommon,  Quality Range = 0 - 0.3\n\t// 2 = Rare, Quality Range = 0.3-0.5\n\t// 3 = Epic, Quality Range = 0.4 - 0.7\n\t// 4 = Legendary, Quality Range = 0.7 - 1.0")]
	public float Rarity;

	public int minGoldDrop;
	public int maxGoldDrop;
	public int minItemDrops;
	public int maxItemDrops;

    public BasicFeature OnMapFeature;

	public List<FeatureDropData> possibleItems;

    public Dictionary<string, double> BiomesData = new Dictionary<string, double>()
    {
        { "open_world_grass", 0.0d },
        { "open_world_sea", 0.0d },
        { "forest", 0.0 },
        { "village", 0.0 },
        { "town", 0.0 }
    };


    public virtual Dictionary<string, object> Serialize ()
	{
		Dictionary<string, object> retVal = new Dictionary<string, object> ();
        retVal.Add("UID", UID);
		retVal.Add ("featureName", FeatureName);
		retVal.Add ("featureType", FeatureType);
		retVal.Add ("Rarity", Rarity);
		retVal.Add ("extraData", extraData);
		retVal.Add ("minGoldDrop", minGoldDrop);
		retVal.Add ("maxGoldDrop", maxGoldDrop);
		retVal.Add ("maxItemDrops", maxItemDrops);
		retVal.Add ("minItemDrops", minItemDrops);
        retVal.Add("isCraftingStation", isCraftingStation);
        retVal.Add("isEncounter", isEncounter);
        retVal.Add("isNode", isNode);
		Dictionary<string, object> list = new Dictionary<string, object> ();
		foreach (FeatureDropData d in possibleItems) {
			if (!list.ContainsKey(d.item.UID))
				list.Add (d.item.UID, d.weight);
		}
        Dictionary<string, object> biomeData = new Dictionary<string, object>();
        foreach (KeyValuePair<string, double> p in BiomesData)
        {
            biomeData.Add(p.Key, p.Value.ToString(CultureInfo.InvariantCulture));
        }
        retVal.Add("BiomesSpawnRate", biomeData);

        retVal.Add ("possibleItems", list);

		return retVal;
	}

	public void Deserialize (Dictionary<string, object> serialized)
	{
		FeatureName = serialized ["featureName"].ToString ();
		FeatureType = serialized ["featureType"].ToString ();
		float.TryParse (serialized ["Rarity"].ToString (), out Rarity);
		extraData = serialized ["extraData"].ToString ();
		int.TryParse (serialized ["minGoldDrop"].ToString (), out minGoldDrop);
		int.TryParse (serialized ["maxGoldDrop"].ToString (), out maxGoldDrop);
		int.TryParse (serialized ["minItemDrops"].ToString (), out minItemDrops);
		int.TryParse (serialized ["maxItemDrops"].ToString (), out maxItemDrops);
		Dictionary<string, object> poss = (Dictionary<string, object>)serialized ["possibleItems"];
		possibleItems = new List<FeatureDropData> ();
		foreach (KeyValuePair<string, object> o in poss) {
			FeatureDropData d = new FeatureDropData ();

			Debug.Log ("Loading item " + Registry.assets.items [o.Key] + " for id " + o.Key);
			d.item = Resources.Load<BaseItem> (Registry.assets.items [o.Key]);
			float.TryParse (o.Value.ToString (), out d.weight);
			possibleItems.Add (d);
		}
        isCraftingStation = (bool)serialized["isCraftingStation"];
        isEncounter = (bool)serialized["isEncounter"];
        isNode = (bool)serialized["isNode"];
        Dictionary<string, object> biomeData = (Dictionary<string, object>)serialized["BiomesSpawnRate"];
        BiomesData = new Dictionary<string, double>();
        foreach (KeyValuePair<string, object> p in biomeData)
        {
            BiomesData.Add(p.Key, double.Parse(p.Value.ToString(), CultureInfo.InvariantCulture));
        }

    }

	[ContextMenu("Upload Item")]
	public void Upload()
	{
		Bridge.UploadFeature (this, (r) => {
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
		Bridge.DownloadFeature (this, (r) => {
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
    [ContextMenu("Generate UID")]
    public void GenerateUID()
    {
        UID = System.Guid.NewGuid().ToString();
    }
}
