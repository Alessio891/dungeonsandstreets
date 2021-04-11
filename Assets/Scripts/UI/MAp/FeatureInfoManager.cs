using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatureInfoManager : MonoBehaviour {

	public FeatureInfoPanel infoPanelPrefab;

	public FeatureInfoPanel panelInstance;

	public static FeatureInfoManager instance;

	void Awake() {
		instance = this;
	}

	public void Clear()
	{
		if (panelInstance != null)
			Destroy (panelInstance.gameObject);
		WorldSpawner.instance.currentSelected = null;
	}

	public FeatureInfoPanel ShowInfo(Transform follow, string name, int rarity, long initialTimer, BasicFeature f)
	{
		FeatureInfoPanel p = GameObject.Instantiate<FeatureInfoPanel> (infoPanelPrefab);
		p.transform.SetParent (transform);
		p.transform.localPosition = Vector3.zero;
		p.transform.localScale = Vector3.one;
		p.follow = follow;
		p.ShowForContainer (name, rarity, initialTimer);
		panelInstance = p;
		p.canvasGroup.alpha = 1;
		p.feature = f;        
		return p;
	}

	public FeatureInfoPanel ShowInfo(Transform follow, string name, string mobAmount, string playersAmunt)
	{
		FeatureInfoPanel p = GameObject.Instantiate<FeatureInfoPanel> (infoPanelPrefab);
		p.transform.SetParent (transform);
		p.transform.localScale = Vector3.one;
		p.follow = follow;
		p.ShowForNpc (name, mobAmount, playersAmunt);
		panelInstance = p;
		p.canvasGroup.alpha = 1;
		return p;
	}

}
