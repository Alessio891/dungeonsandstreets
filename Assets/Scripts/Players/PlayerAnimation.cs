using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
	public Animator animator;
	Vector3 lastPos;
	bool walking = false;
    public float MovingDistanceThreshold = 0.1f;
	// Use this for initialization
	void Start () {
		lastPos = transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		float distance = Vector3.Distance (lastPos, transform.position);
        bool moving = distance > MovingDistanceThreshold;
        
        animator.SetBool("Walking", moving);

		lastPos = transform.position;
	}
}
