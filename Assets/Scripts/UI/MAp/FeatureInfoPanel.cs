using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FeatureInfoPanel : MonoBehaviour {
	public Transform follow;
	public Text mobAmount;
	public Text playersAmount;
	public Text featureName;

	public GameObject mobCounter;
	public GameObject playerCounter;
	public GameObject rarityCounter;
	public GameObject timerCounter;

	public CanvasGroup canvasGroup;

	public long lifeRem;
	public BasicFeature feature;
	bool isContainer = false;

	// Use this for initialization
	void Start () {
		
	}

	public void ShowForNpc(string npcName, string mobCount, string playerCount)
	{
		Vector3 pos = Camera.main.WorldToScreenPoint (follow.position);
		transform.position = pos;

		mobCounter.SetActive (true);
		playerCounter.SetActive (true);

		mobCounter.GetComponentInChildren<Text> ().text = mobCount;
		playerCounter.GetComponentInChildren<Text> ().text = playerCount;

		featureName.text = npcName;
	}


	public void ShowForContainer(string containerName, int rarity, long timer)
	{
		Vector3 pos = Camera.main.WorldToScreenPoint (follow.position);
		transform.position = pos;

		isContainer = true;
		rarityCounter.SetActive (true);
		timerCounter.SetActive (true);
		System.TimeSpan t = System.TimeSpan.FromMilliseconds ((double)timer);

		lifeRem = timer;
		rarityCounter.GetComponentInChildren<Text> ().text = rarity.ToString();
		timerCounter.GetComponentInChildren<Text> ().text = t.Minutes.ToString () + ":" + t.Seconds.ToString ();

		featureName.text = containerName;
	}
	
	// Update is called once per frame
	void Update () {		
		Vector3 pos = Camera.main.WorldToScreenPoint (follow.position);
		transform.position = pos;

		if (isContainer) {
			
			System.TimeSpan t = System.TimeSpan.FromMilliseconds ((double)feature.lifeTime);
			timerCounter.GetComponentInChildren<Text> ().text = t.Minutes.ToString () + ":" + t.Seconds.ToString ();
		}
	}
}
