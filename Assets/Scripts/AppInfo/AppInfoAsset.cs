using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class AppInfoEntry
{
	public string key, value;
}

public class AppInfoAsset : ScriptableObject, IServerSerializable {

	public string Name;
	public List<AppInfoEntry> data;

	public string this [string key] { get { return data.Where<AppInfoEntry> ((s) =>  s.key == key).FirstOrDefault<AppInfoEntry> ().value; } }

    public string Get(string key)
    {
        foreach (AppInfoEntry e in data)
        {
            if (e.key == key)
                return e.value;
        }
        return "";
    }

    public void Set(string key, string value)
    {
        if (string.IsNullOrEmpty(Get(key)))
        {
            AppInfoEntry e = new AppInfoEntry();
            e.key = key;
            e.value = value;
        }
        else
        {
            foreach (AppInfoEntry e in data)
            {
                if (e.key == key)
                {
                    e.value = value;
                    break;
                }
            }
        }
    }

	public Dictionary<string, object> Serialize ()
	{
		Dictionary<string, object> retVal = new Dictionary<string, object> ();
		retVal.Add ("name", Name);

		Dictionary<string, object> uploadData = new Dictionary<string, object> ();

		foreach (AppInfoEntry e in data) {
			uploadData.Add (e.key, e.value);
		}
			
		retVal.Add ("data", uploadData);
		return retVal;
	}

	public void Deserialize (Dictionary<string, object> serialized)
	{
		data = new List<AppInfoEntry> ();
		Name = serialized ["name"].ToString();
		Dictionary<string, object> downloadData = (Dictionary<string, object>)serialized ["data"];
		foreach (KeyValuePair<string, object> pair in downloadData) {
			if (pair.Key == "name")
				continue;
			AppInfoEntry e = new AppInfoEntry ();
			e.key = pair.Key; e.value = pair.Value.ToString ();
			data.Add (e);
		}
	}		

	[ContextMenu("Upload AppInfo")]
	public void Upload()
	{
		string serialized = MiniJSON.Json.Serialize (Serialize ());
		Dictionary<string, object> values = new Dictionary<string, object> ();
		values.Add ("name", Name);
		values.Add ("data", serialized);
		Debug.Log ("Sending " + serialized);
		Bridge.POST (Bridge.url + "UploadAppInfo", values, (r) => {
			Debug.Log(r);
			ServerResponse resp = new ServerResponse(r);
			if (resp.status == ServerResponse.ResultType.Error)
			{
				Debug.Log("ERROR FROM SERVER: " + resp.message);
			} else
			{
				Debug.Log("SUCCESS: " + resp.message);
			}
		});
	}

	[ContextMenu("Download AppInfo")]
	public void Download(System.Action cb = null)
	{
		Bridge.GET (Bridge.url + "DownloadAppInfo?name="+Name, (r) => {
			Debug.Log(r);
			ServerResponse resp = new ServerResponse(r);
			if (resp.status == ServerResponse.ResultType.Error)
			{
				Debug.Log("Error from server: " + resp.message);
			} else
			{
				Debug.Log("Success.");
				this.Deserialize(resp.GetIncomingDictionary());
                if (cb != null)
                    cb();
			}
		});
	}
}
