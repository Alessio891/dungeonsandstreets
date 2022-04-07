using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UITalentEntry : MonoBehaviour, IPointerDownHandler
{
    public string ID;
    public Image TalentIcon;
    UILineRenderer LineRenderer;
    public SkillTreeEntry Talent;

    private void Awake()
    {
        LineRenderer = GetComponent<UILineRenderer>();
        LineRenderer.enabled = false;
    }

    public void Init()
    {
        
    }

    public void LinkTo(float distance)
    {
        LineRenderer.enabled = true;
        LineRenderer.Points = new Vector2[2];
        LineRenderer.Points[0] = new Vector3(0, 20, 0);
        LineRenderer.Points[1] = new Vector3(0, distance, 0);
    }

    

    public void Hide()
    {
        foreach(Image i in GetComponentsInChildren<Image>())
        {
            i.enabled = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UISkillDetails.instance.ShowSkill(Talent);
    }
}
