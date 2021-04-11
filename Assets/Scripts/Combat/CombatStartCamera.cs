using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStartCamera : MonoBehaviour {
    public float speed = 5;
    public bool rotate = true;
    public bool Animating = true;
    public Transform lookAt;    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (rotate)
            transform.RotateAround(lookAt.position, Vector3.up, speed * Time.deltaTime);
        
    }
}
