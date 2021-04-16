using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEquipDetails : MonoBehaviour
{
    public Text ModTextPrefab;
    public Image SocketPrefab;


    public Text ItemName, ItemValue, ItemEffect;
    public VerticalLayoutGroup ModsList;
    public HorizontalLayoutGroup SocketsList;

    List<Text> CurrentModTexts = new List<Text>();
    List<Image> CurrentSockets = new List<Image>();

    BaseEquippable currentItem;

    public GameObject SwapButton, UnequipButton;

    public void Clear()
    {
        ItemName.text = ItemValue.text = ItemEffect.text = "";
        if (SwapButton != null)
            SwapButton.SetActive(false);
        if (UnequipButton != null)
            UnequipButton.SetActive(false);
        foreach (Text t in CurrentModTexts)
            Destroy(t.gameObject);
        foreach (Image i in CurrentSockets)
            Destroy(i.gameObject);
        CurrentModTexts.Clear();
        CurrentSockets.Clear();
        currentItem = null;
    }
    
    public void LoadItem(BaseEquippable item)
    {
        if (item == currentItem)
            return;
        
        Clear();
        currentItem = item;
        ItemName.text = item.Name;
        if (SwapButton != null)
            SwapButton.SetActive(true);
        if (UnequipButton != null)
            UnequipButton.SetActive(true);
        if (item is BaseWeapon)
            ItemValue.text = "Damage: " + (item as BaseWeapon).MinDamage.ToString() + "-" + (item as BaseWeapon).MaxDamage.ToString();
        else if (item is BaseArmor)
            ItemValue.text = "Armor: " + (item as BaseArmor).Defense.ToString();

        foreach(ItemModData mod in item.ItemMods)
        {
            if (mod["mod"] != "socket_mod")
            {
                string text = "";
                if (mod["mod"] == "stat_mod")
                {
                    text = "+" + mod["Value"] + " " + mod["Stat"];
                }
                else if (mod["mod"] == "elemental_mod")
                {
                    text = mod["Value"] + " damage as " + mod["Type"];
                }
                Text modText = GameObject.Instantiate<Text>(ModTextPrefab);
                modText.text = text;
                modText.transform.SetParent(ModsList.transform);
                modText.transform.localScale = Vector3.one;
                CurrentModTexts.Add(modText);
            } else
            {
                Image socketImg = GameObject.Instantiate<Image>(SocketPrefab);
                socketImg.transform.SetParent(SocketsList.transform);
                socketImg.transform.localScale = Vector3.one;
                CurrentSockets.Add(socketImg);
            }
        }
    }
}
