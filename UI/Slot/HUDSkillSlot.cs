using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class HUDSkillSlot : MonoBehaviour, IDropHandler
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
        if (!DragManager.Instance.IsDragging)
            return;

        assigendSkill = DragManager.Instance.DraggedSkill;

        if(assigendSkill != null)
        {
            assigendSkill.HotKey = slotHotKey;
            icon.sprite = assigendSkill.GetSkillData().SkillImage;
            PlayerController.Instance.SkillManager.AssignSkill(this);
        }
    }
    public void StartCoolTime(float _duration)
    {
        coolTimeImg.fillAmount = 1f;
        StartCoroutine(UpdateCoolTime(_duration));
    }

    IEnumerator UpdateCoolTime(float _coolTime)
    {
        float remainingCoolTime = _coolTime;
        while(remainingCoolTime > 0)
        {
            remainingCoolTime -= Time.deltaTime;
            coolTimeImg.fillAmount = remainingCoolTime / _coolTime;
            if(remainingCoolTime >= 1)
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
}
