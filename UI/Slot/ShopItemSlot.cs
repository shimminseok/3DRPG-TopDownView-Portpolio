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


    SaveItemData data;
    public SaveItemData Data => data;
    void Start()
    {
        DeSelectedSlot();
    }
    public void SetBuyItemData(SaveItemData _buyItem)
    {
        ItemData data = _buyItem.ItemData;
        SetItemImage(data.ItemImg);
        SetItemGradeImg(data.ItemGrade);
        ShowItemPrice(data.Price);
        itemName.text = data.Name;

        this.data = _buyItem;
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
        selectedImg.enabled = false;
    }

    public override void SelectedSlot()
    {
        if (data == null)
            return;
        if (UIShop.Instance.selectedItem != this)
        {
            UIShop.Instance.selectedItem?.DeSelectedSlot();
            UIShop.Instance.selectedItem = this;
        }
        UIShop.Instance.AddBuyListItem(data, 1);
        selectedImg.enabled = true;
    }
}
