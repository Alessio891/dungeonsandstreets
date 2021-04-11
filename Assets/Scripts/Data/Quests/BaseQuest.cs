using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class BaseQuest : ScriptableObject
{
    public string UID;
    public string QuestTitle;
    public string QuestDescription;

    public string QuestCompletionText;

    public double SpawnChance = 1.0d;

    public List<QuestRewardData> Rewards;

    public List<QuestObjective> Objectives;
    public bool Repeatable = false;

    [ContextMenu("Generate UID")]
    public void GenerateUID()
    {
        UID = System.Guid.NewGuid().ToString();
    }

    [ContextMenu("Upload")]
    public void Upload()
    {
        Bridge.UpdateQuestOnServer(this, (r) => {
            Debug.Log("UploadQuest Response: " + r);
            ServerResponse resp = new ServerResponse(r);
            if (resp.status == ServerResponse.ResultType.Success)
            {

            }
        });
    }
    [ContextMenu("Download")]
    public void Download()
    {
        Bridge.DownloadQuestFromServer(this, (r) => {
            Debug.Log(r);
            ServerResponse resp = new ServerResponse(r);
            if (resp.status == ServerResponse.ResultType.Error)
            {
                Debug.Log("Error from server: " + resp.message);
            }
            else
            {
                Debug.Log("Success.");
                this.Deserialize(resp.GetIncomingDictionary());
            }
        });
    }

    [ContextMenu("Add To Registry")]
    public void AddToRegistry()
    {
        QuestTableEntry e = new QuestTableEntry();
        e.UID = UID;
        e.quest = this;//UnityEditor.AssetDatabase.GetAssetPath(this).Replace("Assets/Resources/", "").Replace(".asset", "");
        Registry.assets.quests.entries.Add(e);
    }

    public Dictionary<string, object> Serialize()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();

        data.Add("UID", UID);
        data.Add("QuestTitle", QuestTitle);
        data.Add("QuestDescription", QuestDescription);
        data.Add("QuestCompletionText", QuestCompletionText);
        data.Add("Repeatable", Repeatable);
        List<Dictionary<string, object>> rewards = new List<Dictionary<string, object>>();
        foreach(QuestRewardData d in Rewards)
        {
            Dictionary<string, object> rew = new Dictionary<string, object>();
            rew.Add("reward", d.reward);
            rew.Add("amount", d.amount);
            rewards.Add(rew);
        }
        data.Add("Rewards", rewards);
        data.Add("SpawnChance", SpawnChance.ToString(CultureInfo.InvariantCulture));
        List<Dictionary<string, object>> objData = new List<Dictionary<string, object>>();
        foreach(QuestObjective o in Objectives)
        {
            objData.Add(o.Serialize());
        }
        data.Add("Objectives", objData);

        return data;
    }

    public void Deserialize(Dictionary<string, object> data)
    {
        QuestTitle = data["QuestTitle"].ToString();
        QuestDescription = data["QuestDescription"].ToString();
        QuestCompletionText = data["QuestCompletionText"].ToString();
        List<object> rewards = (List<object>)data["Rewards"];
        List<object> objData = (List<object>)data["Objectives"];
        Repeatable = (data["Repeatable"].ToString() == "true") ? true : false;
        Rewards.Clear();
        foreach(object o in rewards)
        {
            Dictionary<string, object> r = (Dictionary<string, object>)o;
            QuestRewardData d = new QuestRewardData()
            {
                reward = r["reward"].ToString(),
                amount = int.Parse(r["amount"].ToString())
            };
            Rewards.Add(d);
        }

        Objectives.Clear();
        foreach(object o in objData)
        {
            Dictionary<string, object> actualData = (Dictionary<string, object>)o;
            QuestObjective objective = new QuestObjective()
            {
                ObjectiveText = actualData["ObjectiveText"].ToString(),
                ObjectiveProgress = int.Parse(actualData["ObjectiveProgress"].ToString()),
                ObjectiveTrigger = actualData["ObjectiveTrigger"].ToString(),
                ObjectiveTriggerData = actualData["ObjectiveTriggerData"].ToString()
                
            };
            Objectives.Add(objective);
        }
    }

}

[System.Serializable]
public class QuestRewardData
{
    //FOR EDITOR ONLY
    public enum RewardType { Gold = 0, XP, Item }
    public RewardType rewardType = RewardType.Gold;
    ////////////////////////////////////////////
    
    public string reward;
    public int amount;
}