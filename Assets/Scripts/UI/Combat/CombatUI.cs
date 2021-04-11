using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour {

    public Camera startCamera;
    public Camera mainCamera;
    public CanvasGroup startInfo;
    public Text startText;

    public Image playerTurnMask;

    public Text weaponName;
    public Text weaponDmg;
    public Text weaponHit;
    public Text weaponCrit;
    public Image weaponIcon;

    public UIMonsterStats monsterStatPrefab;
    List<UIMonsterStats> entries = new List<UIMonsterStats>();

    public Transform TargetMarker;

	public CanvasGroup helpCanvas;
	Coroutine helpRoutine;

	public GameObject playerStats;
    public GameObject bottomUI;
	public Image playerHp;
	public Image playerBG;

	public OtherPlayerCombat otherPlayerPrefab;
	public Transform otherPlayerVertical;
	public List<OtherPlayerCombat> otherPlayers = new List<OtherPlayerCombat>();

	public static CombatUI instance;
	public bool ARIsOn = false;
	public GameObject arCamera;
	public GameObject gamePlane;
	void Awake() {
		instance = this;
	}

	// Use this for initialization
	IEnumerator Start () {
        //UpdatePlayerHp (0.5f);
        mainCamera.enabled = false;
        startInfo.alpha = 0;
		playerHp.fillAmount = 0;
      
        playerStats.SetActive(false);
        bottomUI.SetActive(false);           
		while (!PlayerStats.StatsLoaded) {
			yield return new WaitForEndOfFrame ();        
		}
        EquipData d = null;
        if (PlayerEquip.instance.equip.ContainsKey("rHand")
            && !string.IsNullOrEmpty(PlayerEquip.instance.equip["rHand"].weaponId))
            d = PlayerEquip.instance.equip["rHand"];
        if (d != null)
        {
            BaseWeapon w = Resources.Load<BaseWeapon>(Registry.assets.items[d.weaponId]);
            ItemData data = PlayerInventory.instance.GetItem(d.itemUID);
            weaponName.text = w.Name + "+" + data.currentRefine.ToString();
            weaponDmg.text = "Damage: " + data.Damage.ToString();
            weaponHit.text = "Hit Chance: " + data.HitChance.ToString();
            weaponCrit.text = "Crit Chance: " + data.CritChance.ToString();
            weaponIcon.enabled = true;
            weaponIcon.sprite = w.sprite;
        }
        else
        {
            weaponName.text = "Unarmed";
            weaponDmg.text = "Damage: 1";
            weaponHit.text = "Hit Chance: 85";
            weaponCrit.text = "Crit Chance: 10";
            weaponIcon.enabled = false;
        }
        while (!CombatManager.instance.enemyDataLoaded)
            yield return null;
        startText.text = "You encountered 1 " + CombatManager.instance.MonsterName + "!";
        while (startInfo.alpha < 1)
        {
            startInfo.alpha += Time.deltaTime * 1.0f;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1.5f);
        while (startInfo.alpha > 0)
        {
            startInfo.alpha -= Time.deltaTime * 1.0f;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.5f);
        startInfo.alpha = 0;
        startCamera.GetComponent<CombatStartCamera>().rotate = false;
        iTween.MoveTo(startCamera.gameObject, iTween.Hash("position", mainCamera.transform.position, "time", 1.5f));
        iTween.RotateTo(startCamera.gameObject, iTween.Hash("rotation", mainCamera.transform, "time", 1.5f));
        
        //startCamera.transform.rotation = Camera.main.transform.rotation;
        yield return new WaitForSeconds(1.6f);
        startCamera.GetComponent<CombatStartCamera>().Animating = false;
        startCamera.enabled = false;
        mainCamera.enabled = true;
      
        playerStats.SetActive(true);
        bottomUI.SetActive(true);       
        //	UpdatePlayerHp (PlayerStats.instance.currentHP, false, false);
    }

    public void AddMonsterStat(BaseEnemy monster, string name)
    {
        UIMonsterStats stat = GameObject.Instantiate<UIMonsterStats>(monsterStatPrefab);
        stat.transform.SetParent(transform);
        stat.transform.localScale = Vector3.one;
        stat.Init(monster, name);
        entries.Add(stat);
    }

    public void UpdateMonsterStat(int index, float perc)
    {
        entries[index].MonsterHP.fillAmount = perc;
    }

    public void SelectEnemyTarget(BaseEnemy enemy)
    {
        UITargetMarker.instance.target = enemy.namePos.transform;
        TargetMarker.transform.position = Camera.main.WorldToScreenPoint(enemy.namePos.position + (Vector3.up));
    }
	
	// Update is called once per frame
	void Update () {
	

		
		/*if (CombatManager.instance.spawnedEnemy != null) {
			Vector3 pos = Camera.main.WorldToScreenPoint (CombatManager.instance.spawnedEnemy.transform.position);
			MonsterHP.transform.position = pos - (Vector3.up * 10);
			MonsterName.transform.position = pos + (Vector3.up * 20);
		}*/
	}

    public void SetAnimationTurn(string player)
    {
        foreach (OtherPlayerCombat p in otherPlayers)
        {
            if (p.user == player)
            {
                iTween.PunchScale(p.gameObject, iTween.Hash("amount", new Vector3(0.4f, 0.4f, 0.4f), "loopType", "loop", "time", 1.5f));
            }
            else
            {
                iTween.Stop(p.gameObject);
                p.transform.localScale = Vector3.one;
            }
        }
    }

	public void AddPlayer(string user)
	{
		OtherPlayerCombat other = GameObject.Instantiate<OtherPlayerCombat> (otherPlayerPrefab);
        other.user = user;
		other.transform.SetParent (otherPlayerVertical.transform);
		other.transform.localPosition = Vector3.zero;
		other.transform.localScale = Vector3.one;
		otherPlayers.Add (other);
	}

	public void DeletePlayer()
	{
		if (otherPlayers.Count > 0) {
			Destroy (otherPlayers [0].gameObject);
			otherPlayers.RemoveAt (0);
		}
	}

	public void SwitchAR()
	{
		if (!ARIsOn) {
			ARIsOn = true;
			gamePlane.SetActive (false);
			arCamera.SetActive (true);
		} else {
			ARIsOn = false;
			gamePlane.SetActive (true);
			arCamera.SetActive (false);
		}
	}

	public void Run()
	{
        Dictionary<string, object> values = new Dictionary<string, object>();
        values.Add("userName", PlayerPrefs.GetString("user"));
        values.Add("combatId", CombatManager.instance.combatId);
        Bridge.POST (Bridge.url + "Combat/RunFromCombat", values, (r) => {
			Debug.Log("[FLEE] " + r);
			ServerResponse resp = new ServerResponse(r);
			if (resp.status == ServerResponse.ResultType.Success)
                SceneManager.LoadScene("Map");
		});
    }

	public void ShowHelp()
	{
		if (helpRoutine == null)
			helpRoutine = StartCoroutine (showHelp ());
	}
	IEnumerator showHelp()
	{
		iTween.ValueTo (gameObject, iTween.Hash ("from", 0.0f, "to", 1.0f, "time", 0.2f, "onupdate", "updateCanvasAlpha"));
		yield return new WaitForSeconds (5.0f);
		iTween.ValueTo (gameObject, iTween.Hash ("from", 1.0f, "to", 0.0f, "time", 0.2f, "onupdate", "updateCanvasAlpha"));
		yield return new WaitForSeconds (1.0f);
		helpRoutine = null;
	}

	void updateCanvasAlpha(float value)
	{
		helpCanvas.alpha = value;
	}

    public void UpdatePlayerHp(List<Dictionary<string, object>> events)
    {
        /*
         * [ { "effect" : "damage", "amount" : 1 }, { "effect" : "poison", "amount" : 2 } ]
         * */
        int hp = PlayerStats.instance.currentHP;
        PlayerStats.instance.Sync(() =>
        {
            StartCoroutine(CombatEventsRoutine(events, hp));
        });
    }

    public void AnimatePlayerHp(int amount, bool shake = true)
    {
        float _perc = (float)((float)amount / (float)PlayerStats.instance.maxHP);
        float perc = playerHp.fillAmount - _perc;
        if (shake)
            iTween.ShakePosition(playerStats, Vector3.one * 10, 0.3f);
        iTween.ValueTo(gameObject, iTween.Hash("from", playerHp.fillAmount, "to", perc, "onupdate", "updatePlayerHpBar", "time", 1.0f, "easeType", "easeInOutSin"));
    }

    IEnumerator CombatEventsRoutine(List<Dictionary<string, object>> events, int hp)
    {
        foreach (Dictionary<string, object> e in events)
        {
            if (e["effect"].ToString() == "damage")
            {
                int dmg = 0;
                int.TryParse(e["amount"].ToString(), out dmg);
                hp -= dmg;
                float perc = (float)((float)hp / (float)PlayerStats.instance.maxHP);
                iTween.ShakePosition(playerStats, Vector3.one * 10, 0.3f);
                iTween.ValueTo(gameObject, iTween.Hash("from", playerHp.fillAmount, "to", perc, "onupdate", "updatePlayerHpBar", "time", 1.0f, "easeType", "easeInOutSin"));
                FloatingTextSpawner.Get("player").SpawnText("-" + dmg.ToString(), Color.red);
            }
            else if (e["effect"].ToString() == "poison")
            {
                int dmg = 0;
                int.TryParse(e["amount"].ToString(), out dmg);
                hp -= dmg;
                float perc = (float)((float)hp / (float)PlayerStats.instance.maxHP);
                iTween.ShakePosition(playerStats, Vector3.one * 10, 0.3f);
                iTween.ValueTo(gameObject, iTween.Hash("from", playerHp.fillAmount, "to", perc, "onupdate", "updatePlayerHpBar", "time", 1.0f, "easeType", "easeInOutSin"));
                FloatingTextSpawner.Get("player").SpawnText("-" + dmg.ToString(), Color.gray);
            }
            yield return new WaitForSeconds(0.8f);
        }
    }

    public void UpdatePlayerHp(int toValue, bool shake = true, bool notify = true, bool animate = true)
	{
		int currentHp = PlayerStats.instance.currentHP;
		PlayerStats.instance.Sync (() => {

			bool damaged = currentHp > PlayerStats.instance.currentHP;
			bool healed = currentHp < PlayerStats.instance.currentHP;
			Debug.Log("currentHp: " + currentHp + " | playerCurrentHp: " + PlayerStats.instance.currentHP);
			if (damaged)
			{
				Debug.Log("DAMAGED!");
				float perc = (float)((float)PlayerStats.instance.currentHP / (float)PlayerStats.instance.maxHP);
				if (shake)
					iTween.ShakePosition (playerStats, Vector3.one * 10, 0.3f);
				if (animate)
					iTween.ValueTo (gameObject, iTween.Hash ("from", playerHp.fillAmount, "to", perc, "onupdate", "updatePlayerHpBar", "time", 1.0f, "easeType", "easeInOutSin"));
				else
					updatePlayerHpBar(perc);
				if (notify)
					FloatingTextSpawner.Get ("player").SpawnText ("-" + (currentHp-PlayerStats.instance.currentHP).ToString(), Color.red);				
			} else if (healed)
			{
				float perc = (float)((float)PlayerStats.instance.currentHP / (float)PlayerStats.instance.maxHP);
				if (shake)
					iTween.ShakePosition (playerStats, Vector3.one * 10, 0.3f);
				if (animate)
					iTween.ValueTo (gameObject, iTween.Hash ("from", playerHp.fillAmount, "to", perc, "onupdate", "updatePlayerHpBar", "time", 1.0f, "easeType", "easeInOutSin"));
				else
					updatePlayerHpBar(perc);
				if (notify)
					FloatingTextSpawner.Get ("player").SpawnText ("+" + (currentHp-PlayerStats.instance.currentHP).ToString(), Color.green);				
			}
			else
			{
				Debug.Log("Nothing changed?");
				float perc = (float)((float)PlayerStats.instance.currentHP / (float)PlayerStats.instance.maxHP);
				updatePlayerHpBar(perc);
			}

		});
	}

	void updatePlayerHpBar(float value)
	{
		playerHp.fillAmount = value;
	}

}
