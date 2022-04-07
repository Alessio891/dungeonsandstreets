using System.Collections.Generic;
using Mapbox.Utils;
using System.Collections;
using UnityEngine.SceneManagement;
using Mapbox.Unity.MeshGeneration.Data;
using System;

namespace Mapbox.Examples.LocationProvider
{
    using Mapbox.Unity.Location;
    using Mapbox.Unity.Utilities;
    using Mapbox.Unity.MeshGeneration;
    using UnityEngine;
    using Mapbox.Unity.Map;
    using System.Globalization;

    public class PositionWithLocationProvider : MonoBehaviour
	{

		Vector3 lastPosition;
		public double distanceMade = 0.0d;
		float timer = 0.0f;
		public float updateEvery = 50.0f;
		public AbstractMap m;
        public Transform moveWithTransform;

		public GameObject testPrefab;
		public SpinObject testChest;
		public GameObject playerPrefab;
		public GameObject powerUpPrefab;

        

		public string token;
		Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();
		public Dictionary<string, BasicFeature> spawnedFeatures = new Dictionary<string, BasicFeature>();
		Dictionary<string, GameObject> spawnedPlayers = new Dictionary<string, GameObject>();
		Dictionary<string, Vector3> spawnedPlayersTargetPos = new Dictionary<string, Vector3>();
		public static PositionWithLocationProvider instance;
		void Awake() {
			instance = this;
		}
        /// <summary>
        /// The rate at which the transform's position tries catch up to the provided location.
        /// </summary>
		[SerializeField]
		float _positionFollowFactor;

        /// <summary>
        /// Use a mock <see cref="T:Mapbox.Unity.Location.TransformLocationProvider"/>,
        /// rather than a <see cref="T:Mapbox.Unity.Location.EditorLocationProvider"/>. 
        /// </summary>
        [SerializeField]
        public bool _useTransformLocationProvider;

        /// <summary>
        /// The location provider.
        /// This is public so you change which concrete <see cref="T:Mapbox.Unity.Location.ILocationProvider"/> to use at runtime.
        /// </summary>
		ILocationProvider _locationProvider;
		public ILocationProvider LocationProvider
		{
			get
			{
				if (_locationProvider == null)
				{
                    _locationProvider = _useTransformLocationProvider ? 
                        LocationProviderFactory.Instance.TransformLocationProvider : LocationProviderFactory.Instance.DefaultLocationProvider;
				}

				return _locationProvider;
			}
			set
			{
				if (_locationProvider != null)
				{
					_locationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;

				}
				_locationProvider = value;
				_locationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;
			}
		}

		public bool Moving { get { return Vector3.Distance (transform.position, _targetPosition) > 0.01f; } }

		Vector3 _targetPosition;

		void Start()
		{
#if !UNITY_EDITOR
            _useTransformLocationProvider = false;
#endif
            if (!PlayerPrefs.HasKey ("user")) {
				SceneManager.LoadScene (0);
				return;
			}
			token = PlayerPrefs.GetString ("user");
            //token = "Als" + Random.Range (0, 100).ToString () + Random.Range (0, 100).ToString ();
            //m.Request()

            LocationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;
			StartCoroutine (loadingStart ());
		}

		public void SyncTransformWithLocation()
		{
			StartCoroutine (waitAndSync ());
		}

		IEnumerator waitAndSync()
		{
			yield return new WaitForEndOfFrame ();
			yield return new WaitForEndOfFrame ();
			yield return new WaitForEndOfFrame ();
			yield return new WaitForEndOfFrame ();
            transform.localPosition = AbstractMap.instance.GeoToWorldPosition(LocationProvider.CurrentLocation.LatitudeLongitude);
			yield return new WaitForEndOfFrame();
		}

