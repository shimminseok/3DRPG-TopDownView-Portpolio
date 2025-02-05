using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemTable", menuName = "Table/ItemTable")]
public class ItemTable : ScriptableObject
{

    [SerializeField] List<ItemData> data = new List<ItemData>();

    Dictionary<int, ItemData> itemDataDic = new Dictionary<int, ItemData>();

    private void OnEnable()
    {
        itemDataDic.Clear();
        foreach (var item in data)
        {
            itemDataDic[item.ItemID] = item;
        }
    }
    public ItemData GetItemDataByID(int _id)
    {
        return itemDataDic[_id];
    }
}
