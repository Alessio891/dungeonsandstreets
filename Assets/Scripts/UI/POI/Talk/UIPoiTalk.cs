using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPoiTalk : MonoBehaviour
{
    public UIPoiTalkEntry entryPrefab;

    public Text NpcText;
    public Image NpcAvatar;
    public VerticalLayoutGroup PlayerChoices;

    List<UIPoiTalkEntry> entries = new List<UIPoiTalkEntry>();

    public DialogueAsset currentDialogue;

    bool waitingRewards = false;

    public static UIPoiTalk instance;
    private void Awake()
    {
        instance = this;
    }

    public void LoadDialogue(DialogueAsset dialogue)
    {
        currentDialogue = dialogue;
        NpcText.text = dialogue.DialogueText;
        foreach (UIPoiTalkEntry e in entries)
            Destroy(e.gameObject);
        entries.Clear();
        int count = 0;
        waitingRewards = false;
        foreach(DialogueResponse resp in dialogue.Responses)
        {
            if (PlayerServerSync.instance.claimedDialogues.ContainsKey(dialogue.UID))
            {
                if (PlayerServerSync.instance.claimedDialogues[dialogue.UID].Contains(count))
                {
                    count++;
                    continue;
                }
            }
            UIPoiTalkEntry e = GameObject.Instantiate<UIPoiTalkEntry>(entryPrefab);
            e.transform.SetParent(PlayerChoices.transform);
            e.transform.position = Vector3.zero;
            e.transform.localScale = Vector3.one;
            e.index = count;
            count++;
            e.EntryText.text = resp.Text;
            entries.Add(e);
        }
    }

    public void PickResponse(int index)
    {       
        DialogueResponse resp = currentDialogue.Responses[index];
        bool rewardClaimed = false;
        foreach(DialogueActionData d in resp.Actions)
        {
            switch(d.action)
            {
                case DialogueActionData.ResponseAction.Continue:
                    string dialogueId = d.data1;
                    DialogueAsset nextDialogue = Registry.assets.dialogues[dialogueId];
                    UIPoiTalk.instance.LoadDialogue(nextDialogue);
                    break;
                case DialogueActionData.ResponseAction.End:

                    break;
                case DialogueActionData.ResponseAction.Reward:
                    if (!waitingRewards && !rewardClaimed)
                    {
                        Dictionary<string, object> values = new Dictionary<string, object>();
                        values.Add("user", PlayerPrefs.GetString("user"));
                        values.Add("dialogueId", currentDialogue.UID);
                        values.Add("responseIndex", index.ToString());
                        waitingRewards = true;
                        Bridge.POST(Bridge.url + "DialogueRewards", values, (r) =>
                        {
                            rewardClaimed = true;
                            waitingRewards = false;
                            foreach(DialogueActionData a in resp.Actions)
                            {
                                if (a.action == DialogueActionData.ResponseAction.Reward)
                                {
                                    if (a.data1 == "gold")
                                    {
                                        OnScreenNotification.instance.Show("", OnScreenNotification.instance.GoldSprite);
                                    } else if (a.data1 == "xp")
                                    {
                                        OnScreenNotification.instance.Show("", OnScreenNotification.instance.XPSprite);
                                    } else {
                                        BaseItem item = Resources.Load<BaseItem>(Registry.assets.items[a.data1]);
                                        if (item != null)
                                        {
                                            OnScreenNotification.instance.Show("", item.sprite);
                                        }
                                    }
                                }
                            }
                        });
                    }

                    break;
            }
        }

    }
}
