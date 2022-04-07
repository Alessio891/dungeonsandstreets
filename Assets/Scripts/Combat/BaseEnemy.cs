using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour {

	public Animator animator;

	public bool attacking = false;

	public Transform namePos;
	public Transform hpPos;

    public AudioClip AttackSound;
    public AudioClip GetHitSound;
    public AudioClip DieSound;

	public System.Action OnDeath;

    public int index = 0;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartTurn()
	{
		StartCoroutine (Turn ());
	}

	public virtual IEnumerator Turn()
	{
        Debug.Log("ENEMY TURN START");
		yield return new WaitForSeconds (0.5f);
		//CombatInfoBox.instance.AddText (CombatManager.instance.MonsterName + " is thinking...");
		Dictionary<string, object> values = new Dictionary<string, object> ();
		values.Add ("userName", PlayerPrefs.GetString ("user"));
		values.Add ("combatId", CombatManager.instance.combatId);
		bool receivedResponse = false;
		bool isPlayerDead = false;
		Dictionary<string, object> data = new Dictionary<string, object> ();
		Bridge.POST (Bridge.url + "Combat/MonsterAttack", values, (r) => {
			attacking = true;
			Debug.Log("[Monster Attack REsponse] " + r);
			ServerResponse resp = new ServerResponse(r);
			data = resp.GetIncomingDictionary();
			CombatManager.instance.combatData.Deserialize((Dictionary<string, object>)data["combatData"]);
			receivedResponse = true;

		});

		while (!receivedResponse) {
			yield return new WaitForEndOfFrame ();
		}

		animator.SetTrigger ("Attack");
		yield return new WaitForSeconds (1.0f);
		attacking = false;
        string infoMess = "";
		if (data["hit"].ToString() == "success")
		{
            infoMess = "Enemy hits for ";
            Debug.Log("Monster hit player");
			int dmg = 0;
			int playerHp;
			isPlayerDead = data.ContainsKey("playerDead");
			//CombatInfoBox.instance.AddText(CombatManager.instance.MonsterName + " hits you for " + dmg.ToString() + " damage!");
			int.TryParse(data["dmg"].ToString(), out dmg);
			int.TryParse(data["playerHp"].ToString(), out playerHp);
            //PlayerServerSync.instance.SyncStats ();
            infoMess += dmg.ToString() + "!";            
            CombatUI.instance.UpdatePlayerHp(playerHp);
		} else
		{
            infoMess = "Monster misses!";
			FloatingTextSpawner.Get("player").SpawnText("MISS", Color.white);
			//		CombatInfoBox.instance.AddText(CombatManager.instance.MonsterName + " misses!");
		}
        CombatInfoBox.instance.AddText(infoMess);
		if (!isPlayerDead)
			CombatManager.instance.currentTurn = CombatManager.TurnType.Player;
		else {
			FloatingTextSpawner.Get ("player").SpawnText ("..Faint..", Color.black);
			yield return new WaitForSeconds (3.0f);
			CombatManager.instance.EndCombat ();
		}
	}

    public void Die()
    {
		if (OnDeath != null)
			OnDeath();
        StartCoroutine(fadeOut());
    }

    IEnumerator fadeOut()
    {
        foreach(Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.material.shader = Shader.Find("Effects/Dissolve");
            r.material.SetTexture("_NoiseTex", CombatManager.instance.DissolveTexture);
            r.material.SetFloat("_Cutoff", 0);
        }
        float val = 0.0f;
        bool playedSound = false;
        while (val < 0.9f)
        {
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                r.material.SetFloat("_Cutoff", val);
                
            }
            if (val > 0.4f && !playedSound)
            {
                SoundManager.instance.PlayUI(CombatManager.instance.DissolveSound);
                playedSound = true;
            }
            val += 0.5f * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        yield return null;
    }

    private void OnMouseDown()
    {
        CombatManager.instance.currentTargetIndex = index;
        CombatUI.instance.SelectEnemyTarget(this);
    }
}
