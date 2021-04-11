using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameMenu : MonoBehaviour
{
    Vector2 originalPos;
    public Transform openedIconPos;
    public Transform closedIconPos;

    public Transform IconRoot;

    public CanvasGroup menu;

    bool opened = false;

    private void Start()
    {
        menu.alpha = 0.0f;
        menu.interactable = false;
        menu.blocksRaycasts = false;
        originalPos = (IconRoot as RectTransform).anchoredPosition;
    }

    public void Toggle()
    {
        if (!opened)
            Open();
        else
            Close();
        SoundManager.instance.PlayUI(SoundManager.instance.OpenMenu);
    }

    public void Open()
    {
        menu.alpha = 1.0f;
        menu.interactable = true;
        menu.blocksRaycasts = true;
        opened = true;
        iTween.MoveTo(IconRoot.gameObject, openedIconPos.position, 0.5f);
    }

    public void Close()
    {
        menu.alpha = 0.0f;
        menu.interactable = false;
        menu.blocksRaycasts = false;
        opened = false;
        iTween.MoveTo(IconRoot.gameObject, closedIconPos.position, 0.5f);
    }
}
