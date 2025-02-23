using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotBase : MonoBehaviour
{
    public Image itemImage;
    public Image itemGradeImage;
    public Image selectedImg;

    public virtual void SelectedSlot()
    {
        selectedImg.enabled = true;
    }
    public virtual void DeSelectedSlot()
    {
        selectedImg.enabled = false;
    }


    public virtual void SetItemImage(Sprite _sprite)
    {
        itemImage.sprite = _sprite;
        itemImage.enabled = _sprite != null;
    }
    public virtual void SetItemGradeImg(int _itemGrade = 1)
    {
        itemGradeImage.sprite = SpriteAtlasManager.Instance.GetSprite("Item", $"item_bg_{_itemGrade.ToString("D3")}");
    }
}
