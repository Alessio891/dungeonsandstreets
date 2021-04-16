using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour {   

	public static UIInventory instance;

	public CanvasGroup group;

    public UITabController tabController;

	public UIInventoryDetails detailsGroup;

	public UIItemEntry entry;

	public GridLayoutGroup grid;

    public CanvasGroup weaponDetails;

    public UIItemActionMenu ActionMenu;
    public UIItemEntry currentEntry;

    public Text UseText;
    public GameObject DismantleButton;

	public List<UIItemEntry> currentEntries;
    //public Dictionary<string, int> completeInventory = new Dictionary<string,int> ();
    public List<ItemData> completeInventory = new List<ItemData>();
	//public Text currentTabName;
	public string currentCategory = "weapon";
	public bool refresh = false;

	void Awake() { instance = this; }
	// Use this for initialization
	void Start () {
		group = GetComponent<CanvasGroup> ();
        tabController.OnToggleOn = ChangeTab;
		currentEntries = new List<UIItemEntry> ();

		//PlayerInventory.instance.OnItemAdded += ItemAdded;
		//PlsayerInventory.instance.OnItemRemoved += RemoveItem;

		Hide ();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
            Hide();
	}

	public void Clear()
	{
		foreach (UIItemEntry e in currentEntries) {
			Destroy (e.gameObject);
		}
		currentEntries.Clear ();
	}

	public void Hide()
	{
		group.alpha = 0;
		group.interactable = false;
		group.blocksRaycasts = false;
		if (detailsGroup != null) {
			detailsGroup.group.alpha = 0;
			detailsGroup.group.blocksRaycasts = false;
			detailsGroup.group.interactable = false;
		}
        if (MainUIController.instance != null)
		    MainUIController.instance.Active = false;
		Clear ();
		completeInventory.Clear ();
		currentEntries.Clear ();
        ActionMenu.Hide();
		Resources.UnloadUnusedAssets ();
	}

	
	public void RemoveItem(BaseItem item, int amount)
	{
		bool remove = false;
		int index = 0;
		foreach (UIItemEntry e in currentEntries) {			
			if (e.item.ItemID == item.ItemID) {
				int a = 0;
				int.TryParse (e.amount.text, out a);
				e.amount.text = (a - amount).ToString ();
				if (a - amount <= 0)
					remove = true;
				break;
			}
			index++;
		}
		if (remove) {
			Destroy (currentEntries [index].gameObject);
			currentEntries.RemoveAt (index);
		}
	}

	public void OnInventoryUpdate(List<Dictionary<string, object>> data)
	{
        completeInventory = new List<ItemData>();
			if (refresh) {
				Clear ();
				refresh = false;
			}
        foreach (Dictionary<string, object> item in data)
        {
            int amount = 0;
            int.TryParse(item["amount"].ToString(), out amount);
//
            string itemId = item["itemUID"].ToString();
  

            BaseItem i = Resources.Load<BaseItem>(Registry.assets.items[itemId]);
            ItemData d = new ItemData();
            d.amount = amount;
            d.UID = itemId;
            d.instanceID = ((Dictionary<string, object>)item["data"])["UID"].ToString();
            d.category = i.category;
            Dictionary<string, object> rawData = (Dictionary<string, object>)item["data"];
            d.data = MiniJSON.Json.Serialize(rawData);
            
            //Dictionary<string, object> itemData = (Dictionary<string, object>)item["data"];
            //int.TryParse(itemData["currentRefine"].ToString(), out d.currentRefine);
            completeInventory.Add(d);
         

            if (i.category != currentCategory)
                continue;

            if (i.CanStack)
            {
                UIItemEntry e = GameObject.Instantiate<UIItemEntry>(entry);
                e.transform.SetParent(grid.transform);
                e.transform.localScale = Vector3.one;
                e.item = i;
                e.itemData = (Dictionary<string, object>)item["data"];
                ItemData itemData = d;

                e.OnClick = () =>
                {

                    if (detailsGroup.currentSelected != null && detailsGroup.currentSelected != e)
                        detailsGroup.currentSelected.Unselect();
                    e.Select();
                    detailsGroup.currentSelected = e;
                    detailsGroup.currentItemUID = itemData.UID;
                    currentEntry = e;
                    ActionMenu.Show();
                };
                //Debug.Log("Data for " + i.Name + " is " + e.itemData);

                e.amount.text = amount.ToString();
                if (PlayerEquip.instance.equip != null)
                {
                    foreach (KeyValuePair<string, EquipData> pair in PlayerEquip.instance.equip)
                    {
                        if (pair.Value != null && pair.Value.itemUID == e.itemData["UID"].ToString())
                        {                            
                            e.equippedText.enabled = true;
                            break;
                        }
                    }
                }
                e.Init();
                currentEntries.Add(e);
            }
            else
            {
           
                bool equipped = false;
              //  Debug.Log("Unstackable amount " + amount);
                
                UIItemEntry e = GameObject.Instantiate<UIItemEntry>(entry);
                e.transform.SetParent(grid.transform);
                e.transform.localScale = Vector3.one;
                e.item = i;
                e.itemData = (Dictionary<string, object>)item["data"];
                e.amount.text = "1";                
                e.OnClick = () =>
                {

                    if (detailsGroup.currentSelected != null && detailsGroup.currentSelected != e)
                        detailsGroup.currentSelected.Unselect();
                    e.Select();
                    detailsGroup.currentSelected = e;
                    detailsGroup.currentItemUID = e.itemData["UID"].ToString();
                    currentEntry = e;
                    ActionMenu.Show();
                };
                //   Debug.Log("Data for " + i.Name + " is " + e.itemData);
                if (PlayerEquip.instance.equip != null)
                {
                    if (!equipped)
                    {
                        foreach (KeyValuePair<string, EquipData> pair in PlayerEquip.instance.equip)
                        {
                            if (pair.Value != null && pair.Value.itemUID == e.itemData["UID"].ToString())
                            {
                                equipped = true;
                                e.equippedText.enabled = true;
                                break;
                            }
                        }
                    }
                }
                e.Init();
                currentEntries.Add(e);
                
                
            }

        }
        /*
        if (!string.IsNullOrEmpty(detailsGroup.currentItemUID))
        {
            Debug.Log("Reselecting?");
            bool stillThere = false;
            foreach (UIItemEntry e in currentEntries)
            {
                if (e.itemData["UID"].ToString() == detailsGroup.currentItemUID)
                {
                    Debug.Log("Entry found!");
                    stillThere = true;
                    e.Click();
                    break;
                }
            }
            if (!stillThere)
            {
                Debug.Log("The item must be consumed?");
                detailsGroup.Hide();
            }
        }*/

		PlayerServerSync.instance.OnInventoryUpdate -= OnInventoryUpdate;
	}

	public void LoadItems()
	{
    	PlayerServerSync.instance.OnInventoryUpdate += OnInventoryUpdate;
        if (SceneManager.GetActiveScene().name == "Map")
            PlayerServerSync.instance.SyncStats();
        else
            PlayerServerSync.instance.SyncStats();
	}

	public void Show()
	{
        if (MainUIController.instance != null)
		    MainUIController.instance.Active = true;
		group.alpha = 1;
		group.interactable = true;
		group.blocksRaycasts = true;
		LoadItems ();
	}

	public void SpawnEntries()
	{
		foreach(ItemData s in completeInventory)
		{			
			
			BaseItem i = Resources.Load<BaseItem>(Registry.assets.items[s.UID]);
			if (i.category != currentCategory)
				continue;
			UIItemEntry e = GameObject.Instantiate<UIItemEntry> (entry);
			e.transform.SetParent (grid.transform);
			e.transform.localScale = Vector3.one;
			e.item = i;
			e.amount.text = s.amount.ToString ();
            e.itemData = (Dictionary<string, object>)MiniJSON.Json.Deserialize(s.data);
			e.Init ();

            ItemData itemData = s;

            e.OnClick = () =>
            {
                
                if (detailsGroup.currentSelected != null && detailsGroup.currentSelected != e)
                    detailsGroup.currentSelected.Unselect();
                e.Select();
                detailsGroup.currentSelected = e;
                detailsGroup.currentItemUID = itemData.UID;
                currentEntry = e;
                ActionMenu.Show();
            };
            
            foreach(KeyValuePair<string, EquipData> pair in PlayerEquip.instance.equip)
            {
                if (pair.Value.itemUID == s.instanceID)
                {
                    e.equippedText.enabled = true;
                    break;
                }
            }            
			currentEntries.Add (e);
		
		}
	}

	public void Refresh()
	{
		refresh = true;
		LoadItems ();
	}

	public void Resort()
	{
		Clear ();
		SpawnEntries ();
	}

	public void ChangeTab(string tab)
	{
		currentCategory = tab;
        ActionMenu.Hide(true);
		Resort ();
	}

    public void UseCurrentItem()
    {
        BaseItem item = currentEntry.item;
        if (item != null)
            item.OnUse();
        ActionMenu.Hide();
    }
    public void DestroyCurrentItem()
    {
        ActionMenu.Hide();
    }
    public void DismantleCurrentItem()
    {
        ActionMenu.Hide();
    }    
    
}
