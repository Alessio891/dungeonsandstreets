using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIController : MonoBehaviour {

    bool _active = false;
	public bool Active
    {
        get { return _active; }
        set
        {
            if (value && sideMenu != null)
                sideMenu.SetActive(true);
            else if (!value && sideMenu != null)
                sideMenu.SetActive(false);
            _active = value;
        }
    }
	public static MainUIController instance;
    public GameObject sideMenu;

	void Awake() { instance = this; }

	// Use this for initialization
	void Start () {
        Active = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
