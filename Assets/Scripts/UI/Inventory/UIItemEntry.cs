using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemEntry : MonoBehaviour {

	public BaseItem item;
    public Dictionary<string, object> itemData;
	public Image sprite;
	public Text amount;
	public Text equippedText;
    public Text NameText;
    public Image equippedImg;
    public Image selectedBG;
    public Image unselectedBG;

    public System.Action OnClick;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Click()
	{
        if (OnClick != null)
        {
            OnClick();
        }
       
		//UIInventory.instance.detailsGroup.Show (item, itemData);
	}

    public void Select() {
        SoundManager.instance.ItemSelectionSound();
        selectedBG.enabled = true;
        unselectedBG.enabled = false;
    }
    public void Unselect() {
        selectedBG.enabled = false;
        unselectedBG.enabled = true;
    }

	public void Init()
	{
		sprite.sprite = item.sprite;
        NameText.text = item.Name;
        amount.text = "";
        Unselect();		
	}
}
