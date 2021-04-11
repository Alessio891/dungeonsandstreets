using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelUpStatEntry : MonoBehaviour
{
    int _originalAmount = 0;
    public int OriginalAmount
    {
        get { return _originalAmount; }
        set { _originalAmount = value; Amount.text = _originalAmount.ToString(); }
    }
    public Text Amount;
    public int Value
    {
        get
        {
            return int.Parse(Amount.text);
        }
    }

    public int IncreaseAmount
    {
        get
        {
            return Value - OriginalAmount;
        }
    }

    public void Increase() {
        if (UILevelUp.instance.Points > 0)
        {
            Amount.text = (Value + 1).ToString();
            UILevelUp.instance.Points--;
        }
    }
    public void Decrease() {
        if (Value-1 >= OriginalAmount)
        {
            Amount.text = (Value - 1).ToString();
            UILevelUp.instance.Points++;
        }
    }
}
