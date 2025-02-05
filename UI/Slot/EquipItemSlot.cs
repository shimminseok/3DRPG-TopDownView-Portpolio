using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipItemSlot : SlotBase, IPointerClickHandler, IBeginDragHandler,IDragHandler, IEndDragHandler
{
    [SerializeField] TextMeshProUGUI itemEnhanceCnt; 
    [SerializeField] ItemType itemType;

    bool isDrag = false;
    SaveItemData equipItem;



    public void EquipItem(SaveItemData _item)
    {
        if(_item == null)
        {
            Empty();
            return;
        }
        equipItem = _item;
        SetItemImage(_item.ItemData.ItemImg);
        SetItemGradeImg(_item.ItemData.ItemGrade);
        itemEnhanceCnt.text = _item.enhanceLevel > 0 ? _item.enhanceLevel.ToString() : "";
    }
    void Empty()
    {
        equipItem = null;
        SetItemImage(null);
        itemEnhanceCnt.text = string.Empty;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right && equipItem != null)
        {
            EquipmentManager.Instance.UnEquipItem(itemType);
            Debug.Log("��� ���� �Ǿ����ϴ�.");
            //���� ����
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        DragManager.Instance.StartDrag(equipItem, transform,false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        DragManager.Instance.UpdateDrag(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragManager.Instance.EndDrag();
    }
}
