using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour {
	public Text text;
	public float angle;
	// Use this for initialization
	void Start () {
        Vector3 dir = Quaternion.Euler(0, 0, -angle) * transform.up;  //Quaternion.AngleAxis (angle, new Vector3 (0, 0, 1)) * Vector3.up;
		float dist = 50;
		iTween.MoveTo (gameObject, iTween.Hash("position", transform.position + (dir * dist),"time", 1.5f));
		Destroy (gameObject, 1.5f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
