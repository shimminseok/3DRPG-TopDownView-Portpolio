using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillListSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] Image skillIcon;
    [SerializeField] TextMeshProUGUI skillName;
    [SerializeField] Image selectedImg;

    public SaveSkillData SaveSkillData { get; private set; }

    public void SetSkillSlot(SaveSkillData _data)
    {
        SaveSkillData = _data;
        SkillData tableData = SaveSkillData.GetSkillData();
        skillIcon.sprite = SpriteAtlasManager.Instance.GetSprite("Skill", tableData.SkillImage);
        skillName.text = tableData.Name;
    }

    public void SelectedSlot()
    {
        selectedImg.enabled = true;
        UISkill.Instance.DisplaySkillInfo(this);
    }

    public void DeSelectedSlot()
    {
        selectedImg.enabled = false;
    }
    public void OnClickSlot()
    {
        SelectedSlot();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        DragManager.Instance.StartDrag(SaveSkillData, transform);
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
