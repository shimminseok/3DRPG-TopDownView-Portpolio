using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnhancementItemListSlot : SlotBase
{
    [SerializeField] TextMeshProUGUI targetItemName;
    [SerializeField] TextMeshProUGUI targetItemEnhanceCnt;

    SaveItemData targetItemData;
    public SaveItemData TargetItemData => targetItemData;
    public void SetEnhanceListSlot(SaveItemData _target)
    {
        DeSelectedSlot();
        UpdateEnhancementCount(_target);
        targetItemName.text = _target.ItemData.Name;
        SetItemImage(_target.ItemData.ItemImg);
        SetItemGradeImg(_target.ItemData.ItemGrade);


    }
    public void OnClickEnhanceListSlot()
    {
        SelectedSlot();
    }
    public override void SelectedSlot()
    {
        base.SelectedSlot();
        UIEnhancement.Instance.SetEnhanceTargetItem(this);
        EnhancementManager.Instance.OnEnhancedItem += UpdateEnhancementCount;
    }
    public override void DeSelectedSlot()
    {
        base.DeSelectedSlot();
        EnhancementManager.Instance.OnEnhancedItem -= UpdateEnhancementCount;
    }
    public void UpdateEnhancementCount(SaveItemData _itemData)
    {
        targetItemData = _itemData;
        targetItemEnhanceCnt.text = _itemData.enhanceLevel == 0 ? "" : $"{_itemData.enhanceLevel}´Ü°è";
    }


}
