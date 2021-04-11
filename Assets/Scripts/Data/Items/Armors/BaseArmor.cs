using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseArmor : BaseItem {
	public string Slot;
	public int Defense;

	public override void Deserialize (Dictionary<string, object> serialized)
	{
		base.Deserialize (serialized);
		Slot = serialized ["Slot"].ToString ();
		int.TryParse (serialized ["Defense"].ToString (), out Defense);
	}

	public override Dictionary<string, object> Serialize ()
	{
		Dictionary<string, object> retVal = base.Serialize ();
		retVal.Add ("Slot", Slot);
		retVal.Add ("Defense", Defense.ToString ());
		return retVal;
	}
}
