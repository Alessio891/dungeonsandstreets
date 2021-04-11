using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerQuests : MonoBehaviour
{

    public static PlayerQuests instance;
    private void Awake()
    {
        instance = this;
    }

    public List<PlayerQuestData> Quests = new List<PlayerQuestData>();

    public List<string> notifiedCompletedQuests = new List<string>();

    private void Start()
    {
        PlayerServerSync.instance.OnQuestUpdate += QuestSync;
        PlayerServerSync.instance.OnCompletedQuests += QuestCompleted;
    }
    private void OnEnable()
    {
     
    }
    private void OnDisable()
    {
        PlayerServerSync.instance.OnQuestUpdate -= QuestSync;
    }

    public void QuestSync(List<Dictionary<string, object>> data)
    {
        Quests.Clear();
        foreach (Dictionary<string, object> quest in data)
        {
            BaseQuest q = Registry.assets.quests[quest["questTemplateId"].ToString()];
            PlayerQuestData questData = PlayerQuestData.FromQuestAsset(q);
            List<object> objectivesData = (List<object>)quest["objectives"];
            for(int i = 0; i < objectivesData.Count; i++)
            {
                Dictionary<string, object> objective = (Dictionary<string, object>)objectivesData[i];
                questData.objectives[i].progress = int.Parse(objective["progress"].ToString());
            }
            Quests.Add(questData);            
        }           
    }

    public void QuestCompleted(List<object> data)
    {
        foreach(object o in data)
        {
            string id = o.ToString();
            if (!notifiedCompletedQuests.Contains(id))
            {
                BaseQuest q = Registry.assets.quests[id];
                OnScreenNotification.instance.Show(q.QuestTitle + " Completed!", OnScreenNotification.instance.QuestSprite);
                UIQuestNotification n = UIClickableNotifications.instance.AddNotification() as UIQuestNotification;
                n.questId = id;
            }
        }
    }

    public PlayerQuestData GetQuest(string uid)
    {
        foreach(PlayerQuestData q in Quests)
        {
            if (q.questTemplateId == uid)
                return q;
        }
        return null;
    }

}

[System.Serializable]
public class PlayerQuestData
{    
    public string questTemplateId;
    public List<PlayerQuestObjectiveData> objectives;

    public static PlayerQuestData FromQuestAsset(BaseQuest quest)
    {
        PlayerQuestData data = new PlayerQuestData();

        data.questTemplateId = quest.UID;
        data.objectives = new List<PlayerQuestObjectiveData>();
        foreach(QuestObjective objective in quest.Objectives)
        {
            PlayerQuestObjectiveData o = new PlayerQuestObjectiveData();
            o.objective = objective.ObjectiveText;
            o.objectiveTrigger = objective.ObjectiveTrigger;
            o.objectiveTriggerData = objective.ObjectiveTriggerData;
            o.progress = 0;
            o.progressTarget = objective.ObjectiveProgress;
            data.objectives.Add(o);
        }

        return data;
    }

    public bool Completed()
    {
        foreach(PlayerQuestObjectiveData d in objectives)
        {
            if (d.progress < d.progressTarget)
                return false;
        }
        return true;
    }

}

[System.Serializable]
public class PlayerQuestObjectiveData
{
    public string objective;
    public string objectiveTrigger;
    public string objectiveTriggerData;
    public int progress;
    public int progressTarget;
}
