using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mapbox.Examples.LocationProvider;
using Mapbox.Unity.Utilities;
using Mapbox.Unity.MeshGeneration;
using System.Linq;
using Mapbox.Unity.Map;
using System.Globalization;

public class GameManager : MonoBehaviour {

    public GameObject GrassTest;

	public static NpcFeature LastCombatTarget;
	public Material baseTileMaterial;

	public bool PlayerIsDead = false;

    public bool CommunicationWithServerEnstabilished = true;

	public static GameManager instance;

	public NodeController nodeTester;

	void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	void Start () {
		SceneManager.sceneLoaded += OnSceneLoad;
        Screen.SetResolution(720, 1280, true);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI()
	{
		return;
		
	}

	void OnSceneLoad(Scene scene, LoadSceneMode mode)
	{

		// Check if we came from a Combat scene, in which case interact with the server for the outcome

		if (scene.name == "Map") {
			Camera.main.GetComponent<CameraController> ().LookPlayer = true;
			if (PlayerPrefs.HasKey ("kickedFromCombat") && PlayerPrefs.GetInt ("kickedFromCombat") == 1) {
				PlayerPrefs.SetInt ("kickedFromCombat", 0);
				PopupManager.ShowPopup("Timeout", "You were afk and kicked from the combat.", (s,b) => {});
			}
			
				/*float storedX = PlayerPrefs.GetFloat("transformX");
				float storedY = PlayerPrefs.GetFloat ("transformY");
				PlayerStats.instance.transform.position = new Vector3 (storedX, 0, storedY);*/							
		}
	}
}

public static class DoubleExt
{
    public static string ToStringEx(this double d)
    {
        return d.ToString().Replace(',', '.');
    }
    public static string ToStringEx(this float f)
    {
        return f.ToString().Replace(',', '.');
    }
}