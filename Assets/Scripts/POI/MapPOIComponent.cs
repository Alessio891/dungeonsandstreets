using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPOIComponent : MonoBehaviour
{
    public string poiUID;
    public double posX;
    public double posY;

    public virtual bool ClickedOnThis()
    {
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (GetComponent<Collider>().Raycast(r, out hit, 1000.0f) && Input.GetMouseButtonDown(0))
        {
            if (hit.collider.gameObject == gameObject)
            {
                return true;
            }
        }
        return false;
    }

    private void Update()
    {
        if (ClickedOnThis())
        {
            Bridge.GET(Bridge.url + "Shop/GetSpawnedShop?user="+ PlayerPrefs.GetString("user") + "&posX=" + posX.ToStringEx() + "&posY=" + posY.ToStringEx() + "&shopId=" + poiUID, (r) =>
                      {
                          Debug.Log("[POI RESPONSE]: " + r);
                          ServerResponse resp = new ServerResponse(r);
                          if (resp.status == ServerResponse.ResultType.Success)
                          {
                              Dictionary<string, object> data = resp.GetIncomingDictionary();
                              MapPOI poi = Registry.assets.pois[data["poiTemplateId"].ToString()];
                              if (poi != null)
                              {
                                  UIPoi.instance.POIData = data;
                                  UIPoi.POIFeature flags = UIPoi.POIFeature.None;
                                  List<object> quests = (List<object>)data["Quests"];
                                  if (quests.Count > 0)
                                  {
                                      flags |= UIPoi.POIFeature.Quests;
                                  }
                                  if (poi.Dialogue != null)
                                      flags |= UIPoi.POIFeature.Talk;
                                  UIPoi.instance.Show(flags | UIPoi.POIFeature.Inspect);
                              }
                          }
                      });
        }
    }

    public void LoadPOI()
    {
        UIPoi.instance.Show(UIPoi.POIFeature.Quests);
        UIPoi.instance.LoadQuests();
    }
}
