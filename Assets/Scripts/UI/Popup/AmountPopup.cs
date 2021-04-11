using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmountPopup : BasePopup {
	public int MaxAmount = 0;
	public Text maxAmountText;

	public InputField input;

    bool plusDown = false;
    bool minusDown = false;

    float changeSpeed = 4.0f;
    float timer = 0;
    float changeTimer = 0;
	public void Init(int maxAmount)
	{
        changeSpeed = 0.5f;
        timer = 0.5f;
        input.text = "1";
		MaxAmount = maxAmount;
		maxAmountText.text = "/ " + MaxAmount.ToString ();
	}

	public void AssertValue(string value)
	{
		int amount = 0;
		int.TryParse (value, out amount);
		if (amount > MaxAmount)
			input.text = MaxAmount.ToString ();
		if (amount < 1)
			amount = 1;
	}
    private void Update()
    {
        if (plusDown || minusDown)
        {
            changeTimer += Time.deltaTime;
            if (changeTimer>0.8f)
            {
                changeTimer = 0;
                if (changeSpeed > 0.2f)
                    changeSpeed -= 0.2f;
            }
        } else
        {
            changeTimer = 0;
        }
        if (plusDown)
        {
            timer += Time.deltaTime;
            if (timer > changeSpeed)
            {
                IncreaseValue();
                timer = 0;
                
            }
        }

        if (minusDown)
        {
            timer += Time.deltaTime;

            if (timer > changeSpeed)
            {
                DecreaseValue();
                timer = 0;
                
            }
        }

        if (!plusDown && !minusDown)
        {
            changeSpeed = 0.5f;
            timer = 0.5f;
        }
    }


    public void ButtonDown(bool plus) {
        if (plus)
            plusDown = true;
        else
            minusDown = true;
    }
    public void ButtonUp(bool plus) {
        if (plus)
            plusDown = false;
        else
            minusDown = false;
    }

    public void IncreaseValue()
    {        
        int val = int.Parse(input.text);
        if (val < MaxAmount)
            val++;
        input.text = val.ToString();
    }
    public void DecreaseValue()
    {
        int val = int.Parse(input.text);
        if (val > 1)
            val--;
        input.text = val.ToString();
    }
}
