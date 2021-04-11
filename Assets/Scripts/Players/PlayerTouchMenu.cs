using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Examples.LocationProvider;
using Mapbox.Examples;

public class PlayerTouchMenu : MonoBehaviour {
	BoxCollider coll;
	PositionWithLocationProvider p;
	// Use this for initialization
	void Start () {
		coll = GetComponent<BoxCollider> ();
		p = GetComponent<PositionWithLocationProvider> ();
//
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			Ray r = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (coll.Raycast (r, out hit, 1000.0f)) {
				Debug.Log ("Touch!");
				//if (!StatsMenu.instance.Open)
				//	StatsMenu.instance.Show ();
			}
		}

		if (Input.GetKeyDown (KeyCode.Return)) {
			DockBar.instance.UpdateGold (500);
		} 
		if (Input.GetKeyDown (KeyCode.Space)) {
			
		}
	}
}
