using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQuestNotification : UIClickableNotificationEntry
{
    public string questId;

    public override void Click()
    {

        // UIClickableNotifications.instance.RemoveNotification(this);
        StartCoroutine(clicked());
    }

    IEnumerator clicked()
    {
        iTween.MoveTo(gameObject, UIClickableNotifications.instance.CenterScreenTransform.position, 0.6f);
        yield return new WaitForSeconds(0.6f);
        QuestCompletedPopup popup = PopupManager.ShowPopup<QuestCompletedPopup>("QuestCompletedPopup", "Quest Completed!", "Quest Completed!", null);
        popup.questId = questId;
        popup.Init();
        Destroy(gameObject);

    }
}
