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

    //���� ������
    List<InventorySlot> buyItemSlots = new List<InventorySlot>();


    //�Ǹ� ������
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

    #region[����]
    /// <summary>
    /// �������� ���Ÿ�Ͽ� �߰��ϴ� �Լ�
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
        Debug.Log($"�������� ���� ����Ʈ�� {_qty}�� �߰��߽��ϴ�.");
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
