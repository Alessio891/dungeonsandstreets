using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITabController : MonoBehaviour {

    public Dictionary<string, UITab> tabs;
    public string currentSelected;

    public System.Action<string> OnToggleOn = (s) => { };

	// Use this for initialization
	void Start () {
        tabs = new Dictionary<string, UITab>();
        bool first = true;
        foreach (UITab tab in GetComponentsInChildren<UITab>())
        {
            tab.OnClick = HandleClick;
            if (first)
            {
                tab.Select();
                currentSelected = tab.ID;
                first = false;
            }
            else
                tab.Unselect();
            tabs.Add(tab.ID, tab);
        }		
	}

    public void HandleClick(string id)
    {
        if (currentSelected != id)
        {
            tabs[currentSelected].Unselect();
        }
        currentSelected = id;
        tabs[id].Select();
        OnToggleOn(id);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
