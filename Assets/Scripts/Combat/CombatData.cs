using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatData {
	
	public string UID;
	public List<object> players;
	public List<object> monsters;

	public string featureReferenceId;
	public int currentPlayerIndex;
	public int currentMonsterIndex;

	public int localPlayerIndex;

    public List<Dictionary<string, object>> events;

	public CombatManager.TurnType turn;



	public string GetPlayerUserById(int id)
	{
		List<object> players = (List<object>)this.players;
		if (id < players.Count) {
			Dictionary<string, object> player = (Dictionary<string, object>)players [id];
			return player ["user"].ToString ();
		}
		return "";
	}

	public void Deserialize(Dictionary<string, object> data)
	{        
		UID = data ["UID"].ToString ();
		featureReferenceId = data ["featureReferenceId"].ToString ();
		int.TryParse (data ["currentPlayerId"].ToString (), out currentPlayerIndex);
		int.TryParse (data ["currentMonsterId"].ToString (), out currentMonsterIndex);
		bool isPlayerTurn = (bool)data ["isPlayerTurn"];
		players = (List<object>)data ["playersId"];
		monsters = (List<object>)data ["monstersId"];
		if (isPlayerTurn) {
			if (GetPlayerUserById(currentPlayerIndex) == PlayerPrefs.GetString("user"))
			{
				turn = CombatManager.TurnType.Player;
			} else
				turn = CombatManager.TurnType.Neither;
		} else
			turn = CombatManager.TurnType.Enemy;

        events = new List<Dictionary<string, object>>();
        List<object> evs = (List<object>)data["events"];
        foreach(object o in evs)
        {
            Dictionary<string, object> ev = (Dictionary<string, object>)o;
            events.Add(ev);
        }

       // if (CombatManager.instance.combatData.turn == CombatManager.TurnType.Enemy)
         //   CombatManager.instance.spawnedEnemy.StartTurn();

    }

}
