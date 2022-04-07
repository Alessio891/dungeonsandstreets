using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISkillTab : MonoBehaviour
{
    public string TabId;

    void OnTabSelected() {
        UISkills.instance.SwitchTree(TabId, 0);
        
    }
    void OnTabUnselected() {
        
    }
}
