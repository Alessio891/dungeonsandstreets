using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRecipeEntry : MonoBehaviour
{
    public BaseRecipe recipe;
    public UIRecipeIngredient ingredientPrefab;
    public Image BG;
    public Text ResultName, PointsGain;
    public GridLayoutGroup IngredientsList;

    public void Init(BaseRecipe recipe, int deltaValue)
    {
        if (ResultName != null)
            ResultName.text = recipe.Result[0].Item.Name;
        PointsGain.text = "1pt";
        
        foreach(BaseRecipe.RecipeItemData ingredient in recipe.Ingredients)
        {
            UIRecipeIngredient e = GameObject.Instantiate<UIRecipeIngredient>(ingredientPrefab);
            e.transform.SetParent(IngredientsList.transform);
            e.transform.localScale = Vector3.one;
            e.transform.localPosition = Vector3.zero;

            e.Init(ingredient);
        }

        if (deltaValue >= 0 && deltaValue < 2)
            BG.color = UIProfessions.instance.HardColor;
        else if (deltaValue >= 2 && deltaValue < 5)
            BG.color = UIProfessions.instance.MediumColor;
        else if (deltaValue >= 5 && deltaValue < 9)
            BG.color = UIProfessions.instance.EasyColor;
        else
            BG.color = UIProfessions.instance.NoPointsColor;


        this.recipe = recipe;
    }

    public void Select()
    {
        UIProfessions.instance.SelectRecipe(this);
    }
}
