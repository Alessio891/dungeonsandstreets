using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Examples.LocationProvider;
using Mapbox.Unity.Location;
using Mapbox.Utils;
using UnityEngine.SceneManagement;
using Mapbox.Examples;
using Mapbox.Unity.Map;
using System.Globalization;

public class PlayerServerSync : MonoBehaviour {

	PositionWithLocationProvider p;
	public ILocationProvider playerLocation { get { return LocationProviderFactory.Instance.DefaultLocationProvider; } }

	public event System.Action<Dictionary<string, object>> OnPlayerStatsUpdate;
	public event System.Action<List<Dictionary<string, object>>> OnInventoryUpdate;
	public event System.Action<Dictionary<string, object>> OnEquipUpdate;
	public event System.Action<Dictionary<string, object>> OnPositionUpdate;
	public event System.Action<Dictionary<string, object>> OnFeaturesUpdate;
    public event System.Action<List<string>> OnSkillsUpdate;
	public event System.Action<string> OnTradeRequest;
    public event System.Action<List<Dictionary<string, object>>> OnQuestUpdate;
    public event System.Action<List<object>> OnCompletedQuests;
    public event System.Action<List<object>> OnProfessionsUpdate;

	public string activeTradeId;

    public string TEST_BIOME = "";

    public Dictionary<string, List<int>> claimedDialogues = new Dictionary<string, List<int>>();

    bool syncingStats = false;
	public static PlayerServerSync instance;

	public float SyncEvery = 4;
	float timer = 0;


	void Awake() { instance = this; }

	// Use this for initialization
	void Start () {
		p = GetComponent<PositionWithLocationProvider> ();
		timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (SceneManager.GetActiveScene ().name == "Map") {
            if (!GameManager.instance.CommunicationWithServerEnstabilished)
                return;
			timer += Time.deltaTime;
			if (timer > SyncEvery) {
				Dictionary<string, object> values = new Dictionary<string, object> ();
				values.Add ("token", PlayerPrefs.GetString ("user"));
                if (p._useTransformLocationProvider)
                {
                    Vector2d v = AbstractMap.instance.WorldToGeoPosition(p.moveWithTransform.position);
                    values.Add("x", v.x.ToStringEx());
                    values.Add("y", v.y.ToStringEx());
                }
                else
                {
                    values.Add("x", playerLocation.CurrentLocation.LatitudeLongitude.x.ToStringEx());
                    values.Add("y", playerLocation.CurrentLocation.LatitudeLongitude.y.ToStringEx());
                }
                
                Debug.Log("Distance sent: " + p.distanceMade + " - " + p.distanceMade.ToString(CultureInfo.InvariantCulture));
                if (p.distanceMade > 500)
                    p.distanceMade = 0;
				values.Add ("distance", p.distanceMade.ToString(CultureInfo.InvariantCulture));
				Bridge.POST (Bridge.url + "UpdatePosition", values, HandleUpdateResponse);
				timer = 0;
			}
		}
	}


	void HandleUpdateResponse(string response)
	{
		Debug.Log ("[Update Response] " + response);
		ServerResponse r = new ServerResponse (response);
        if (r.status == ServerResponse.ResultType.Success)
        {
            List<Dictionary<string, object>> data = r.GetIncomingList();

            foreach (Dictionary<string, object> incomingData in data)
            {

                if (incomingData.ContainsKey("dist"))
                {
                    if (DockBar.instance != null)
                    {

                        double distance = double.Parse(incomingData["dist"].ToString(), CultureInfo.InvariantCulture);
                        double guaranteed = double.Parse(Registry.assets.ServerConfig.Get("guaranteedDistanceSpawn"), CultureInfo.InvariantCulture);
                        double perc = System.Math.Truncate((float)((float)distance / (float)guaranteed) * 100);
                        //System.Math.Truncate(distance)
                        DockBar.instance.distanceText.text = perc.ToString();
                        DockBar.instance.distanceFillImg.fillAmount = (float)((float)distance /(float)guaranteed);
                    }
                    ProcessNearbyPlayers((List<object>)incomingData["near"]);
                }
                else if (incomingData.ContainsKey("offline"))
                {
                    ProcessOfflinePlayers(incomingData["offline"].ToString());
                }
                else if (incomingData.ContainsKey("tradeId"))
                {
                    string tradeId = incomingData["tradeId"].ToString();
                    if (!string.IsNullOrEmpty(tradeId))
                    {
                        if (!TradeUI.Trading)
                        {
                            if (OnTradeRequest != null)
                                OnTradeRequest(tradeId);
                        }
                    }
                } else if (incomingData.ContainsKey("biome"))
                {
                    TEST_BIOME = incomingData["biome"].ToString();
                }
            }
        }
        else
        {
            if (r.errorCode == "600")
            {
                Bridge.DispatchError("You have been inactive for too much time. Check your internet connection.", true);
            } else
                Bridge.DispatchError("Error updating with server! Try logging in again.", true);
        }
	}

