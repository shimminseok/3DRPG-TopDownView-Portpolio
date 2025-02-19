using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnhanceMaterialSlot : SlotBase
{

    [SerializeField] TextMeshProUGUI CountTxt;

    SaveItemData targetItem;


    public void SetMaterialSlot(MaterialRequirement _data)
    {
        if (_data == null)
        {
            Empty();
            return;
        }
        ItemData targetTbData = TableLoader.Instance.GetTable<ItemTable>().GetItemDataByID(_data.MaterialID);
        SetItemImage(targetTbData.ItemImg);
        SetItemGradeImg(targetTbData.ItemGrade);

        targetItem = InventoryManager.Instance.GetInventoryItemByItemID(_data.MaterialID);
        UpdateSlot(_data.Quantity);
    }
    public void Empty()
    {
        targetItem = null;
        CountTxt.text = string.Empty;
    }
    public void UpdateSlot(int _requiredCnt)
    {
        string colorTxt = targetItem == null || targetItem.Quantity < _requiredCnt  ? "<color=red>" : "<color=green>";

        int quantity = targetItem == null ? 0 : targetItem.Quantity;
        CountTxt.text = $"{colorTxt}{quantity}</color>/{_requiredCnt}";
    }
}
