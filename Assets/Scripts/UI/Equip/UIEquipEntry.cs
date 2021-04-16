using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UIEquipEntry : MonoBehaviour, IPointerDownHandler {

	public string key;
	public BaseItem item;

	public Image sprite;

    public Image Background;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Refresh()
	{
		if (item != null) {
			sprite.enabled = true;
			sprite.sprite = item.sprite;
		} else
			sprite.enabled = false;
	}

    public void OnPointerDown(PointerEventData eventData)
    {
        if (item != null)
        {
            UIEquip.instance.Select(key);
            UIEquip.instance.Details.LoadItem(item as BaseEquippable);
        }
    }
}
