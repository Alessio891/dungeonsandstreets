using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIProfessions : MonoBehaviour
{
    public Image ProfessionIcon_1, ProfessionIcon_2, ProfessionProgress_1, ProfessionProgress_2;
    public Text ProfessionName_1, ProfessionName_2, ProfessionValue_1, ProfessionValue_2;


    public GameObject RecipeContents;
    public Transform RecipesList;

    public GridLayoutGroup CraftingIngredientsList;
    public Image CratingResultIcon;
    public Text CraftingResultName, CraftingResultAmount, CraftingTimesText;
    public UIRecipeIngredient ResultItem;

    public Image CraftProgressBar;    

    public GameObject CraftingContents;

    CanvasGroup group;

    public Color NoPointsColor;
    public Color EasyColor, MediumColor, HardColor;
    public Sprite NoProfessionSprite;

    public UIRecipeEntry RecipePrefab;
    public UIRecipeIngredient CraftIngredientPrefab;

    List<UIRecipeEntry> recipeEntries = new List<UIRecipeEntry>();

    List<UIRecipeIngredient> craftIngredientEntries = new List<UIRecipeIngredient>();
    public UIRecipeEntry currentRecipeEntry;
    int currentProfIndex = 0;
    public static UIProfessions instance;
    private void Awake()
    {
        instance = this;
        group = GetComponent<CanvasGroup>();
    }
    private void Start()
    {
        Hide();
    }    
    void SyncProfessionsValues()
    {
        if (PlayerProfessions.instance.Professions.Count > 0)
        {
            if (PlayerProfessions.instance.Professions[0] != null)
            {
                ProfessionIcon_1.sprite = PlayerProfessions.instance.Professions[0].Profession.Icon;
                ProfessionName_1.text = PlayerProfessions.instance.Professions[0].Profession.Name;
                ProfessionProgress_1.enabled = true;
                ProfessionProgress_1.fillAmount = ((float)PlayerProfessions.instance.Professions[0].Value) / 150.0f;
                ProfessionValue_1.text = PlayerProfessions.instance.Professions[0].Value.ToString() + "/150";
            }
            else
            {
                ProfessionIcon_1.sprite = NoProfessionSprite;
                ProfessionName_1.text = "None";
                ProfessionProgress_1.enabled = false;
                ProfessionValue_1.enabled = false;
            }
        }
        else
        {
            ProfessionIcon_1.sprite = NoProfessionSprite;
            ProfessionName_1.text = "None";
            ProfessionProgress_1.enabled = false;
            ProfessionValue_1.enabled = false;
        }
        if (PlayerProfessions.instance.Professions.Count > 1)
        {
            if (PlayerProfessions.instance.Professions[1] != null)
            {
                ProfessionIcon_2.sprite = PlayerProfessions.instance.Professions[1].Profession.Icon;
                ProfessionName_2.text = PlayerProfessions.instance.Professions[1].Profession.Name;
                ProfessionProgress_2.enabled = true;
                ProfessionValue_2.text = PlayerProfessions.instance.Professions[1].Value.ToString() + "/150";
                ProfessionProgress_2.fillAmount = ((float)PlayerProfessions.instance.Professions[1].Value) / 150.0f;
            }
            else
            {
                ProfessionIcon_2.sprite = NoProfessionSprite;
                ProfessionName_2.text = "None";
                ProfessionProgress_2.enabled = false;
                ProfessionValue_2.enabled = false;
            }
        }
        else
        {
            ProfessionIcon_2.sprite = NoProfessionSprite;
            ProfessionName_2.text = "None";
            ProfessionProgress_2.enabled = false;
            ProfessionValue_2.enabled = false;
        }
    }
    public void Show() {
        group.alpha = 1.0f;
        group.interactable = true;
        group.blocksRaycasts = true;
        SyncProfessionsValues();
        
    }
    public void Hide() {
        group.alpha = 0.0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    public void Load(int profIndex)
    {
        currentProfIndex = profIndex;
        if (PlayerProfessions.instance.Professions[profIndex].Profession.type == BaseProfession.Type.Gathering)
        {
            RecipeContents.SetActive(false);
            CraftingContents.SetActive(false);
            return;
        }
        RecipeContents.SetActive(true);
        
        CraftingContents.gameObject.SetActive(false);
        foreach (UIRecipeEntry e in recipeEntries)
            Destroy(e.gameObject);

        recipeEntries.Clear();
        PlayerProfessionData profession = PlayerProfessions.instance.Professions[profIndex];

        foreach(BaseRecipe r in profession.KnownRecipes)
        {
            UIRecipeEntry e = GameObject.Instantiate<UIRecipeEntry>(RecipePrefab);
            e.transform.SetParent(RecipesList);
            e.transform.localPosition = Vector3.zero;
            e.transform.localScale = Vector3.one;
            e.Init(r, profession.Value-r.MinimumNeededValue);
            recipeEntries.Add(e);
        }
    }
    public void SelectRecipe(UIRecipeEntry recipeEntry)
    {
        CraftingContents.gameObject.SetActive(true);
        foreach (UIRecipeIngredient i in craftIngredientEntries)
            Destroy(i.gameObject);
        craftIngredientEntries.Clear();
        foreach(BaseRecipe.RecipeItemData ingredient in recipeEntry.recipe.Ingredients)
        {
            UIRecipeIngredient e = GameObject.Instantiate<UIRecipeIngredient>(CraftIngredientPrefab);
            e.transform.SetParent(CraftingIngredientsList.transform);
            e.transform.localScale = Vector3.one;
            e.transform.localPosition = Vector3.zero;

            e.Init(ingredient);
            craftIngredientEntries.Add(e);
        }
        currentRecipeEntry = recipeEntry;
        ResultItem.Init(recipeEntry.recipe.Result[0]);

    }
    public void Craft() {
        bool canCraft = true;
        foreach(BaseRecipe.RecipeItemData ingr in currentRecipeEntry.recipe.Ingredients)
        {
            if (!PlayerInventory.instance.HasItem(ingr.Item.ItemID, ingr.Amount))
            {
                canCraft = false;
                break;
            }
        }
        if (canCraft)
        {
            StartCoroutine(craftRoutine());
        } else
        {
            Debug.Log("NO MATERIALS");
        }
    }

    public void MoreCraft()
    {
        int amount = int.Parse(CraftingTimesText.text);
        amount++;
        CraftingTimesText.text = amount.ToString();
    }
    public void LessCraft()
    {
        int amount = int.Parse(CraftingTimesText.text);
        amount--;
        if (amount < 1)
            amount = 1;
        CraftingTimesText.text = amount.ToString();
    }

    IEnumerator craftRoutine()
    {
        while(CraftProgressBar.fillAmount < 0.5f)
        {
            CraftProgressBar.fillAmount += 0.5f * Time.deltaTime;
            yield return null;
        }
        yield return null;
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("user", PlayerPrefs.GetString("user"));
        data.Add("recipeId", currentRecipeEntry.recipe.UID);
        data.Add("professionId", PlayerProfessions.instance.Professions[currentProfIndex].Profession.UID);
        bool craftResultReceived = false;
        int resultType = 0; // 0 = success | 1 = failed | 2 = not enough materials
        Bridge.POST(Bridge.url + "CraftItem", data, (r) =>
           {
               Debug.Log("[CraftItem Response] " + r);
               ServerResponse resp = new ServerResponse(r);
               if (resp.status == ServerResponse.ResultType.Success)
               {
                   resultType = 0;
                   PlayerServerSync.instance.SyncStats();
               }
               else
               {
                   if (resp.errorCode == "1001")
                   {
                       resultType = 1;
                   } else if (resp.errorCode == "1000")
                   {
                       resultType = 2;
                   }
               }
               craftResultReceived = true;
           });
        while (!craftResultReceived)
        {
            yield return null;
        }
        while (CraftProgressBar.fillAmount < 1.0f)
        {
            CraftProgressBar.fillAmount += 0.5f * Time.deltaTime;
            yield return null;
        }
        CraftProgressBar.fillAmount = 0;

        if (resultType == 0)
        {
            Debug.Log("CRAFT SUCCESSFUL");
            OnScreenNotification.instance.Show("Craft successful!", currentRecipeEntry.recipe.Result[0].Item.sprite);
        }
        else if (resultType == 1)
        {
            Debug.Log("CRAFT FAILED!");
        }
        else if (resultType == 2)
        {
            Debug.Log("NOT ENOUGH MATERIALS");
        }
        Load(currentProfIndex);
        SyncProfessionsValues();
        CraftingContents.SetActive(true);
        yield return null;
    }
}
