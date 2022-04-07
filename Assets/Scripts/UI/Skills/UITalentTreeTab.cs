using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITalentTreeTab : MonoBehaviour
{
    
    public int index;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTabSelected() {
        UISkills.instance.SwitchTree(UISkills.instance.CurrentSkillTree, index);
    }
    void OnTableUnselected()
    {

    }
}
