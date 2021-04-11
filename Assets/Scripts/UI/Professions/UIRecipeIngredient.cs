using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRecipeIngredient : MonoBehaviour
{
    public Image Icon;
    public Text Amount, Name;

    public void Init(BaseRecipe.RecipeItemData ingredient)
    {
        Icon.sprite = ingredient.Item.sprite;
        Amount.text = ingredient.Amount.ToString();
        if (Name != null)
            Name.text = ingredient.Item.Name;
    }
}
