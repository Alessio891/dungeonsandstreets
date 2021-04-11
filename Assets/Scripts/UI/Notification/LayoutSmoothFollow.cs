using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class LayoutSmoothFollow : MonoBehaviour {


	public Transform followTransform;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance (transform.position, followTransform.position) > 0.001f) {
			transform.position = Vector3.Lerp (transform.position, followTransform.position, Time.deltaTime * 10.0f);
		}
	}
}
