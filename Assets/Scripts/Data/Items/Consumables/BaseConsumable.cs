using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseConsumable : BaseItem {
	public string statsToModify;
	public int amountToModify;

	public override Dictionary<string, object> Serialize ()
	{
		Dictionary<string, object> baseVal = base.Serialize ();

		baseVal.Add ("statsToModify", statsToModify);
		baseVal.Add ("amountToModify", amountToModify);

		return baseVal;
	}

	public override void Deserialize (Dictionary<string, object> serialized)
	{
		base.Deserialize (serialized);
		statsToModify = serialized ["statsToModify"].ToString ();
		int.TryParse (serialized ["amountToModify"].ToString (), out amountToModify);
	}

    /*
	public override void OnUse ()
	{		
		if (GameManager.instance.PlayerIsDead) {
			PopupManager.ShowPopup ("You are dead!", "You are dead and can't perform this action. Use a Spirit Recombiner to resurrect yourself.", (s, p) => p.Close ());
			return;
		}        

		Dictionary<string, object> values = new Dictionary<string, object> ();
		values.Add ("userName", PlayerPrefs.GetString ("user"));
        if (statsToModify == "hp")
        {
            int amount = Mathf.Clamp(PlayerStats.instance.currentHP + amountToModify, 0, PlayerStats.instance.maxHP);
            Debug.Log("Restoring " + amount + " HP?");
            values.Add(statsToModify, amount);
        }
        else
            values.Add(statsToModify, amountToModify);
        values.Add("consumeItem", UID);
		Bridge.POST (Bridge.url + "UpdateStats", values, (r) => {
			Debug.Log("[CONSUMABLE USED] " + r);
            //PlayerServerSync.instance.SyncStats();
            if (CombatManager.instance == null)
                UIInventory.instance.Refresh();
		});
		

       

	}

    */
}
