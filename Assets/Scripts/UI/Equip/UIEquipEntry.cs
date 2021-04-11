using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIEquipEntry : MonoBehaviour {

	public string key;
	public BaseItem item;

	public Image sprite;

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
}
