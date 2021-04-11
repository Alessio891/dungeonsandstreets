using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Examples.LocationProvider;
using Mapbox.Examples;

[RequireComponent(typeof(Collider))]
public class BasicFeature : MonoBehaviour {

	public string UID;
	public string type;
	public double x;
	public double y;
	public long lifeTime;
	public long spawnTime;
	public string extraData;
	public Transform overHeadUIPos;
	public string FeatureName;
	public int rarity;
    public bool isNode = false;

	protected BasicFeature currentSelected { get { return WorldSpawner.instance.currentSelected; } set { WorldSpawner.instance.currentSelected = value; } }
	protected bool selected { get { return currentSelected == this; } }

	protected IEnumerator resetSelected()
	{
		yield return new WaitForSeconds (4.0f);
		if (!selected)
			yield return null;
		else
		{
			WorldSpawner.instance.currentSelected = null;
			FeatureInfoManager.instance.Clear ();
		}
	}

	public bool ClickedOnThis()
	{
        if (MainUIController.instance.Active)
            return false;
		Ray r = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (GetComponent<Collider> ().Raycast (r, out hit, 1000.0f) && Input.GetMouseButtonDown (0)) {
			if (hit.collider.gameObject == gameObject) {
				return true;
			}
		}
		return false;
	}
	public virtual void UpdateFunc()
	{
		if (ClickedOnThis ()) {

			if (!selected) {
				FeatureInfoManager.instance.Clear ();
				currentSelected = this;
				StartCoroutine (resetSelected ());

				FeatureInfoManager.instance.ShowInfo (overHeadUIPos, FeatureName, rarity, lifeTime, this);
			} else if (selected) {
                string query = "userName=" + PlayerPrefs.GetString("user") + "&id=" + UID + "&posX=" + x.ToStringEx() + "&posY=" + y.ToStringEx();
                if (isNode)
                    query += "&isNode=true";
                Debug.Log("[GetRandomLoot] QUERY: " + query);
                Bridge.GET (Bridge.url + "InteractWithFeature?" + query,
					(r) => {

						Debug.Log ("ITEM RESULT: " + r);
						ServerResponse resp = new ServerResponse (r);
                        if (resp.status == ServerResponse.ResultType.Success)
                        {
                            iTween.MoveBy(gameObject, transform.up * 20, 1.5f);
                            Dictionary<string, object> entries = resp.GetIncomingDictionary();
                            Dictionary<BaseItem, int> received = new Dictionary<BaseItem, int>();

                            foreach (KeyValuePair<string, object> pair in entries)
                            {
                                //Debug.Log("Rec " + pair.Key);
                                if (pair.Key == "gold")
                                {
                                    NotificationCenter.instance.AddNotification("Received " + pair.Value.ToString() + " gold!");
                                    int recAmount = 0;
                                    int.TryParse(pair.Value.ToString(), out recAmount);
                                    DockBar.instance.UpdateGold(DockBar.instance.gold + recAmount);
                                }
                                else
                                {
                                    BaseItem i = Resources.Load<BaseItem>(Registry.assets.items[pair.Key]);
                                    if (i != null)
                                    {
                                        int a = 0;
                                        int.TryParse(pair.Value.ToString(), out a);
                                        received.Add(i, a);
                                    }
                                }

                            }
                            foreach (KeyValuePair<BaseItem, int> itemRec in received)
                            {
                                NotificationCenter.instance.AddNotification("You found " + itemRec.Key.Name + "x" + itemRec.Value.ToString() + "!", itemRec.Key.sprite);
                            }
                            Destroy(gameObject, 2.0f);
                        } else
                        {
                            if (resp.errorCode == "100")
                            {
                                Debug.Log("Not enough gathering profession");
                            }
                        }

					});
			} else {
				currentSelected = this;
			}
		}
	}
	void Update() {
		lifeTime = 60000 - (PositionWithLocationProvider.CurrentTimeMillis () - spawnTime);
		UpdateFunc ();
	}

	void OnDestroy()
	{
		if (selected)
			FeatureInfoManager.instance.Clear ();
	}

}
