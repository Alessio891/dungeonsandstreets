using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintItem : BaseItem {
    public string StationType;

  

    public override Dictionary<string, object> Serialize()
    {
        Dictionary<string, object> retVal = base.Serialize();
        retVal.Add("StationType", StationType);

        return retVal;
    }

    public override void Deserialize(Dictionary<string, object> serialized)
    {
        base.Deserialize(serialized);
        StationType = serialized["StationType"].ToString();
    }
}
