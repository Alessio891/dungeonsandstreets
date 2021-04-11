using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITab : MonoBehaviour, IPointerClickHandler {

    public GameObject selectedGO;
    public GameObject unselectedGO;

    public bool UseGameobjects = false;

    public Image selectedTab;
    public Image unselectedTab;

    public string ID;

    public System.Action<string> OnClick = (s) => { };

    public void Select()
    {
        SoundManager.instance.TabPickSound();
        if (!UseGameobjects)
        {
            selectedTab.enabled = true;
            unselectedTab.enabled = false;
           
        }
        else
        {
            selectedGO.SetActive(true);
            unselectedGO.SetActive(false);
        }
    }

    public void Unselect()
    {
        if (!UseGameobjects)
        {
            selectedTab.enabled = false; 
            unselectedTab.enabled = true;
            
        }
        else
        {
            selectedGO.SetActive(false);
            unselectedGO.SetActive(true);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick(ID);
    }
}
