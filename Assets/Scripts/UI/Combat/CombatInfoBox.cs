using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatInfoBox : MonoBehaviour {

    public Text infoText;
    public CanvasGroup group;
    float timer = 0;
    bool routineStarted = false;

    public Queue<string> messages;

	public static CombatInfoBox instance;
	void Awake() { instance = this; }

	// Use this for initialization
	void Start () {
        messages = new Queue<string>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddText(string text)
	{
        Show();
        messages.Enqueue(text);
        if (messages.Count == 1)
        {        
            StartCoroutine(GetNext());
        }
        
	}

    IEnumerator GetNext()
    {
        infoText.text = messages.Peek();
        Color oldC = infoText.color;
        oldC.a = 255;
        infoText.color = oldC;
        timer = 0;
        while (timer < 3)
        {
            timer += Time.deltaTime;
            if (messages.Count > 1 && timer > 1)
                break;
            yield return null;
        }
        
        messages.Dequeue();
        if (messages.Count > 0)
        {
            StartCoroutine(GetNext());
        }
        else
        {
            Hide();
        }
    }

    IEnumerator HideAfterLast()
    {
        routineStarted = true;
        while (timer < 5)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        Hide();
        routineStarted = false;
        timer = 0;
    }

    public void Show()
    {
        group.alpha = 1;
    }
    public void Hide()
    {
        group.alpha = 0;
    }
}
