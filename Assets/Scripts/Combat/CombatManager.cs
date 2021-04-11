using Mapbox.Examples.LocationProvider;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour {
    public ParticleSystem DebugParticles;

	public enum TurnType { Player = 0, Enemy = 1, Neither = 2 }

	public TurnType currentTurn;


    public List<Transform> enemySpawnPositions;
    public List<BaseEnemy> spawnedEnemies = new List<BaseEnemy>();

	public Transform enemySpawnPos;
    public bool enemyDataLoaded = false;
    int enemyLoadedCount = 0;

	public string MonsterName;
	public int MonsterMaxHP;
	public int MonsterHP;
	public int MonsterBaseDamage;

    public int currentTargetIndex = 0;

    public int statusIndex = 0;

    public int currentEventIndex = 0;

	public string extraData;

	public string combatId;

    public bool CombatEnded = false;

    public Texture DissolveTexture;
    public AudioClip DissolveSound;

	public static CombatManager instance;

	public CombatData combatData;
	public CombatData oldCombatData;
    
    public bool Initialized = false;

    bool _canAct = false;
    public bool CanAct
    {
        get { return _canAct; }
        set
        {
            CombatUI.instance.playerTurnMask.enabled = !value;
            _canAct = value;
        }
    }
    public bool ParsingEvents = false;
	float updateTimer = 0;
    bool justStarted = true;

	void Awake() { instance = this; }

	// Use this for initialization
	void Start () {
		combatId = PlayerPrefs.GetString ("combatId");
		currentTurn = TurnType.Neither;
        currentEventIndex = -1;
        enemyDataLoaded = false;
		
        SoundManager.instance.ChangeBGM(SoundManager.instance.BattleMusic, true);
		bool canProceed = false;

        string playerX = PositionWithLocationProvider.instance.LocationProvider.CurrentLocation.LatitudeLongitude.x.ToStringEx();
        string playerY = PositionWithLocationProvider.instance.LocationProvider.CurrentLocation.LatitudeLongitude.y.ToStringEx();


        Bridge.GET(Bridge.url + "GetCombat?user="+PlayerPrefs.GetString("user")+"&combatId=" + combatId+"&posX="+playerX+"&posY="+playerY, (r) => {
            ServerResponse resp = new ServerResponse(r);
            if (resp.status != ServerResponse.ResultType.Error)
            {
                combatData = new CombatData();
                combatData.Deserialize(resp.GetIncomingDictionary());
                StartCoroutine(spawnEnemies());
            }
        });		
	}

    IEnumerator spawnEnemies()
    {         
        for (int i = 0; i < combatData.monsters.Count; i++)
        {
            Dictionary<string, object> mData = (Dictionary<string, object>)combatData.monsters[i];
            LoadEnemyData(mData["UID"].ToString(), i);
            while (enemyLoadedCount == i)
                yield return null;
        }

        CombatUI.instance.SelectEnemyTarget(spawnedEnemies[0]);
        enemyDataLoaded = true;   
        yield return null;
        SyncCombat();

        PlayerServerSync.instance.SyncStats();
    }


	public void SyncCombat()
	{
        if (ParsingEvents)
            return;
		oldCombatData = combatData;
        string playerX = PositionWithLocationProvider.instance.LocationProvider.CurrentLocation.LatitudeLongitude.x.ToStringEx();
        string playerY = PositionWithLocationProvider.instance.LocationProvider.CurrentLocation.LatitudeLongitude.y.ToStringEx();
        Bridge.GET (Bridge.url + "GetCombat?user=" + PlayerPrefs.GetString("user") + "&combatId=" + combatId + "&posX=" + playerX + "&posY=" + playerY, (r) => {
			Debug.Log("[GETCOMBAT] " + r);
			ServerResponse resp = new ServerResponse(r);		
			if (resp.status == ServerResponse.ResultType.Success)
			{
				combatData = new CombatData();
				combatData.Deserialize(resp.GetIncomingDictionary());
                
				if (oldCombatData != null)
				{
                    bool allDead = true;
                    for (int i = 0; i < combatData.monsters.Count; i++)
                    {
                        object m = (object)combatData.monsters[i];
                        Dictionary<string, object> monster = (Dictionary<string, object>)m;
                        Dictionary<string, object> oldMonster = (Dictionary<string, object>)m;
                        float mHp = 0;
                        float oldmHp = 0;

                        float.TryParse(monster["CurrentHp"].ToString(), out mHp);
                        float.TryParse(oldMonster["CurrentHp"].ToString(), out oldmHp);
                        float perc = mHp / (float)MonsterMaxHP;

                        CombatUI.instance.UpdateMonsterStat(i, perc);
                        if (mHp > 0)
                            allDead = false;
                        if (oldmHp != mHp)
                        {
                            if (mHp > 0)
                            {
                                spawnedEnemies[i].animator.SetTrigger("TakeHit");
                            }
                        }
                    }	
				}
				bool playerPresent = false;
				foreach(object o in combatData.players)
				{
					Dictionary<string, object> player = (Dictionary<string, object>)o;
					if (player["user"].ToString() == PlayerPrefs.GetString("user"))
					{
						playerPresent = true;
						break;
					}
				}
				if (!playerPresent)
				{
					PlayerPrefs.SetInt("kickedFromCombat", 1);
					SceneManager.LoadScene("Map");

					return;
				}

				// If a player leaves during his turn, trigger the enemy attack if needed
				// and avoid being stuck.


				if (!justStarted)
				{
					//CheckNewPlayers();
				} else
				{
					SpawnOtherPlayers((List<object>)combatData.players);
                    justStarted = false;
				}
				UpdatePlayersValues();

                if (!Initialized)
                {
                    if (combatData.events.Count > 1)
                        currentEventIndex = combatData.events.Count - 1;                    
                    Initialized = true;
                }
                StartCoroutine(ParseEvents());           

			} else
			{
				Debug.Log("Combat timed out.");
				PopupManager.ShowPopup("Timeout", "The combat was inactive for too much time and has been canceled.",
					(s, p) => p.Close(), () => SceneManager.LoadScene("Map") );
			}
		});
	}

	/*void OnGUI()
	{
		if (combatData == null)
			return;
		Color c = GUI.color;
		GUI.color = Color.black;
		GUILayout.Label ("Turn: " + combatData.turn.ToString ());
		GUILayout.Label ("Players: " + combatData.players.Count.ToString ());
		if (combatData.turn != TurnType.Enemy)
			GUILayout.Label ("PLAYER INDEX: " + combatData.currentPlayerIndex.ToString());
		else
			GUILayout.Label ("MONSTER INDEX: " + combatData.currentMonsterIndex.ToString());
		GUI.color = c;
	}
	*/
	// Update is called once per frame
	void Update () {
		//if (combatData.turn != TurnType.Enemy) {
			updateTimer += Time.deltaTime;
			if (updateTimer > 3) {
				SyncCombat ();
				updateTimer = 0;
				//CheckNewPlayers ();
			}
	//	} else
	//		updateTimer = 0;
	}

	public void UpdatePlayersValues()
	{
		int i = 0;
		foreach (object o in combatData.players) {			
			Dictionary<string, object> player = (Dictionary<string, object>)o;
			if (player ["user"].ToString () == PlayerPrefs.GetString ("user"))
				continue;
			int hp = 0; int maxHp = 0;
			int.TryParse (player ["hp"].ToString (), out hp);
			int.TryParse (player ["MaxHp"].ToString (), out maxHp);
			float perc = (float)((float)hp / (float)maxHp);
			if (i < CombatUI.instance.otherPlayers.Count) {
				CombatUI.instance.otherPlayers [i].SetFill (perc);
				CombatUI.instance.otherPlayers [i].nameText.text = player ["user"].ToString ();
				if ((combatData.players [combatData.currentPlayerIndex] as Dictionary<string, object>) ["user"].ToString () == player ["user"].ToString ())
					CombatUI.instance.otherPlayers [i].bg.color = Color.blue;
				else
					CombatUI.instance.otherPlayers [i].bg.color = Color.gray;
			}
			
			
			i++;
		}
	}
        
	public void SpawnOtherPlayers(List<object> players)
	{
		foreach (object o in players) {
			Dictionary<string, object> player = (Dictionary<string, object>)o;
            Debug.Log("Checking player " + player["user"].ToString() + " - current player is " + PlayerPrefs.GetString("user"));
			if (player ["user"].ToString () != PlayerPrefs.GetString ("user")) {
                CombatUI.instance.AddPlayer(player["user"].ToString());
			}
		}
	}

    // OBSOLETE
	public void CheckNewPlayers()
	{
		if (oldCombatData != null && oldCombatData.players.Count < combatData.players.Count) {
			for (int i = oldCombatData.players.Count - 1; i < combatData.players.Count - 1; i++) {
				OnPlayerJoin ((Dictionary<string, object>)combatData.players [i]);
			}
		} else if (oldCombatData != null && oldCombatData.players.Count > combatData.players.Count)
		{
			for (int i = combatData.players.Count - 1; i < oldCombatData.players.Count - 1; i++) {
				OnPlayerLeft ((Dictionary<string, object>)oldCombatData.players [i+1]);
			}
			Debug.Log ("A player fled!");
		}
	}

	void OnPlayerJoin(Dictionary<string, object> player)
	{
		//CombatUI.instance.AddPlayer ();
	}
	void OnPlayerLeft(Dictionary<string, object> player)
	{
	/*	CombatUI.instance.DeletePlayer ();
		Debug.Log ("The last player was " + oldCombatData.GetPlayerUserById (oldCombatData.currentPlayerIndex) + ", " + player ["user"].ToString () + " just left.");
		if (oldCombatData.GetPlayerUserById(oldCombatData.currentPlayerIndex) == player["user"].ToString())			
		{
			Debug.Log("The last player isnt the same as before.");
			if (combatData.GetPlayerUserById(0) == PlayerPrefs.GetString("user"))
			{
				Debug.Log("I'm the one who started the combat, let's trigger the enemy");
				spawnedEnemy.StartTurn ();
			}
		}*/
	}

	public void LoadEnemyData(string uid, int index)
	{
		Bridge.GET (Bridge.url + "DownloadMonster?UID=" + uid, (r) => {
			Debug.Log(r);
			ServerResponse resp = new ServerResponse(r);
			Dictionary<string, object> data = resp.GetIncomingDictionary();
			BaseEnemy prefab = Resources.Load<BaseEnemy>(data["ModelPath"].ToString());
			if (prefab != null)
			{
				BaseEnemy spawnedEnemy = GameObject.Instantiate<BaseEnemy>(prefab);
				spawnedEnemy.transform.SetParent(enemySpawnPositions[index].transform);
				spawnedEnemy.transform.localPosition = Vector3.zero;
                spawnedEnemy.transform.localEulerAngles = Vector3.zero;
                spawnedEnemy.index = index;
                spawnedEnemies.Add(spawnedEnemy);
			}			
			int.TryParse(data["MaxHp"].ToString(), out MonsterMaxHP);
			int.TryParse(data["BaseDamage"].ToString(), out MonsterBaseDamage);
		//s	extraData = resp.incomingData["extraData"].ToString();
			
            CombatUI.instance.AddMonsterStat(spawnedEnemies[index], data["Name"].ToString());

            //			CombatInfoBox.instance.AddText(MonsterName + " appeared!");    
            enemyLoadedCount++;
		});
	}

    private void OnGUI()
    {
        GUILayout.Label("CanAct: " + CanAct.ToString());
        GUILayout.Label("ParsingEvents: " + ParsingEvents.ToString());
        
    }
    bool waitingServer = false;
	public void PlayerAttack()
	{
		if (CanAct && !ParsingEvents) {
            CanAct = false;
			Dictionary<string, object> values = new Dictionary<string, object> ();
			values.Add ("combatId", combatId);
			values.Add ("user", PlayerPrefs.GetString ("user"));
            values.Add("targetIndex", currentTargetIndex);
			waitingServer = true;
			Bridge.POST (Bridge.url + "Combat/PlayerAttack", values, (s) => {
				waitingServer = false;
				Debug.Log("[Attack Response] " + s);
				ServerResponse r = new ServerResponse(s);
				if (r.status == ServerResponse.ResultType.Success)
				{
                    combatData.Deserialize((Dictionary<string, object>)r.GetIncomingDictionary()["combatData"]);
                    int attackEventIndex = int.Parse(r.GetIncomingDictionary()["eventIndex"].ToString());
                    string infoMess = "";

                    Dictionary<string, object> hitEvent = (Dictionary<string, object>)combatData.events[attackEventIndex]["actionResult"];
					if (hitEvent["hit"].ToString() == "success")
					{
                        iTween.PunchPosition(Camera.main.gameObject, Camera.main.transform.right * 4.5f, 0.5f);
                        SoundManager.instance.PlayUI(SoundManager.instance.SwordSlash);
                        DebugParticles.transform.position = spawnedEnemies[currentTargetIndex].transform.position + (spawnedEnemies[currentTargetIndex].transform.forward);
                        DebugParticles.Play();
                        infoMess = "You hit " + MonsterName + " for ";
                        int dmgDone = 0;
						int mobHp = 0;
						bool isMobDead = (hitEvent.ContainsKey("monsterDead"));
						int.TryParse(hitEvent["damage"].ToString(), out dmgDone);

                        infoMess += dmgDone.ToString() + ".";

                        if (hitEvent.ContainsKey("crit"))
                        {
                            infoMess += " Massive damage!";
                            FloatingTextSpawner.Get("enemy_"+currentTargetIndex.ToString()).SpawnText("[CRIT!] -" + dmgDone.ToString(), Color.red);
                        }
                        else
                            FloatingTextSpawner.Get("enemy_" + currentTargetIndex.ToString()).SpawnText("-" + dmgDone.ToString(), Color.red);

                        /*if (r.GetIncomingDictionary().ContainsKey("elemental"))
                        {
                            string elementalEffect = r.GetIncomingDictionary()["elemental"].ToString();
                            if (elementalEffect == "effective")
                            {
                                FloatingTextSpawner.Get("enemy").SpawnText("WEAK!", Color.green);
                                infoMess += " The attack was super effective!";
                            }
                            else if (elementalEffect == "resistant")
                            {
                                infoMess += " " + MonsterName + " resisted!";
                                FloatingTextSpawner.Get("enemy").SpawnText("RESIST!", Color.red);
                            }
                            else if (elementalEffect == "gain")
                            {
                                infoMess += " " + MonsterName + " seems to gain strength!";
                                FloatingTextSpawner.Get("enemy").SpawnText("ABSORB!", Color.blue);
                            }
                        }*/

						spawnedEnemies[currentTargetIndex].animator.SetTrigger ("TakeHit");
						float perc = (float)((float)mobHp / (float)MonsterMaxHP);
                        CombatUI.instance.UpdateMonsterStat(currentTargetIndex, perc);
						/*if (isMobDead) {
                            currentTurn = TurnType.Neither;
                            combatData.turn = TurnType.Neither;
							StartCoroutine (PlayerWin ());
							return;
						}*/
					} else
					{
                        infoMess = "You missed " + MonsterName + "!";
						FloatingTextSpawner.Get("enemy_"+currentTargetIndex.ToString()).SpawnText("MISS", Color.white, 45);
						//CombatInfoBox.instance.AddText("You missed your target...");
					}
                    CombatInfoBox.instance.AddText(infoMess);
					
                    StartCoroutine(ParseEvents());
                    //if (combatData.turn == TurnType.Enemy)
                      //  spawnedEnemy.StartTurn();
				}
			});
		}
	}

    IEnumerator ParseEvents()
    {
        if (CombatEnded)
        {
            yield break;
        }
        ParsingEvents = true;
        Debug.Log("Events:" + combatData.events.Count + "|index:" + currentEventIndex);
        if (combatData.events.Count - 1 > currentEventIndex)
        {
            for (int i = currentEventIndex+1; i < combatData.events.Count; i++)
            {
                Dictionary<string, object> ev = combatData.events[i];
                Debug.Log("Parsing " + ev["who"].ToString() + " -> " + ev["what"].ToString());
                if (ev["what"].ToString() == "combat_end")
                {                    
                    StartCoroutine(PlayerWin(ev));
                    break;
                } else if (ev["what"].ToString() == "party_defeated")
                {
                    yield return new WaitForSeconds(2.5f);
                    EndCombat();
                    break;
                }
                else if (ev["what"].ToString() == "turnStart")
                {
                    Debug.Log("Starting turn");
                    CombatUI.instance.SetAnimationTurn(ev["who"].ToString());
                    if (ev["who"].ToString() == PlayerPrefs.GetString("user"))
                    {
                        CombatInfoBox.instance.AddText("It's your turn!");
                        CanAct = true;
                        Debug.Log("Local start turn");
                    }
                    else
                    {
                        CombatInfoBox.instance.AddText("It's " + ev["who"].ToString() + "'s turn!");
                    }
                }
                else if (ev["what"].ToString() == "died")
                {
                    if (ev["who"].ToString().StartsWith("monster_"))
                    {
                        int index = int.Parse(ev["who"].ToString().Split('_')[1]);
                        spawnedEnemies[index].Die();
                        if (currentTargetIndex == index)
                        {
                            currentTargetIndex = -1;
                            for (int i1 = 0; i1 < combatData.monsters.Count; i1++)
                            {
                                Dictionary<string, object> monster = (Dictionary<string, object>)combatData.monsters[i1];
                                int hp = int.Parse(monster["CurrentHp"].ToString());
                                if (hp > 0)
                                {                                    
                                    CombatUI.instance.SelectEnemyTarget(spawnedEnemies[i1]);
                                    currentTargetIndex = i1;
                                }
                            }
                            if (currentTargetIndex == -1)
                            {
                                UITargetMarker.instance.marker.enabled = false;
                            }
                        }
                        //spawnedEnemies[index].animator.SetTrigger("Death");
                        string killer = ((Dictionary<string, object>)ev["actionResult"])["killedBy"].ToString();
                        if (killer == PlayerPrefs.GetString("user"))
                            killer = "You";
                        CombatInfoBox.instance.AddText(killer + " defeated " + MonsterName + "!");
                        PlayerPrefs.SetInt("JustDefeatedMonster", 1);                        
                        yield return new WaitForSeconds(2.5f);
                        currentEventIndex = i;                        
                        break;
                    }
                    else if (ev["who"].ToString() == PlayerPrefs.GetString("user"))
                    {
                        //spawnedEnemy.animator.SetTrigger("Attack");
                        yield return new WaitForSeconds(0.5f);
                        CombatInfoBox.instance.AddText("You got knocked out... Leaving combat.");
                        CombatUI.instance.AnimatePlayerHp(PlayerStats.instance.currentHP);
                        yield return new WaitForSeconds(2.5f);
                        currentEventIndex = i;
                        
                        break;
                    }
                }
                else if (ev["what"].ToString() == "joined")
                {
                    CombatInfoBox.instance.AddText(ev["who"].ToString() + " joined you in combat!");
                    CombatUI.instance.AddPlayer(ev["who"].ToString());
                    UpdatePlayersValues();
                }
                else if (ev["what"].ToString() == "fled")
                {
                    CombatInfoBox.instance.AddText(ev["who"].ToString() + " left combat!");
                    CombatUI.instance.DeletePlayer();
                }
                else if (ev["what"].ToString().StartsWith("attack"))
                {
                    if (ev["who"].ToString().StartsWith("monster_"))
                    {
                        int dmg = 0;
                        Dictionary<string, object> actionResult = (Dictionary<string,object>)ev["actionResult"];//(Dictionary<string, object>)MiniJSON.Json.Deserialize(ev["actionResult"].ToString());
                        int atkrIndex = int.Parse(ev["who"].ToString().Split('_')[1]);
                        spawnedEnemies[atkrIndex].animator.SetTrigger("Attack");
                        yield return new WaitForSeconds(0.5f);
                        if (actionResult["attackedPlayer"].ToString() == PlayerPrefs.GetString("user"))
                        {
                            if (actionResult["hit"].ToString() != "miss")
                            {
                                int.TryParse(actionResult["damage"].ToString(), out dmg);
                                FloatingTextSpawner.Get("player").SpawnText("-" + dmg.ToString(), Color.red);
                                CombatUI.instance.AnimatePlayerHp(dmg);
                                CombatInfoBox.instance.AddText("Monster hits you for " + dmg.ToString());
                                iTween.PunchPosition(Camera.main.gameObject, Camera.main.transform.forward * 4.5f, 0.5f);
                            }
                            else
                            {
                                FloatingTextSpawner.Get("player").SpawnText("Miss!", Color.white);
                                CombatInfoBox.instance.AddText("Monster misses!");
                            }
                        }
                        else
                        {
                            if (actionResult["hit"].ToString() != "miss")
                            {
                                int.TryParse(actionResult["damage"].ToString(), out dmg);
                                FloatingTextSpawner.Get(actionResult["attackedPlayer"].ToString()).SpawnText("-" + dmg.ToString(), Color.red);
                                // CombatUI.instance.AnimatePlayerHp(dmg);
                                CombatInfoBox.instance.AddText("Monster hits " + actionResult["attackedPlayer"].ToString() + " for " + dmg.ToString());
                            }
                            else
                            {
                                FloatingTextSpawner.Get(actionResult["attackedPlayer"].ToString()).SpawnText("Miss!", Color.white);
                                CombatInfoBox.instance.AddText("Monster misses " + actionResult["attackedPlayer"].ToString() + "!");
                            }
                        }
                    }
                    else if (ev["who"].ToString() != PlayerPrefs.GetString("user"))
                    {
                        int dmg = 0;
                        Dictionary<string, object> actionResult = (Dictionary<string, object>)MiniJSON.Json.Deserialize(ev["actionResult"].ToString());
                        int monsterIndex = int.Parse(ev["what"].ToString().Split('_')[1]);

                        if (actionResult["hit"].ToString() != "miss")
                        {
                            int.TryParse(actionResult["dmg"].ToString(), out dmg);
                            CombatInfoBox.instance.AddText(ev["who"].ToString() + " hits for " + dmg.ToString());
                            FloatingTextSpawner.Get("enemy_"+monsterIndex.ToString()).SpawnText("[" + ev["who"].ToString() + "] -" + dmg.ToString(), Color.red);
                            spawnedEnemies[monsterIndex].animator.SetTrigger("TakeHit");

                        }
                        else
                        {
                            FloatingTextSpawner.Get("enemy_"+monsterIndex.ToString()).SpawnText("[" + ev["who"].ToString() + "] Miss!", Color.white);
                            CombatInfoBox.instance.AddText(ev["who"].ToString() + " misses!");
                        }
                    }
                }
                else if (ev["what"].ToString() == "usedItem")
                {
                    BaseItem itemused = Resources.Load<BaseItem>(Registry.assets.items[ev["actionResult"].ToString()]);
                    CombatInfoBox.instance.AddText(ev["who"].ToString() + " used " + itemused.Name + ".");
                }
                else if (ev["what"].ToString() == "healed")
                {
                    if (ev["who"].ToString() == PlayerPrefs.GetString("user"))
                    {
                        int amount = 0;
                        double a = 0;
                        double.TryParse(ev["actionResult"].ToString(), out a);
                        amount = (int)a;
                        FloatingTextSpawner.Get("player").SpawnText("+" + amount.ToString(), Color.green);
                        CombatInfoBox.instance.AddText("You healed for " + amount.ToString());
                        CombatUI.instance.AnimatePlayerHp(-amount, false);
                    }
                    else
                    {
                        FloatingTextSpawner.Get(ev["who"].ToString()).SpawnText("+" + ev["actionResult"].ToString(), Color.green);
                        CombatInfoBox.instance.AddText(ev["who"].ToString() + " healed for " + ev["actionResult"].ToString());
                        UpdatePlayersValues();
                    }
                }
                else if (ev["what"].ToString() == "appliedPoison")
                {
                    string user = ev["who"].ToString();
                    if (user == PlayerPrefs.GetString("user"))
                    {
                        CombatInfoBox.instance.AddText("You have been infected with poison!");
                        StatusController.instance.PoisonStatus.SetActive(true);
                    }
                    else
                    {
                        CombatInfoBox.instance.AddText(user + " has been infected with poison!");
                    }
                }
                else if (ev["what"].ToString() == "curedStatus")
                {
                    string who = ev["who"].ToString();
                    string message = who + " is cured from poison!";
                    if (ev["who"].ToString() == PlayerPrefs.GetString("user"))
                    {
                        message = "You are cured from poison!";
                        StatusController.instance.PoisonStatus.SetActive(false);
                    }
                    CombatInfoBox.instance.AddText(message);
                }
                else if (ev["what"].ToString() == "statusEffect")
                {
                    Dictionary<string, object> actionResult = (Dictionary<string, object>)MiniJSON.Json.Deserialize(ev["actionResult"].ToString());
                    string user = ev["who"].ToString();
                    if (user == PlayerPrefs.GetString("user"))
                    {
                        if (actionResult["effect"].ToString() == "poison")
                        {
                            int dmg = 0;
                            int.TryParse(actionResult["dmg"].ToString(), out dmg);

                            CombatInfoBox.instance.AddText("You suffer " + dmg.ToString() + " damage from poison.");
                            FloatingTextSpawner.Get("player").SpawnText("-" + dmg.ToString(), Color.yellow);
                            CombatUI.instance.AnimatePlayerHp(dmg);
                            if (actionResult.ContainsKey("playerDied"))
                            {
                                CombatInfoBox.instance.AddText("You died from poison...");
                                yield return new WaitForSeconds(2.5f);
                                currentEventIndex = i;
                                EndCombat();
                                break;
                            }
                            if (actionResult.ContainsKey("expired"))
                            {
                                StatusController.instance.PoisonStatus.SetActive(false);
                                CombatInfoBox.instance.AddText("The poison effect expired.");
                            }
                            //CombatInfoBox
                        }
                    }
                    else
                    {
                        if (actionResult["effect"].ToString() == "poison")
                        {
                            int dmg = 0;
                            int.TryParse(actionResult["dmg"].ToString(), out dmg);

                            CombatInfoBox.instance.AddText(user + " suffers " + dmg.ToString() + " damage from poison.");
                            FloatingTextSpawner.Get(user).SpawnText("-" + dmg.ToString(), Color.yellow);
                            UpdatePlayersValues();
                            //CombatInfoBox
                        }
                    }
                }

                currentEventIndex = i;
                yield return new WaitForSeconds(1.0f);
            }
        }
        yield return null;
        ParsingEvents = false;
    }

	public void PlayerDefend()
	{}

	public void PlayerCast()
	{}

	public void PlayerInventory()
	{
        if (CanAct)
        {
            //FloatingTextSpawner.Get ("player").SpawnText ("CIAO!", Color.blue);
            UIInventory.instance.Show();
        }
	}

	public void EndCombat()
	{
		Dictionary<string, object> values = new Dictionary<string, object> ();
		values.Add ("userName", PlayerPrefs.GetString ("user"));
		values.Add ("combatId", CombatManager.instance.combatId);
		/*Bridge.POST (Bridge.url + "Combat/RunFromCombat", values, (r) => {
			Debug.Log("[FLEE] " + r);
			ServerResponse resp = new ServerResponse(r);
			if (resp.status == ServerResponse.ResultType.Success)
		});*/        
		SceneManager.LoadScene ("Map");
	}

	IEnumerator PlayerWin(Dictionary<string,object> winData)
	{
        //spawnedEnemy.animator.SetTrigger ("Death");
        //		CombatInfoBox.instance.AddText ("You defeated " + MonsterName + "!");
        CombatEnded = true;
		PlayerPrefs.SetInt ("JustDefeatedMonster", 1);
        UICombatEnd.instance.Load(winData);
        UICombatEnd.instance.Show();
        yield return null;

        

	}
}
