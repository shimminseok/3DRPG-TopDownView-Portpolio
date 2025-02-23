using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragManager : MonoBehaviour
{
    public static DragManager Instance;

    public SaveSkillData DraggedSkill { get; private set; }
    public InventorySlot DraggedInventoryItem { get; private set; }
    public SaveItemData DraggedEquipItemData { get; private set; }
    [SerializeField] Image dragImage;

    Canvas dragCanvas;

    public bool IsDragging { get; private set; }
    private void Awake()
    {
        Instance = this;
        dragImage.gameObject.SetActive(false);
    }

    public void StartDrag(SaveSkillData _skill, Transform _source)
    {

        DraggedSkill = _skill;
        SetDragSlot(_skill.GetSkillData().SkillImage);
    }
    public void StartDrag(SaveItemData _item, Transform _source, bool _isInvenSlot)
    {
        DraggedEquipItemData = _item;
        SetDragSlot(_item.ItemData.ItemImg);
    }
    public void StartDrag(InventorySlot _slot, Transform _source)
    {
        if (_slot.SaveItemData == null)
            return;
        DraggedInventoryItem = _slot;
        SetDragSlot(_slot.SaveItemData.ItemData.ItemImg);
    }
    public void UpdateDrag(Vector2 _position)
    {
        dragImage.transform.position = _position;
    }
    public void EndDrag()
    {
        if (DraggedSkill != null)
            DraggedSkill = null;

        if (DraggedEquipItemData != null)
            DraggedEquipItemData = null;

        if (DraggedInventoryItem != null)
            DraggedInventoryItem = null;

        dragImage.gameObject.SetActive(false);
        IsDragging = false;
    }

    void SetDragSlot(Sprite _img)
    {
        dragImage.gameObject.SetActive(true);
        dragImage.sprite = _img;
        dragImage.transform.SetAsLastSibling();
        IsDragging = true;
    }
}
