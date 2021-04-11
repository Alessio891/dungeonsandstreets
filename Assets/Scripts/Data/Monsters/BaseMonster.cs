using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[System.Serializable]
public class ElementalData
{
    public string ElementName;
    public int amount;
}

public enum MonsterType
{
    Humanoid = 0,
    Beast,
    Undead,
    Mechanical,
    Horror,
    Elemental,
    Demon,
    Plant
}

public class BaseMonster : ScriptableObject, IServerSerializable {

	public string UID;
	public string ModelPath;

    public MonsterType Type;

	public string Name;
	public int MaxHp;

    public int Str;
    public int Int;
    public int Dex;

	public int BaseDamage;

	public int Level;

    public int XP;

    public List<ElementalData> elementalData = new List<ElementalData>();

    public List<FeatureDropData> Drops;

    public int minGold, maxGold;

    public Dictionary<string, double> BiomesData = new Dictionary<string, double>()
    {
        { "open_world_grass", 0.0d },
        { "open_world_sea", 0.0d },
        { "forest", 0.0 },
        { "village", 0.0 },
        { "town", 0.0 }
    };

	public Dictionary<string, object> Serialize ()
	{
		Dictionary<string, object> retVal = new Dictionary<string, object> ();			
		retVal.Add ("UID", UID);
		retVal.Add ("ModelPath", ModelPath);
		retVal.Add ("Name", Name);
        retVal.Add("Strength", Str);
        retVal.Add("Dexterity", Dex);
        retVal.Add("Intelligence", Int);
		retVal.Add ("MaxHp", MaxHp.ToString());
		retVal.Add ("BaseDamage", BaseDamage.ToString ());
		retVal.Add ("Level", Level.ToString ());
        retVal.Add("XP", XP.ToString());
        retVal.Add("minGold", minGold);
        retVal.Add("maxGold", maxGold);
        Dictionary<string, object> elemData = new Dictionary<string, object>();
        foreach (ElementalData d in elementalData)
        {           
            elemData.Add(d.ElementName, d.amount);            
        }
        retVal.Add("ElementDefense", elemData);
        retVal.Add("Type", Type.ToString());
        Dictionary<string, object> drops = new Dictionary<string, object>();
        foreach(FeatureDropData d in Drops)
        {
            drops.Add(d.item.UID, d.weight);
        }
        retVal.Add("Drops", drops);

        Dictionary<string, object> biomeData = new Dictionary<string, object>();
        foreach(KeyValuePair<string, double> p in BiomesData)
        {
            biomeData.Add(p.Key, p.Value.ToString(CultureInfo.InvariantCulture));
        }
        retVal.Add("BiomesSpawnRate", biomeData);

		return retVal;
	}

	public void Deserialize (Dictionary<string, object> serialized)
	{
		ModelPath = serialized ["ModelPath"].ToString ();
		Name = serialized ["Name"].ToString ();
		int.TryParse (serialized ["MaxHp"].ToString(), out MaxHp);
		int.TryParse (serialized ["BaseDamage"].ToString(), out BaseDamage);
		int.TryParse (serialized ["Level"].ToString (), out Level);
        int.TryParse(serialized["Strength"].ToString(), out Str);
        int.TryParse(serialized["Dexterity"].ToString(), out Dex);
        int.TryParse(serialized["Intelligence"].ToString(), out Int);
        int.TryParse(serialized["XP"].ToString(), out XP);
        minGold = int.Parse(serialized["minGold"].ToString());
        maxGold = int.Parse(serialized["maxGold"].ToString());
        Type = (MonsterType)System.Enum.Parse(typeof(MonsterType), serialized["Type"].ToString());
        Dictionary<string, object> elemData = (Dictionary<string, object>)serialized["ElementDefense"];
        elementalData = new List<ElementalData>();
        foreach (KeyValuePair<string, object> o in elemData)
        {
            ElementalData d = new ElementalData();
            d.ElementName = o.Key;
            int.TryParse(o.Value.ToString(), out d.amount);
            elementalData.Add(d);
        }
        Dictionary<string, object> drops = (Dictionary<string, object>)serialized["Drops"];
        Drops = new List<FeatureDropData>();
        foreach(KeyValuePair<string, object> o in drops)
        {
            FeatureDropData d = new FeatureDropData();
            d.item = Resources.Load<BaseItem>(Registry.assets.items[o.Key]);
            d.weight = float.Parse(o.Value.ToString());
            Drops.Add(d);
        }

        Dictionary<string, object> biomeData = (Dictionary<string, object>)serialized["BiomesSpawnRate"];
        BiomesData = new Dictionary<string, double>();
        foreach(KeyValuePair<string, object> p in biomeData)
        {
            BiomesData.Add(p.Key, double.Parse(p.Value.ToString(), CultureInfo.InvariantCulture));
        }
	}

	[ContextMenu("Generate UID")]
	public void GenerateUID()
	{
		UID = System.Guid.NewGuid ().ToString ();
	}

	[ContextMenu("Upload Item")]
	public void Upload()
	{
		Bridge.UploadMonster (this, (r) => {
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
		Bridge.DownloadMonster (this, (r) => {
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
