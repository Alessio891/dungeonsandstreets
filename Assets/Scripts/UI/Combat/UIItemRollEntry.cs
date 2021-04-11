using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemRollEntry : MonoBehaviour
{
    public Image ItemIcon;
    public Image BG;
    public Text ItemName;
    public Text rollResultText;

    public GameObject rollButton;
    public GameObject passButton;    

    public int index = 0;

    bool receivedResult = false;
    int rollResult = 0;

    public void Roll() {
        rollButton.SetActive(false);
        passButton.SetActive(false);
        rollResultText.gameObject.SetActive(true);
        SoundManager.instance.PlayUI(SoundManager.instance.DiceRoll, true);
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("user", PlayerPrefs.GetString("user"));
        data.Add("combatId", CombatManager.instance.combatData.UID);
        data.Add("itemIndex", index);
        Bridge.POST(Bridge.url + "Combat/ItemRoll", data, (r) =>
        {
            ServerResponse resp = new ServerResponse(r);
            if (resp.status == ServerResponse.ResultType.Success)
            {
                receivedResult = true;
                rollResult = int.Parse(resp.GetIncomingDictionary()["roll"].ToString());
            }
        });
        StartCoroutine(rollRoutine());
        
    }

    IEnumerator rollRoutine()
    {
        float timer = 0;
        while(!receivedResult)
        {
            rollResultText.text = Random.Range(0, 101).ToString();
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }        
        while(timer < 3.5f)
        {
            timer += 0.1f;
            rollResultText.text = Random.Range(0, 101).ToString();
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
        SoundManager.instance.PlayUI(SoundManager.instance.DiceRollEnd);
        rollResultText.text = rollResult.ToString();
        yield return new WaitForSeconds(2.0f);
        UICombatEnd.instance.NextRoll(this);
    }

    public void Pass() {
        UICombatEnd.instance.NextRoll(this);
    }
}
