using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public static class PopupManager {
	public static Canvas GetMainCanvas()
	{		
		Canvas c = null; //(Canvas)GameObject.FindObjectOfType (typeof(Canvas));
		foreach (Canvas can in GameObject.FindObjectsOfType<Canvas>()) {              
			if (can.gameObject.tag == "MainCanvas") {
                return can;
			}
		}

		GameObject canvas = new GameObject ();
		c = canvas.AddComponent<Canvas> ();
		c.renderMode = RenderMode.ScreenSpaceOverlay;
		return c;
	}

	public static T ShowPopup<T> (string popupType, string title, string text, System.Action<string, BasePopup> OnButtonClick, System.Action OnClose = null) where T : BasePopup
	{
		T p = GameObject.Instantiate (Resources.Load<T> ("UI/" + popupType));

		p.Title.text = title;
        if (p.Content != null)
		    p.Content.text = text;

		p.OnButtonPressed = OnButtonClick;
		p.OnClosed = OnClose;

		Canvas c = GetMainCanvas ();
		p.transform.SetParent (c.transform);
		p.transform.localScale = Vector3.one;
		p.transform.localPosition = Vector3.zero;
		p.Show ();
		return (T)p;
	}

   

	public static BasePopup ShowPopup(string title, string text, System.Action<string, BasePopup> OnButtonClick, System.Action OnClose = null)
	{
		return ShowPopup<BasePopup> ("Popup", title, text, OnButtonClick, OnClose);
	}
}
