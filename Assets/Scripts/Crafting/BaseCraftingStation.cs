using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCraftingStation : BasicFeature {
    public override void UpdateFunc()
    {
        if (ClickedOnThis())
        {
            UIRefine.instance.Show();
        }
    }
}
