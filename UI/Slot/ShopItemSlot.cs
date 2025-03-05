using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemSlot : SlotBase, ISelectableSlot
{
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemQty;
    [SerializeField] TextMeshProUGUI itemPrice;
    [SerializeField] Image selectedImg;



    ItemData itemData;
    bool isSelected;
    void Start()
    {
        DeSelectedSlot();
    }
    public void SetBuyItemData(ItemData _buyItem)
    {
        itemData = _buyItem;
        SetItemImage(SpriteAtlasManager.Instance.GetSprite("Item", _buyItem.ItemImg));
        SetItemGradeImg(_buyItem.ItemGrade);
        ShowItemPrice(_buyItem.Price);
        itemName.text = _buyItem.Name;

    }
    public void SetSaleItemData(ItemData _saleItem, int _qty)
    {
        SetItemImage(SpriteAtlasManager.Instance.GetSprite("Item", _saleItem.ItemImg));
        SetItemGradeImg(_saleItem.ItemGrade);
        itemQty.text = _qty.ToString();
        ShowItemPrice(_saleItem.Price, _qty);
    }
    void ShowItemPrice(int _price, int _qty = 1)
    {
        int totalPrice = _price * _qty;
        itemPrice.text = totalPrice.ToString("N0");
    }
    public void EmptySlot()
    {
        SetItemImage(null);
        SetItemGradeImg(1);
        itemName.text = string.Empty;
        itemPrice.text = string.Empty;
    }
    public void DeSelectedSlot()
    {
        isSelected = false;
    }

    public void SelectedSlot()
    {
        if (itemData == null)
            return;
        if (UIShop.Instance.SelectedItem != this)
        {
            UIShop.Instance.SelectedItem?.DeSelectedSlot();
            UIShop.Instance.SelectedItem = this;
        }
        UIShop.Instance.AddBuyListItem(itemData, 1);

        isSelected = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSelected)
            DeSelectedSlot();
        else
            SelectedSlot();

        selectedImg.enabled = isSelected;
    }
}
