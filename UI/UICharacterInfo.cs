using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UICharacterInfo : UIPanel
{
    public static UICharacterInfo Instance { get; private set; }

    [SerializeField] TextMeshProUGUI nickName;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] List<EquipItemSlot> equipItemSlots = new List<EquipItemSlot>();
    [SerializeField] List<TextMeshProUGUI> statValueTexts = new List<TextMeshProUGUI>();

    [Header("CharacterAvartar")]
    [SerializeField] Transform avatarRoot;
    [SerializeField] string avatarPath = "Avatar/";
    string[] avatarName = new string[2] { "Elf_Warrior_Gn", "HuF_Assassin_Gn" };

    protected override void Awake()
    {
        base.Awake();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else
            Destroy(gameObject);


    }
    void Start()
    {
        for (int i = 0; i < equipItemSlots.Count; ++i)
        {
            equipItemSlots[i].EquipItem(EquipmentManager.Instance.GetEquipmentItem((ItemType)i));
        }
        nickName.text = PlayerController.Instance.characterName;


        GameObject avatar = Instantiate(Resources.Load<GameObject>($"{avatarPath}{avatarName[PlayerController.Instance.jobID - 1]}"),avatarRoot);
        
    }
    public void UpdateStatUI(Stat _stat)
    {
        if ((int)_stat.Type - 1 < statValueTexts.Count && _stat.IsChangeStat)
        {
            statValueTexts[(int)(_stat.Type - 1)].text = _stat.FinalValue.ToString();
            _stat.IsChangeStat = false;
        }
        if(_stat.Type == StatType.Level && _stat.IsChangeStat)
        {
            levelText.text = $"Lv.{_stat.FinalValue}";
        }
    }

    public override void OnClickOpenButton()
    {
        base.OnClickOpenButton();
        EquipmentManager.Instance.OnEquipmentChanged += UpdateEquipItem;
        for (int i = 0; i < Enum.GetValues(typeof(ItemType)).Length; ++i)
        {
            UpdateEquipItem((ItemType)i);
        }
        foreach (var stat in PlayerController.Instance.characterStat.Stats.Values.Where(x => x.IsChangeStat))
        {
            UpdateStatUI(stat);
        }
    }
    public override void OnClickCloseButton()
    {
        base.OnClickCloseButton();
        EquipmentManager.Instance.OnEquipmentChanged -= UpdateEquipItem;
    }


    void UpdateEquipItem(ItemType _type)
    {
        if (EquipmentManager.Instance.IsEquipableType(_type))
            equipItemSlots[(int)_type].EquipItem(EquipmentManager.Instance.GetEquipmentItem(_type));
    }
}
