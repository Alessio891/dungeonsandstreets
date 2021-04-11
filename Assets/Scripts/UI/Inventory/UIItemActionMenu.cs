using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemActionMenu : MonoBehaviour
{    
    public VerticalLayoutGroup List;

    public Text NameText;

    CanvasGroup group;
    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Show()
    {        
        group.alpha = 1;
        group.interactable = true;
        group.blocksRaycasts = true;
        NameText.text = UIInventory.instance.currentEntry.item.Name;
        if (UIInventory.instance.currentEntry.item is BaseWeapon)
        {
            UIInventory.instance.UseText.text = "Equip";
            UIInventory.instance.DismantleButton.SetActive(true);
        } else
        {
            UIInventory.instance.UseText.text = "Use";
            UIInventory.instance.DismantleButton.SetActive(false);
        }
    }
    public void Hide(bool unselected = true)
    {
        group.alpha = 0;
        group.interactable = false;
        group.blocksRaycasts = false;
        if (UIInventory.instance.currentEntry != null && unselected)
        {
            UIInventory.instance.currentEntry.Unselect();
            UIInventory.instance.currentEntry = null;
        }
    }

    public void ShowDetails()
    {
        UIInventory.instance.detailsGroup.Show();
        Hide(false);
    }
}
