using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TradeUI : MonoBehaviour {

	public CanvasGroup canvasGroup;
    public Text Player1Name;
    public Text Player2Name;
	public string userName1 { get { return PlayerPrefs.GetString ("user"); } }
	public string userName2;

	public InputField goldAmount;
	public Text player2GoldAmount;

	public GameObject Player1WaitingMask;
	public GameObject Player2WaitingMask;
	public Text Player2StateText;

	public List<Dictionary<string, int>> itemsToTrade = new List<Dictionary<string, int>>();

	public string tradeId;
	public string tradeState { get { 
			return (tradeData != null && tradeData.ContainsKey("state")) ? tradeData["state"].ToString() : "N\\A";
		} }

	public GridLayoutGroup player1Grid;
	public GridLayoutGroup player2Grid;

	List<TradeItemEntry> player1Entries;
	List<TradeItemEntry> player2Entries;

	public GameObject confirmButton;
	public GameObject acceptButton;

	Dictionary<string, object> tradeData;

	public static bool Trading = false;

	float updateTimer = 0;

	public TradeItemEntry entryPrefab;

	public static TradeUI instance;
	void Awake() { instance = this; }

	void Start () {
		Hide ();
		PlayerServerSync.instance.OnTradeRequest += TradeRequest;
	}

	public void CancelTrade()
	{

		bool confirmed = false;
		bool otherConfirmed = false;
		bool finalized = false;
		bool otherFinalized = false;



		if (tradeData ["user1"].ToString () == userName1) {
			finalized = (bool)tradeData ["user1Finalized"];
			otherFinalized = (bool)tradeData ["user2Finalized"];
			confirmed = (bool)tradeData ["user1Confirmed"];
			otherConfirmed = (bool)tradeData ["user2Confirmed"];
		} else {
			finalized = (bool)tradeData ["user2Finalized"];
			otherFinalized = (bool)tradeData ["user1Finalized"];
			confirmed = (bool)tradeData ["user2Confirmed"];
			otherConfirmed = (bool)tradeData ["user1Confirmed"];
		}

		if (finalized) {
			Dictionary<string, object> values = new Dictionary<string, object> ();
			values.Add ("user", userName1);
			values.Add ("tradeId", tradeId);
			Bridge.POST (Bridge.url + "Trade/CancelFinalize", values, (r) => {
				acceptButton.GetComponent<Button> ().interactable = true;
				Player1WaitingMask.SetActive (false);
				acceptButton.SetActive(false);
				confirmButton.SetActive(true);
			});
		} else if (confirmed) {
			Dictionary<string, object> values = new Dictionary<string, object> ();
			values.Add ("user", userName1);
			values.Add ("tradeId", tradeId);
			Bridge.POST (Bridge.url + "Trade/CancelTradedItems", values, (r) => {
				Debug.Log ("[CANCEL TRADED ITEMS] " + r);
				Player1WaitingMask.SetActive (false);
				confirmButton.SetActive (true);
				acceptButton.SetActive (false);
			});
		} else {
			Dictionary<string, object> values = new Dictionary<string, object> ();
			values.Add ("user", userName1);
			values.Add ("tradeId", tradeId);
			Bridge.POST (Bridge.url + "Trade/CancelTrade", values, (r) => {
				Debug.Log ("[CANCEL TRADE] " + r);
				ServerResponse resp = new ServerResponse (r);
				tradeId = "";
				Hide ();
				Trading = false;
			});
		}			
	}

	public void TradeRequest(string tradeId)
	{
		this.tradeId = tradeId;
		PopupManager.ShowPopup ("Trade", "Someone wants to trade!", (s, p) => {
			Dictionary<string, object> values = new Dictionary<string, object>();
			values.Add("tradeId", tradeId);
			Bridge.POST(Bridge.url+"Trade/AcceptTrade", values, (r) => {
				Debug.Log("[ACCEPT TRADE] " + r);
				ServerResponse resp = new ServerResponse(r);
				if (resp.status == ServerResponse.ResultType.Success)
				{
					tradeData = resp.GetIncomingDictionary();
					Trading = true;
					PlayerServerSync.instance.activeTradeId = tradeData["tradeId"].ToString();
					userName2 = tradeData["user1"].ToString();
                    Player1Name.text = userName1;
                    Player2Name.text = userName2;
					Destroy(p.gameObject);
					Show();
				}
			});
		}, () => {
			CancelTrade();
		});
	}

	public void ConfirmItems()
	{
		Dictionary<string, object> values = new Dictionary<string, object> ();
		values.Add ("user", userName1);
		values.Add ("tradeId", tradeId);
		values.Add ("gold", goldAmount.text);
		values.Add ("itemList", MiniJSON.Json.Serialize (TradeUI.instance.itemsToTrade));
		Bridge.POST (Bridge.url + "Trade/ConfirmTradedItems", values, (r) => {
			Debug.Log("[CONFIRM ITEMS] " + r);
			ServerResponse resp = new ServerResponse(r);
			if (resp.status == ServerResponse.ResultType.Success)
			{
				Player1WaitingMask.SetActive(true);
				confirmButton.SetActive(false);
				acceptButton.SetActive(true);
			}
		});
	}

	// Update is called once per frame
	void Update () {
		if (canvasGroup.alpha <= 0)
			return;
		updateTimer += Time.deltaTime;
		if (updateTimer > 3) {
			updateTimer = 0;
			Bridge.GET (Bridge.url + "Trade/UpdateTrade?tradeId=" + tradeId, (r) => {
				Debug.Log("[UPDATE TRADE] " + r);
				ServerResponse resp = new ServerResponse(r);
				if (resp.status == ServerResponse.ResultType.Success)
				{					
					tradeData = resp.GetIncomingDictionary();
					List<object> otherPlayerItems = null;
					bool otherPlayerConfirmed = false;
					bool confirmed = false;
					if (tradeData["user1"].ToString() != userName1)
					{
						if ((bool)tradeData["user1Confirmed"])
						{
							Debug.Log("OTHER PLAYER CONFIRM!");
							otherPlayerConfirmed = true;
							otherPlayerItems = (List<object>)tradeData["user1TradedItems"];
							player2GoldAmount.text = tradeData["player1GoldTrade"].ToString();
						}

						if ((bool)tradeData["user2Confirmed"])
							confirmed = true;
					} else if (tradeData["user2"].ToString() != userName1)
					{
						if ((bool)tradeData["user2Confirmed"])
						{
							otherPlayerConfirmed = true;
							Debug.Log("OTHER PLAYER CONFIRM!");
							otherPlayerItems = (List<object>)tradeData["user2TradedItems"];
							player2GoldAmount.text = tradeData["player2GoldTrade"].ToString();
						}
						if ((bool)tradeData["user1Confirmed"])
							confirmed = true;
					}	

					if ((bool)tradeData["user2Accepted"])
					{
						if (!confirmed && Player1WaitingMask.activeSelf)
							Player1WaitingMask.SetActive(false);
						Player2WaitingMask.SetActive(true);
						Player2StateText.text = userName2 + " is selecting his trade...";
					}
					else
					{
						
						if (confirmed && !Player1WaitingMask.activeSelf)
							Player1WaitingMask.SetActive(true);
						Player2WaitingMask.SetActive(true);
						Player2StateText.text = "Waiting for " + userName2 + " to accept...";
					}

					if (!otherPlayerConfirmed)
					{
						
						//Player1WaitingMask.SetActive(false);
					} else
					{
						Player2WaitingMask.SetActive(false);
						foreach(Transform t in player2Grid.GetComponentsInChildren<Transform>())
						{
							if (t.gameObject == player2Grid.gameObject)
								continue;
							Destroy(t.gameObject);
						}
						if (player2Entries == null)
							player2Entries = new List<TradeItemEntry> ();
						player2Entries.Clear();

						foreach(object o in otherPlayerItems)
						{
							Dictionary<string, object> playerData = (Dictionary<string, object>)o;
							foreach(KeyValuePair<string, object> pair in playerData)
							{
								TradeItemEntry e = GameObject.Instantiate<TradeItemEntry> (entryPrefab);
								e.transform.SetParent (player2Grid.transform);
								e.transform.localScale = Vector3.one;
								string itemId = pair.Key;
								BaseItem i = Resources.Load<BaseItem> (Registry.assets.items [itemId]);
								e.item = i;
								e.amount = 0;
								e.IsOtherPlayer = true;
								int.TryParse (pair.Value.ToString (), out e.amount);
								e.Refresh ();
								player2Entries.Add(e);
							}
						}
					}
                    Player1Name.text = userName1;
                    Player2Name.text = userName2;
                } else
				{
					Debug.Log("Trade canceled? Closing");
					Hide();
				}

			});
		}
	}

	public void LoadItems()
	{
		PlayerServerSync.instance.OnInventoryUpdate += OnItemsLoaded;	
		PlayerServerSync.instance.SyncStats ();
	}

	void OnItemsLoaded(List<Dictionary<string, object>> data)
	{

		if (player1Entries == null)
			player1Entries = new List<TradeItemEntry> ();

		foreach (TradeItemEntry e in player1Entries) {
			Destroy (e.gameObject);
		}
		player1Entries.Clear ();
		goldAmount.text = "0";
        foreach (Dictionary<string, object> itemData in data)
        {
            TradeItemEntry e = GameObject.Instantiate<TradeItemEntry>(entryPrefab);
            e.transform.SetParent(player1Grid.transform);
            e.transform.localScale = Vector3.one;
            string itemId = itemData["itemUID"].ToString();
            BaseItem i = Resources.Load<BaseItem>(Registry.assets.items[itemId]);
            e.item = i;
            e.amount = 0;
            int.TryParse(itemData["amount"].ToString(), out e.amount);
            e.Refresh();
            player1Entries.Add(e);
        }
		
		PlayerServerSync.instance.OnInventoryUpdate -= OnItemsLoaded;
	}

	public void FinalizeTrade()
	{
		Dictionary<string, object> values = new Dictionary<string, object> ();
		values.Add ("user", userName1);
		values.Add ("tradeId", tradeId);
		Bridge.POST (Bridge.url + "Trade/FinalizeTrade", values, (r) => {
			Debug.Log("[FINALIZE] " + r);
			ServerResponse resp = new ServerResponse(r);
			if (resp.status == ServerResponse.ResultType.Success)
			{
				Debug.Log("Finalized!");
				PlayerServerSync.instance.SyncStats();
				tradeId = "";
				Hide();
			} else
			{
				Debug.Log("Wait other player...");
				acceptButton.GetComponent<Button>().interactable = false;
			}

		});
	}

	public void Show()
	{
		canvasGroup.alpha = 1;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;	
		itemsToTrade.Clear ();
		confirmButton.SetActive (true);
		acceptButton.SetActive(false);
		player2GoldAmount.text = "0";
		if (player2Entries != null) {
			foreach (TradeItemEntry e in player2Entries) {
				Destroy (e.gameObject);
			}
			player2Entries.Clear ();
		}
		Player1WaitingMask.SetActive (true);
		LoadItems ();
	}
	public void Hide()
	{
		canvasGroup.alpha = 0;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		Trading = false;
        acceptButton.GetComponent<Button>().interactable = true;
    }
}
