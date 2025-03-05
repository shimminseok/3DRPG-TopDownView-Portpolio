using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISkill : UIPanel
{
    public static UISkill Instance;

    [Header("SkillList")]
    [SerializeField] SkillListSlot slotPrefab;
    [SerializeField] UnityEngine.UIElements.ScrollView slotScrollView;
    [SerializeField] Transform slotRoot;

    [Header("Skill Info")]
    [SerializeField] GameObject skillInfoObj;
    [SerializeField] UnityEngine.UI.Image skillIcon;
    [SerializeField] TextMeshProUGUI skillName;
    [SerializeField] TextMeshProUGUI skillLv;
    [SerializeField] TextMeshProUGUI requiredMP;
    [SerializeField] TextMeshProUGUI coolTimeTxt;
    [SerializeField] TextMeshProUGUI skillInfo;
    [SerializeField] TextMeshProUGUI remainSkillPointTxt;


    public SkillListSlot currentSkillSlot;


    protected override void Awake()
    {
        base.Awake();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        //InstanteSkillListSlot();
    }

    public void InstanteSkillListSlot()
    {
        List<SaveSkillData> skillList = PlayerController.Instance.SkillManager.AvailableSkills;
        if(slotRoot.childCount == 0)
        {
            for (int i = 0; i < skillList.Count; i++)
            {
                SkillListSlot slot = Instantiate(slotPrefab, slotRoot);
                slot.SetSkillSlot(skillList[i]);
            }
        }

    }


    public void DisplaySkillInfo(SkillListSlot _slot)
    {
        skillInfoObj.SetActive(true);
        if (currentSkillSlot != _slot)
        {
            currentSkillSlot?.DeSelectedSlot();
            currentSkillSlot = _slot;
        }
        SkillData tbData = _slot.SaveSkillData.GetSkillData();
        skillIcon.sprite = SpriteAtlasManager.Instance.GetSprite("Skill", tbData.SkillImage.name);
        skillName.text = tbData.Name;
        skillLv.text = $"Lv.{_slot.SaveSkillData.SkillLevel}";
        requiredMP.text = $"소모 MP: {tbData.RequiredMP}";
        coolTimeTxt.text = $"쿨타임: {tbData.CoolTime}초";
        skillInfo.text = tbData.Description;
    }
    void UpdateSkillPoint(int _remainPoint)
    {
        remainSkillPointTxt.text = _remainPoint.ToString("D0");
    }
    public override void OnClickOpenButton()
    {
        base.OnClickOpenButton();
        InstanteSkillListSlot();
        skillInfoObj.SetActive(false);
    }
    public override void OnClickCloseButton()
    {
        base.OnClickCloseButton();
        currentSkillSlot = null;
    }
}
