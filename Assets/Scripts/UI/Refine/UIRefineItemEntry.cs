using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRefineItemEntry : MonoBehaviour {

    public Image bg;
    public Image itemIcon;
    public int index;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Click()
    {
        if (UIRefine.instance.currentSelectedEntry != null)
            UIRefine.instance.currentSelectedEntry.bg.color = Color.white;
        UIRefine.instance.currentSelectedEntry = this;
        bg.color = Color.green;
        UIRefine.instance.ShowItem(index);
    }
}
