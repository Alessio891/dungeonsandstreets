using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UIQuestController : MonoBehaviour
{
    CanvasGroup group;

    public Accordion AccordionGroup;
    public UIPoiQuestEntry entryPrefab;
    List<UIPoiQuestEntry> entries = new List<UIPoiQuestEntry>();

    public static UIQuestController instance;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        group = GetComponent<CanvasGroup>();
        Hide();
    }

    public void LoadQuests()
    {
        foreach(UIPoiQuestEntry e in entries)
        {
            if (e != null)
                Destroy(e.gameObject);
        }
        entries.Clear();
        Debug.Log("RELOADING UI QUESTS");
        foreach(PlayerQuestData q in PlayerQuests.instance.Quests)
        {
            Debug.Log("ADDING QUEST " + q.questTemplateId);
            UIPoiQuestEntry e = GameObject.Instantiate<UIPoiQuestEntry>(entryPrefab);
            e.transform.SetParent(AccordionGroup.transform);
            e.transform.localPosition = Vector3.zero;
            e.transform.localScale = Vector3.one;
            e.Init(Registry.assets.quests[q.questTemplateId], true);
            if (q.Completed())
            {
                e.QuestCompleted();
            }
            entries.Add(e);
        }
    }

    public void Hide()
    {
        group.alpha = 0;
        group.interactable = false;
        group.blocksRaycasts = false;    
        MainUIController.instance.Active = false;       
    }

    public void Show()
    {
        LoadQuests();
        group.alpha = 1;
        group.interactable = true;
        group.blocksRaycasts = true;
        MainUIController.instance.Active = true;
    }
}
