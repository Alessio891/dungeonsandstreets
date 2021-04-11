using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPOI : ScriptableObject
{
    public string UID;
    public string POIName;
    public string POIText;
    public Sprite NpcAvatar;

    public List<BaseQuest> PossibleQuests;    

    //public List<>

    public string Type = "";
    public List<string> Categories = new List<string>();

    public List<string> spawnTags;
    public MapPOIComponent prefab;

    public DialogueAsset Dialogue;

    public virtual Dictionary<string, object> Serialize()
    {
        Dictionary<string, object> retVal = new Dictionary<string, object>();

        retVal.Add("UID", UID);
        retVal.Add("POIName", POIName);
        retVal.Add("POIDescription", POIText);
        retVal.Add("spawnTags", spawnTags);
        retVal.Add("Type", Type);
        retVal.Add("Categories", Categories);
        retVal.Add("DialogueID", (Dialogue != null) ? Dialogue.UID : "null");
        List<string> quests = new List<string>();
        foreach(BaseQuest q in PossibleQuests)
        {
            quests.Add(q.UID);
        }
        retVal.Add("QuestsID", quests);
        return retVal;
    }
    public virtual void Deserialize(Dictionary<string, object> serialized)
    {
  
        POIName = serialized["POIName"].ToString();
        Dialogue = Registry.assets.dialogues[serialized["DialogueID"].ToString()];
        Type = serialized["Type"].ToString();
        Categories.Clear();
        foreach(object o in (List<object>)serialized["Categories"])
        {
            Categories.Add(o.ToString());
        }
       
        PossibleQuests.Clear();
        List<object> quests = (List<object>)serialized["QuestsID"];
        Debug.Log("Counting " + quests.Count + " quests");
        foreach(object q in quests)
        {
            Debug.Log("QuestID: " + q.ToString());
            PossibleQuests.Add(Registry.assets.quests[q.ToString()]);
        }
    }

    [ContextMenu("Upload POI")]
    public virtual void Upload()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("UID", UID);
        data.Add("data", MiniJSON.Json.Serialize(Serialize()));

        Bridge.POST(Bridge.url + "POI/UploadPOI", data, (r) => {
            Debug.Log(r);
            ServerResponse resp = new ServerResponse(r);
            if (resp.status == ServerResponse.ResultType.Error)
            {
                Debug.Log("ERROR FROM SERVER: " + resp.message);
            }
            else
            {
                Debug.Log("SUCCESS: " + resp.message);
            }
        });
    }

    [ContextMenu("Generate UID")]
    public void GenerateUID()
    {
        UID = System.Guid.NewGuid().ToString();
    }


    [ContextMenu("Download POI")]
    public void Download()
    {
        Bridge.GET(Bridge.url + "POI/DownloadPOI?UID=" + UID, (r) => {
            Debug.Log(r);
            ServerResponse resp = new ServerResponse(r);
            if (resp.status == ServerResponse.ResultType.Error)
            {
                Debug.Log("Error from server: " + resp.message);
            }
            else
            {
                Debug.Log("Success.");
                this.Deserialize(resp.GetIncomingDictionary());
            }
        });
    }
}
