using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillListSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] Image skillIcon;
    [SerializeField] TextMeshProUGUI skillName;
    [SerializeField] TextMeshProUGUI SkillcurrentLevel;
    [SerializeField] Image selectedImg;
    
    public SaveSkillData saveSkillData;

    public void SetSkillSlot(SaveSkillData _data)
    {
        saveSkillData = _data;
        SkillData tableData = saveSkillData.GetSkillData();
        skillIcon.sprite = SpriteAtlasManager.Instance.GetSprite("Skill", tableData.SkillImage);
        SkillcurrentLevel.text = $"Lv.{saveSkillData.SkillLevel}";
        skillName.text = tableData.Name;
    }

    public  void SelectedSlot()
    {
        selectedImg.enabled = true;
        UISkill.Instance.DisplaySkillInfo(this);
    }

    public  void DeSelectedSlot()
    {
        selectedImg.enabled = false;
    }
    public void OnClickSlot()
    {
        SelectedSlot();
    }

    public void OnClickLevelUPBtn()
    {
        //스킬 포인트가 있으면
        saveSkillData.SkillLevel++;
    }
    public void OnClickLevelDownBtn()
    {
        saveSkillData.SkillLevel--;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        DragManager.Instance.StartDrag(saveSkillData, transform);
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
