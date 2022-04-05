using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIStatCreationEntry : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	public string statKey;
    [TextArea(3,6)]
    public string Description;

	public Text amount;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Init()
    {
        Debug.Log(statKey + " is " + UIStatCreation.instance.stats[statKey]);
        amount.text = UIStatCreation.instance.stats[statKey].ToString();    
    }

	public void Increase()
	{
		if (UIStatCreation.instance.availablePoints > 0) {
			int am = 0;
			int.TryParse (amount.text, out am);
			UIStatCreation.instance.availablePoints--;
			//UIStatCreation.instance.stats [statKey] = ++am;
			am++;
			UIStatCreation.instance.UpdateStat (statKey, am);
			amount.text = am.ToString ();
		}
	}
	public void Decrease()
	{
		int am = 0;
        int min = 1;
		int.TryParse (amount.text, out am);
        int.TryParse(UIStatCreation.instance.minStats[statKey].ToString(), out min);
		if (am  > min) {
			UIStatCreation.instance.availablePoints++;
			am--;
			UIStatCreation.instance.UpdateStat (statKey, am);
			amount.text = am.ToString ();
		}
	}

    public void OnPointerUp(PointerEventData eventData)
    {
        UICharController.instance.HideInfo();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UICharController.instance.ShowInfo(Description, eventData.position);
    }
}
