using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardSlot : MonoBehaviour
{

    [SerializeField] Image rewardItemIcon;
    [SerializeField] Image rewardItemGradeImg;
    [SerializeField] TextMeshProUGUI rewardQtyTxt;


    public void SetRewardItem(SaveItemData _data)
    {
        rewardItemIcon.sprite = SpriteAtlasManager.Instance.GetSprite("Item", _data.ItemData.ItemImg.name);
        rewardQtyTxt.text = _data.Quantity == 0 ? string.Empty : $"x{_data.Quantity}";
    }
    public void SetRewardGold(int _gold)
    {
        rewardItemIcon.sprite = SpriteAtlasManager.Instance.GetSprite("UI", "com_item_gold_001");
        rewardQtyTxt.text = $"x{_gold}";
    }
    public void SetRewardExp(int _exp)
    {
        rewardItemIcon.sprite = SpriteAtlasManager.Instance.GetSprite("UI", "Exp");
        rewardQtyTxt.text = $"x{_exp}";
    }


}