		IEnumerator loadingStart()
		{
			LoadingMask.instance.Show ();
			LoadingMask.instance.SetProgress (0);


			float count = 0;

#if !UNITY_EDITOR
			LoadingMask.instance.Show("Accessing GPS...");
			DeviceLocationProvider dL = (LocationProvider as DeviceLocationProvider);
			float timer = 0;
            
			while (Input.location.status != LocationServiceStatus.Running)
			{
				count += 0.1f;
				if (count > 0.9f)
					count = 0.9f;
				LoadingMask.instance.SetProgress(count);
				timer += Time.deltaTime;
				if (timer > 10 && timer < 30)
				{
					timer = 50;
					PopupManager.ShowPopup("GPS Error", "The game was unable to find your gps data, maybe your location services are disabled?", (s, b) => {
						
			            Destroy (dL.transform.parent.gameObject);
						SceneManager.LoadScene ("login");
			        }, () =>  {  Destroy (dL.transform.parent.gameObject); SceneManager.LoadScene ("login"); });
				}
				yield return new WaitForEndOfFrame();
			}
			transform.position = Conversions.GeoToWorldPosition(LocationProvider.CurrentLocation.LatitudeLongitude.x, LocationProvider.CurrentLocation.LatitudeLongitude.y, AbstractMap.instance.CenterLatitudeLongitude, AbstractMap.instance.AbsoluteZoom).ToVector3xz();
			LoadingMask.instance.SetProgress(1);
#endif

            //LoadingMask.instance.Hide ();
            LoadingMask.instance.Show("Downloading data...");
			count = 0;
			string response = null;
			yield return new WaitForEndOfFrame ();
			yield return new WaitForEndOfFrame ();
			/*while (true) {
				bool allLoaded = true;
				int loadedCount = 0;
				int tileCount = 9;
				Debug.Log ("Tile count: " + tileCount);
				Vector2d centerTile = Conversions.LatitudeLongitudeToTileId ((float)LocationProvider.Location.x, (float)LocationProvider.Location.y, m.Zoom);
				for (int i = -1; i <= 1; i++) {
					for (int j = -1; j <= 1; j++) {
						Vector2 thisTile = new Vector2 ((float)centerTile.x + (float)i, (float)centerTile.y + (float)j);
						if (m._tiles.ContainsKey (thisTile)) {
							if (m._tiles [thisTile].ImageDataState == Mapbox.Unity.MeshGeneration.Enums.TilePropertyState.Loaded)
							{
								loadedCount++;
							}
						} else {
							Debug.Log ("No tile for " + thisTile.ToString ());
							//loadedCount++;
						}
					}
				}
				 
				float progress = (float)((float)loadedCount / 9.0f);
				LoadingMask.instance.SetProgress (progress);
				//LoadingMask.instance.SetProgress (0.5f);
				if (loadedCount >= tileCount) {					
					break;
				}
				yield return new WaitForEndOfFrame ();
			}*/
			/*
			count = 0.5f;
			while (string.IsNullOrEmpty (response)) {
				LoadingMask.instance.SetProgress (count);
				if (count > 0.9f)
					count = 0.9f;
				yield return new WaitForSeconds (0.1f);
				count += 0.1f;
			}*/
			LoadingMask.instance.SetProgress (1);
			yield return new WaitForEndOfFrame ();
			LoadingMask.instance.Hide ();

		}

		void OnDestroy()
		{
			if (LocationProvider != null)
			{
				LocationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;
			}
		}

		void LocationProvider_OnLocationUpdated(Location loc)
		{

            _targetPosition = Conversions.GeoToWorldPosition(loc.LatitudeLongitude,
                LocationProviderFactory.Instance.mapManager.CenterMercator, LocationProviderFactory.Instance.mapManager.WorldRelativeScale).ToVector3xz();
                
                //LocationProviderFactory.Instance.mapManager.GeoToWorldPosition(loc.LatitudeLongitude);

			distanceMade += Vector3.Distance (_targetPosition, transform.position);
			transform.rotation = Quaternion.LookRotation (_targetPosition - transform.position);

		}

