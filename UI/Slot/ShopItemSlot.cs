using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemSlot : SlotBase
{
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemQty;
    [SerializeField] TextMeshProUGUI itemPrice;


    ItemData itemData;
    void Start()
    {
        DeSelectedSlot();
    }
    public void SetBuyItemData(ItemData _buyItem)
    {
        itemData = _buyItem;
        SetItemImage(_buyItem.ItemImg);
        SetItemGradeImg(_buyItem.ItemGrade);
        ShowItemPrice(_buyItem.Price);
        itemName.text = _buyItem.Name;

    }
    public void SetSaleItemData(ItemData _saleItem, int _qty)
    {
        SetItemImage(_saleItem.ItemImg);
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
    public override void DeSelectedSlot()
    {
        base.DeSelectedSlot();
    }

    public override void SelectedSlot()
    {
        if (itemData == null)
            return;
        base.SelectedSlot();
        if (UIShop.Instance.SelectedItem != this)
        {
            UIShop.Instance.SelectedItem?.DeSelectedSlot();
            UIShop.Instance.SelectedItem = this;
        }
        UIShop.Instance.AddBuyListItem(itemData, 1);
    }
}
