using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TestUI : MonoBehaviour {
	public InputField user;
	public InputField pass;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void Register()
	{
		Dictionary<string, object> values = new Dictionary<string, object> ();
		values.Add ("userName", user.text);
		values.Add ("password", pass.text);
		Bridge.POST ("http://localhost:8080/DungeonsAndStreets/Register", values, (s) => Debug.Log (s));	
	}

	public void Login()
	{
		Dictionary<string, object> values = new Dictionary<string, object> ();
		values.Add ("userName", user.text);
		values.Add ("password", pass.text);
		Bridge.POST ("http://localhost:8080/DungeonsAndStreets/Auth", values, (s) => Debug.Log (s));
	}
}
