using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OtherPlayerCombat : MonoBehaviour {
	public Image fillImage;
	public Text nameText;
	public Image bg;
    public string user;   

    void Start()
    {
        GetComponentInChildren<FloatingTextSpawner>().ID = user;
        nameText.text = user;
    }

	public void SetFill(float perc)
	{
		fillImage.fillAmount = perc;
	}
}
