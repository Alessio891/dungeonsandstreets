using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueAsset : ScriptableObject
{
    public string UID;

    public string DialogueTitle;

    public string DialogueText;

    public List<DialogueResponse> Responses = new List<DialogueResponse>();

    public Dictionary<string, object> Serialize()
    {
        Dictionary<string, object> retVal = new Dictionary<string, object>();

        retVal.Add("UID", UID);
        retVal.Add("DialogueTitle", DialogueTitle);
        retVal.Add("DialogueText", DialogueText);
        List<object> responses = new List<object>();
        foreach(DialogueResponse r in Responses)
        {
            responses.Add(r.Serialize());
        }
        retVal.Add("Responses", responses);
        return retVal;
    }

    public void Deserialize(Dictionary<string, object> data)
    {
       // DialogueTitle = data["DialogueTitle"].ToString();
        DialogueText = data["DialogueText"].ToString();
        List<object> responses = (List<object>)data["Responses"];
        Responses = new List<DialogueResponse>();
        foreach(object o in responses)
        {
            Dictionary<string, object> respData = (Dictionary<string, object>)o;
            DialogueResponse resp = new DialogueResponse();
            resp.Deserialize(respData);
            Responses.Add(resp);
        }
    }

    [ContextMenu("Generate UID")]
    public void GenerateUID()
    {
        UID = System.Guid.NewGuid().ToString();
    }

    public void Upload() {
        Dictionary<string, object> data = new Dictionary<string, object>();

        data.Add("uid", UID);
        data.Add("data", MiniJSON.Json.Serialize(Serialize()));
        Bridge.POST(Bridge.url + "UploadDialogue", data, (r) =>
        {
            Debug.Log("[Upload Dialogue Response] " + r);
        });
    }
    public void Download() {
        Bridge.GET(Bridge.url + "DownloadDialogue?uid=" + this.UID, (r) =>
          {
              Debug.Log("[Dialogue Download Response] " + r);
              ServerResponse resp = new ServerResponse(r);
              if (resp.status == ServerResponse.ResultType.Success)
              {
                  Deserialize(resp.GetIncomingDictionary());//(Dictionary<string,object>)MiniJSON.Json.Deserialize(resp.incomingData.ToString()));
              }

          });
    }
}

[System.Serializable]
public class DialogueResponse
{
   
    public string Text;

    public List<DialogueActionData> Actions = new List<DialogueActionData>();

    public Dictionary<string, object> Serialize()
    {
        Dictionary<string, object> retVal = new Dictionary<string, object>();

        retVal.Add("Text", Text);

        List<object> actions = new List<object>();
        foreach(DialogueActionData d in Actions)
        {
            actions.Add(d.Serialize());
        }
        retVal.Add("Actions", actions);
        
        return retVal;
    }

    public void Deserialize(Dictionary<string, object> data)
    {
        Text = data["Text"].ToString();
        List<object> actions = (List<object>)data["Actions"];
        Actions = new List<DialogueActionData>();
        foreach(object o in actions)
        {
            Dictionary<string, object> d = (Dictionary<string, object>)o;
            DialogueActionData actionData = new DialogueActionData();
            actionData.Deserialize(d);
            Actions.Add(actionData);
        }

    }

}

[System.Serializable]
public class DialogueActionData
{
    public enum ResponseAction
    {
        Continue = 0,
        End,
        Reward
    }
    public ResponseAction action;
    public string data1;
    public string data2;

    public Dictionary<string, object> Serialize()
    {
        Dictionary<string, object> retVal = new Dictionary<string, object>();

        retVal.Add("action", action.ToString());
        retVal.Add("data1", data1);
        retVal.Add("data2", data2);

        return retVal;
    }

    public void Deserialize(Dictionary<string, object> data)
    {
        action = (ResponseAction)System.Enum.Parse(typeof(ResponseAction), data["action"].ToString());
        data1 = data["data1"].ToString();
        data2 = data["data2"].ToString();
    }
}