using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingMask : MonoBehaviour {

	public static LoadingMask instance;
	public Text loadingText;

	void Awake()
	{
		instance = this;
		Hide ();
	}

	public Image loadingImage;
	public CanvasGroup group;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetProgress(float progress)
	{
		var rT = loadingImage.rectTransform;
		rT.sizeDelta = new Vector2 (progress * 180, rT.sizeDelta.y);
	}
	public void Hide()
	{
		group.alpha = 0;
		group.blocksRaycasts = false;
		group.interactable = false;
	}
	public void Show(string t = "Loading...")
	{
		group.alpha = 1;
		group.blocksRaycasts = true;
		group.interactable = true;
		loadingText.text = t;
	}

}
