using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationCenter : MonoBehaviour {

	public VerticalLayoutGroup layout;
	public Transform entryRoot;

	public GameObject layoutElementPrefab;
	public NotificationEntry entryPrefab;

	public static NotificationCenter instance;

	void Awake() {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddNotification(string text, Sprite img = null)
	{
		GameObject layoutElement = GameObject.Instantiate<GameObject> (layoutElementPrefab);
		layoutElement.transform.SetParent (layout.transform);
		layoutElement.transform.localPosition = Vector3.zero;
		layoutElement.transform.localScale = Vector3.one;
		layoutElement.transform.SetAsFirstSibling ();


		NotificationEntry e = GameObject.Instantiate<NotificationEntry> (entryPrefab);
		e.transform.SetParent (entryRoot);
		e.transform.position = layoutElement.transform.position;
		e.transform.localScale = Vector3.one;
		//(e.transform as RectTransform).sizeDelta = (layutElement.transform as RectTransform).sizeDelta;
		e.text.text = text;
		e.sprite.sprite = img;
		e.GetComponent<LayoutSmoothFollow> ().followTransform = layoutElement.transform;
	}
}
