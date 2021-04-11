using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using Mapbox.Utils;
using Mapbox.Unity.MeshGeneration;
using Mapbox.Unity.Utilities;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;

public class PlayerController : MonoBehaviour {

	public Vector2d latLon;
	public Vector2 currentTile;
	public Vector2 lastTile;
	public Vector2 startingPos;
	public Vector3 _targetPosition;
	public int Range = 1;
	public AbstractMap m;

	ILocationProvider locationProvider;

	// Use this for initialization
	void Start () {
		locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
		StartCoroutine (delayStart ());
	}
	IEnumerator delayStart()
	{
	//	yield return new WaitForEndOfFrame ();
	//	yield return new WaitForEndOfFrame ();

		Input.location.Start (15);

		int maxWait = 20;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
			yield return new WaitForSeconds (1);
			maxWait--;
		}
		if (maxWait < 1) {
			Debug.Log ("Service didn't start.");
			yield break;
		}
		if (Input.location.status == LocationServiceStatus.Failed) {
			Debug.Log ("Error gps");
			yield break;
		} else {
			/*startingPos = new Vector2 (Input.location.lastData.latitude, Input.location.lastData.longitude);
			Debug.Log ("Location: " + Input.location.lastData.latitude + " - " + Input.location.lastData.longitude);
			Vector2d p = Conversions.GeoToWorldPosition ((double)Input.location.lastData.latitude, (double)Input.location.lastData.longitude, new Vector2d (0, 0), m.Zoom);
			m.enabled = true;
			m.Execute (Input.location.lastData.latitude, Input.location.lastData.longitude, m.Zoom, new Vector4 (1, 1, 1, 1));
			transform.position = new Vector3 (startingPos.y, 1, startingPos.x);*/
            
			locationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;

		}

        Vector3 v = transform.position / m.WorldRelativeScale;
		currentTile = Conversions.MetersToTile (m.CenterLatitudeLongitude, m.AbsoluteZoom);

		lastTile = currentTile;
	}


	void LocationProvider_OnLocationUpdated(Location loc)
	{

        _targetPosition = m.GeoToWorldPosition(loc.LatitudeLongitude);
		Dictionary<string, object> values = new Dictionary<string, object> ();
		values.Add ("token", "Als");
		values.Add ("x", Input.location.lastData.latitude.ToString());
		values.Add ("y", Input.location.lastData.longitude.ToString ());
		Bridge.POST ("http://localhost:8080/DungeonsAndStreets/UpdatePosition", values, (s) => Debug.Log (s));
	}

	void Update()
	{
		transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * 0.5f);
	}

	void RequestNew()
	{
		for (int i = -Range; i <= Range; i++) {
			for (int j = -Range; j <= Range; j++) {
				Vector2d n = new Vector2d (currentTile.x + i, currentTile.y + j);
				Debug.Log ("Requestin " + n.ToString ());
                m.UpdateMap(n, m.AbsoluteZoom);
			}
		}
	}

}
