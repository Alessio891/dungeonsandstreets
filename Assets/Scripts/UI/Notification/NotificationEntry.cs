using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationEntry : MonoBehaviour {
	public Text text;
	public Image sprite;
	float WaitBeforeDestroy = 5;

	void Start()
	{
		StartCoroutine (destroyThis ());
		if (sprite.sprite == null)
			sprite.enabled = false;
	}
	IEnumerator destroyThis()
	{
		yield return new WaitForSeconds (WaitBeforeDestroy);
		Destroy (GetComponent<LayoutSmoothFollow> ().followTransform.gameObject);
		Destroy (gameObject);
	}
}
