using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnScreenNotificationData
{
    public string text;
    public Sprite icon;
}

public class OnScreenNotification : MonoBehaviour
{
    CanvasGroup canvas; 
    Vector2 originalPos;
    public GameObject IconRoot;
    public Image icon;
    public CanvasGroup Header;

    public Transform target1;
    public Transform target2;

    public Text Text;

    public Sprite QuestSprite;
    public Sprite XPSprite;
    public Sprite GoldSprite;

    public List<OnScreenNotificationData> queue = new List<OnScreenNotificationData>();
    bool Animating = false;

    public static OnScreenNotification instance;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
        canvas = GetComponent<CanvasGroup>();
        canvas.alpha = 0;
        originalPos = (IconRoot.transform as RectTransform).anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Animating && queue.Count > 0)
        {
            OnScreenNotificationData data = queue[0];
            queue.RemoveAt(0);
            _show(data.text, data.icon);
        }
    }

    void _show(string text, Sprite icon_sprite = null)
    {
        Text.text = text;
        if (icon_sprite != null)
        {
            icon.sprite = icon_sprite;
        }
        Animate();
    }

    public void Show(string text, Sprite icon_sprite = null)
    {
        OnScreenNotificationData data = new OnScreenNotificationData()
        {
            text = text,
            icon = icon_sprite
        };
        queue.Add(data);
    }
    public void Hide()
    {
        canvas.alpha = 0;
    }


    public void Animate() {
        if (Animating)
            return;
        StopCoroutine("AnimateRoutine");
        Reset();
        canvas.alpha = 1.0f;
        Animating = true;
        StartCoroutine("AnimateRoutine");
    }
    IEnumerator AnimateRoutine()
    {
        iTween.MoveTo(IconRoot, iTween.Hash("position", target1.position, "time", 1.0f, "easeType", "easeOutSine"));
        if (!string.IsNullOrEmpty(Text.text))
        {
            while (Header.alpha < 1)
            {
                Header.alpha += 0.6f * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        } else
        {
            yield return new WaitForSeconds(0.5f);
        }
        iTween.MoveTo(IconRoot, iTween.Hash("position", target2.position, "time", 0.3f, "easeType", "easeInCubic"));
        yield return new WaitForSeconds(0.2f);
        if (!string.IsNullOrEmpty(Text.text))
        {
            while (Header.alpha > 0)
            {
                Header.alpha -= 0.9f * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        } else
        {
            yield return new WaitForSeconds(1.0f);
        }
        Reset();
    }
    public void Reset()
    {
        iTween.Stop(gameObject);
        canvas.alpha = 0.0f;
        Header.alpha = 0.0f;
        Animating = false;
        (IconRoot.transform as RectTransform).anchoredPosition = originalPos;
    }
}
