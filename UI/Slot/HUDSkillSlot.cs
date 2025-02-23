using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HUDSkillSlot : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] int index;
    [SerializeField] Image icon;
    [SerializeField] Image coolTimeImg;
    [SerializeField] TextMeshProUGUI coolTimeText;
    [SerializeField] TextMeshProUGUI hotKeyText;
    public KeyCode slotHotKey;
    public SaveSkillData assigendSkill;


    void Start()
    {
        hotKeyText.text = slotHotKey.ToString();
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        if (!DragManager.Instance.IsDragging)
            return;

        assigendSkill = DragManager.Instance.DraggedSkill;

        if (assigendSkill != null)
        {
            AssingedSkill();
        }
    }
    void AssingedSkill()
    {
        assigendSkill.HotKey = slotHotKey;
        icon.enabled = true;
        icon.sprite = assigendSkill.GetSkillData().SkillImage;
        PlayerController.Instance.SkillManager.AssignSkill(this);
    }
    public void UnAssigendSkill()
    {
        icon.enabled = false;
        PlayerController.Instance.SkillManager.UnAssignSkill(this);
        assigendSkill = null;
    }
    public void StartCoolTime(float _duration)
    {
        coolTimeImg.fillAmount = 1f;
        StartCoroutine(UpdateCoolTime(_duration));
    }

    IEnumerator UpdateCoolTime(float _coolTime)
    {
        float remainingCoolTime = _coolTime;
        while (remainingCoolTime > 0)
        {
            remainingCoolTime -= Time.deltaTime;
            coolTimeImg.fillAmount = remainingCoolTime / _coolTime;
            if (remainingCoolTime >= 1)
            {
                coolTimeText.text = $"{remainingCoolTime.ToString("F0")}√ ";
            }
            else
            {
                coolTimeText.text = $"{remainingCoolTime.ToString("F1")}√ ";
            }
            yield return null;
        }
        coolTimeImg.fillAmount = 0f;
        coolTimeText.text = string.Empty;
        PlayerController.Instance.SkillManager.RemoveCoolTime(assigendSkill.SkillID);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (assigendSkill != null)
            DragManager.Instance.StartDrag(assigendSkill, transform);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (assigendSkill != null)
            DragManager.Instance.UpdateDrag(eventData.position);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("EndDrag");
        DragManager.Instance.EndDrag();
        UnAssigendSkill();
    }
}
