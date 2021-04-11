using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProfessions : MonoBehaviour
{

    public List<PlayerProfessionData> Professions = new List<PlayerProfessionData>();

    public static PlayerProfessions instance;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("user", PlayerPrefs.GetString("user"));
        data.Add("professionId", "e4ae8cee-c811-4d50-a692-f3b07d9b1c78");
        data.Add("recipeId", "f0908d61-eeef-48b7-8382-26aa638cc779");
        Bridge.POST(Bridge.url + "AddProfession", data, (r) =>
        {
            Debug.Log("[AddProfessionResp] " + r);
        });
        PlayerServerSync.instance.OnProfessionsUpdate += ProfessionSync;
    }
    private void OnDisable()
    {
        PlayerServerSync.instance.OnProfessionsUpdate -= ProfessionSync;
    }

    void ProfessionSync(List<object> data)
    {
        Professions.Clear();
        foreach(object o in data)
        {
            Dictionary<string, object> prof = (Dictionary<string, object>)o;
            PlayerProfessionData newProf = new PlayerProfessionData();
            newProf.Profession = Registry.assets.professions[prof["professionId"].ToString()];
            newProf.Value = int.Parse(prof["Value"].ToString());
            newProf.KnownRecipes = new List<BaseRecipe>();
            foreach(object r in (List<object>)prof["KnownRecipes"])
            {
                Dictionary<string, object> recipeData = (Dictionary<string, object>)r;
                BaseRecipe recipe = Registry.assets.recipes[recipeData["UID"].ToString()];
                newProf.KnownRecipes.Add(recipe);
            }
            Professions.Add(newProf);
        }
    }
}
