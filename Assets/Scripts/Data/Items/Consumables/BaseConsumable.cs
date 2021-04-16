using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseConsumable : BaseItem {
	public string statsToModify;
	public int amountToModify;

    public bool UsableInCombat = true;

    public string logic;

    public override Dictionary<string, object> Serialize ()
	{
		Dictionary<string, object> baseVal = base.Serialize ();

		baseVal.Add ("statsToModify", statsToModify);
		baseVal.Add ("amountToModify", amountToModify);
        baseVal.Add("logic", logic);
        baseVal.Add("UsableInCombat", UsableInCombat);
        return baseVal;
	}

	public override void Deserialize (Dictionary<string, object> serialized)
	{
		base.Deserialize (serialized);
		statsToModify = serialized ["statsToModify"].ToString ();
		int.TryParse (serialized ["amountToModify"].ToString (), out amountToModify);
        logic = serialized["logic"].ToString();
        UsableInCombat = (bool)serialized["UsableInCombat"];
    }

    
	public override void OnUse ()
	{
        if (CombatManager.instance != null && !UsableInCombat)
        {
            PopupManager.ShowPopup("Error", "You can't use this item in combat.", (s, p) => p.Close());
            return;
        }

        Dictionary<string, object> values = new Dictionary<string, object>();
        values.Add("user", PlayerPrefs.GetString("user"));
        values.Add("itemUid", UIInventory.instance.currentEntry.itemData["UID"]);

        Bridge.POST(Bridge.url + "Items/UseItem", values, (r) => {
            ServerResponse response = new ServerResponse(r);
            Debug.Log("[UsedItem] " + r);
            if (response.status != ServerResponse.ResultType.Success)
            {
                PopupManager.ShowPopup("Connection error", "Something went wrong, the item couldn't be used. Try again.", (s, p) => p.Close());
            }
            else
            {
                if (response.message == "Item used")
                {
                    if (CombatManager.instance != null)
                    {
                        Dictionary<string, object> v = new Dictionary<string, object>();
                        v.Add("combatId", CombatManager.instance.combatId);
                        v.Add("user", PlayerPrefs.GetString("user"));
                        v.Add("itemId", this.ItemID);
                        Bridge.POST(Bridge.url + "Combat/UsedItem", v, (re) =>
                        {
                            Debug.Log("[USED ITEM RESPONSE] " + re);
                            ServerResponse resp = new ServerResponse(re);
                            if (resp.status == ServerResponse.ResultType.Success)
                            {

                                CombatManager.instance.combatData.Deserialize(resp.GetIncomingDictionary());
                                CombatManager.instance.CanAct = false;
                                UIInventory.instance.Hide();

                            }
                        });
                    }
                    if (UIInventory.instance.group.alpha > 0)
                        UIInventory.instance.Refresh();
                }
                else
                {
                    PopupManager.ShowPopup("Error", "You can't perform this action at this moment.", (s, p) => p.Close());
                }
            }
        });




    }

    
}
