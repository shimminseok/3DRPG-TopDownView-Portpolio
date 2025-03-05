using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnhancementItemListSlot : SlotBase,ISelectableSlot
{
    [SerializeField] TextMeshProUGUI targetItemName;
    [SerializeField] TextMeshProUGUI targetItemEnhanceCnt;
    [SerializeField] Image selectedImg;
    SaveItemData targetItemData;
    bool isSelected;
    public SaveItemData TargetItemData => targetItemData;
    public void SetEnhanceListSlot(SaveItemData _target)
    {
        DeSelectedSlot();
        UpdateEnhancementCount(_target);
        targetItemName.text = _target.GetItemData().Name;
        SetItemImage(SpriteAtlasManager.Instance.GetSprite("Item",_target.GetItemData().ItemImg));
        SetItemGradeImg(_target.GetItemData().ItemGrade);


    }
    public void SelectedSlot()
    {
        UIEnhancement.Instance.SetEnhanceTargetItem(this);
        EnhancementManager.Instance.OnEnhancedItem += UpdateEnhancementCount;
    }
    public void DeSelectedSlot()
    {
        EnhancementManager.Instance.OnEnhancedItem -= UpdateEnhancementCount;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(isSelected)
            DeSelectedSlot();
        else
            SelectedSlot();

        isSelected = !isSelected;
        selectedImg.enabled = isSelected;

    }
    public void UpdateEnhancementCount(SaveItemData _itemData)
    {
        targetItemData = _itemData;
        targetItemEnhanceCnt.text = _itemData.enhanceLevel == 0 ? "" : $"{_itemData.enhanceLevel}´Ü°è";
    }


}
