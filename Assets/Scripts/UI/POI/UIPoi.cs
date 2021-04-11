using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPoi : MonoBehaviour
{
    [System.Flags]
    public enum POIFeature
    {
        None = 0,
        Quests = 1,
        Talk = 2,
        Inspect = 4,
        Dungeon = 8,
        Boss = 16
    }

    public GameObject testPhotoUI;
    public DeviceCameraController camControl;

    public POIFeature Features;
    public Dictionary<string, object> POIData;
    #region Canvas Groups
    public CanvasGroup group;

    public CanvasGroup QuestGroup;
    public CanvasGroup TalkGroup;
    public CanvasGroup InspectGroup;
    public CanvasGroup DungeonGroup;
    public CanvasGroup BossGroup;
    public CanvasGroup CurrentGroup;
    #endregion
    #region Buttons
    public GameObject QuestButton;
    public GameObject TalkButton;
    public GameObject InspectButton;
    public GameObject DungeonButton;
    public GameObject BossButton;
    #endregion
    public static UIPoi instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Hide();
    }

    private void Update()
    {
       
    }

    public void Show(POIFeature flags)
    {
        CheckButton(flags, POIFeature.Quests, QuestButton);
        CheckButton(flags, POIFeature.Talk, TalkButton);
        CheckButton(flags, POIFeature.Inspect, InspectButton);
        CheckButton(flags, POIFeature.Dungeon, DungeonButton);
        CheckButton(flags, POIFeature.Boss, BossButton);

        if ( (flags & POIFeature.Quests) != POIFeature.None)
        {
            LoadQuests();
        }

        Show();
    }

    public void LoadQuests()
    {
        QuestGroup.GetComponent<UIPoiQuests>().LoadQuests();
    }

    void CheckButton(POIFeature flags, POIFeature check, GameObject button)
    {
        if ((flags & check) == POIFeature.None)
        {
            button.SetActive(false);
        }
        else
        {
            button.SetActive(true);
        }

    }

    public void Show()
    {
        group.alpha = 1.0f;
        group.interactable = true;
        group.blocksRaycasts = true;
        ToggleCanvas(TalkGroup, false);
        ToggleCanvas(InspectGroup, false);
        ToggleCanvas(DungeonGroup, false);
        ToggleCanvas(BossGroup, false);
        ToggleCanvas(QuestGroup);
    }
    public void Hide()
    {
        group.alpha = 0.0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    public void ShowGroup(int index)
    {
        if (CurrentGroup != null)
            ToggleCanvas(CurrentGroup, false);
        switch(index)
        {
            case 0:
                ToggleCanvas(QuestGroup);
                break;
            case 1:
                ToggleCanvas(TalkGroup);
                MapPOI poi = Registry.assets.pois[POIData["poiTemplateId"].ToString()];
                UIPoiTalk.instance.LoadDialogue(poi.Dialogue);
                break;
            case 2:
                ToggleCanvas(InspectGroup);
                UIPoiInspect.instance.GetInfo();
                UIPoiInspect.instance.GetPicture();
                break;
            case 3:
                ToggleCanvas(DungeonGroup);
                break;
            case 4:
                ToggleCanvas(BossGroup);
                break;
        }
    }

    public void ToggleCanvas(CanvasGroup canvas, bool show = true)
    {
        canvas.alpha = (show) ? 1 : 0;
        canvas.interactable = show;
        canvas.blocksRaycasts = show;
        if (show)
            CurrentGroup = canvas;
    }
}
