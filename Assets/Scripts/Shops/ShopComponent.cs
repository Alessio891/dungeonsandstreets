using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopComponent : MapPOIComponent {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (ClickedOnThis ()) {
			LoadShop ();	
		}
	}

	void LoadShop()
	{
		Bridge.GET(Bridge.url + "Shop/GetSpawnedShop?user="+ PlayerPrefs.GetString("user")+"&posX="+posX.ToStringEx()+"&posY="+posY.ToStringEx()+"&shopId="+poiUID, (r) => {
			ServerResponse resp = new ServerResponse(r);
			if (resp.status == ServerResponse.ResultType.Success)
			{
				UIShop.instance.shopData = resp.GetIncomingDictionary();
                string id = UIShop.instance.shopData["shopTemplateId"].ToString();
                BaseShop s = Registry.assets.shops[id];
                UIShop.instance.ShopName.text = s.POIText;
                UIShop.instance.ShopAvatar.sprite = s.NpcAvatar;
                UIShop.instance.Show();
			}
		});
	}
}
