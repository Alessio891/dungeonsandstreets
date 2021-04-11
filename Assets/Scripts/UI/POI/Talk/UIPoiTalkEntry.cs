using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPoiTalkEntry : MonoBehaviour
{
    public Text EntryText;
    public int index = 0;
    public void Click()
    {
        UIPoiTalk.instance.PickResponse(index);
    }
}
