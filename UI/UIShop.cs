using System.Collections;
using System.Collections.Generic;
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
    public NPCData curShopNPC;
    public List<Toggle> shopTabToggles = new List<Toggle>();

    //구매 아이템
    List<InventorySlot> buyItemSlots = new List<InventorySlot>();


    //판매 아이템
    List<ShopItemSlot> itemToSell = new List<ShopItemSlot>();
    ShopItemSlot selectedItem;


    int totalBuyItemPrice = 0;
    Toggle currentTg;

    public ShopItemSlot SelectedItem
    {
        get { return selectedItem; }
        set { selectedItem = value; }
    }
    protected override void Awake()
    {
        base.Awake();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OpenShop(List<ItemData> _saleData)
    {
        ItemTable itemTb = TableLoader.Instance.GetTable<ItemTable>();
        for (int i = 0; i < itemForSale.Count; i++)
        {
            if (i < _saleData.Count)
            {
                ItemData item = _saleData[i];
                itemForSale[i].SetBuyItemData(item);
            }
            else
            {
                itemForSale[i].EmptySlot();
            }
        }
        for (int i = 0; i < itemInCart.Count; i++)
        {
            itemInCart[i].Empty();
        }
        Open();
    }

    public override void Open()
    {
        base.Open();
        UIInventory.Instance.Open();
    }
    public override void Close()
    {
        base.Close();
        ClearCart();
    }

    #region[구매]
    /// <summary>
    /// 아이템을 구매목록에 추가하는 함수
    /// </summary>
    /// <param name="_data"></param>
    /// <param name="_qty"></param>
    public void AddBuyListItem(ItemData _data, int _qty = 1)
    {
        if (buyItemSlots.Count == itemInCart.Count)
            return;

        if (!AccountManager.Instance.IsEnoughtGold(totalBuyItemPrice + _data.Price))
        {
            UIHUD.Instance.OnAletMessage?.Invoke("골드가 부족합니다.");
            return;
        }

        var item = itemInCart.Find(x => !x.IsEmpty && x.SaveItemData.ItemID == _data.ItemID);
        if (item == null)
        {
            item = itemInCart.Find(x => x.IsEmpty);
            item.SetItemInfo(_data);
            buyItemSlots.Add(item);
        }
        item.AddQuantity(_qty);
        totalBuyItemPrice += _data.Price;
        UIHUD.Instance.OnAletMessage?.Invoke($"아이템을 구매 리스트에 {_qty}개 추가했습니다.");
    }
    public void OnClickBuyItemBtn()
    {
        foreach (var item in buyItemSlots)
        {
            InventoryManager.Instance.AddItem(item.SaveItemData);
        }
        AccountManager.Instance.UseGold(totalBuyItemPrice);

        ClearCart();
    }

    void ClearCart()
    {
        foreach (var item in itemInCart)
        {
            item.SetItemInfo();
        }
        buyItemSlots.Clear();
        totalBuyItemPrice = 0;
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
            switch (_tg.name)
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
