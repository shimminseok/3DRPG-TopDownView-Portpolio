using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUDDropItemSlot : SlotBase
{
    [SerializeField] CanvasGroup cavas;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemQtyTxt;



    public bool IsShow => gameObject.activeSelf;
    public void SetItemInfo(SaveItemData _data)
    {
        SetItemImage(SpriteAtlasManager.Instance.GetSprite("Item", _data.GetItemData().ItemImg));
        itemName.text = _data.GetItemData().Name;
        itemQtyTxt.text = $"x{_data.Quantity:N0}";
        SetItemGradeImg();
        ShowItemSlot();
    }
    public void SetGoldInfo(int _amount)
    {
        SetItemImage(SpriteAtlasManager.Instance.GetSprite("UI", "com_item_gold_001"));
        itemName.text = "°ñµå";
        itemQtyTxt.text = $"x{_amount:N0}";
        ShowItemSlot();
    }
    void ShowItemSlot()
    {
        cavas.DOKill(true);
        gameObject.SetActive(true);
        cavas.alpha = 1;
        transform.SetAsFirstSibling();
        
        HideItemSlot();
    }

    void HideItemSlot()
    {
        DOVirtual.DelayedCall(1f, () =>
        {
            cavas.DOFade(0, 0.5f).SetEase(Ease.InQuad).OnComplete(() => gameObject.SetActive(false));
        });
    }
}
