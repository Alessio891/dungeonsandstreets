using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.MeshGeneration;
using Mapbox.Unity.Utilities;
using Mapbox.Unity.Map;

public class Test : MonoBehaviour {
	public AbstractMap m;
	public Vector2 currentTile;
	public int Range = 3;
	public Vector3 lastTile;
	// Use this for initialization
	void Start () {
		StartCoroutine (delayStart ());

	}

	IEnumerator delayStart()
	{
		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();

		Input.location.Start ();

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
			Debug.Log ("Location: " + Input.location.lastData.latitude + " - " + Input.location.lastData.longitude);
		}

		lastTile = currentTile;
	}

	// Update is called once per frame
	void Update () {
	}

	void RequestNew()
	{
		for (int i = -Range; i <= Range; i++) {
			for (int j = -Range; j <= Range; j++) {
				Vector2 n = new Vector2 (currentTile.x + i, currentTile.y + j);
				Debug.Log ("Requestin " + n.ToString ());
			}
		}
	}
}
