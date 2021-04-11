using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SidebarIcon : MonoBehaviour {
	public Vector3 targetPos;
	// Use this for initialization
	void Start () {
		targetPos = transform.position;
		transform.position = SideBar.instance.center.position;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Show()
	{
		iTween.MoveTo (gameObject, targetPos, 0.5f);
		StartCoroutine (alpha (1, 0.1f));
	}
	public void Hide()
	{
		iTween.MoveTo (gameObject, SideBar.instance.center.position, 0.5f);
		StartCoroutine (alpha (0, 0.5f));
	}

	IEnumerator alpha(float amount, float time)
	{
		yield return new WaitForSeconds (time);
		gameObject.GetComponentInChildren<Image> ().color = new Color (1, 1, 1, amount);
	}
}
