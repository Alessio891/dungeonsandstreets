using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelUp : MonoBehaviour {

    public Text levelupText;

    public Text PointsText;

    public Text HPText, MPText;

    public CanvasGroup canvasGroup;

    public static UILevelUp instance;

    public int Points = 5;

    public UILevelUpStatEntry Strength;
    public UILevelUpStatEntry Dexterity;
    public UILevelUpStatEntry Toughness;
    public UILevelUpStatEntry Intelligence;
    public UILevelUpStatEntry Luck;

    public bool Active = false;   

    void Awake() { instance = this; }

    
	// Use this for initialization
	void Start () {
        Hide();
	}

    // Update is called once per frame
    void Update()
    {
        HPText.text = (PlayerStats.instance.level + 1 + 10 + (PlayerStats.instance.toughness / 2)).ToString();
        PointsText.text = "Points to spend: " + Points.ToString() + "/5";
    }

    public void Load()
    {
        Strength.OriginalAmount = PlayerStats.instance.strength;
        Dexterity.OriginalAmount = PlayerStats.instance.dexterity;
        Toughness.OriginalAmount = PlayerStats.instance.toughness;
        Intelligence.OriginalAmount = PlayerStats.instance.intelligence;
        Luck.OriginalAmount = PlayerStats.instance.luck;
        levelupText.text = "You are stronger than ever! The time spent adventuring payed off, and now you are LEVEL " + (PlayerStats.instance.level + 1).ToString();
        Points = 5;
        Active = true;
    }
   

    public void Show()
    {
        if (MainUIController.instance != null)
            MainUIController.instance.Active = true;
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void LevelUp()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("user", PlayerPrefs.GetString("user"));

        data.Add("str", Strength.IncreaseAmount);
        data.Add("dex", Dexterity.IncreaseAmount);
        data.Add("toughness", Toughness.IncreaseAmount);
        data.Add("int", Intelligence.IncreaseAmount);
        data.Add("luck", Luck.IncreaseAmount);

        Bridge.POST(Bridge.url + "PlayerLevelUP", data, (r) =>
        {
            Debug.Log("[LevelUp Response] " + r);
            ServerResponse resp = new ServerResponse(r);
            if (resp.status == ServerResponse.ResultType.Success)
            {
                Hide();
                PlayerServerSync.instance.SyncStatsHandler(resp);
            }
        });
    }

    public void Hide()
    {
        Active = false;
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        if (MainUIController.instance != null)
            MainUIController.instance.Active = false;
        Resources.UnloadUnusedAssets();
    }
}
