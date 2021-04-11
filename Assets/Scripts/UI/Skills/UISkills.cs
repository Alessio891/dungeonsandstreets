using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkills : MonoBehaviour {
    public CanvasGroup group;

    public List<SkillEntry> entries;

    public SkillEntry entryPrefab;

    public GridLayoutGroup layout;
	// Use this for initialization
	void Start () {
        Hide();
	}
	
	// Update is called once per frame
	void Update () {
    
	}

    public void LoadSkills(List<string> data)
    {
        string request = Bridge.url + "Skills/GetSkills?skills=";
        foreach (string s in data)
        {
            request += s + ",";
        }
        if (request.EndsWith(","))
            request = request.Substring(0, request.Length - 1);
        

        PlayerServerSync.instance.OnSkillsUpdate -= LoadSkills;
    }

    public void Clear()
    {
        foreach (SkillEntry e in entries)
        {
            Destroy(e.gameObject);
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

        PlayerServerSync.instance.OnSkillsUpdate += LoadSkills;
        PlayerServerSync.instance.SyncStats();  
    }
}
