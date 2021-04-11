using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Examples.LocationProvider;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;
using Mapbox.Unity.MeshGeneration;
using Mapbox.Examples;
using Mapbox.Unity.Map;
using Mapbox.Unity.Location;
using System.Globalization;

public class WorldSpawner : MonoBehaviour {

	public BasicFeature currentSelected = null;

	public GameObject otherPlayerPrefab;

	public PositionWithLocationProvider playerLocator;

	Dictionary<string, GameObject> spawnedSpots = new Dictionary<string, GameObject>();
	Dictionary<string, GameObject> spawnedFeatures = new Dictionary<string, GameObject>();
	Dictionary<string, GameObject> spawnedPlayers = new Dictionary<string, GameObject>();
	Dictionary<string, Vector3> spawnedPlayersTargetPos = new Dictionary<string, Vector3>();

	public static WorldSpawner instance;

	float timer = 0;

	void Awake() {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer > 4) {
			timer = 0;
			Bridge.GET (Bridge.url + "Shop/GetShopsNearby?posX=" + playerLocator.LocationProvider.CurrentLocation.LatitudeLongitude.x.ToStringEx () + "&posY=" + playerLocator.LocationProvider.CurrentLocation.LatitudeLongitude.y.ToStringEx (),
				(r) => {
					Debug.Log("[UPDATE SHOPS] " + r);

					ServerResponse resp = new ServerResponse(r);
					if (resp.status == ServerResponse.ResultType.Success)
					{
						List<Dictionary<string, object>> shops = resp.GetIncomingList();
                        List<string> toRemove = new List<string>();
                        foreach (KeyValuePair<string, GameObject> pair in spawnedSpots)
                        {
                            bool present = false;
                            foreach (Dictionary<string, object> shop in shops)
                            {
                                string uid = shop["UID"].ToString();
                                
                                if (uid == pair.Key)
                                {
                                    present = true;
                                    break;
                                }
                            }
                            if (!present)
                            {
                                toRemove.Add(pair.Key);
                            }
                        }

                        foreach (string s in toRemove)
                        {
                            Destroy(spawnedSpots[s]);
                            spawnedSpots.Remove(s);
                        }

						foreach(Dictionary<string, object> shop in shops)
						{
                            string uid = shop["UID"].ToString();
                            double posX = 0; double posY = 0;
                            posX = (double)shop["posX"];// double.Parse(shop["posX"].ToString(), CultureInfo.InvariantCulture);
                            posY = (double)shop["posY"]; //double.Parse(shop["posY"].ToString(), CultureInfo.InvariantCulture);

                            if (!shop.ContainsKey("items"))
                            {
                                Debug.Log("Found a POI instead of a shop. Skipping for now");
                                string poiId = shop["poiTemplateId"].ToString();
                                MapPOI poi = Registry.assets.pois[poiId];
                                if (poi != null)
                                {
                                    string key = uid;
                                    if (!spawnedSpots.ContainsKey(key))
                                    {
                                        MapPOIComponent poiPrefab = GameObject.Instantiate<MapPOIComponent>(poi.prefab);
                                        poiPrefab.posX = (double)shop["posX"];
                                        poiPrefab.posY = (double)shop["posY"];
                                        poiPrefab.poiUID = uid;
                                        Vector3 pos = Conversions.GeoToWorldPosition(posX, posY,
                                            LocationProviderFactory.Instance.mapManager.CenterMercator, LocationProviderFactory.Instance.mapManager.WorldRelativeScale).ToVector3xz();
                                        pos.y = 0;
                                        poiPrefab.transform.position = pos;

                                        spawnedSpots.Add(key, poiPrefab.gameObject);
                                    }
                                }
                            }
                            else
                            {
                                string shopId = shop["shopTemplateId"].ToString();
                                BaseShop shopData = Registry.assets.shops[shopId];
                                if (shopData != null)
                                {
                                    string key = uid;

                                    if (!spawnedSpots.ContainsKey(key))
                                    {
                                        //Debug.Log("Spawning");
                                        ShopComponent shopPrefab = GameObject.Instantiate<ShopComponent>(shopData.prefab as ShopComponent);
                                        shopPrefab.posX = (double)shop["posX"];
                                        shopPrefab.posY = (double)shop["posY"];
                                        shopPrefab.poiUID = uid;
                                        Vector3 pos = Conversions.GeoToWorldPosition(posX, posY,
                                             LocationProviderFactory.Instance.mapManager.CenterMercator, LocationProviderFactory.Instance.mapManager.WorldRelativeScale).ToVector3xz();
                                        pos.y = 0;
                                        shopPrefab.transform.position = pos;

                                        spawnedSpots.Add(key, shopPrefab.gameObject);
                                    }
                                    else
                                    {
                                        //Debug.Log("Already present!");
                                    }
                                }
                            }
						}
					}
				});
            Resources.UnloadUnusedAssets();
		}
	}

	void LateUpdate()
	{
		foreach (KeyValuePair<string, GameObject> pair in spawnedPlayers) {
			spawnedPlayers[pair.Key].transform.position = Vector3.Lerp(spawnedPlayers[pair.Key].transform.position, spawnedPlayersTargetPos[pair.Key], Time.deltaTime * 0.5f);
		}
	}

	public void SpawnSpot(string id, GameObject o) {
		if (!spawnedSpots.ContainsKey (id))
			spawnedSpots.Add (id, o);
	}
	public void SpawnFeature(string id, GameObject o) {
		if (!spawnedFeatures.ContainsKey (id))
			spawnedFeatures.Add (id, o);
	}

	public void SpawnOrUpdatePlayer(string player, double lat, double lon)
	{
		if (!spawnedPlayers.ContainsKey (player)) {
			SpawnOtherPlayer (player, lat, lon);
		} else {
			UpdatePlayer (player, lat, lon);
		}
	}

	public void UpdatePlayer(string player, double lat, double lon)
	{
		Vector2d pos = Conversions.GeoToWorldPosition (lat, lon, AbstractMap.instance.CenterMercator, AbstractMap.instance.WorldRelativeScale);
		if (spawnedPlayersTargetPos.ContainsKey (player)) {
			spawnedPlayersTargetPos [player] = pos.ToVector3xz();
		}
	}

	public void SpawnOtherPlayer(string player, double lat, double lon) {
		if (!spawnedPlayers.ContainsKey (player)) {
			Vector2d pos = Conversions.GeoToWorldPosition (lat, lon, AbstractMap.instance.CenterMercator, AbstractMap.instance.WorldRelativeScale);

			GameObject o = GameObject.Instantiate<GameObject> (otherPlayerPrefab);
			o.transform.position = pos.ToVector3xz () + (Vector3.up * 3);
			o.GetComponent<OtherPlayers> ().nameText.text = player;
			spawnedPlayers.Add (player, o);
			spawnedPlayersTargetPos.Add (player, pos.ToVector3xz ());
		}
	}

	public void RemovePlayer(string player)
	{
		if (spawnedPlayers.ContainsKey (player)) {
			Destroy (spawnedPlayers [player]);
			spawnedPlayers.Remove (player);
			spawnedPlayersTargetPos.Remove (player);
		}
	}

	public void RemoveAllSpawnedFeature()
	{
		foreach (KeyValuePair<string, BasicFeature> pair in PositionWithLocationProvider.instance.spawnedFeatures) {
			Destroy (pair.Value);
		}

		PositionWithLocationProvider.instance.spawnedFeatures.Clear ();
	}

}
