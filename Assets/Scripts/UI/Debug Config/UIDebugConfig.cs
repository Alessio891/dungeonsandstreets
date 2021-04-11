using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDebugConfig : MonoBehaviour {
    

    public CanvasGroup canvasGroup;

    public InputField minDist, maxDist, maxSpawn, guaranteedDist;

	// Use this for initialization
	void Start () {
        GetConfig();
        Hide();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void UploadConfig()
    {
        Registry.assets.ServerConfig.Set("minUpdateDist", minDist.text);
        Registry.assets.ServerConfig.Set("maxUpdateDist", maxDist.text);
        Registry.assets.ServerConfig.Set("guaranteedDistanceSpawn", guaranteedDist.text);
        Registry.assets.ServerConfig.Set("maxSpawnPerTile", maxSpawn.text);

        Registry.assets.ServerConfig.Upload();
    }

    public void GetConfig()
    {
        Registry.assets.ServerConfig.Download(() => refreshConfigValues());
    }

    public void refreshConfigValues()
    {
        minDist.text = Registry.assets.ServerConfig.Get("minUpdateDist").ToString();
        maxDist.text = Registry.assets.ServerConfig.Get("maxUpdateDist").ToString();
        guaranteedDist.text = Registry.assets.ServerConfig.Get("guaranteedDistanceSpawn").ToString();
        maxSpawn.text = Registry.assets.ServerConfig.Get("maxSpawnPerTile");
    }

    public void Show()
    {
        MainUIController.instance.Active = true;
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        GetConfig();
    }
    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        MainUIController.instance.Active = false;
    }
}
