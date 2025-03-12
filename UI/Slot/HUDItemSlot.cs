using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class HUDItemSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] int index;
    [SerializeField] Image icon;
    [SerializeField] Image coolTimeImg;
    [SerializeField] TextMeshProUGUI coolTimeText;
    [SerializeField] TextMeshProUGUI itemQtyText;
    [SerializeField] TextMeshProUGUI hotKeyText;
    public KeyCode slotHotKey;

    SaveItemData itemData;
    public int Index => index;
    public int registedInventoryIndex = -1;
    private void Start()
    {
        hotKeyText.text = slotHotKey.ToString();
        InputHandler.Instance.OnUseItem += UseItem;
    }
    public void Empty()
    {
        UnRegistedItem();
    }
    public void SetItemSlot(int _index)
    {
        SaveItemData data = InventoryManager.Instance.GetInventoryItemAtSlot(_index);
        itemData = data;
        if (itemData == null)
        {
            Empty();
            return;
        }
        RegistedItem(_index);
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (!DragManager.Instance.IsDragging || DragManager.Instance.DraggedInventoryItem == null)
            return;

        InventorySlot draggedItem = DragManager.Instance.DraggedInventoryItem;
         
        if (draggedItem.SaveItemData.GetItemData().IsUseable && draggedItem.SaveItemData.GetItemData().ItemType == ItemType.Potion)
        {
            SetItemSlot(draggedItem.Index);
            InventoryManager.Instance.ResisterdItems[slotHotKey] = itemData;
        }
    }
    void RegistedItem(int _index)
    {
        icon.enabled = true;
        icon.sprite = SpriteAtlasManager.Instance.GetSprite("Item", itemData.GetItemData().ItemImg);
        itemQtyText.text = itemData.Quantity.ToString();
        registedInventoryIndex = _index;

        InventoryManager.Instance.OnInventorySlotUpdate += UpdateHUDSlot;
    }
    void UnRegistedItem()
    {
        itemData = null;
        icon.enabled = false;
        itemQtyText.text = string.Empty;
        registedInventoryIndex = -1;
        InventoryManager.Instance.OnInventorySlotUpdate -= UpdateHUDSlot;
    }
    public void UseItem(KeyCode _code)
    {
        if (_code != slotHotKey)
            return;

        if (itemData != null && registedInventoryIndex >= 0 && registedInventoryIndex < InventoryManager.Instance.GetInventoryItem().Count)
        {
            InventoryManager.Instance.UseItem(registedInventoryIndex, itemData);
        }
    }
    void UpdateHUDSlot(int _index)
    {
        if (_index != registedInventoryIndex)
            return;


        itemQtyText.text = itemData.Quantity.ToString();
        //SetItemSlot(_index);
    }

    public SaveItemData GetItemData()
    {
        return itemData;
    }
}
