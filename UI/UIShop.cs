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
    public NPCData curShopNPC;
    public List<Toggle> shopTabToggles = new List<Toggle>();

    //���� ������
    List<InventorySlot> buyItemSlots = new List<InventorySlot>();


    //�Ǹ� ������
    List<ShopItemSlot> itemToSell = new List<ShopItemSlot>();
    ShopItemSlot selectedItem;
    

    bool isBuying = true;
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
                itemForSale[i].SetBuyItemData(item);
            }
            else
            {
                itemForSale[i].EmptySlot();
            }
        }
        for(int i = 0; i <itemInCart.Count; i++)
        {
            itemInCart[i].Empty();
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

    #region[����]
    /// <summary>
    /// �������� ���Ÿ�Ͽ� �߰��ϴ� �Լ�
    /// </summary>
    /// <param name="_data"></param>
    /// <param name="_qty"></param>
    public void AddBuyListItem(ItemData _data, int _qty = 1)
    {
        if (buyItemSlots.Count == itemInCart.Count)
            return;
        var item = itemInCart.Find(x => !x.IsEmpty && x.SaveItemData.ItemID == _data.ItemID);
        if (item == null)
        {
            item = itemInCart.Find(x => x.IsEmpty); 
            item.SetItemInfo(_data);
            buyItemSlots.Add(item);
        }
        item.AddQuantity(_qty);
        Debug.Log($"�������� ���� ����Ʈ�� {_qty}�� �߰��߽��ϴ�.");
    }
    public void OnClickBuyItemBtn()
    {
        foreach (var item in buyItemSlots)
        {
            InventoryManager.Instance.AddItem(item.SaveItemData);
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
    #endregion[����]
    #region[�Ǹ�]

    #endregion[�Ǹ�]

    #region[���]
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
                    Debug.LogWarning("�� �� ���� ��");
                    break;
            }
            currentTg = _tg;
        }

    }
    #endregion[���]
}
