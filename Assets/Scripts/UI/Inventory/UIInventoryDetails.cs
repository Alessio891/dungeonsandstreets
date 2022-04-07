using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryDetails : MonoBehaviour {

	public CanvasGroup group;
	BaseItem item;

	public Text nameText;
	public Text desc;

    public Text damageText;
    public Text hitText;
    public Text critText;
    public Text dmgTypeText;
    public Text elementText;
    public Text refinedText;

    public Image damageArrowUp, damageArrowDown;
    public Image hitArrowUp, hitArrowDown;
    public Image critArrowUp, critArrowDown;       

    public UIItemEntry currentSelected;

    public static string clickedItemUID;

    public string currentItemUID;

	// Use this for initialization
	void Awake () {
		group = GetComponent<CanvasGroup> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void Show()
    {
        BaseItem item = UIInventory.instance.currentEntry.item;
        Dictionary<string, object> data = UIInventory.instance.currentEntry.itemData;
        Show(item, data);
    }

	public void Show(BaseItem item, Dictionary<string, object> data)
	{        
		group.alpha = 1;
		group.interactable = true;
		group.blocksRaycasts = true;
		nameText.text = item.Name;
		desc.text = item.Description;
		this.item = item;
        if (item is BaseWeapon)
        {
            UIInventory.instance.weaponDetails.alpha = 1;              
            int damage = 0;
            int hit = 0;
            int crit = 0;
            Debug.Log("Item " + item.Name + " data " + data.ToString());
            int.TryParse(data["Damage"].ToString(), out damage);
            int.TryParse(data["HitChance"].ToString(), out hit);
            int.TryParse(data["CritChance"].ToString(), out crit);            

            damageText.text = damage.ToString();
            hitText.text = hit.ToString();
            critText.text = crit.ToString();
            dmgTypeText.text = "Normal";
            string elem = data["DamageType"].ToString();            
            elementText.text = char.ToUpper(elem[0]) + elem.Substring(1);
           // refinedText.text = "x"+data["currentRefine"].ToString();

            currentItemUID = data["UID"].ToString();

            EquipData equipData = PlayerEquip.instance.equip["rHand"];
            int currentDmg = 1;
            double currentHit = 85;
            double currentCrit = 10;
            if (!string.IsNullOrEmpty(equipData.weaponId))
            {
                ItemData d = PlayerInventory.instance.GetItem(equipData.itemUID);

                currentDmg = d.Damage;
                currentHit = d.HitChance;
                currentCrit = d.CritChance;
            }

            damageArrowDown.enabled = false;
            damageArrowUp.enabled = false;

            if (damage > currentDmg)
                damageArrowUp.enabled = true;
            else if (damage < currentDmg)
                damageArrowDown.enabled = true;

            hitArrowDown.enabled = false;
            hitArrowUp.enabled = false;
            if (hit > currentHit)
                hitArrowUp.enabled = true;
            else if (hit < currentHit)
                hitArrowDown.enabled = true;

            critArrowDown.enabled = false;
            critArrowUp.enabled = false;
            if (crit > currentCrit)
                critArrowUp.enabled = true;
            else if (crit < currentCrit)
                critArrowDown.enabled = true;

        }
        else
        {           
            UIInventory.instance.weaponDetails.alpha = 0;            
        }

    }

	public void Hide()
	{
		group.alpha = 0;
		group.interactable = false;
		group.blocksRaycasts = false;
        if (UIInventory.instance.currentEntry != null)
        {
            UIInventory.instance.currentEntry.Unselect();
            UIInventory.instance.currentEntry = null;
        }
	}

	public void Use() {
        clickedItemUID = currentItemUID;
		item.OnUse ();
		//Hide ();
	}
	public void Drop()
	{
		Hide();
	}


}
