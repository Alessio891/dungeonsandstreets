using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UICombatEnd : MonoBehaviour
{
    CanvasGroup group;

    public static UICombatEnd instance;
    private void Awake()
    {
        instance = this;
        group = GetComponent<CanvasGroup>();
    }

    public Transform ItemsList;
    public Transform ItemRolls;
    public Transform XPReward;
    public Transform RollResults;

    public Color Uncommon;
    public Color Rare;
    public Color Epic;
    public Color Legendary;

    public ScrollSnap RollSnap;

    public PopupQuestRewardItemEntry itemPrefab;
    public UICombatXPEntry xpPrefab;
    public UIItemRollEntry rollPrefab;
    public UIRollResultEntry resultPrefab;

    List<PopupQuestRewardItemEntry> itemEntries = new List<PopupQuestRewardItemEntry>();
    List<UIItemRollEntry> rollEntries = new List<UIItemRollEntry>();
    List<UICombatXPEntry> xpEntries = new List<UICombatXPEntry>();
    List<UIRollResultEntry> resultEntries = new List<UIRollResultEntry>();

    bool resultsReceived = false;
    List<Dictionary<string, object>> results;

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        SoundManager.instance.ChangeBGM(SoundManager.instance.BattleWonMusic, true);
        group.alpha = 1.0f;
        group.interactable = true;
        group.blocksRaycasts = true;
    }
    public void Hide()
    {
        group.alpha = 0.0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    public void updateRollSize(float value)
    {
        Vector2 size = (ItemRolls.parent.transform as RectTransform).sizeDelta;
        size.y = value;
        (ItemRolls.parent.transform as RectTransform).sizeDelta = size;
    }

    IEnumerator showRollResults()
    {
        foreach(UIItemRollEntry e in rollEntries)
        {
            e.gameObject.SetActive(false);
        }
        RollResults.gameObject.SetActive(true);
        iTween.ValueTo(gameObject, iTween.Hash("from", 60.0f, "to", 170.0f, "time", 0.4f, "easeType", "easeOutSine", "onupdate", "updateRollSize"));
        yield return new WaitForSeconds(0.4f);

        Bridge.GET(Bridge.url + "RollResults?user=" + PlayerPrefs.GetString("user") + "&combatId=" + CombatManager.instance.combatData.UID, (r) =>
        {
            Debug.Log("RESULT RESPONSE: " + r);
            ServerResponse resp = new ServerResponse(r);
            if (resp.status == ServerResponse.ResultType.Success)
            {
                List<Dictionary<string,object>> resultsRaw = resp.GetIncomingList();
                results = resultsRaw;
                resultsReceived = true;
            }
        });
        while (!resultsReceived)
            yield return null;
        bool allAssigned = true;
        foreach (Dictionary<string, object> result in results)
        {
            UIRollResultEntry e = GameObject.Instantiate<UIRollResultEntry>(resultPrefab);
            if (!(bool)result["allRolled"])
            {
                allAssigned = false;
            }
            e.transform.SetParent(RollResults);
            e.transform.localPosition = Vector3.zero;
            e.transform.localScale = Vector3.one;
            e.Init(result);
            resultEntries.Add(e);
            yield return StartCoroutine(e.Show());
        }
        yield return new WaitForSeconds(3.0f);
        resultsReceived = false;
        
        while (!allAssigned)
        {
            Debug.Log("Checking roll status");
            Bridge.GET(Bridge.url + "RollResults?user=" + PlayerPrefs.GetString("user") + "&combatId=" + CombatManager.instance.combatData.UID, (r) =>
            {
                ServerResponse resp = new ServerResponse(r);
                if (resp.status == ServerResponse.ResultType.Success)
                {
                    List<Dictionary<string, object>> resultsRaw = resp.GetIncomingList();
                    results = resultsRaw;
                    resultsReceived = true;
                }
            });
            while (!resultsReceived)
                yield return null;
            resultsReceived = false;
            allAssigned = true;
            for (int i = 0; i < results.Count; i++)
            {
                resultEntries[i].UpdateData(results[i]);
                if (!(bool)results[i]["allRolled"])
                    allAssigned = false;
            }
            yield return new WaitForSeconds(3.0f);
        }

        Debug.Log("EVERYONE ROLLED!!! GG!");
    }

    public void NextRoll(UIItemRollEntry lastPage)
    {
        if (lastPage.index == rollEntries.Count-1)
        {
            StartCoroutine(showRollResults());
            Debug.Log("DONE ALL ROLL");
        }
        else
            RollSnap.NextScreen();
    }    

    public void Load(Dictionary<string, object> endData)
    {
        Dictionary<string, object> actionResult = (Dictionary<string, object>)endData["actionResult"];

        Dictionary<string, object> items = (Dictionary<string, object>)actionResult["AssignedItems"];
        List<object> rolls = (List<object>)actionResult["Rolls"];
        List<object> xp = (List<object>)actionResult["ExperienceGain"];

        foreach (PopupQuestRewardItemEntry e in itemEntries)
            e.Destroy(e.gameObject);
        foreach (UIItemRollEntry e in rollEntries)
            e.Destroy(e.gameObject);
        foreach (UICombatXPEntry e in xpEntries)
            e.Destroy(e.gameObject);
        foreach (KeyValuePair<string, object> i in items)
        {
            PopupQuestRewardItemEntry e = GameObject.Instantiate<PopupQuestRewardItemEntry>(itemPrefab);
            e.transform.SetParent(ItemsList);
            e.transform.localPosition = Vector3.zero;
            e.transform.localScale = Vector3.one;
            BaseItem item = Resources.Load<BaseItem>(Registry.assets.items[i.Key]);
            e.Init(item, 1);
            itemEntries.Add(e);
        }

        int index = 0;
        foreach (object roll in (List<object>)actionResult["Rolls"])
        {
            Dictionary<string, object> rollData = (Dictionary<string, object>)roll;
            UIItemRollEntry e = GameObject.Instantiate<UIItemRollEntry>(rollPrefab);
            e.index = index;
            index++;
            e.transform.SetParent(ItemRolls);
            e.transform.localPosition = Vector3.zero;
            e.transform.localScale = Vector3.one;
            BaseItem item = Resources.Load<BaseItem>(Registry.assets.items[rollData["itemId"].ToString()]);
            e.ItemName.text = item.Name;
            e.ItemIcon.sprite = item.sprite;
            switch(item.ItemQuality)
            {
                case BaseItem.Quality.Uncommon:
                    e.BG.color = Uncommon;
                    break;
                case BaseItem.Quality.Rare:
                    e.BG.color = Rare;
                    break;
                case BaseItem.Quality.Epic:
                    e.BG.color = Epic;
                    break;
                case BaseItem.Quality.Legendary:
                    e.BG.color = Legendary;
                    break;
            }
            rollEntries.Add(e);
            
        }

        foreach(object o in xp)
        {
            string d = (string)o;
            string[] xpData = d.Split(':');
            UICombatXPEntry e = GameObject.Instantiate<UICombatXPEntry>(xpPrefab);
            e.transform.SetParent(XPReward);
            e.transform.localPosition = Vector3.zero;
            e.transform.localScale = Vector3.one;
            e.What.text = xpData[0];
            e.XPAmount.text = xpData[1]+"xp";
            xpEntries.Add(e);
        }

    }

    public void Claim()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("user", PlayerPrefs.GetString("user"));
        data.Add("combatId", CombatManager.instance.combatData.UID);
        Bridge.POST(Bridge.url + "ExitCombat", data, (r) =>
        {
            CombatManager.instance.EndCombat();
        });
    }
}
