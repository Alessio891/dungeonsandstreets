using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideBar : MonoBehaviour {

	public bool Open = false;

	public Transform center;

	public List<SidebarIcon> icons;

	public static SideBar instance;
	void Awake() {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Show()
	{
		Open = true;
		foreach (SidebarIcon i in icons) {
			i.Show ();
		}
	}
	public void Hide()
	{
		Open = false;
		foreach (SidebarIcon i in icons) {
			i.Hide ();
		}
	}
	public void Toggle()
	{
		if (!Open)
			Show ();
		else
			Hide ();
	}
}
