using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 { status: "error", "message": "Errore", incomingData: { blabla: "blublu" } }
 * */
using System.Linq;

[System.Serializable]
public class PlayerDataStruct  {
	public string user;
	public int hp, MaxHp, mana, MaxMana, Strength, Dexterity, Intelligence;
}

public class ServerResponse
{

	public const string SUCCESS_STRING = "success",
						ERROR_STRING = "error";

	public enum ResultType { Success = 0, Error };

	public ResultType status;
	public string message;
	public string errorCode;
	public object incomingData;
	
	public string rawMessage;

	public Dictionary<string, object> rawData;

	public Dictionary<string, object> GetIncomingDictionary()
	{
		return (Dictionary<string, object>)incomingData;
	}

	public List<Dictionary<string, object>> GetIncomingList()
	{
		return (List<Dictionary<string, object>>)incomingData;
	}

	public ServerResponse(string rawResponse)
	{
		
		rawMessage = rawResponse;
		rawData = new Dictionary<string, object> ();
		rawData = (Dictionary<string, object>)MiniJSON.Json.Deserialize (rawResponse);

		if (rawData.ContainsKey ("status")) {
			if (rawData ["status"].ToString () == SUCCESS_STRING) {
				status = ResultType.Success;
			} else {
				status = ResultType.Error;
				errorCode = rawData ["errorCode"].ToString ();
			}
		}

		if (rawData.ContainsKey ("message")) {
			message = rawData ["message"].ToString ();
		}

		if (rawData.ContainsKey ("incomingData") && rawData["incomingData"] != null) {
            if (rawData["incomingData"].GetType() == typeof(List<object>))
            {
                List<object> original = (List<object>)rawData["incomingData"];
                IEnumerable<Dictionary<string, object>> temp = original.Cast<Dictionary<string, object>>();
                List<Dictionary<string, object>> casted = new List<Dictionary<string, object>>();
                foreach (Dictionary<string, object> o in temp)
                    casted.Add(o);
                incomingData = casted;
            }
            else
            {
                try
                {
                    incomingData = (Dictionary<string, object>)rawData["incomingData"];
                } catch
                {
                    incomingData = rawData["incomingData"].ToString();
                }
            }
		}
	
	}		

}


public interface IServerSerializable
{
	Dictionary<string, object> Serialize();
	void Deserialize(Dictionary<string, object> serialized);
}