using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
public class FloatingTextSpawner : MonoBehaviour {

	public FloatingText text;
	public string ID;
	float currentAngle = 45;
	public static List<FloatingTextSpawner> instances = new List<FloatingTextSpawner>();
	public static FloatingTextSpawner Get(string key) { return instances.Where<FloatingTextSpawner>( (s) => s.ID == key ).FirstOrDefault<FloatingTextSpawner>(); }

	void OnEnable()
	{
		instances.Add (this);
	}

	void OnDisable()
	{
		instances.Remove (this);
	}

	public void SpawnText(string message, Color c, int size = 25)
	{
		FloatingText t = GameObject.Instantiate<FloatingText> (text);
		t.text.color = c;
		t.text.fontSize = size;
		t.angle = currentAngle;
		currentAngle -= 25;
		if (currentAngle < -95)
			currentAngle = 95;
		t.transform.SetParent (transform);
		t.transform.localPosition = Vector3.zero;
		t.transform.localScale = Vector3.one;
		t.text.text = message;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
