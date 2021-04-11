using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRollResultEntry : MonoBehaviour
{
    public Image Icon, BG;
    public Text ItemName, Winner, Amount;
    public CanvasGroup group;

    public Color wonColor;
    public Color lostColor;
    public Color stillRollingColor;

    public IEnumerator Show()
    {
        while(group.alpha < 1.0f)
        {
            group.alpha += 0.3f * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public void Init(Dictionary<string, object> data)
    {
        BaseItem item = Resources.Load<BaseItem>(Registry.assets.items[data["itemId"].ToString()]);
        Icon.sprite = item.sprite;
        ItemName.text = item.Name;
        Winner.text = data["winnerName"].ToString();
        Amount.text = data["result"].ToString();
        if (!(bool)data["allRolled"])
        {
            BG.color = stillRollingColor;
        } else
        {
            if (data["winnerName"].ToString() == PlayerPrefs.GetString("user"))
            {
                BG.color = wonColor;
            } else
            {
                BG.color = lostColor;
            }
        }
    }

    public void UpdateData(Dictionary<string, object> data)
    {
        Winner.text = data["winnerName"].ToString();
        Amount.text = data["result"].ToString();
        if (!(bool)data["allRolled"])
        {
            BG.color = stillRollingColor;
        }
        else
        {
            if (data["winnerName"].ToString() == PlayerPrefs.GetString("user"))
            {
                BG.color = wonColor;
            }
            else
            {
                BG.color = lostColor;
            }
        }
    }

}
