using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseWeapon : BaseEquippable {
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
		return retVal;
	}
}
