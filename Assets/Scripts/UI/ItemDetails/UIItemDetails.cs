using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UIItemDetails : MonoBehaviour
{

    public Text Name;
    public Image Icon;

    public Text Description;

    public UIItemDetailsEntry DetailPrefab;

    public GridLayoutGroup Grid;

    public List<UIItemDetailsEntry> Entries;

    public CanvasGroup group;


    private void Awake()
    {
        if (group == null)
            group = GetComponent<CanvasGroup>();   
    }

    public void AddEntry(string name, string value, bool compare = false, double compareTo = 0.0)
    {
        UIItemDetailsEntry e = GameObject.Instantiate<UIItemDetailsEntry>(DetailPrefab);
        e.InfoName.text = name;
        e.InfoValue.text = value;
        if (compare)
        {
            double v = double.Parse(value, CultureInfo.InvariantCulture);
            if (v < compareTo)
            {
                e.DownArrow.enabled = true;
                e.UpArrow.enabled = false;
            } else if (v > compareTo)
            {
                e.DownArrow.enabled = false;
                e.UpArrow.enabled = true;
            } else
            {
                e.DownArrow.enabled = false;
                e.UpArrow.enabled = false;
            }
        }
        else
        {
            e.DownArrow.enabled = false;
            e.UpArrow.enabled = false;
        }
        e.transform.SetParent(Grid.transform);
        e.transform.localPosition = Vector3.zero;
        e.transform.localScale = Vector3.one;
        Entries.Add(e);
    }

    public virtual void AddItemInfo(ItemData item)
    {
        BaseItem itemAsset = (BaseItem)Resources.Load<BaseItem>(Registry.assets.items[item.UID]);
        Name.text = item.Name;
        Description.text = itemAsset.Description;
        Icon.sprite = itemAsset.sprite;
        foreach (UIItemDetailsEntry entry in Entries)
        {
            Destroy(entry.gameObject);
        }
        Entries.Clear();

        if (itemAsset is BaseWeapon)
        {
            BaseWeapon weapon = (itemAsset as BaseWeapon);
           

            AddEntry("Damage", item.Damage.ToString());
            AddEntry("Hit Chance", item.HitChance.ToString());
            AddEntry("Crit Chance", item.CritChance.ToString());
            AddEntry("Damage Type", weapon.DamageType);
        }
        else if (itemAsset is BaseConsumable)
        {
            BaseConsumable consumable = (itemAsset as BaseConsumable);
            if (consumable.amountToModify != 0)
                AddEntry(consumable.statsToModify, consumable.amountToModify.ToString(), true, 0);

        }
    }

    public virtual void AddItemInfo(BaseItem item)
    {
        Name.text = item.Name;
        Description.text = item.Description;
        Icon.sprite = item.sprite;
        foreach(UIItemDetailsEntry entry in Entries)
        {
            Destroy(entry.gameObject);
        }
        Entries.Clear();

        if (item is BaseWeapon)
        {
            BaseWeapon weapon = (item as BaseWeapon);
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

            AddEntry("Damage", weapon.Damage.ToString(), true, currentDmg);
            AddEntry("Hit Chance", weapon.HitChance.ToString(), true, currentHit);
            AddEntry("Crit Chance", weapon.CritChance.ToString(), true, currentCrit);
            AddEntry("Damage Type", weapon.DamageType);            
        } else if (item is BaseConsumable)
        {
            BaseConsumable consumable = (item as BaseConsumable);
            if (consumable.amountToModify != 0)
                AddEntry(consumable.statsToModify, consumable.amountToModify.ToString(), true, 0);

        }
    }

    public void Show() {
        
        group.alpha = 1.0f;
        group.interactable = true;
        group.blocksRaycasts = true;
    }
    public void Hide() {
        group.alpha = 0;
        group.interactable = false;
        group.blocksRaycasts = false;
    }
}
