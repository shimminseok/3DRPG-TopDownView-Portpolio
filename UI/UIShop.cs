using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UI;


public enum ShopTabType
{
    Buy,
    Sell,
    Repurchase
}
public class UIShop : UIPanel
{
    public static UIShop Instance;
    public List<ShopItemSlot> itemForSale = new List<ShopItemSlot>();
    public List<InventorySlot> itemInCart = new List<InventorySlot>();
    public ShopItemSlot selectedItem;
    public NPCData curShopNPC;
    public List<Toggle> shopTabToggles = new List<Toggle>();

    //구매 아이템
    List<InventorySlot> buyItemSlots = new List<InventorySlot>();


    //판매 아이템
    List<ShopItemSlot> itemToSell = new List<ShopItemSlot>();
    

    bool isBuying = true;

    Toggle currentTg;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root);
        }
        else
        {
            Destroy(transform.root);
        }
    }

    public void OpenShop(NPCData _data)
    {
        curShopNPC = _data;
        ItemTable itemTb = TableLoader.Instance.GetTable<ItemTable>();
        for (int i = 0; i < itemForSale.Count; i++)
        {
            if (i < _data.SaleItemIDs.Count)
            {
                ItemData item = itemTb.GetItemDataByID(_data.SaleItemIDs[i]);
                SaveItemData itemData = new SaveItemData();
                itemData.ItemID = item.ItemID;
                itemForSale[i].SetBuyItemData(itemData);
            }
            else
            {
                itemForSale[i].EmptySlot();
            }
        }
        OnClickOpenButton();
    }

    public override void OnClickOpenButton()
    {
        base.OnClickOpenButton();
        UIInventory.Instance.OnClickOpenButton();
    }
    public override void OnClickCloseButton()
    {
        base.OnClickCloseButton();
        ClearCart();
    }

    #region[구매]
    /// <summary>
    /// 아이템을 구매목록에 추가하는 함수
    /// </summary>
    /// <param name="_data"></param>
    /// <param name="_qty"></param>
    public void AddBuyListItem(SaveItemData _data, int _qty = 1)
    {
        var item = itemInCart.Find(x => !x.IsEmpty && x.SaveItemData.ItemID == _data.ItemID);
        if (item == null)
        {
            item = itemInCart.Find(x => x.IsEmpty); 
            item.SetItemInfo(_data.DeepCopy());
            itemInCart.Add(item);
            buyItemSlots.Add(item);
        }
        item.AddQuantity(_qty);
        Debug.Log($"아이템을 구매 리스트에 {_qty}개 추가했습니다.");
    }
    public void OnClickBuyItemBtn()
    {
        foreach (var item in buyItemSlots)
        {
            InventoryManager.Instance.AddItem(item.SaveItemData);
            //UIInventory.Instance.AddItem(item.SaveItemData);
        }
        ClearCart();
    }

    void ClearCart()
    {
        foreach (var item in itemInCart)
        {
            item.SetItemInfo();
        }
        buyItemSlots.Clear();
    }
    #endregion[구매]
    #region[판매]

    #endregion[판매]

    #region[토글]
    public void OnClickShopTab(Toggle _tg)
    {
        if (!_tg.isOn)
        {
            return;
        }
        if (currentTg != _tg)
        {
            switch(_tg.name)
            {
                case "BuyTab":
                    break;
                case "SellTab":
                    break;
                case "RepurchaseTab":
                    break;
                default:
                    Debug.LogWarning("알 수 없는 탭");
                    break;
            }
            currentTg = _tg;
        }

    }
    #endregion[토글]
}