        public string xy2tile(string posX, string posY)
        {
            Double x = Double.Parse(posX);
            Double y = Double.Parse(posY);
            int lon = lon2x(x);
            int lat = lat2y(y);
            return xy2tile(lon, lat);
        }
        int long2tilex(double lon, int z)
        {
            return (int)(Math.Floor((lon + 180.0) / 360.0 * (1 << z)));
        }
        double ToRadians(double val)
        {
            return val * Math.PI / 180.0;
        }
        int lat2tiley(double lat, int z)
        {
            return (int)Math.Floor((1 - Math.Log(Math.Tan(ToRadians(lat)) + 1 / Math.Cos(ToRadians(lat))) / Math.PI) / 2 * (1 << z));
        }
        string xy2tile(int x, int y)
        {
            string tile = "";

            int tileX = long2tilex(LocationProvider.CurrentLocation.LatitudeLongitude.y, 17);
            int tileY = lat2tiley(LocationProvider.CurrentLocation.LatitudeLongitude.x, 17);
            tile = tileX.ToString() + ":" + tileY.ToString();
            return tile;
        }

       int lon2x(double lon)
        {
            return (int)Math.Round((lon + 180.0) * 65535.0 / 360.0);
        }

        int lat2y(double lat)
        {
            return (int)Math.Round((lat + 90.0) * 65535.0 / 180.0);
        }

