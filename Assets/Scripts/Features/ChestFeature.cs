using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestFeature : BasicFeature {
	public void toberemoved ()
	{
		
		if (ClickedOnThis ()) {
			if (!selected && currentSelected == null) {
				FeatureInfoManager.instance.Clear ();			
				StartCoroutine (resetSelected ());
				FeatureInfoManager.instance.ShowInfo (overHeadUIPos, FeatureName, 1, lifeTime, this);
				currentSelected = this;
			} else if (selected) {
                string query = "userName = " + PlayerPrefs.GetString("user") + " & id = " + UID + " & posX = " + x.ToString() + " & posY = " + y.ToString();
                if (isNode)
                    query += "&isNode=true";
                Bridge.GET (Bridge.url + "GetRandomLoot?"+query,
					(r) => {
						if (selected)
							FeatureInfoManager.instance.Clear();
						Debug.Log ("ITEM RESULT: " + r);
						ServerResponse resp = new ServerResponse (r);
						iTween.MoveBy (gameObject, transform.up * 20, 1.5f);
                        //Destroy(gameObject, 1.5f)
						List<object> entries = (List<object>)resp.GetIncomingDictionary () ["data"];
						foreach (object o in entries) {
							Dictionary<string, object> item = (Dictionary<string, object>)o;
							foreach (KeyValuePair<string, object> pair in item) {
								if (pair.Key == "gold") {
									NotificationCenter.instance.AddNotification ("Ricevuti " + pair.Value.ToString () + " gold!");
								} else if (pair.Key == "item") {
									BaseItem i = Resources.Load<BaseItem> (Registry.assets.items [pair.Value.ToString ()]);
									if (i != null) {
										NotificationCenter.instance.AddNotification ("Ricevuto " + i.Name + "!", i.sprite);
									}
								}
							}
						}
					});
			}
		}
	}
}
