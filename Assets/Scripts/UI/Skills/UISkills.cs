using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkills : MonoBehaviour {
    public CanvasGroup group;

    public SkillTreeAsset TestTree;

    public List<SkillTreeAsset> CombatTrees, MagicTrees, UtilityTrees;


    public VerticalLayoutGroup Content;

    public UISkillLevel LevelPrefab;
    public UITalentEntry TalentEntry;

    List<UITalentEntry> CurrentEntries = new List<UITalentEntry>();
    List<UISkillLevel> CurrentLevels = new List<UISkillLevel>();

    public List<Text> TalentTreeTabNames;
    public string CurrentSkillTree = "Combat";

    public static UISkills instance;

    private void Awake()
    {
        instance = this;
    }

    void Start () {
        SwitchTree("Combat", 0);
        Populate(CombatTrees[0]);
        Hide();
	}   

    public UITalentEntry Get(string levelid, string id)
    {        
        foreach(UISkillLevel level in CurrentLevels)
        {
            if (level.LevelID == levelid)
            {
                foreach(UITalentEntry e in level.Entries)
                {
                    if (e.ID == id)
                        return e;
                }
            }
        }
        return null;
    }

    public int Distance(UITalentEntry e, UITalentEntry other)
    {
        int firstIndex = 0;
        int secondIndex = 0;
        for(int i = 0; i < CurrentLevels.Count; i++)
        {
            UISkillLevel level = CurrentLevels[i];
            for(int j = 0; j < level.Entries.Length;j++)
            {
                if (level.Entries[j].ID == e.ID)
                {
                    firstIndex = i;
                } else if (level.Entries[j].ID == other.ID)
                {
                    secondIndex = i;
                }
            }
        }
        return Mathf.Abs(firstIndex - secondIndex);
    }
	
	// Update is called once per frame
	void Update () {
    
	}

    public void SwitchTree(string type, int tree)
    {
        switch(type)
        {
            case "Combat":
                for (int i = 0; i < 3; i++)
                {
                    TalentTreeTabNames[i].text = CombatTrees[i].Name;
                }
                CurrentSkillTree = "Combat";
                Populate(CombatTrees[tree]);
                
                break;
            case "Magic":
                for (int i = 0; i < 3; i++)
                {
                    TalentTreeTabNames[i].text = MagicTrees[i].Name;
                }
                CurrentSkillTree = "Magic";
                Populate(MagicTrees[tree]);
                break;
            case "Utility":
                for (int i = 0; i < 3; i++)
                {
                    TalentTreeTabNames[i].text = UtilityTrees[i].Name;
                }
                CurrentSkillTree = "Utility";
                Populate(UtilityTrees[tree]);
                break;
        }
    }

    public void Populate(SkillTreeAsset tree)
    {
        foreach (UITalentEntry e in CurrentEntries)
            Destroy(e.gameObject);
        CurrentEntries.Clear();
        foreach (UISkillLevel level in CurrentLevels)
            Destroy(level.gameObject);
        CurrentLevels.Clear();

        foreach(SkillTreeLevel level in tree.Levels)
        {
            UISkillLevel level_go = GameObject.Instantiate<UISkillLevel>(LevelPrefab);
            level_go.transform.SetParent(Content.transform);
            level_go.transform.localScale = Vector3.one;
            level_go.LevelID = level.ID;
            CurrentLevels.Add(level_go);
            int talentCount = level.Skills.Count;
            for (int i = 0; i < 3; i++)
            {
                if (level.GetForSlot(i) == null)
                {
                    level_go.Entries[i].Hide();
                    level_go.Entries[i].ID = "";
                }
                else
                {
                    int index = i;

                    SkillTreeEntry entry = level.GetForSlot(i);
                    UITalentEntry e = level_go.Entries[i];
                    e.Talent = entry;
                    e.TalentIcon.sprite = entry.sprite;
                    e.transform.SetParent(level_go.transform);
                    e.transform.localScale = Vector3.one;
                    e.ID = entry.ID;
                    if (!string.IsNullOrEmpty(entry.Requirements.Talent))
                    {
                        string[] split_req = entry.Requirements.Talent.Split(':');
                        string level_id = split_req[0];
                        string talent_id = split_req[1];

                        UITalentEntry linked = Get(level_id, talent_id);
                        int dist = Distance(e, linked);
                        e.LinkTo( ( (dist-1)*20) + (dist*2)*20   );

                    }
                }
            }
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
        if (MainUIController.instance != null)
            MainUIController.instance.Active = true;
        group.alpha = 1;
        group.interactable = true;
        group.blocksRaycasts = true;       
    }
}
