using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPoiInspect : MonoBehaviour
{
    public Image PoiImage;
    public Text PoiName;
    public Text PoiDescription;

    public static UIPoiInspect instance;
    private void Awake()
    {
        instance = this;
    }

    public void SubmitPicture() {

        DeviceCameraController.instance.Show(() =>
        {
           string posX = UIPoi.instance.POIData["posX"].ToString();
           string posY = UIPoi.instance.POIData["posY"].ToString();
           byte[] picture = DeviceCameraController.instance.TakePictureData();
           Dictionary<string, object> data = new Dictionary<string, object>();
           data.Add("posX", posX);
           data.Add("posY", posY);           
           data.Add("data", System.Convert.ToBase64String(picture));
           Bridge.POST(Bridge.url + "UploadPOIPicture", data, (r) =>
           {
               Debug.Log("[UploadPic Resp]: " + r);
               GetPicture();
           });
        }, true);

        
    }

    public void SubmitInfo()
    {
        PopupManager.ShowPopup<POIInfoPopup>("POIInfoPopup", "Submit", "", (s, b) =>
        {
            if (s == "ok")
            {
                POIInfoPopup popup = (POIInfoPopup)b;
                string name = popup.poiName.text;
                string desc = popup.poiDesc.text;
                string posX = UIPoi.instance.POIData["posX"].ToString();
                string posY = UIPoi.instance.POIData["posY"].ToString();
                Dictionary<string, object> values = new Dictionary<string, object>();
                values.Add("posX", posX);
                values.Add("posY", posY);
                Dictionary<string, object> data = new Dictionary<string, object>()
                {
                    { "POIName", name },
                    { "POIDescription", desc }
                };
                values.Add("data", MiniJSON.Json.Serialize(data));
                Bridge.POST(Bridge.url + "UploadPOIInfo", values, (r) =>
                {
                    PoiName.text = name;
                    PoiDescription.text = desc;
                    b.Close();
                });
            }
        });
    }

    public void ReviewInfo() { }

    public void GetInfo()
    {
        string posX = UIPoi.instance.POIData["posX"].ToString();
        string posY = UIPoi.instance.POIData["posY"].ToString();
        Bridge.GET(Bridge.url + "GetPOIInfo?posX=" + posX + "&posY=" + posY, (r) =>
        {
            ServerResponse resp = new ServerResponse(r);
            if (resp.status == ServerResponse.ResultType.Success)
            {
                string name = resp.GetIncomingDictionary()["POIName"].ToString();
                string desc = resp.GetIncomingDictionary()["POIDescription"].ToString();
                PoiName.text = name;
                PoiDescription.text = desc;
            }            
        });
    }

    public void GetPicture()
    {
        string posX = UIPoi.instance.POIData["posX"].ToString();
        string posY = UIPoi.instance.POIData["posY"].ToString();

        Bridge.GetTexture(Bridge.url + "GetPOIPicture?posX=" + posX + "&posY=" + posY, (t) =>
        {
            Sprite s = Sprite.Create((Texture2D)t, new Rect(0,0, t.width, t.height), new Vector2(0.5f, 0.5f));
            PoiImage.sprite = s;
        });
    }
}