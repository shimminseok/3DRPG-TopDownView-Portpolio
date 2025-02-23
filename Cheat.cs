using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheat : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            PlayerController.Instance.characterStat.GainExp(1000);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            PlayerController.Instance.characterStat.Attack.ModifyBaseValue(10, 1, int.MaxValue);
        }

        if(Input.GetKeyDown (KeyCode.F3))
        {
            PlayerController.Instance.TakeDamage(500);
        }

        if(Input.GetKeyDown(KeyCode.F4))
        {
            ItemTable itemTb = TableLoader.Instance.GetTable<ItemTable>();

            for (int i = 0; i < itemTb.Data.Count; i++)
            {
                 SaveItemData saveItemData = new SaveItemData();
                saveItemData.ItemID = itemTb.Data[i].ItemID;
                 saveItemData.Quantity = itemTb.Data[i].ItemType <= ItemType.Shoes ? 1 : 30;
                InventoryManager.Instance.AddItem(saveItemData);
            }
        }
        if(Input.GetKeyDown(KeyCode.F5))
        {
            AccountManager.Instance.AddGold(1000);
        }
    }
}
