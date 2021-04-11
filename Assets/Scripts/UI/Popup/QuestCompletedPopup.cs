using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QuestCompletedPopup : BasePopup
{
    public GridLayoutGroup ItemsGrid;
    public Text GoldText;
    public Text XPText;
    public Text XP2Text;

    public Image GoldIcon;

    public PopupQuestRewardItemEntry entryPrefab;

    public string questId;

    List<PopupQuestRewardItemEntry> entries = new List<PopupQuestRewardItemEntry>();
    int goldAmount;
    int xpAmount;

    public void Init()
    {
        BaseQuest quest = Registry.assets.quests[questId];
        GoldText.text = "0";
        XPText.text = "0";
        foreach(QuestRewardData d in quest.Rewards)
        {
            if (d.reward == "gold")
            {
                goldAmount = d.amount;
            } else if (d.reward == "xp")
            {
                xpAmount = d.amount;
            } else
            {
                BaseItem item = Resources.Load<BaseItem>(Registry.assets.items[d.reward]);
                if (item != null)
                {
                    PopupQuestRewardItemEntry e = GameObject.Instantiate<PopupQuestRewardItemEntry>(entryPrefab);
                    e.canvas.alpha = 0;
                    e.Init(item, d.amount);
                    e.transform.SetParent(ItemsGrid.transform);
                    e.transform.localPosition = Vector3.zero;
                    e.transform.localScale = Vector3.one;
                    entries.Add(e);
                }

            }
        }

        StartCoroutine(ShowRewards());
    }

    IEnumerator ShowRewards()
    {
        foreach(PopupQuestRewardItemEntry e in entries)
        {
            while(e.canvas.alpha < 1)
            {
                e.canvas.alpha += 0.9f * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        yield return new WaitForSeconds(0.5f);
        int currentAmount = 0;
        while(currentAmount < goldAmount)
        {
            currentAmount += 20;
            GoldText.text = currentAmount.ToString();
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.5f);
        currentAmount = 0;
        while(currentAmount < xpAmount)
        {
            currentAmount += 20;
            XPText.text = currentAmount.ToString();
            yield return new WaitForEndOfFrame();
        }        
    }

    public void CompleteQuest()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("user", PlayerPrefs.GetString("user"));
        data.Add("questId", questId);
        Bridge.POST(Bridge.url + "Quests/CompleteQuest", data, (r) => {
            Debug.Log("[CompleteQuest Response] " + r);
            ServerResponse resp = new ServerResponse(r);
            if (resp.status == ServerResponse.ResultType.Success)
            {
                Debug.Log("Quest completed, reward assigned. Syncinc");
                PlayerServerSync.instance.SyncStats( () =>
                {
                    if (this.OnButtonPressed != null)
                    {
                        this.OnButtonPressed("complete", this);
                    }
                });
                PlayerQuests.instance.notifiedCompletedQuests.Remove(questId);
                
                this.Close();
            }
        });
    }

}
