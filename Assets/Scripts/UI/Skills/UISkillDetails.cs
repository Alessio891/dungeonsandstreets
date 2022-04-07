using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillDetails : MonoBehaviour
{

    public Text SkillName, SkillDescription, SkillPoints;
    public Image Icon;

    public Text[] Requirements;    
    

    public static UISkillDetails instance;
    private void Awake()
    {
        instance = this;
    }

    public void ShowSkill(SkillTreeEntry entry)
    {
        SkillName.text = entry.Name;
        SkillDescription.text = entry.Description;

        Icon.sprite = entry.sprite;

        SkillPoints.text = "0/"+entry.MaxPoints.ToString();

        if (entry.Requirements.PlayerLevel != -1)
        {
            Requirements[0].enabled = true;
            Requirements[0].text = "Requires player level " + entry.Requirements.PlayerLevel.ToString();
        } else
        {
            Requirements[0].enabled = false;
        }

        if (!string.IsNullOrEmpty(entry.Requirements.Stat) && !entry.Requirements.Stat.StartsWith("None"))
        {
            Requirements[1].enabled = true;
            string[] split = entry.Requirements.Stat.Split(':');
            Requirements[1].text = "Requires " + split[1] + " points in " + split[0];
        } else
        {
            Requirements[1].enabled = false;
        }

        if (!string.IsNullOrEmpty(entry.Requirements.Talent))
        {
            Requirements[2].enabled = true;
            string[] split = entry.Requirements.Talent.Split(':');
            Requirements[2].text = "Requires " + split[1] + " points in " + split[0];
        } else
        {
            Requirements[2].enabled = false;
        }
    }
}
