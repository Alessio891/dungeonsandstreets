using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeItem : BaseItem
{
    public BaseRecipe Recipe;
    public BaseProfession Profession;

    public override Dictionary<string, object> Serialize()
    {
        Dictionary<string, object> data = base.Serialize();
        data.Add("recipeId", Recipe.UID);
        data.Add("professionId", Profession.UID);
        return data;
    }
}
