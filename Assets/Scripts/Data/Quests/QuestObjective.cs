using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestObjective
{
    public string ObjectiveTrigger;
    public string ObjectiveTriggerData;
    public string ObjectiveText;
    public int ObjectiveProgress = 0;

    public Dictionary<string, object> Serialize()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("ObjectiveTrigger", ObjectiveTrigger);
        data.Add("ObjectiveTriggerData", ObjectiveTriggerData);
        data.Add("ObjectiveText", ObjectiveText);
        data.Add("ObjectiveProgress", ObjectiveProgress);

        return data;
    }

    public void Deserialize(Dictionary<string, object> data)
    {
        ObjectiveTrigger = data["ObjectiveTrigger"].ToString();
        ObjectiveTriggerData = data["ObjectiveTriggerData"].ToString();
        ObjectiveText = data["ObjectiveText"].ToString();
        ObjectiveProgress = int.Parse(data["ObjectiveProgress"].ToString());
    }
}