	void ProcessOfflinePlayers(string data)
	{
		string[] split = data.Split (',');
		for (int i = 0; i < split.Length; i++) {
			WorldSpawner.instance.RemovePlayer (split [i]);
		}
	}

	void ProcessNearbyPlayers(List<object> data)
	{
       // Debug.Log("Processing nearby players " + data.Count);
		foreach (object o in data) {
			Dictionary<string, object> player = (Dictionary<string, object>)o;

			double lat, lon;
            lat = double.Parse(player["posX"].ToString(), CultureInfo.InvariantCulture);
            lon = double.Parse(player["posY"].ToString(), CultureInfo.InvariantCulture);            
			WorldSpawner.instance.SpawnOrUpdatePlayer (player["userName"].ToString(), lat, lon);
		}						
	}

    public void SyncStatsHandler(ServerResponse resp, System.Action onEnd = null)
    {
        if (resp.status == ServerResponse.ResultType.Success)
        {
            if (OnPlayerStatsUpdate != null)
                OnPlayerStatsUpdate(resp.GetIncomingDictionary());
            if (OnEquipUpdate != null)
                OnEquipUpdate((Dictionary<string, object>)resp.GetIncomingDictionary()["Equip"]);
            if (OnInventoryUpdate != null)
            {
                List<Dictionary<string, object>> invData = new List<Dictionary<string, object>>();
                List<object> rawInvData = (List<object>)resp.GetIncomingDictionary()["inventoryData"];
                foreach (object o in rawInvData)
                {
                    Dictionary<string, object> casted = (Dictionary<string, object>)o;
                    //
                    invData.Add(casted);
                }
                OnInventoryUpdate(invData);
            }
            if (OnSkillsUpdate != null)
            {

                OnSkillsUpdate((List<string>)resp.GetIncomingDictionary()["KnownSkills"]);
            }
            if (OnQuestUpdate != null)
            {
                List<Dictionary<string, object>> questData = new List<Dictionary<string, object>>();
                List<object> quests = (List<object>)resp.GetIncomingDictionary()["Quests"];
                foreach (object o in quests)
                {
                    Dictionary<string, object> casted = (Dictionary<string, object>)o;
                    questData.Add(casted);
                }
                OnQuestUpdate(questData);
            }

            if (OnCompletedQuests != null)
            {
                OnCompletedQuests((List<object>)resp.GetIncomingDictionary()["CompletedQuests"]);
            }

            if (resp.GetIncomingDictionary().ContainsKey("Professions"))
            {
                if (OnProfessionsUpdate != null)
                    OnProfessionsUpdate((List<object>)resp.GetIncomingDictionary()["Professions"]);
            }

            Dictionary<string, object> claimedDialogue = (Dictionary<string, object>)resp.GetIncomingDictionary()["ClaimedDialogueRewards"];
            claimedDialogues.Clear();
            foreach (KeyValuePair<string, object> c in claimedDialogue)
            {
                List<object> val = (List<object>)c.Value;
                List<int> new_val = new List<int>();
                foreach (object o in val)
                    new_val.Add(int.Parse(o.ToString()));
                claimedDialogues.Add(c.Key, new_val);
            }

            if (resp.GetIncomingDictionary().ContainsKey("LevelUP"))
            {
                int levelUP = int.Parse(resp.GetIncomingDictionary()["LevelUP"].ToString());
                if (levelUP > 0 && !UILevelUp.instance.Active)
                {
                    PlayerStats.instance.LevelUP();
                }
            }

        }
        else
        {
            Bridge.DispatchError("Something went wrong with the server. Please login again.", true);
        }
        if (onEnd != null)
            onEnd();
        syncingStats = false;
    }

	public void SyncStats(System.Action onEnd = null)
	{
        if (syncingStats)
            return;
        syncingStats = true;
		Bridge.GET (Bridge.url + "GetStats?userName=" + PlayerPrefs.GetString("user"), (s) => {
			Debug.Log("[SyncStats] " + s);
			ServerResponse resp = new ServerResponse(s);
            SyncStatsHandler(resp, onEnd);
		});	
	}

    public void SyncInventory()
    {
        Bridge.GET(Bridge.url + "GetStats?userName=" + PlayerPrefs.GetString("user"), (s) =>
        {
            Debug.Log("[SyncStats] " + s);
            ServerResponse resp = new ServerResponse(s);
            if (resp.status == ServerResponse.ResultType.Success)
            {
                if (OnInventoryUpdate != null)
                {
                    List<Dictionary<string, object>> invData = new List<Dictionary<string, object>>();
                    List<object> rawInvData = (List<object>)resp.GetIncomingDictionary()["inventoryData"];
                    foreach (object o in rawInvData)
                    {
                        Dictionary<string, object> casted = (Dictionary<string, object>)o;
                        invData.Add(casted);
                    }
                    OnInventoryUpdate(invData);
                }

            }
        });
        }

}
