using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UICombatEnd : MonoBehaviour
{
    CanvasGroup group;

    public Text XPValue, GoldValue;
    public GridLayoutGroup ItemGrid;

    public UIItemEntry ItemEntryPrefab;
    public UIEquipDetails Details;

    public float CounterDelayTime = 0.1f;
    public float FadeInSpeed = 2.5f;

    public static UICombatEnd instance;
    private void Awake()
    {
        instance = this;
        group = GetComponent<CanvasGroup>();
    }


    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        SoundManager.instance.ChangeBGM(SoundManager.instance.BattleWonMusic, true);
        Details.Clear();
        group.alpha = 1.0f;
        group.interactable = true;
        group.blocksRaycasts = true;
        XPValue.text = GoldValue.text = "0";
        XPValue.color = GoldValue.color = new Color(1, 1, 1, 0);
    }
    public void Hide()
    {
        group.alpha = 0.0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    public void Load(Dictionary<string, object> endData)
    {
        StartCoroutine(AnimationRoutine(endData));
    }

    IEnumerator AnimationRoutine(Dictionary<string, object> endData)
    {
        Dictionary<string, object> actionResult = (Dictionary<string, object>)endData["actionResult"];

        
        int xp = int.Parse(actionResult["ExperienceGain"].ToString());
        int gold = int.Parse(actionResult["Gold"].ToString());

        float alpha = 0;
        while(alpha <= 1)
        {
            alpha += FadeInSpeed * Time.deltaTime;
            XPValue.color = new Color(1, 1, 1, alpha);
            yield return new WaitForEndOfFrame();
        }
        int counter = 0;
        while(counter <= xp)
        {
            XPValue.text = counter.ToString();
            counter += 10;
            if (counter > xp)
            {
                counter = xp;
                XPValue.text = counter.ToString();
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        alpha = 0;
        counter = 0;
        while (alpha <= 1)
        {
            alpha += FadeInSpeed * Time.deltaTime;
            GoldValue.color = new Color(1, 1, 1, alpha);
            yield return new WaitForEndOfFrame();
        }
       
        while (counter <= gold)
        {
            GoldValue.text = counter.ToString();
            counter += 10;
            if (counter > gold)
            {
                counter = gold;
                GoldValue.text = counter.ToString();
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        List<object> DroppedItems = (List<object>)actionResult["DroppedItems"];

        foreach(Dictionary<string, object> item in DroppedItems)
        {
            UIItemEntry e = GameObject.Instantiate<UIItemEntry>(ItemEntryPrefab);
            e.transform.SetParent(ItemGrid.transform);
            e.transform.localScale = Vector3.one;

            BaseItem itemInstance = null;
            if (item["Category"].ToString().ToLower() == "armor")
            {
                itemInstance = new BaseArmor();
                (itemInstance as BaseArmor).Deserialize(item);
            } else if (item["Category"].ToString().ToLower() == "weapon")
            {
                itemInstance = new BaseWeapon();
                (itemInstance as BaseWeapon).Deserialize(item);
            } else if (item["Category"].ToString().ToLower() == "consumable")
            {
                itemInstance = new BaseConsumable();
                (itemInstance as BaseConsumable).Deserialize(item);
            } else
            {
                itemInstance = new BaseItem();
                itemInstance.Deserialize(item);
            }
            string itemId = item["ItemID"].ToString();
            BaseItem template = Resources.Load<BaseItem>(Registry.assets.items[itemId]);
            itemInstance.sprite = template.sprite;
            e.item = itemInstance;

            e.OnClick = () =>
            {
                Details.LoadItem(e.item as BaseEquippable);
            };

            e.Init();
            CanvasGroup cg = e.gameObject.AddComponent<CanvasGroup>();
            cg.alpha = 0;
            while(cg.alpha<1)
            {
                cg.alpha += 1.5f * Time.deltaTime;
                if (cg.alpha > 0.9f)
                    break;
                yield return new WaitForEndOfFrame();
            }
        }


    }

    public void Claim()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("user", PlayerPrefs.GetString("user"));
        data.Add("combatId", CombatManager.instance.combatData.UID);
        Bridge.POST(Bridge.url + "ExitCombat", data, (r) =>
        {
            CombatManager.instance.EndCombat();
        });
    }
}
