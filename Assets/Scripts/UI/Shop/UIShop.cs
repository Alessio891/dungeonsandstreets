using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UIShop : MonoBehaviour {
    
    public enum State { Buy = 0, Sell };

    public State state = State.Buy;
    public UIShopEntry currentSelected;
    public int currentSelectedIndex;

	public Text ShopName;
    public Image ShopAvatar;
    public Text PlayerGold;
	public Image BuyButtonActive;
	public Image SellButtonActive;
    public Image BuyButtonInactive;
    public Image SellButtonInactive;

    public GameObject BuyGroup;
	public GameObject SellGroup;

    public GameObject DetailsBuyButton;
    public GameObject DetailsSellButton;

    public ScrollSnap ScrollSnap;

    public GridLayoutGroup BuyGrid;
	public GridLayoutGroup SellGrid;
    
    public UIShopItemDetails detailsPrefab;
    public CanvasGroup detailsCanvas;
    public Transform detailsContent;
    public List<UIShopItemDetails> currentDetails;

	public Text ItemName;
	public Text ItemDesc;
	public Text ItemPrice;

	public CanvasGroup canvasGroup;

	public UIShopEntry entryPrefab;

    public List<UIShopEntry> shopEntries = new List<UIShopEntry>();

	public Dictionary<string, object> shopData;

	public bool Buying = true;

	public static UIShop instance;

	float timer = 0;
    int oldDetailsPage = 1;
	void Awake() {
		instance = this;
	}

    private void OnEnable()
    {
    }
    private void OnDisable()
    {
    }

    // Use this for initialization
    void Start () {
		Hide ();
        HideDetails();
        ScrollSnap.onPageChange += ScrollSnap_onPageChange;        
	}

    private void ScrollSnap_onPageChange(int page)
    {
        if (canvasGroup.alpha > 0)
        {
            if (currentSelectedIndex >= 0 && currentSelectedIndex < shopEntries.Count)
            {
                if (shopEntries[currentSelectedIndex] != null)
                    shopEntries[currentSelectedIndex].Unselect();
            }
            currentSelectedIndex = page;
            shopEntries[currentSelectedIndex].Select();
        }
    }

    // Update is called once per frame
    void Update () {
		if (canvasGroup.alpha < 1)
			return;
		timer += Time.deltaTime;
        PlayerGold.text = PlayerStats.instance.gold.ToString();
		if (timer > 4) {
			if (BuyGroup.activeSelf) {
				Bridge.GET(Bridge.url + "Shop/GetSpawnedShop?user="+PlayerPrefs.GetString("user")+"&posX="+((double)shopData["posX"]).ToStringEx()+"&posY="+((double)shopData["posY"]).ToStringEx()+"&shopId="+shopData["UID"].ToString(), (r) => {
					Debug.Log("[UPDATE SHOP] " + r);
					ServerResponse resp = new ServerResponse(r);
					if (resp.status == ServerResponse.ResultType.Success)
					{
						shopData = resp.GetIncomingDictionary();
                       
						LoadShopInventory ();
					}
				});
			} else
				LoadPlayerInventory ();
			timer = 0;
		}
	}

	public void Show()
	{
		MainUIController.instance.Active = true;
        ScrollSnap.ChangePage(1);
		canvasGroup.alpha = 1;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
		LoadShopInventory ();
	}
	public void Hide()
	{
		canvasGroup.alpha = 0;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		MainUIController.instance.Active = false;
	}

    public void HideDetails()
    {
        detailsCanvas.alpha = 0;
        detailsCanvas.interactable = false;
        detailsCanvas.blocksRaycasts = false;
    }
    public void ShowDetails()
    {
        detailsCanvas.alpha = 1;
        detailsCanvas.interactable = true;
        detailsCanvas.blocksRaycasts = true;
    }

    // TO BE REMOVED
    public void DEBUG_REMOVE_THIS()
    {
        Debug.Log("CLICK!");
    }

	public void LoadShopInventory()
	{
        BuyButtonActive.enabled = true;
        BuyButtonInactive.enabled = false;
        SellButtonActive.enabled = false;
        SellButtonInactive.enabled = true;

        SellGroup.SetActive (false);
		BuyGroup.SetActive (true);
        DetailsBuyButton.SetActive(true);
        DetailsSellButton.SetActive(false);
        state = State.Buy;
		List<object> items = (List<object>)shopData ["items"];
        string selectedItemId = "";
        foreach (UIShopEntry e in shopEntries)
        {
            if (e != null)
            {
                if (e.Selected)
                {
                    selectedItemId = e.itemId;
                }
                Destroy(e.gameObject);
            }
        }
        shopEntries.Clear();
        if (currentDetails == null)
            currentDetails = new List<UIShopItemDetails>();
        // Magari facciamo un pool invece di distruggere ogni volta...
        foreach (UIItemDetails d in currentDetails)
            Destroy(d.gameObject);
        currentDetails.Clear();
        int index = 0;
		foreach (object o in items) {
			Dictionary<string, object> item = (Dictionary<string, object>)o;
			UIShopEntry e = GameObject.Instantiate<UIShopEntry> (entryPrefab);
            e.shopIndex = index;
            index++;
			e.itemId = item ["itemId"].ToString ();
            BaseItem itemAsset = (BaseItem)Resources.Load<BaseItem>(Registry.assets.items[e.itemId]);
			e.amount = 0;
			e.price = 0;
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("UID", itemAsset.ItemID);
            if (itemAsset is BaseWeapon)
            {
                BaseWeapon weapon = (BaseWeapon)itemAsset;
                data.Add("Damage", weapon.Damage);
                data.Add("HitChance", weapon.HitChance);
                data.Add("CritChance", weapon.CritChance);
                data.Add("DamageType", weapon.DamageType);                
            }
            e.data = data;
			int.TryParse (item ["price"].ToString (), out e.price);
			int.TryParse (item ["amount"].ToString (), out e.amount);
			e.Init ();            
			e.transform.SetParent (BuyGrid.transform);
			e.transform.localScale = Vector3.one;
			e.transform.localPosition = Vector3.zero;
            if (e.itemId == selectedItemId)
                e.Select();
            UIShopItemDetails d = GameObject.Instantiate<UIShopItemDetails>(detailsPrefab);
            d.AddItemInfo(itemAsset);
            d.transform.SetParent(detailsContent.transform);
            d.transform.localPosition = Vector3.zero;
            d.transform.localScale = Vector3.one;
            d.Gold.text = e.price.ToString();
            currentDetails.Add(d);
            shopEntries.Add(e);
            
		}
        ScrollSnap.UpdateListItemsSize();
        ScrollSnap.UpdateListItemPositions();
	}

	public void LoadPlayerInventory()
	{
        BuyButtonActive.enabled = false;
        BuyButtonInactive.enabled = true;
        SellButtonActive.enabled = true;
        SellButtonInactive.enabled = false;
        

        SellGroup.SetActive (true);
		BuyGroup.SetActive (false);
        DetailsBuyButton.SetActive(false);
        DetailsSellButton.SetActive(true);
        state = State.Sell;
        foreach (UIShopEntry e in shopEntries)
        {
            if (e != null)
            {             
                Destroy(e.gameObject);
            }
        }
        shopEntries.Clear();

        if (currentDetails == null)
            currentDetails = new List<UIShopItemDetails>();
        // Magari facciamo un pool invece di distruggere ogni volta...
        foreach (UIItemDetails d in currentDetails)
            Destroy(d.gameObject);
        currentDetails.Clear();
        int index = 0;
        foreach (ItemData item in PlayerInventory.instance.inventoryData) {
		
			UIShopEntry e = GameObject.Instantiate<UIShopEntry> (entryPrefab);
			e.itemId = item.UID;
            e.instanceId = item.instanceID;
            e.amount = item.amount;
            e.price = 100;
            e.shopIndex = index;
            index++;
            //e.price = item.;

            BaseItem itemAsset = (BaseItem)Resources.Load<BaseItem>(Registry.assets.items[e.itemId]);

            e.Init ();
			e.transform.SetParent (SellGrid.transform);
			e.transform.localScale = Vector3.one;
			e.transform.localPosition = Vector3.zero;

            UIShopItemDetails d = GameObject.Instantiate<UIShopItemDetails>(detailsPrefab);
            d.AddItemInfo(item);
            d.transform.SetParent(detailsContent.transform);
            d.transform.localPosition = Vector3.zero;
            d.transform.localScale = Vector3.one;
            d.Gold.text = itemAsset.Value.ToString();
            currentDetails.Add(d);
            shopEntries.Add(e);

        }
	}

	public void SellCurrent()
	{
		PopupManager.ShowPopup <AmountPopup> ("AmountPopup", "Amount", "Amount", (s, b) => {
			AmountPopup popup = (AmountPopup)b;
			int amountToBuy = 0;
			int.TryParse (popup.input.text, out amountToBuy);
			if (amountToBuy == 0)
				amountToBuy = 1;
            Debug.Log("Selling " + amountToBuy + " of " + shopEntries[currentSelectedIndex].item.Name);
			Dictionary<string, object> values = new Dictionary<string, object>();
			values.Add("user", PlayerPrefs.GetString("user"));
			//values.Add("shopId", shopData["UID"]);
			values.Add("itemId", shopEntries[currentSelectedIndex].itemId);
			values.Add("amount", amountToBuy);
			values.Add("posX", ((double)shopData["posX"]).ToStringEx());
			values.Add("posY", ((double)shopData["posY"]).ToStringEx());

			Bridge.POST(Bridge.url + "Shop/SellItem", values, (r) =>
				{
					Debug.Log("[SELL ITEM] " + r);
					ServerResponse resp = new ServerResponse(r);
					if (resp.status == ServerResponse.ResultType.Success)
					{
						Debug.Log("Sold!");
						PlayerServerSync.instance.SyncStats();
						PlayerServerSync.instance.OnInventoryUpdate += ResyncInventoryAfterSell;
						shopData = resp.GetIncomingDictionary();
						//LoadPlayerInventory();
					}
					b.Close ();	

				});

		}).Init (shopEntries[currentSelectedIndex].amount);
	}

	public void ResyncInventoryAfterSell(List<Dictionary<string, object>> data)
	{
		LoadPlayerInventory ();
		PlayerServerSync.instance.OnInventoryUpdate -= ResyncInventoryAfterSell;
	}

	public void ShowItemDetails(UIShopEntry entry)
	{
        ScrollSnap.SnapTo(entry.shopIndex);
        ShowDetails();
        currentSelectedIndex = entry.shopIndex;	
	}
	public void BuyCurrent()
	{
		PopupManager.ShowPopup <AmountPopup> ("AmountPopup", "Amount", "Amount", (s, b) => {
			AmountPopup popup = (AmountPopup)b;
			int amountToBuy = 0;
			int.TryParse (popup.input.text, out amountToBuy);
			if (amountToBuy == 0)
				amountToBuy = 1;
			Debug.Log ("Buying " + amountToBuy + " items");
			Dictionary<string, object> values = new Dictionary<string, object>();
			values.Add("user", PlayerPrefs.GetString("user"));
			values.Add("shopId", shopData["UID"]);
			values.Add("itemId", BuyGrid.transform.GetChild(currentSelectedIndex).GetComponent<UIShopEntry>().item.ItemID);
			values.Add("amount", amountToBuy);
			values.Add("posX", ((double)shopData["posX"]).ToStringEx());
			values.Add("posY", ((double)shopData["posY"]).ToStringEx());

			Bridge.POST(Bridge.url + "Shop/BuyItem", values, (r) =>
				{
					Debug.Log("[BUY ITEM] " + r);
					ServerResponse resp = new ServerResponse(r);
					if (resp.status == ServerResponse.ResultType.Success)
					{
						Debug.Log("Bought!");
						PlayerServerSync.instance.SyncStats();
						shopData = resp.GetIncomingDictionary();
						LoadShopInventory();
					}
					b.Close ();	
					
				});

		}).Init (BuyGrid.transform.GetChild(currentSelectedIndex).GetComponent<UIShopEntry>().amount);
	}
}
