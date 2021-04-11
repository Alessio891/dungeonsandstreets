using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipData
{
    public string weaponId;
    public string itemUID;
}

public class PlayerEquip : MonoBehaviour, IServerSerializable {
	public Dictionary<string, EquipData> equip;

	public static PlayerEquip instance;

	void Awake() { instance = this; }
	// Use this for initialization
	void Start () {
		PlayerServerSync.instance.OnEquipUpdate += Deserialize;
	}

	// Update is called once per frame
	void Update () {

	}

    public EquipData GetEquipByItemId(string id)
    {
        foreach (KeyValuePair<string, EquipData> pair in equip)
        {
            if (pair.Value.weaponId == id)
                return pair.Value;
        }
        return null;
    }

	#region IServerSerializable implementation
    
	public Dictionary<string, object> Serialize ()
	{
		Dictionary<string, object> retVal = new Dictionary<string, object> ();

		return retVal;
	}

	public void Deserialize (Dictionary<string, object> serialized)
	{
		//Debug.Log ("SYNC EQUIP!");
		equip = new Dictionary<string, EquipData> ();
		foreach(KeyValuePair<string, object> pair in serialized)
		{
            EquipData d = new EquipData();
            Dictionary<string, object> data = (Dictionary<string, object>)pair.Value;
            string itemId = data["UID"].ToString();
            string weaponId = data["weaponId"].ToString();
            d.itemUID = itemId;
            d.weaponId = weaponId;			
			equip.Add (pair.Key, d);
		}
	}

	#endregion
}
