using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryTab : MonoBehaviour {
	public string category;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void SwitchTab(bool toggle)
	{
		//Debug.Log ("Toggle is " + toggle + " for " + category);
		//if (toggle) {
			UIInventory.instance.ChangeTab (category);
		//}
	}
}
