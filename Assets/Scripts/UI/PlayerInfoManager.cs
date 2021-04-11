using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoManager : MonoBehaviour {

	public OtherPlayerInfoPanel panelPrefab;
	public OtherPlayerInfoPanel panelInstance;


	public static PlayerInfoManager instance;

	public string selectedPlayerName = "";

	float timer = 0;

	void Awake() {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator destroyAfterTimer()
	{
		while (timer < 20) {
			timer += Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}
		if (panelInstance != null)
			Destroy (panelInstance.gameObject);
		timer = 0;

	}

	public void ShowInfo(OtherPlayers player)
	{
		timer = 0;
		if (panelInstance == null) {
			panelInstance = GameObject.Instantiate<OtherPlayerInfoPanel> (panelPrefab);
			panelInstance.transform.SetParent (transform);
			panelInstance.transform.localScale = Vector3.one;
			panelInstance.player = player;
			StartCoroutine (destroyAfterTimer ());
		}
		if (selectedPlayerName == player.nameText.text) {
			if (panelInstance.expanded)
				panelInstance.Collapse ();
			else
				panelInstance.Expand ();
		} else {
			panelInstance.player = player;
			panelInstance.playerName.text = player.nameText.text;
			selectedPlayerName = player.nameText.text;
		}
	}
	public void HideInfo()
	{
		if (panelInstance != null) {
			Destroy (panelInstance.gameObject);
		}
	}
}
