using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public static class Bridge {

    public static bool DebugMode = true;


    public class Data
	{
		public enum Result
		{
			Success = 0,
			Error
		};

		public Result result;

		public string raw;
		public string rawMessage;
		public string errorCode;
		public const string Separator = "###";
		public List<Dictionary<string, string>> values;


		public Data()
		{}

		public void ParseMapResponse(string response)
		{
			
			values = new List<Dictionary<string, string>> ();
			string[] lines = response.Split (new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
			if (lines.Length <= 1)
				return;
			int count = -1;
			for (int i = 1; i < lines.Length; i++) {
			//	Debug.Log ("Process line " + lines [i]);
				if (lines[i].StartsWith("[0]")) {//!lines [i].StartsWith ("[" + count.ToString () + "]")) {
					count++;
					Dictionary<string, string> v = new Dictionary<string, string> ();
					string raw = lines [i].Split (']') [1].Trim ();
					string[] rawSplit = raw.Split (new string[] { Separator }, System.StringSplitOptions.RemoveEmptyEntries);
					if (rawSplit.Length <= 1) {
						Debug.Log ("Couldnt split for - :( [ " + raw + " ]");
						continue;
					}
					v.Add (rawSplit [0].Trim (), rawSplit [1].Trim ());
					values.Add (v);
				} else {										
					string raw = lines [i].Split (']') [1].Trim ();
					string[] rawSplit = raw.Split (new string[] { Separator }, System.StringSplitOptions.RemoveEmptyEntries);
					if (rawSplit.Length <= 1) {
						Debug.Log ("Couldnt split for - :(");
						continue;
					}
					values[count].Add (rawSplit [0].Trim (), rawSplit [1].Trim ());
				}

			}
		}


		public Data(string response)
		{
			if (response.StartsWith("[Success]"))
			{
				result = Result.Success;
			} else
			{
				result = Result.Error;
			}
			rawMessage = response.Split(']')[1];
			raw = response;
		}

	}

	static RoutineRunner _rr;

#if UNITY_EDITOR
    public static string IP = "http://localhost:8080/";// "https://aqueous-beyond-72197.herokuapp.com/";
#else
    public static string IP = "http://93.41.236.162:8080/"; //"https://aqueous-beyond-72197.herokuapp.com/";
#endif
    public static string url { get { return IP + "DungeonsAndStreets/"; } }

	public static int routinesRunning = 0;

	public static RoutineRunner routineRunner
	{
		get { 
			if (_rr == null) {
                _rr = GameObject.FindObjectOfType<RoutineRunner>();                                
                if (_rr == null)
                {
                    GameObject g = new GameObject("[Routine Runner]");

                    _rr = g.AddComponent<RoutineRunner>();                    
                }
			}
			return _rr.GetComponent<RoutineRunner>();
		}
	}

	public static void GET(string url, System.Action<string> callback = null)
	{
		routineRunner.StartCoroutine (GETReq (url, callback));
	}

    static IEnumerator GetTextureRoutine(string url, System.Action<Texture> callback = null)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            if (callback != null)
                callback(myTexture);
        }
    }

    static IEnumerator GETReq(string url, System.Action<string> callback = null)
	{
		routinesRunning++;
		WWW www = new WWW (url);
		yield return www;
		string resp = www.text;
		if (www.error != null) {
			resp = "{\"status\":\"error\", \"errorCode\":\"0\", \"message\":\""+www.error+"\"}";
		}
		if (callback != null)
			callback (resp);		
		routinesRunning--;
	}

    public static void DispatchError(string message, bool logOut = false)
    {
        if (GameManager.instance != null)
            GameManager.instance.CommunicationWithServerEnstabilished = false;
        PopupManager.ShowPopup("Error With Server", message, (s, p) => p.Close(), () => {
            if (logOut)
            {
                SceneManager.LoadScene("Login");
            }
        });
    }

	public static void POST(string url, Dictionary<string, object> values, System.Action<string> callback = null)
	{
		routineRunner.StartCoroutine (POSTReq (url, values, callback));
	}

    public static void GetTexture(string url, System.Action<Texture> callback = null)
    {
        routineRunner.StartCoroutine(GetTextureRoutine(url, callback));
    }

	static IEnumerator POSTReq(string url, Dictionary<string, object> values, System.Action<string> callback = null)
	{
        if (DebugMode)
        {
            string debugString = "[STARTING POST REQ] url: " + url + "\n";
            debugString += "Values:\n";
            foreach(KeyValuePair<string, object> p in values)
            {
                debugString += p.Key + ":" + p.Value.ToString()+"\n";
            }
            Debug.Log(debugString);
        }
		routinesRunning++;
		WWWForm form = new WWWForm ();
		foreach (KeyValuePair<string, object> pair in values) {
			form.AddField (pair.Key, pair.Value.ToString ());
		}

		WWW www = new WWW (url, form);
		yield return www;
		string resp = www.text;
		if (www.error != null) {
			resp = "{\"status\":\"error\", \"errorCode\":\"0\", \"message\":\""+www.error+"\"}";
		}
		if (callback != null)
			callback (resp);
		routinesRunning--;
	}

	public static void UploadMonster(BaseMonster feature, System.Action<string> callback = null)
	{
		Dictionary<string, object> data = new Dictionary<string, object> ();
		data.Add ("UID", feature.UID);
		data.Add ("data", MiniJSON.Json.Serialize (feature.Serialize ()));

		POST (url + "UploadMonster", data, callback);
	}

	public static void DownloadMonster(BaseMonster feature, System.Action<string> callback = null)
	{		
		GET (url + "DownloadMonster?UID=" + feature.UID, callback);
	}


	public static void UploadFeature(FeatureAsset feature, System.Action<string> callback = null)
	{
		Dictionary<string, object> data = new Dictionary<string, object> ();
		data.Add ("UID", feature.UID);
        Dictionary<string, object> dataDict = feature.Serialize();        
        string d = MiniJSON.Json.Serialize(dataDict);
        Debug.Log("JSON: \n" + d);
        data.Add ("data", d);

		POST (url + "UploadFeature", data, callback);
	}

	public static void DownloadFeature(FeatureAsset feature, System.Action<string> callback = null)
	{
		GET (url + "DownloadFeature?UID=" + feature.UID, callback);
	}

	public static void UpdateItemOnServer(BaseItem item, System.Action<string> callback = null)
	{
		Dictionary<string, object> data = new Dictionary<string, object> ();
		data.Add ("uid", item.ItemID);
		data.Add ("data", MiniJSON.Json.Serialize (item.Serialize ()));
		
		POST (url + "UploadItem", data, callback);
	}
    public static void UpdateQuestOnServer(BaseQuest item, System.Action<string> callback = null)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("UID", item.UID);
        data.Add("data", MiniJSON.Json.Serialize(item.Serialize()));

        POST(url + "UploadQuest", data, callback);
    }
    public static void DownloadQuestFromServer(BaseQuest item, System.Action<string> callback = null)
    {
        GET(url + "DownloadQuest?uid=" + item.UID, callback);
    }


    public static void DownloadItemFromServer(BaseItem item, System.Action<string> callback = null)
	{
		GET (url + "DownloadItem?uid=" + item.ItemID, callback);
	}

	public static Data ParseResponse(string response)
	{
		return new Data(response);
	}
}
