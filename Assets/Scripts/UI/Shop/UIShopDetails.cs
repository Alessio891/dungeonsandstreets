using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShopDetails : UIInventoryDetails
{
    [System.Serializable]
   public class DetailsReferences
    {
        public Text NameText;
        public Text Desc;
        public Text DamageText;
        public Text HitText;
        public Text CritText;
        public Text DmgTypeText;
        public Text ElementText;
        public Image DamageUpArrow;
        public Image DamageDownArrow;
        public Image HitUpArrow;
        public Image HitDownArrow;
        public Image CritUpArrow;
        public Image CritDownArrow;
    }

    public DetailsReferences CurrentDetails;
    public DetailsReferences PreviousDetails;
    public DetailsReferences NextDetails;
}
