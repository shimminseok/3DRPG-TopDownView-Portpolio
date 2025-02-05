using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : SlotBase, IPointerClickHandler, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] TextMeshProUGUI itemCountTxt;
    public int Index;

    ItemData data;
    SaveItemData saveItemData;
    public int Quantity => saveItemData.Quantity;
    public SaveItemData SaveItemData => saveItemData;
    public bool IsEmpty => saveItemData == null;

    public void SetItemInfo(SaveItemData _data = null)
    {
        if (_data == null)
        {
            Empty();
            return;
        }
        saveItemData = _data;
        data = _data.ItemData;
        SetItemImage(data.ItemImg);
        SetItemGradeImg(data.ItemGrade);
        itemCountTxt.text = saveItemData.Quantity <= 1 ? string.Empty : saveItemData.Quantity.ToString("N0");
        DeSelectedSlot();
        data.ItemStats.Initialize();
    }
    public void UpdateItemInfo()
    {
        itemCountTxt.text = saveItemData.Quantity <= 1 ? string.Empty : saveItemData.Quantity.ToString();
    }
    public void AddQuantity(int _amount)
    {
        saveItemData.Quantity += _amount;
        UpdateItemInfo();
    }
    public void RemoveQuantity(int _amount)
    {
        if(saveItemData.Quantity - _amount < 0)
        {
            Debug.LogError("보유 수량이 0보다 적습니다.");
            return;
        }
        saveItemData.Quantity -= _amount;
        UpdateItemInfo();
    }
    
    void Empty()
    {
        saveItemData = null;
        data = null;
        SetItemImage(null);
        itemCountTxt.text = string.Empty;
    }
    public void OnClickSlot()
    {
        SelectedSlot();
    }
    public override void SelectedSlot()
    {
        selectedImg.enabled = true;
        //TO Do
        //캐릭터Info라면 다르게 작동해야함
        UIInventory.Instance.SelecteItem(this);
    }
    public override void DeSelectedSlot()
    {
        selectedImg.enabled = false;
        Debug.Log($"{data?.Name}이 비활성화 되었습니다");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsEmpty || !EventSystem.current.IsPointerOverGameObject())
            return;

        if(eventData.button == PointerEventData.InputButton.Left)
        {
            SelectedSlot();
        }
        else if (eventData.button == PointerEventData.InputButton.Right) // 오른쪽 클릭 → 아이템 사용/장착
        {
            UseOrEquipItem();
        }
    }

    public void UseOrEquipItem()
    {
        if (EquipmentManager.Instance.IsEquipableType(saveItemData.ItemData.ItemType))
        {
            Debug.Log($"{saveItemData.ItemData.Name} 장착!");
            var equipItem = saveItemData.DeepCopy();
            InventoryManager.Instance.RemoveItem(Index);
            EquipmentManager.Instance.EquipItem(equipItem);

        }
        else if (saveItemData.ItemData.ItemType == ItemType.Potion)
        {
            Debug.Log($"{saveItemData.ItemData.Name} 사용!");
            InventoryManager.Instance.UseItem(Index,saveItemData);
        }
    }
    void SwichInvenSlot(InventorySlot _swich)
    {
        InventoryManager.Instance.SwichItem(_swich.Index, Index);
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (!DragManager.Instance.IsDragging)
            return;

        if(DragManager.Instance.DraggedInventoryItem != null)
        {
            SwichInvenSlot(DragManager.Instance.DraggedInventoryItem);
        }
        else if(DragManager.Instance.DraggedEquipItemData != null)
        {
            if (saveItemData != null)
                return;
            else
            {
                saveItemData = DragManager.Instance.DraggedEquipItemData;
                SetItemInfo(saveItemData);
                EquipmentManager.Instance.UnEquipItem(saveItemData.ItemData.ItemType);
            }
        }


    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        DragManager.Instance.StartDrag(this, transform);
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
