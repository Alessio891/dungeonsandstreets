using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseArmor : BaseEquippable {
	
	public int Defense;

	public override void Deserialize (Dictionary<string, object> serialized)
	{
		base.Deserialize (serialized);
		
		int.TryParse (serialized ["Defense"].ToString (), out Defense);
	}

	public override Dictionary<string, object> Serialize ()
	{
		Dictionary<string, object> retVal = base.Serialize ();
		
		retVal.Add ("Defense", Defense.ToString ());
		return retVal;
	}
}
