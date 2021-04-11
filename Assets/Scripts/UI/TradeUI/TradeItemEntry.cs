using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class TradeItemEntry : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	public bool IsOtherPlayer = false;

	public Image normalBg;
	public Image chosenBg;

	public Image sprite;
	public Text amountText;

	public BaseItem item;
	public int amount;

	public bool Chosen = false;
	public int chosenAmount = 0;
	bool touched = false;
	float touchTimer = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (touched) {
			touchTimer += Time.deltaTime;
			if (touchTimer > 1.0f && touchTimer < 10.0f) {
				UnSelect ();
				touchTimer = 10.0f;
			}
		} else
			touchTimer = 0;
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		if (IsOtherPlayer)
			return;
		touched = true;
	}
	public void OnPointerUp (PointerEventData eventData)
	{
		touched = false;
		if (touchTimer < 1.0f) {
			Select ();
		}
	}

	public void UnSelect()
	{
		if (TradeUI.instance.tradeState != "choosing_items") {
			return;
		}	
		if (Chosen) {
			chosenAmount--;
			if (chosenAmount <= 0) {
				Chosen = false;
				chosenBg.enabled = false;
				normalBg.enabled = true;
				for (int i = 0; i < TradeUI.instance.itemsToTrade.Count; i++) {
					if (TradeUI.instance.itemsToTrade [i] .ContainsKey(item.UID)) {
						TradeUI.instance.itemsToTrade.RemoveAt (i);
						break;
					}
				}
			}
			amountText.text = chosenAmount.ToString () + "/" + amount.ToString ();
		}
	}

	public void Select()
	{		
		if (TradeUI.instance.tradeState != "choosing_items") {
			return;
		}
        SoundManager.instance.ItemSelectionSound();
		if (Chosen) {
            
			chosenAmount++;
			if (chosenAmount > amount)
				chosenAmount = amount;
			amountText.text = chosenAmount.ToString () + "/" + amount.ToString ();
			for (int i = 0; i < TradeUI.instance.itemsToTrade.Count; i++) {
				if (TradeUI.instance.itemsToTrade [i].ContainsKey(item.UID)) {
					TradeUI.instance.itemsToTrade [i] [item.UID] = chosenAmount;
					break;
				}
			}
		} else {
			Chosen = true;
			chosenAmount = 1;
			chosenBg.enabled = true;
			normalBg.enabled = false;
			amountText.text = chosenAmount.ToString () + "/" + amount.ToString ();
			TradeUI.instance.itemsToTrade.Add (new Dictionary<string, int> () { { item.UID, 1 } });
		}
	}

	public void Refresh() {
		sprite.sprite = item.sprite;
		if (!IsOtherPlayer)
			amountText.text = chosenAmount.ToString () + "/" + amount.ToString ();
		else
			amountText.text = amount.ToString ();
	}
}
