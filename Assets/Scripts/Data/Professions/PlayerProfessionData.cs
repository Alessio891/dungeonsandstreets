using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerProfessionData
{
    public BaseProfession Profession;
    public int Value;
    public List<BaseRecipe> KnownRecipes;
}
