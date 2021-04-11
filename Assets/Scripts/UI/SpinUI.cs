using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinUI : MonoBehaviour {
	public float amount = 360;
	// Use this for initialization
	void Start () {
		iTween.PunchRotation (gameObject, iTween.Hash("amount", new Vector3 (0, amount, 0), "time", 1.5f, "loopType", "loop"));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
