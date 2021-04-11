using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRefine : MonoBehaviour {

    public HorizontalLayoutGroup itemSelectionHorizontal;
    public UIRefineItemEntry itemSelectionPrefab;

    public VerticalLayoutGroup requiredMaterialsVertical;
    public UIRefineRequiredMaterial requiredMaterialPrefab;

    public VerticalLayoutGroup resultVerticalGroup;
    public UIRefineResultEntry resultPrefab;
    public Text resultItemName;
    public Button refineButton;
    public CanvasGroup canvasGroup;

    public static UIRefine instance;

    public List<Dictionary<string, object>> lastData;
    public int currentSelected = 0;
    public UIRefineItemEntry currentSelectedEntry;

    void Awake()
    {
        instance = this;
    }


    public void ClearItemSelection()
    {
        foreach (UIRefineItemEntry e in itemSelectionHorizontal.GetComponentsInChildren<UIRefineItemEntry>())
        {
            Destroy(e.gameObject);
        }
    }
    public void ClearRequiredMaterials()
    {
        foreach(UIRefineRequiredMaterial e in requiredMaterialsVertical.GetComponentsInChildren<UIRefineRequiredMaterial>())
        {
            Destroy(e.gameObject);
        }
    }
    public void ClearResult()
    {
        foreach (UIRefineResultEntry e in resultVerticalGroup.GetComponentsInChildren<UIRefineResultEntry>())
        {
            Destroy(e.gameObject);
        }        
    }


    public void ShowItem(int index)
    {
        currentSelected = index;
        ClearRequiredMaterials();
        ClearResult();

        Dictionary<string, object> item = (Dictionary<string, object>)lastData[index]["data"];
        Dictionary<string, object> refineNeeded = (Dictionary<string, object>)item["refineItemsNeeded"];
        BaseWeapon baseItem = Resources.Load<BaseWeapon>(Registry.assets.items[lastData[index]["itemUID"].ToString()]);

        foreach (KeyValuePair<string, object> refine in refineNeeded)
        {
            BaseItem refineItem = Resources.Load<BaseItem>(Registry.assets.items[refine.Key]);
            UIRefineRequiredMaterial m = GameObject.Instantiate<UIRefineRequiredMaterial>(requiredMaterialPrefab);
            m.itemIcon.sprite = refineItem.sprite;
            int playerQuantity = PlayerInventory.instance.GetQuantity(refine.Key);
            int refineValue = 0;
            int.TryParse(refine.Value.ToString(), out refineValue);
            m.materialInfo.text = refineItem.Name + " - "+playerQuantity.ToString()+"/" + refine.Value.ToString();
            if (playerQuantity < refineValue)
                m.materialInfo.color = Color.red;
            else
                m.materialInfo.color = Color.green;
            m.transform.SetParent(requiredMaterialsVertical.transform);
            m.transform.localScale = Vector3.one;
        }

        int currentRefine = 0;
        int.TryParse(item["currentRefine"].ToString(), out currentRefine);
        resultItemName.text = baseItem.Name + " +" + (currentRefine + 1).ToString();
        if (currentRefine < 9)
        {            
            foreach (GrowthData d in baseItem.refineGrowth)
            {
                UIRefineResultEntry e = GameObject.Instantiate<UIRefineResultEntry>(resultPrefab);
                e.transform.SetParent(resultVerticalGroup.transform);
                e.transform.localScale = Vector3.one;
                e.StatName.text = d.stat;
                e.value.text = "+" + d.growth[currentRefine + 1].ToString();
            }
            refineButton.gameObject.SetActive(true);
            refineButton.GetComponentInChildren<Text>().text = "Refine! (" + (GetChance(currentRefine + 1) * 100).ToString() + "%)";
        }
        else
        {
            UIRefineResultEntry e = GameObject.Instantiate<UIRefineResultEntry>(resultPrefab);
            e.transform.SetParent(resultVerticalGroup.transform);
            e.transform.localScale = Vector3.one;
            e.StatName.text = "You can't upgrade this item further.";
            e.StatName.verticalOverflow = VerticalWrapMode.Overflow;
            e.value.text = "";
            refineButton.gameObject.SetActive(false);
        }
    }
    float GetChance(int refineLevel)
    {
        if (refineLevel < 4)
            return 1;
        if (refineLevel == 4)
            return 0.5f;
        else if (refineLevel == 5)
            return 0.4f;
        else if (refineLevel == 6)
            return 0.33f;
        else if (refineLevel == 7)
            return 0.2f;
        else if (refineLevel == 8)
            return 0.18f;
        else if (refineLevel == 9)
            return 0.1f;
        return 0;
    }
    public void Refine()
    {
        Dictionary<string, object> item = (Dictionary<string, object>)lastData[currentSelected]["data"];
        Dictionary<string, object> values = new Dictionary<string, object>();
        int currentRefine = 0;
        int.TryParse(item["currentRefine"].ToString(), out currentRefine);

        if (currentRefine > 9)
            return;

        Dictionary<string, object> refineNeeded = (Dictionary<string, object>)item["refineItemsNeeded"];
        BaseWeapon baseItem = Resources.Load<BaseWeapon>(Registry.assets.items[lastData[currentSelected]["itemUID"].ToString()]);
        bool canRefine = true;
        foreach (KeyValuePair<string, object> refine in refineNeeded)
        {
            BaseItem refineItem = Resources.Load<BaseItem>(Registry.assets.items[refine.Key]);
            
            int playerQuantity = PlayerInventory.instance.GetQuantity(refine.Key);
            int refineValue = 0;
            int.TryParse(refine.Value.ToString(), out refineValue);
            if (playerQuantity < refineValue)
            {
                canRefine = false;
                break;
            }
        }
        if (!canRefine)
            return;
        values.Add("user", PlayerPrefs.GetString("user"));
        values.Add("itemIndex", baseItem.UID);
        Bridge.POST(Bridge.url + "Refine/RefineItem", values, (r) => {
            Debug.Log("[REFINE ITEM RESPONSE] " + r);
            ServerResponse resp = new ServerResponse(r);
            if (resp.status == ServerResponse.ResultType.Success)
            {
                if (resp.message == "Refine Fail")
                {
                    Debug.Log("Refine failed :(");
                    lastData = resp.GetIncomingList();
                    PlayerServerSync.instance.SyncStats(() => ReloadSelection());
                }
                else
                {
                    Debug.Log("Refine success!");
                    lastData = resp.GetIncomingList();
                    PlayerServerSync.instance.SyncStats(() => ShowItem(currentSelected));
                }
                
            }
        });
    }

    public void ReloadSelection()
    {
        ClearItemSelection();
        int itemIndex = 0;
        foreach (Dictionary<string, object> itemRaw in lastData)
        {
            Dictionary<string, object> item = (Dictionary<string, object>)itemRaw["data"];
            BaseWeapon baseItem = Resources.Load<BaseWeapon>(Registry.assets.items[item["UID"].ToString()]);
            UIRefineItemEntry itemEntry = GameObject.Instantiate<UIRefineItemEntry>(itemSelectionPrefab);
            itemEntry.transform.SetParent(itemSelectionHorizontal.transform);
            itemEntry.transform.localScale = Vector3.one;
            itemEntry.itemIcon.sprite = baseItem.sprite;
            itemEntry.index = itemIndex;
            itemIndex++;
        }
        ShowItem(0);

    }

    public void Load()
    {
        ClearItemSelection();
        ClearRequiredMaterials();
        ClearResult();

        PlayerServerSync.instance.SyncStats();

        Bridge.GET(Bridge.url + "Refine/GetRefineInfo?user=" + PlayerPrefs.GetString("user"), (r) => {
            Debug.Log("[REFINE RESPONSE] " + r);

            ServerResponse resp = new ServerResponse(r);

            if (resp.status == ServerResponse.ResultType.Success)
            {
                List<Dictionary<string, object>> entries = resp.GetIncomingList();
                lastData = entries;
                int itemIndex = 0;
                foreach (Dictionary<string, object> itemRaw in entries)
                {
                    Dictionary<string, object> item = (Dictionary<string, object>)itemRaw["data"];
                    BaseWeapon baseItem = Resources.Load<BaseWeapon>(Registry.assets.items[itemRaw["itemUID"].ToString()]);
                    UIRefineItemEntry itemEntry = GameObject.Instantiate<UIRefineItemEntry>(itemSelectionPrefab);
                    itemEntry.transform.SetParent(itemSelectionHorizontal.transform);
                    itemEntry.transform.localScale = Vector3.one;
                    itemEntry.itemIcon.sprite = baseItem.sprite;
                    itemEntry.index = itemIndex;
                    if (itemIndex == 0)
                    {
                        currentSelectedEntry = itemEntry;
                        itemEntry.bg.color = Color.green;
                    }
                    itemIndex++;
                }

                ShowItem(0);
            }
            else
                Bridge.DispatchError("Error connection");

        });
    }

    public void Show()
    {
        if (MainUIController.instance != null)
            MainUIController.instance.Active = true;
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        Load();    
    }
    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        if (MainUIController.instance != null)
            MainUIController.instance.Active = false;
        Resources.UnloadUnusedAssets();
    }

    // Use this for initialization
    void Start () {
        Hide();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
