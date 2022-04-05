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

    public static UICharController instance;

    public CanvasGroup InfoBox;
    public Text InfoText;

    private void Awake()
    {
        instance = this;
    }

    public void ShowInfo(string text, Vector2 pos)
    {
        InfoBox.alpha = 1.0f;
        InfoText.text = text;
        InfoBox.transform.position = pos+new Vector2(0,1);
    }

    public void HideInfo()
    {
        InfoBox.alpha = 0;
    }

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
        HideInfo();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
