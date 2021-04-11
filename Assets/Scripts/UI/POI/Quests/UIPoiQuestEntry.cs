using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UIPoiQuestEntry : MonoBehaviour
{
    public Image background;
    public UIPoiObjectiveEntry entryPrefab;

    public Text QuestTitle;
    public Text QuestDescription;
    public VerticalLayoutGroup ObjectiveList;

    AccordionElement accordionElement;

    public GameObject CompletedButton;

    BaseQuest quest;
    Color originalColor;

    public void Init(BaseQuest quest, bool PlayerQuest = false)
    {        
        if (CompletedButton != null)
            CompletedButton.SetActive(false);
        this.quest = quest;
        if (background != null)
            originalColor = background.color;
        QuestTitle.text = quest.QuestTitle;
        QuestDescription.text = quest.QuestDescription;
        accordionElement = GetComponent<AccordionElement>();
        accordionElement.isOn = false;
        for (int i = 0; i < quest.Objectives.Count; i++)
        {
            QuestObjective o = quest.Objectives[i];
            UIPoiObjectiveEntry e = GameObject.Instantiate<UIPoiObjectiveEntry>(entryPrefab);
            e.ObjectiveText.text = o.ObjectiveText;
            if (PlayerQuest)
            {
                PlayerQuestData q = PlayerQuests.instance.GetQuest(quest.UID);
                e.Objective.text = q.objectives[i].progress.ToString() + "/ " + q.objectives[i].progressTarget.ToString();

            } else
                e.Objective.text = "0/ " + o.ObjectiveProgress.ToString();
            e.transform.SetParent(ObjectiveList.transform);
            e.transform.localPosition = Vector3.zero;
            e.transform.localScale = Vector3.one;
            
        }
    }

    public void Complete()
    {
        QuestCompletedPopup popup = PopupManager.ShowPopup<QuestCompletedPopup>("QuestCompletedPopup", "Quest Completed!", "Quest Completed!", (s, p) => {
            UIQuestController.instance.LoadQuests();
            UIClickableNotifications.instance.RemoveQuestNotification(quest.UID);
        }
        );
        popup.questId = quest.UID;
        popup.Init();
    }

    public void Abandon()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("user", PlayerPrefs.GetString("user"));
        data.Add("questId", quest.UID);
        Bridge.POST(Bridge.url + "Quests/AbandonQuest", data, (r) =>
        {
            Debug.Log("[AbandonQuestResp] " + r);
            ServerResponse resp = new ServerResponse(r);
            if (resp.status == ServerResponse.ResultType.Success)
            {
                PlayerServerSync.instance.SyncStats(() => UIQuestController.instance.LoadQuests());
            }
        });
    }

    public void AcceptQuest()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("user", PlayerPrefs.GetString("user"));
        data.Add("questId", quest.UID);
        Bridge.POST(Bridge.url + "Quests/AcceptQuest", data, (r) =>
        {
            Debug.Log("[AcceptQuest Response] " + r);
            ServerResponse resp = new ServerResponse(r);
            if (resp.status == ServerResponse.ResultType.Success)
            {
                Debug.Log("Accepted quest");
                Destroy(gameObject);
                OnScreenNotification.instance.Show("New Quest Accepted!", OnScreenNotification.instance.QuestSprite);              
                PlayerServerSync.instance.SyncStats();

            }
        });        
    }

    public void QuestCompleted()
    {
        CompletedButton.SetActive(true);
        StopCoroutine("questCompleteAnim");
        StartCoroutine("questCompleteAnim");
    }

    IEnumerator questCompleteAnim()
    {
        float timer = 0;
        bool invert = false;
        
        while(true)
        {
            if (accordionElement.isOn)
            {
                background.color = originalColor;
                timer = 0;
                invert = false;
                yield return null;
                continue;
            }
            timer += Time.deltaTime;
            if (timer > 0.7f)
            {
                invert = !invert;
                timer = 0;
            }
            background.color = Color.Lerp(background.color, (invert) ? originalColor : Color.yellow, 1.0f * Time.deltaTime);
            yield return null;
        }
    }

    
}
