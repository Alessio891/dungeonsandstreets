using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherPlayers : MonoBehaviour {
	public Text nameText;
    public Animator animator;
    Vector3 lastPos;
	// Use this for initialization
	void Start () {
        lastPos = transform.position;
	}

    void LateUpdate()
    {
        float distance = Vector3.Distance(lastPos, transform.position);

        float speed = distance / Time.deltaTime;

        animator.SetFloat("speed", speed);

        lastPos = transform.position;
    }

    // Update is called once per frame
    void Update () {
		if (ClickedOnThis ()) {
			PlayerInfoManager.instance.ShowInfo (this);
		}	
	}

	public bool ClickedOnThis()
	{
		Ray r = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (GetComponent<Collider> ().Raycast (r, out hit, 1000.0f) && Input.GetMouseButtonDown (0)) {
			if (hit.collider.gameObject == gameObject) {
				return true;
			}
		}
		return false;
	}

}
