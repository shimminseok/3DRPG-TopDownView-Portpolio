using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopNPC : MonoBehaviour, INPCFunction
{
    List<ItemData> saleItems= new List<ItemData>();

    public NPCFunction FuncType => NPCFunction.Shop;

    public void Execute()
    {
        UIManager.Instance.AllClosePanel();
        OpenShopUI();
    }
    public void Initialize(NPCData _data)
    {
        RegisterSaleItem(_data);
    }

    void OpenShopUI()
    {
        UIShop.Instance.OpenShop(saleItems);
    }
    public void RegisterSaleItem(NPCData _data)
    {
        saleItems.Clear();
        ItemTable itemTb = TableLoader.Instance.GetTable<ItemTable>();
        foreach (int id in _data.SaleItemIDs)
        {
            ItemData saleItem = itemTb.GetItemDataByID(id);
            if (saleItem != null)
            {
                saleItems.Add(saleItem);
            }
        }
        Debug.LogWarning("판매 아이템을 등록했습니다");
    }
}