        void UpdatePositionResponse(string resp)
		{
			/*
			Bridge.Data d = Bridge.ParseResponse (resp);
			if (d.result == Bridge.Data.Result.Success) {
				
			}
			Debug.Log (d.rawMessage);*/
			Debug.Log (resp);
			//Bridge.Data d = new Bridge.Data();
			//d.ParseMapResponse (resp);
		}
        private void OnGUI()
        {
            double x = LocationProvider.CurrentLocation.LatitudeLongitude.x;
            double y = LocationProvider.CurrentLocation.LatitudeLongitude.y;
            GUILayout.Label("X:" + x.ToStringEx());
            GUILayout.Label("Y:" + y.ToStringEx());
            GUILayout.Label("TILE ID:" + xy2tile(x.ToString(), y.ToString()));
            GUILayout.Label("User:" + PlayerPrefs.GetString("user"));
        }
        private static readonly DateTime Jan1st1970 = new DateTime
			(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static long CurrentTimeMillis()
		{
			return (long) (DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
		}

		void Update()
		{
			timer += Time.deltaTime;
			if (timer > updateEvery) {
               // m.UpdateMap();
				Dictionary<string, object> values = new Dictionary<string, object> ();
				
				values.Add ("x", LocationProvider.CurrentLocation.LatitudeLongitude.x.ToStringEx());
				values.Add ("y", LocationProvider.CurrentLocation.LatitudeLongitude.y.ToStringEx());
				values.Add ("distance", distanceMade.ToString(CultureInfo.InvariantCulture));
//				Debug.Log ("Sending " + values ["x"] + ", " + values ["y"] + " dist " + distanceMade);
				//Bridge.POST (Bridge.url + "UpdatePosition", values, UpdatePositionResponse);

				values = new Dictionary<string, object>();
                values.Add("user", PlayerPrefs.GetString("user"));
#if !UNITY_EDITOR
                values.Add("x", LocationProvider.CurrentLocation.LatitudeLongitude.x.ToStringEx());
				values.Add("y", LocationProvider.CurrentLocation.LatitudeLongitude.y.ToStringEx());
#else
                if (_useTransformLocationProvider)
                {
                    Vector2d geoPos = AbstractMap.instance.WorldToGeoPosition(moveWithTransform.position);
                    values.Add("x", geoPos.x.ToString(CultureInfo.InvariantCulture));
                    values.Add("y", geoPos.y.ToString(CultureInfo.InvariantCulture));
                } else
                {
                    values.Add("x", LocationProvider.CurrentLocation.LatitudeLongitude.x.ToString(CultureInfo.InvariantCulture));
                    values.Add("y", LocationProvider.CurrentLocation.LatitudeLongitude.y.ToString(CultureInfo.InvariantCulture));
                }
#endif
                if (!GameManager.instance.PlayerIsDead) {
					Bridge.POST (Bridge.url + "GetFeaturesNearby", values, (s) => {
                        						
                        Debug.Log (s);
						ServerResponse resp = new ServerResponse (s);

						List<object> nearMe = resp.Data.GetAs<List<object>>("data");
						List<object> nearNodes = resp.Data.GetAs<List<object>>("nodes");
						//List<object> nearMe = (List<object>)resp.GetIncomingDictionary () ["data"];
						//List<object> nearNodes = (List<object>)resp.GetIncomingDictionary()["nodes"];

						List<string> toBeRemovedObject = new List<string> ();
						foreach (KeyValuePair<string, BasicFeature> spawnedPair in spawnedFeatures) {
							bool stillPresent = false;
							foreach (object _entry in nearMe) {
								Dictionary<string, object> entry = (Dictionary<string, object>)_entry;
								if (entry ["id"].ToString () == spawnedPair.Key) {								
									stillPresent = true;
                                    break;
								}
							}
                            foreach (object _entry in nearNodes)
                            {
                                Dictionary<string, object> entry = (Dictionary<string, object>)_entry;
                                if (entry["id"].ToString() == spawnedPair.Key)
                                {
                                    stillPresent = true;
                                    break;
                                }
                            }
                            if (!stillPresent) {
								toBeRemovedObject.Add (spawnedPair.Key);
							}
						}

						foreach (string tbR in toBeRemovedObject) {
							Debug.Log ("REMOVING " + spawnedFeatures [tbR].name);
							Destroy (spawnedFeatures [tbR].gameObject);
							spawnedFeatures.Remove (tbR);
						}

						foreach (object _entry in nearMe) {
							Dictionary<string, object> entry = (Dictionary<string, object>)_entry;
                            HandleFeatureData(entry);
						}
                        foreach (object _entry in nearNodes)
                        {
                            Dictionary<string, object> entry = (Dictionary<string, object>)_entry;
                            HandleFeatureData(entry);
                        }
                    });
				}
				timer = 0.0f;
				distanceMade = 0.0f;
			}
			transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _positionFollowFactor);

			foreach (KeyValuePair<string, GameObject> pair in spawnedPlayers) {
				spawnedPlayers[pair.Key].transform.position = Vector3.Lerp(spawnedPlayers[pair.Key].transform.position, spawnedPlayersTargetPos[pair.Key], Time.deltaTime * _positionFollowFactor);
			}

		}
        void HandleFeatureData(Dictionary<string, object> entry)
        {
            string pos = entry["posX"].ToString() + "," + entry["posY"].ToString();
            string id = entry["id"].ToString();
            string type = entry["uid"].ToString();
            string[] splitPos = pos.Split(',');
            Vector2d posV = new Vector2d();
            posV.x = double.Parse(entry["posX"].ToString(), NumberStyles.Float);// double.Parse(splitPos[0], CultureInfo.InvariantCulture);
            posV.y = double.Parse(entry["posY"].ToString(), NumberStyles.Float);//double.Parse(splitPos[1], CultureInfo.InvariantCulture);
            if (!spawnedFeatures.ContainsKey(id))
            {
                BasicFeature o = GameObject.Instantiate<BasicFeature>(Registry.assets.features[type].OnMapFeature);
                o.transform.position = Conversions.GeoToWorldPosition((double)posV.x, (double)posV.y,
                                                                     LocationProviderFactory.Instance.mapManager.CenterMercator, LocationProviderFactory.Instance.mapManager.WorldRelativeScale).ToVector3xz();
                //Conversions.GeoToWorldPosition ((double)posV.x, (double)posV.y, AbstractMap.instance.CenterLatitudeLongitude, AbstractMap.instance.WorldRelativeScale).ToVector3xz ();
                o.transform.position += Vector3.up;
                BasicFeature f = o.GetComponent<BasicFeature>();
                f.UID = id;
                f.type = type;
                f.x = posV.x;
                f.y = posV.y;
                f.extraData = MiniJSON.Json.Serialize(entry["extraData"]);
                if (entry.ContainsKey("isNode"))
                    f.isNode = (bool)entry["isNode"];
                else
                    f.isNode = false;
                int.TryParse(entry["rarity"].ToString(), out f.rarity);
                long spawnTime = 0;
                long.TryParse(entry["spawn_time"].ToString(), out spawnTime);
                long now = 0;
                long.TryParse(entry["now_time"].ToString(), out now);
                long elapsed = now - spawnTime;
                f.lifeTime = 60000 - elapsed;
                f.spawnTime = spawnTime;
                spawnedFeatures.Add(id, o);
            }
        }
    }

   
}
