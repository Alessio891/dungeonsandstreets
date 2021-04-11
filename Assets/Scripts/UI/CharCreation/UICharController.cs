using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UICharController : MonoBehaviour {

	public int currentStep = 0;

	public CanvasGroup[] steps;
	public string[] stepsButtonName;

	public Text buttonText;

	public void NextStep()
	{
		if (currentStep + 1 < steps.Length) {
			steps [currentStep].alpha = 0;
			currentStep++;
			steps [currentStep].alpha = 1;
			buttonText.text = stepsButtonName [currentStep];
		} else {
			if (UIStatCreation.instance.availablePoints <= 0) {
				Dictionary<string, object> values = new Dictionary<string, object> ();
				values.Add ("userName", PlayerPrefs.GetString ("user"));
				values.Add ("Strength", UIStatCreation.instance.stats ["Strength"]);
				values.Add ("Dexterity", UIStatCreation.instance.stats ["Dexterity"]);
				values.Add ("Intelligence", UIStatCreation.instance.stats ["Intelligence"]);
				values.Add ("Luck", UIStatCreation.instance.stats ["Luck"]);
				values.Add ("Toughness", UIStatCreation.instance.stats ["Toughness"]);
				values.Add ("MaxHp", UIStatCreation.instance.stats ["MaxHp"]);
				values.Add ("hp", UIStatCreation.instance.stats ["MaxHp"]);
				Bridge.POST (Bridge.url + "UpdateStats", values,
					(r) => {
						SceneManager.LoadScene ("Map");
					});
			}
		}
	}

	// Use this for initialization
	void Start () {
		foreach (CanvasGroup g in steps)
			g.alpha = 0;
		steps [0].alpha = 1;
		buttonText.text = stepsButtonName [currentStep];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
