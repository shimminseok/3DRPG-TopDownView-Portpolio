using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopNPC : MonoBehaviour, INPCFunction
{
    NPCData npcData;
    List<ItemData> saleItems= new List<ItemData>();
    void Start()
    {
        
    }
    void Update()
    {
        
    }

    public void Execute()
    {
        Debug.Log("Open Shop");
        OpenShopUI();
    }
    public void Initialize(NPCData _data)
    {
        npcData = _data;
        RegisterSaleItem();
    }

    void OpenShopUI()
    {
        UIShop.Instance.OpenShop(npcData);
    }
    public void RegisterSaleItem()
    {
        //ItemTable itemTb = TableLoader.Instance.GetTable<ItemTable>();
        //foreach(int id in npcData.SaleItemIDs)
        //{
        //    ItemData saleItem = itemTb.GetItemDataByID(id);
        //    if (saleItem != null)
        //    {
        //        saleItems.Add(saleItem);
        //    }
        //}
        //Debug.LogWarning("판매 아이템을 등록했습니다");
    }
}
