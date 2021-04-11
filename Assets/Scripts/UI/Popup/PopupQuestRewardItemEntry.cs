using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupQuestRewardItemEntry : MonoBehaviour
{    

    public BaseItem item;
    public Image ItemIcon;
    public Text ItemName;
    public Text ItemAmount;
    public CanvasGroup canvas;

    public void Init(BaseItem i, int amount)
    {
        item = i;
        ItemIcon.sprite = i.sprite;
        ItemName.text = i.Name;
        ItemAmount.text = amount.ToString();
    }

}
