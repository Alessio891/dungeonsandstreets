using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OtherPlayerInfoPanel : MonoBehaviour {

	public Text playerName;

	public GameObject tradeButton;

	public OtherPlayers player;
	bool animating = false;
	public bool expanded = false;
	// Use this for initialization
	void Start () {
		Vector3 pos = Camera.main.WorldToScreenPoint (player.transform.position + Vector3.up * 2);
		transform.position = pos;
		tradeButton.SetActive (false);
	}

	void complete()
	{
		animating = false;
	}

	public void Collapse()
	{
		if (animating)
			return;
		animating = true;
		expanded = false;
		tradeButton.SetActive (false);
		RectTransform rT = (transform as RectTransform);
		iTween.ValueTo (gameObject, iTween.Hash ("from", rT.sizeDelta.x, "to", 100, "time", 0.2f, "onupdate", "updateX", "easeType", "easeOutBounce", "oncomplete", "complete"));
		iTween.ValueTo (gameObject, iTween.Hash ("from", rT.sizeDelta.y, "to", 73, "time", 0.2f, "onupdate", "updateY", "easeType", "easeOutBounce"));
	}

	public void Expand()
	{
		if (animating)
			return;
		animating = true;
		expanded = true;
		tradeButton.SetActive (true);
		RectTransform rT = (transform as RectTransform);
		iTween.ValueTo (gameObject, iTween.Hash ("from", rT.sizeDelta.x, "to", 170, "time", 0.2f, "onupdate", "updateX", "easeType", "easeOutBounce", "oncomplete", "complete"));
		iTween.ValueTo (gameObject, iTween.Hash ("from", rT.sizeDelta.y, "to", 123, "time", 0.2f, "onupdate", "updateY", "easeType", "easeOutBounce"));
	}

	void updateX(float v)
	{
		Vector2 s = (transform as RectTransform).sizeDelta;
		s.x = v;
		(transform as RectTransform).sizeDelta = s;
	}
	void updateY(float v)
	{
		Vector2 s = (transform as RectTransform).sizeDelta;
		s.y = v;
		(transform as RectTransform).sizeDelta = s;
	}
	// Update is called once per frame
	void Update () {
		Vector3 pos = Camera.main.WorldToScreenPoint (player.transform.position + Vector3.up * 2);
		transform.position = pos;
	}

	public void Trade()
	{
		Dictionary<string, object> values = new Dictionary<string, object> ();
		values.Add ("user1", PlayerPrefs.GetString ("user"));
		values.Add ("user2", player.nameText.text);
		Bridge.POST (Bridge.url + "Trade/StartTrade", values, (r) => {
			Debug.Log("[START TRADE] " + r);
			ServerResponse resp = new ServerResponse(r);
			if (resp.status == ServerResponse.ResultType.Success)
			{
				TradeUI.instance.tradeId = resp.GetIncomingDictionary()["tradeId"].ToString();
				TradeUI.instance.userName2 = player.nameText.text;
				TradeUI.Trading = true;
				Debug.Log("Started trade, set tradeId to " + TradeUI.instance.tradeId);
				PlayerInfoManager.instance.HideInfo();
				TradeUI.instance.Show ();
			} else
			{
				if (resp.errorCode == "1")
				{
					Debug.Log("player already in trade :(");
				}
			}
		});

	}
}
