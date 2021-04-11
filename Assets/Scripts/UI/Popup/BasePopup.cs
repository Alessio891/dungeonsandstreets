using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasePopup : MonoBehaviour {

	public Text Title;
	public Text Content;

	CanvasGroup canvasGroup;

	public System.Action<string, BasePopup> OnButtonPressed = (s, p) => {
	};
	public System.Action OnClosed = () => {};

	void Awake() {
		canvasGroup = GetComponent<CanvasGroup> ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public virtual void Show()
	{}

	public void ButtonClick(string buttonId)
	{
		OnButtonPressed (buttonId, this);
	}

	public void Close()
	{
		if (OnClosed != null)
			OnClosed ();
		Destroy (gameObject);
	}
}
