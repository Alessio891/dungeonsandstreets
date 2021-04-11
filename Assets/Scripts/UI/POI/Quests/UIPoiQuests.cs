using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIPoiQuests : MonoBehaviour
{
    public UIPoiQuestEntry entryPrefab;

    public VerticalLayoutGroup QuestList;
    List<UIPoiQuestEntry> currentEntries = new List<UIPoiQuestEntry>();

    public void LoadQuests()
    {
        foreach(UIPoiQuestEntry e in currentEntries)
        {
            if (e != null)
                Destroy(e.gameObject);
        }
        currentEntries.Clear();
        List<object> quests = (List<object>)UIPoi.instance.POIData["Quests"];
        foreach(object q in quests)
        {
            BaseQuest questData = Registry.assets.quests[q.ToString()];
            if (questData != null)
            {
                if (PlayerQuests.instance.GetQuest(questData.UID) == null)
                {
                    UIPoiQuestEntry e = GameObject.Instantiate<UIPoiQuestEntry>(entryPrefab);
                    e.Init(questData);
                    e.transform.SetParent(QuestList.transform);
                    e.transform.localPosition = Vector3.zero;
                    e.transform.localScale = Vector3.one;
                    currentEntries.Add(e);
                }
            }
        }
    }    
}
