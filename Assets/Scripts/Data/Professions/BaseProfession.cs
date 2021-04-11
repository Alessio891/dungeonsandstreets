using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProfession : ScriptableObject, IServerSerializable
{
    public const int MAX_LEVEL = 150;

    public string UID;

    public string Name;    

    public enum Type { Gathering = 0, Crafting }
    public Type type = Type.Crafting;
    public Sprite Icon;
    public List<BaseRecipe> Recipes = new List<BaseRecipe>();


    public virtual void Deserialize(Dictionary<string, object> serialized)
    {
        Name = serialized["Name"].ToString();
    }

    public virtual Dictionary<string, object> Serialize()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("UID", UID);
        data.Add("Name", Name);
        return data;
    }
    [ContextMenu("Generate UID")]
    public void GenerateUID()
    {
        UID = System.Guid.NewGuid().ToString();
    }

    [ContextMenu("Download")]
    public void Download()
    {
        Bridge.GET(Bridge.url + "DownloadProfession?UID=" + this.UID, (r) =>
        {
            Debug.Log("[DownloadProfession response] " + r);
            ServerResponse resp = new ServerResponse(r);
            if (resp.status == ServerResponse.ResultType.Success)
            {
                this.Deserialize(resp.GetIncomingDictionary());
            }
        });
    }

    [ContextMenu("Upload")]
    public void Upload()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("UID", this.UID);
        data.Add("data", MiniJSON.Json.Serialize(this.Serialize()));        
        Bridge.POST(Bridge.url + "UploadProfession", data, (r) =>
        {
            Debug.Log("[UploadProfession Response] " + r);
        });
    }
}