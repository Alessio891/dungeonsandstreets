using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShopEntry : MonoBehaviour {
    public int shopIndex = 0;
	public string itemId;
    public string instanceId;
	public int price;
	public int amount;
    public Text ItemName;
	public Image itemSprite;
	public Text amountText;

	public BaseItem item;
    public Dictionary<string, object> data;

	public Image SelectedImage;

    public bool Selected = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Init()
	{
		item = Resources.Load<BaseItem> (Registry.assets.items [itemId]);
		itemSprite.sprite = item.sprite;
		amountText.text = amount.ToString ();
        ItemName.text = item.Name;
	}

	public void RefreshAmount(int am)
	{
		amount = am;
		amountText.text = am.ToString ();
	}

	public void Click()
	{
        Select();
		UIShop.instance.ShowItemDetails (this);
	}
    public void Select()
    {
        SoundManager.instance.ItemSelectionSound();
        SelectedImage.enabled = true;
        Selected = true;
    }
        
    
    public void Unselect()
	{
        Selected = false;
        SelectedImage.enabled = false;
	}
}
