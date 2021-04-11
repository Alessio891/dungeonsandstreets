using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRecipe : ScriptableObject, IServerSerializable
{
    [System.Serializable]
    public class RecipeItemData
    {
        public BaseItem Item;
        public int Amount;
    }
    public string UID;
    public List<RecipeItemData> Ingredients = new List<RecipeItemData>();
    public List<RecipeItemData> Result = new List<RecipeItemData>();

    public int MinimumNeededValue = 0;

    public Dictionary<string, object> Serialize()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("MinimumNeededValue", MinimumNeededValue);
        data.Add("UID", UID);
        List<Dictionary<string, object>> ingredientsData = new List<Dictionary<string, object>>();
        foreach(RecipeItemData d in Ingredients)
        {
            Dictionary<string, object> ingr = new Dictionary<string, object>();
            ingr.Add("itemId", d.Item.UID);
            ingr.Add("amount", d.Amount);
            ingredientsData.Add(ingr);
        }
        data.Add("Ingredients", ingredientsData);

        List<Dictionary<string, object>> resultsData = new List<Dictionary<string, object>>();
        foreach (RecipeItemData d in Result)
        {
            Dictionary<string, object> ingr = new Dictionary<string, object>();
            ingr.Add("itemId", d.Item.UID);
            ingr.Add("amount", d.Amount);
            resultsData.Add(ingr);
        }
        data.Add("Results", resultsData);

        return data;
    }

    public void Deserialize(Dictionary<string, object> serialized)
    {
        MinimumNeededValue = int.Parse(serialized["MinimumNeededValue"].ToString());

        List<object> ingredients = (List<object>)serialized["Ingredients"];
        List<object> results = (List<object>)serialized["Results"];

        Ingredients.Clear();
        foreach(object o in ingredients)
        {
            Dictionary<string, object> ingrData = (Dictionary<string, object>)o;
            RecipeItemData i = new RecipeItemData();
            i.Amount = int.Parse(ingrData["amount"].ToString());
            i.Item = Resources.Load<BaseItem>(Registry.assets.items[ingrData["itemId"].ToString()]);
            Ingredients.Add(i);
        }
        Result.Clear();
        foreach (object o in results)
        {
            Dictionary<string, object> ingrData = (Dictionary<string, object>)o;
            RecipeItemData i = new RecipeItemData();
            i.Amount = int.Parse(ingrData["amount"].ToString());
            i.Item = Resources.Load<BaseItem>(Registry.assets.items[ingrData["itemId"].ToString()]);
            Result.Add(i);
        }
    }

    [ContextMenu("Download")]
    public void Download()
    {
        Bridge.GET(Bridge.url + "DownloadProfession?UID=" + this.UID + "&recipe=true", (r) =>
        {
            Debug.Log("[DownloadRecipe response] " + r);
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
        data.Add("recipe", true);
        Bridge.POST(Bridge.url + "UploadProfession", data, (r) =>
        {
            Debug.Log("[UploadRecipe Response] " + r);
        });
    }
    [ContextMenu("Generate UID")]
    public void GenerateUID()
    {
        UID = System.Guid.NewGuid().ToString();
    }
}
