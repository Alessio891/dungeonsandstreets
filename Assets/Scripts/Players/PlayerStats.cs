using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour, IServerSerializable {

    public GameObject levelUpFX;

    public static bool StatsLoaded = false;

	public int level;
	public int currentHP;
	public int maxHP;
	public int currentMana;
	public int maxMana;
	public int strength;
	public int dexterity;
	public int intelligence;
	public int toughness;
	public int luck;

	public int gold;

	public int currentXp;

	public static PlayerStats instance;

	public bool isAlive { get { return currentHP > 0; } }

	public System.Action onStatsUpdated;

	void Awake() {
		instance = this;

	}

	// Use this for initialization
	void Start () {

		PlayerServerSync.instance.OnPlayerStatsUpdate += OnStatsUpdate;

		if (SceneManager.GetActiveScene ().name == "Map")
			Sync ();
		else {
			Debug.Log ("COMBAT START; UPDATE STATS");
			CombatUI.instance.UpdatePlayerHp (0, false, false, false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Dictionary<string, object> Serialize ()
	{
		Dictionary<string, object> retVal = new Dictionary<string, object> ();

		retVal.Add ("hp", currentHP.ToString ());
		retVal.Add ("MaxHp", maxHP.ToString ());
		retVal.Add ("mana", currentMana.ToString ());
		retVal.Add ("MaxMana", maxMana.ToString ());
		retVal.Add ("str", strength.ToString ());
		retVal.Add ("dex", dexterity.ToString ());
		retVal.Add ("int", intelligence.ToString ());
		retVal.Add ("Gold", gold.ToString ());
		retVal.Add ("xp", currentXp.ToString ());
		retVal.Add ("toughness", toughness.ToString ());
		retVal.Add ("luck", luck.ToString ());
		retVal.Add ("level", level.ToString ());
		retVal.Add ("user", PlayerPrefs.GetString ("user"));
		return retVal;
	}

	public void Deserialize (Dictionary<string, object> serialized)
	{
		if (serialized.ContainsKey ("hp"))
			int.TryParse (serialized ["hp"].ToString (), out currentHP);
		if (serialized.ContainsKey ("MaxHp"))
			int.TryParse (serialized ["MaxHp"].ToString (), out maxHP);
		if (serialized.ContainsKey ("mana"))
			int.TryParse (serialized ["mana"].ToString (), out currentMana);
		if (serialized.ContainsKey ("MaxMana"))
			int.TryParse (serialized ["MaxMana"].ToString (), out maxMana);
		if (serialized.ContainsKey ("Strength"))
			int.TryParse (serialized ["Strength"].ToString (), out strength);
		if (serialized.ContainsKey ("Dexterity"))
			int.TryParse (serialized ["Dexterity"].ToString (), out dexterity);
		if (serialized.ContainsKey ("Intelligence"))
			int.TryParse (serialized ["Intelligence"].ToString (), out intelligence);
		if (serialized.ContainsKey ("Gold"))
			int.TryParse (serialized ["Gold"].ToString (), out gold);
		if (serialized.ContainsKey ("XP"))
			int.TryParse (serialized ["XP"].ToString (), out currentXp);	
		if (serialized.ContainsKey ("Level"))
			int.TryParse (serialized ["Level"].ToString (), out level);
		if (serialized.ContainsKey ("Toughness"))
			int.TryParse (serialized ["Toughness"].ToString (), out toughness);
		if (serialized.ContainsKey ("Luck"))
			int.TryParse (serialized ["Luck"].ToString (), out luck);
		
		
		if (onStatsUpdated != null)
			onStatsUpdated ();
	}		

	public void OnStatsUpdate(Dictionary<string, object> data)
	{
		Deserialize (data);
		if (DockBar.instance != null) {
			DockBar.instance.setXpBarValue (currentXp);
			DockBar.instance.playerLevel.text = level.ToString ();
			DockBar.instance.hpFill.fillAmount = (float)((float)currentHP / (float)maxHP);
			DockBar.instance.goldText.text = gold.ToString ();
			DockBar.instance.hpText.text = currentHP.ToString () + "/" + maxHP.ToString ();            
		}
		if (!PlayerStats.instance.isAlive)
		{
			WorldSpawner.instance.RemoveAllSpawnedFeature();
			Debug.Log("Player is dead");
			GameManager.instance.PlayerIsDead = true;
			//if (CameraController.instance != null)
				//CameraController.instance.pp.enabled = true;

		} else
		{
			GameManager.instance.PlayerIsDead = false;
			//if (CameraController.instance != null)
				//CameraController.instance.pp.enabled = false;
		}
		StatsLoaded = true;
		/*
		Dictionary<string, object> inventory = (Dictionary<string, object>)resp.GetIncomingDictionary()["Inventory"];
		PlayerInventory.instance.Deserialize(inventory);
		Dictionary<string, object> equip = (Dictionary<string, object>)resp.GetIncomingDictionary()["equip"];
		PlayerEquip.instance.Deserialize(equip);	*/
	}

	public void Sync(System.Action onEnd = null)
	{
		onStatsUpdated = onEnd;
		PlayerServerSync.instance.SyncStats ();
	}

	public int XPNeededForNextLevel()
	{
		return (level + 1) * 20;
	}

    public void LevelUP()
    {
        UILevelUp.instance.Active = true;
        SoundManager.instance.PlayUI(SoundManager.instance.LevelUP);
        StartCoroutine(levelUpRoutine());
    }

    IEnumerator levelUpRoutine()
    {
        levelUpFX.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        levelUpFX.SetActive(false);
        UILevelUp.instance.Load();
        UILevelUp.instance.Show();
    }

    IEnumerator disableLevelUp()
    {
        yield return new WaitForSeconds(3.5f);
        levelUpFX.SetActive(false);
        UILevelUp.instance.Show();
    }
		
}
