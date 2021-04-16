using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEquip : MonoBehaviour {

	public List<UIEquipEntry> entries;
	public CanvasGroup canvasGroup;
	public static UIEquip instance;
    public UIEquipDetails Details;

    public Sprite SelectedBG, UnselectedBG;


	void Awake() {
		instance = this;
	}

	// Use this for initialization
	void Start () {
        Details.Clear();
		Hide ();	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Select(string key)
    {
        foreach(UIEquipEntry e in entries)
        {
            if (e.key == key)
            {
                e.Background.sprite = SelectedBG;
            } else
            {
                e.Background.sprite = UnselectedBG;
            }
        }
    }

	public UIEquipEntry Get(string key)
	{
		foreach (UIEquipEntry e in entries) {
			if (e.key == key)
				return e;
		}
		return null;
	}

	public void Equip(string key, BaseItem item)
	{
		Get (key).item = item;
		Get (key).Refresh ();
	}

	public void Show()
	{
		canvasGroup.alpha = 1;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;        
		Load ();
        foreach(UIEquipEntry e in entries)
        {
            e.Background.sprite = UnselectedBG;
        }
	}
	public void Hide()
	{
        Details.Clear();
		canvasGroup.alpha = 0;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
	}
	void Load()
	{
		PlayerServerSync.instance.OnEquipUpdate += OnPlayerEquipLoaded;
		PlayerServerSync.instance.SyncStats ();
	}

	void OnPlayerEquipLoaded(Dictionary<string, object> data)
	{

		foreach (UIEquipEntry e in entries) {
			if (data.ContainsKey (e.key)) {
                Dictionary<string, object> mapData = (Dictionary<string, object>)data[e.key];
                string itemId = mapData["itemID"].ToString ();
				if (!string.IsNullOrEmpty (itemId)) {
                    BaseItem i = PlayerInventory.instance.GetItem(mapData["UID"].ToString()).itemInstance; //Resources.Load<BaseItem> (Registry.assets.items [itemId]);
					e.item = i;
					e.Refresh ();
				} else {
					e.item = null;
					e.Refresh ();
				}
			} else {
				e.item = null;
				e.Refresh ();
			}

		}

		PlayerServerSync.instance.OnEquipUpdate -= OnPlayerEquipLoaded;
	}
}
