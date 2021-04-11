using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class RefineItemEntry
{
    public string key;
    public int amount;
}

[System.Serializable]
public class GrowthData
{
    public string stat;
    public  List<int> growth = new List<int>(10);
    public int Get(int index)
    {
        if (index > 9)
        {
            index = 9;
        }
        return growth[index];
    }
}

public class BaseWeapon : BaseItem {
    public int Damage;
    public double HitChance;
    public double CritChance;
    public string StatUsed = "Str";


	public int MinDamage;
	public int MaxDamage;

	public int DexMalus;

	public string DamageType;

    public AudioClip HitSound;
    public AudioClip MissSound;

    public ParticleSystem HitParticles;
         

    
    public List<RefineItemEntry> refineItemsNeeded = new List<RefineItemEntry>();
    public List<string> sockets = new List<string>();
    public List<GrowthData> refineGrowth = new List<GrowthData>();

	public override void Deserialize (Dictionary<string, object> serialized)
	{
		base.Deserialize (serialized);
        int.TryParse(serialized["Damage"].ToString(), out Damage);
        double.TryParse(serialized["HitChance"].ToString(), out HitChance);
        double.TryParse(serialized["CritChance"].ToString(), out CritChance);
        StatUsed = serialized["StatUsed"].ToString();
        // Obsolete
		int.TryParse (serialized ["MinDamage"].ToString (), out MinDamage);
		int.TryParse (serialized ["MaxDamage"].ToString (), out MaxDamage);
		int.TryParse (serialized ["DexMalus"].ToString (), out DexMalus);
        //

		DamageType = serialized ["DamageType"].ToString ();
        Dictionary<string, object> refNeeded = (Dictionary<string, object>)serialized["refineItemsNeeded"];
        refineItemsNeeded = new List<RefineItemEntry>();
        foreach (KeyValuePair<string, object> p in refNeeded)
        {
            RefineItemEntry e = new RefineItemEntry();
            e.key = p.Key;
            e.amount = 0;
            int.TryParse(p.Value.ToString(), out e.amount);
            refineItemsNeeded.Add(e);
        }
        List<object> serializedSockets = (List<object>)serialized["sockets"];
        sockets = new List<string>();
        foreach (object o in serializedSockets)
        {
            sockets.Add(o.ToString());
        }
        Dictionary<string, object> growthData = (Dictionary<string, object>)serialized["growthData"];
        refineGrowth = new List<GrowthData>();
        foreach (KeyValuePair<string, object> pair in growthData)
        {
            GrowthData d = new GrowthData();
            d.stat = pair.Key;
            List<object> growthValues = (List<object>)pair.Value;
            d.growth = new List<int>();
            foreach (object o in growthValues)
            {
                int i = 0;
                int.TryParse(o.ToString(), out i);
                d.growth.Add(i);
            }
            refineGrowth.Add(d);
        }

	}
	public override Dictionary<string, object> Serialize ()
	{
		Dictionary<string, object> retVal = base.Serialize ();
        retVal.Add("Damage", Damage);
        retVal.Add("HitChance", HitChance);
        retVal.Add("CritChance", CritChance);
		retVal.Add ("MinDamage", MinDamage.ToString ());
		retVal.Add ("MaxDamage", MaxDamage.ToString ());
		retVal.Add ("DexMalus", DexMalus.ToString ());
		retVal.Add ("DamageType", DamageType);
        retVal.Add ("StatUsed", StatUsed);
        Dictionary<string, object> refNeeded = new Dictionary<string, object>();
        foreach (RefineItemEntry e in refineItemsNeeded)
        {
            refNeeded.Add(e.key, e.amount);
        }
        retVal.Add("refineItemsNeeded", refNeeded);
        retVal.Add("sockets", sockets);
        retVal.Add("currentRefine", 0);

        Dictionary<string, object> growthData = new Dictionary<string, object>();
        foreach (GrowthData d in refineGrowth)
        {
            growthData.Add(d.stat, d.growth);
        }
        retVal.Add("growthData", growthData);

		return retVal;
	}
}
