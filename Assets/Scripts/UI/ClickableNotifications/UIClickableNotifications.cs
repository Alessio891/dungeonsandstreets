using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIClickableNotifications : MonoBehaviour
{

    public Transform CenterScreenTransform;

    public UIClickableNotificationEntry prefab;

    List<UIClickableNotificationEntry> entries = new List<UIClickableNotificationEntry>();

    public static UIClickableNotifications instance;
    private void Awake()
    {
        instance = this;
    }

    public UIClickableNotificationEntry AddNotification()
    {
        UIClickableNotificationEntry e = GameObject.Instantiate<UIClickableNotificationEntry>(prefab);
        e.transform.SetParent(transform);
        e.transform.localPosition = Vector3.zero;
        e.transform.localScale = Vector3.one;
        entries.Add(e);
        return e;
    }

    public void RemoveQuestNotification(string questId)
    {
        for(int i = 0; i < entries.Count; i++)
        {
            if (entries[i] is UIQuestNotification)
            {
                UIQuestNotification q = (UIQuestNotification)entries[i];
                if (q.questId == questId)
                {
                    Destroy(entries[i].gameObject);
                    entries.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public UIQuestNotification GetQuestNotification(string questId)
    {
        foreach(UIClickableNotificationEntry e in entries)
        {
            if (e is UIQuestNotification)
            {
                UIQuestNotification val = (UIQuestNotification)e;
                if (val.questId == questId)
                    return val;
            }
        }
        return null;
    }


}
