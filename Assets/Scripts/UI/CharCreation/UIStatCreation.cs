using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIStatCreation : MonoBehaviour {

	public Dictionary<string, object> stats;
    public Dictionary<string, object> minStats;

	public int availablePoints = 10;

	public Text pointsText;
	public Text hpText;
	public Text mpText;

	public static UIStatCreation instance;
	void Awake()
	{
		instance = this;
	}

	public int CalculateMaxHp()
	{
		int toughness = 0;
		int.TryParse (stats ["Toughness"].ToString (), out toughness);
		int retVal = 10 + (toughness / 2);
		stats ["MaxHp"] = retVal;
		return retVal;
	}

	// Use this for initialization
	void Start () {
        if (SceneManager.GetActiveScene().name == "Map")
        {
            /*
            stats = new Dictionary<string, object>();
            stats.Add("str", PlayerStats.instance.strength);
            stats.Add("dex", PlayerStats.instance.dexterity);
            stats.Add("toughness", PlayerStats.instance.toughness);
            stats.Add("int", PlayerStats.instance.intelligence);
            stats.Add("luck", PlayerStats.instance.luck);
            stats.Add("MaxHp", 10);
            pointsText.text = "Points: " + availablePoints.ToString();
            hpText.text = "HP: " + CalculateMaxHp().ToString();*/
        }
        else
        {
            stats = new Dictionary<string, object>();
            minStats = new Dictionary<string, object>();
            minStats.Add("Strength", 1);
            minStats.Add("Dexterity", 1);
            minStats.Add("Toughness", 1);
            minStats.Add("Intelligence", 1);
            minStats.Add("Luck", 1);
            minStats.Add("MaxHp", 10);
            stats.Add("Strength", 1);
            stats.Add("Dexterity", 1);
            stats.Add("Toughness", 1);
            stats.Add("Intelligence", 1);
            stats.Add("Luck", 1);
            stats.Add("MaxHp", 10);
            pointsText.text = "Points: " + availablePoints.ToString();
            hpText.text = "HP: " + CalculateMaxHp().ToString();
        }
	}

    public void Load()
    {
        stats = new Dictionary<string, object>();
        minStats = new Dictionary<string, object>();
        stats.Add("Strength", PlayerStats.instance.strength);
        stats.Add("Dexterity", PlayerStats.instance.dexterity);
        stats.Add("Toughness", PlayerStats.instance.toughness);
        stats.Add("Intelligence", PlayerStats.instance.intelligence);
        stats.Add("Luck", PlayerStats.instance.luck);
        stats.Add("MaxHp", PlayerStats.instance.maxHP);
        minStats.Add("Strength", PlayerStats.instance.strength);
        minStats.Add("Dexterity", PlayerStats.instance.dexterity);
        minStats.Add("Toughness", PlayerStats.instance.toughness);
        minStats.Add("Intelligence", PlayerStats.instance.intelligence);
        minStats.Add("Luck", PlayerStats.instance.luck);
        minStats.Add("MaxHp", PlayerStats.instance.maxHP);

        pointsText.text = "Points: " + availablePoints.ToString();
        hpText.text = "HP: " + CalculateMaxHp().ToString();

        foreach (UIStatCreationEntry e in GetComponentsInChildren<UIStatCreationEntry>())
        {
            e.Init();
        }
    }

    public void UploadChanges()
    {
        stats.Add("userName", PlayerPrefs.GetString("user"));
        Bridge.POST(Bridge.url + "UpdateStats", stats, (r) => {
           // ServerResponse resp = new ServerResponse(r);
            Debug.Log("[Confirm LevelUp Response] " + r);
            PlayerServerSync.instance.SyncStats();
            UILevelUp.instance.Hide();
            
        });
    }

	public void UpdateStat(string key, int value)
	{
		stats [key] = value;
		pointsText.text = "Points: " + availablePoints.ToString ();
		hpText.text = "HP: " + CalculateMaxHp ().ToString ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
