using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DockBar : MonoBehaviour {

    public Text distanceText;
    public Image distanceFillImg;
	public Text hpText;
	public Text goldText;
	public int gold
	{
		get { return PlayerStats.instance.gold; }
		set { PlayerStats.instance.gold = value; }
	}

	public Text playerName;
	public Text playerClass;
	public Text playerLevel;

	public Image hpFill;

	public Image fillXpBar;

	public static DockBar instance;

	void Awake() { instance = this; }

	// Use this for initialization
	IEnumerator Start () {
		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();
		//GetGoldFromServer ();
		playerName.text = PlayerPrefs.GetString ("user");
		playerLevel.text = PlayerStats.instance.level.ToString ();
	}
	
	// Update is called once per frame
	void Update () {		
		
	}

	public void UpdateGold(int newAmount)
	{
		gold = newAmount;
		goldText.text = newAmount.ToString ();
	}

	public void setXpBarValue(int xp)
	{
		int nextxp = PlayerStats.instance.XPNeededForNextLevel ();
		float perc = (float)((float)xp / (float)nextxp);
		fillXpBar.fillAmount = perc;
	}

	public void GetGoldFromServer()
	{
		Bridge.GET (Bridge.url + "GetStats?userName=" + PlayerPrefs.GetString ("user"), (r) => {
			ServerResponse resp = new ServerResponse(r);
			Debug.Log(r);
			if (resp.status == ServerResponse.ResultType.Success)
			{
				goldText.text = resp.GetIncomingDictionary()["Gold"].ToString();
				int g = 0;
				int.TryParse(goldText.text, out g);
				gold = g;
			}
		});
	}


}
