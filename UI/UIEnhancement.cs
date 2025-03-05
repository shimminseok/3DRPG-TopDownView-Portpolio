using Michsky.MUIP;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEnhancement : UIPanel
{
    public static UIEnhancement Instance;

    [SerializeField] Transform enhanceItemListSlotRoot;
    [SerializeField] EnhancementItemListSlot enhanceItemListSlotPrefab = new EnhancementItemListSlot();
    EnhancementItemListSlot selectedEnhanceListSlot;

    [Header("TargetItemEnhanceInfo")]
    [SerializeField] Image targetItemImg;
    [SerializeField] TextMeshProUGUI needGoldText;
    [SerializeField] TextMeshProUGUI currentGoldText;
    [SerializeField] List<EnhanceMaterialSlot> materialItems = new List<EnhanceMaterialSlot>();
    List<EnhancementItemListSlot> enhancementItemListSlots = new List<EnhancementItemListSlot>();

    [SerializeField] SkeletonGraphic enhanceResultEffect;


    SaveItemData targetItem;
    EnhanceData targetEnhanceData;



    protected override void Awake()
    {
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
    public void SetEnhanceTargetItem(EnhancementItemListSlot _itemData)
    {
        if (selectedEnhanceListSlot == null || selectedEnhanceListSlot != _itemData)
        {
            selectedEnhanceListSlot?.DeSelectedSlot();
            selectedEnhanceListSlot = _itemData;
            targetItem = selectedEnhanceListSlot.TargetItemData;
            targetEnhanceData = TableLoader.Instance.GetTable<EnhancementTable>().GetEnhanceDataByLevelAndGrade(targetItem.enhanceLevel, targetItem.GetItemData().ItemGrade);
            targetItemImg.enabled = true;
            targetItemImg.sprite = SpriteAtlasManager.Instance.GetSprite("Item",targetItem.GetItemData().ItemImg);
        }
        needGoldText.text = targetEnhanceData.GoldCost.ToString("N0");
        UpdateEnhancementUI(targetItem);
    }
    public void OnClickEnhancement()
    {
        EnhancementManager.Instance.TryEnhance(targetItem);

    }

    public override void OnClickOpenButton()
    {
        base.OnClickOpenButton();
        EnhancementManager.Instance.OnEnhancedItem += UpdateEnhancementUI;
        EnhancementManager.Instance.OnEnhanceSuccess += EnhanceSuccess;
        EnhancementManager.Instance.OnEnhanceFail += EnhanceFailure;
        AccountManager.Instance.OnChangedGold += UpdateCurrentGoldUI;

        for (int i = 0; i < Enum.GetValues(typeof(ItemType)).Length; i++)
        {
            if (i < (int)ItemType.Potion)
            {
                var targetItem = EquipmentManager.Instance.GetEquipmentItem((ItemType)i);
                if (targetItem == null)
                    continue;

                var slot = Instantiate(enhanceItemListSlotPrefab, enhanceItemListSlotRoot);
                slot.SetEnhanceListSlot(targetItem);
            }
        }
        UpdateCurrentGoldUI(AccountManager.Instance.Gold);
        ResetUI();
    }
    public override void OnClickCloseButton()
    {
        base.OnClickCloseButton();
        EnhancementManager.Instance.OnEnhancedItem -= UpdateEnhancementUI;
        EnhancementManager.Instance.OnEnhanceSuccess -= EnhanceSuccess;
        EnhancementManager.Instance.OnEnhanceFail -= EnhanceFailure;
        AccountManager.Instance.OnChangedGold -= UpdateCurrentGoldUI;

        for (int i = 0; i < enhanceItemListSlotRoot.childCount; i++)
        {
            Destroy(enhanceItemListSlotRoot.GetChild(i).gameObject);
        }
    }

    public void UpdateEnhancementUI(SaveItemData _data)
    {
        if (_data != targetItem)
            return;

        for (int i = 0; i < materialItems.Count; i++)
        {
            if (i < targetEnhanceData.Requirements.Count)
            {
                materialItems[i].gameObject.SetActive(true);
                materialItems[i].SetMaterialSlot(targetEnhanceData.Requirements[i]);
            }
            else
            {
                materialItems[i].gameObject.SetActive(false);
                materialItems[i].SetMaterialSlot(null);
            }
        }
    }
    void UpdateCurrentGoldUI(int _currentGold)
    {
        currentGoldText.color = targetEnhanceData?.GoldCost > _currentGold ? Color.red : Color.white;
        currentGoldText.text = _currentGold.ToString("N0");
    }
    void EnhanceSuccess()
    {
        UIManager.Instance.PlaySkeletonAnimation(enhanceResultEffect,"success");
        targetEnhanceData = TableLoader.Instance.GetTable<EnhancementTable>().GetEnhanceDataByLevelAndGrade(targetItem.enhanceLevel, targetItem.GetItemData().ItemGrade);
        SetEnhanceTargetItem(selectedEnhanceListSlot);
    }
    void EnhanceFailure()
    {
        UIManager.Instance.PlaySkeletonAnimation(enhanceResultEffect, "fail");
    }

    void ResetUI()
    {
        for (int i = 0; i < materialItems.Count; i++)
        {
            materialItems[i].SetMaterialSlot(null);
        }
        selectedEnhanceListSlot = null;
        if (targetItem != null)
            targetItem = null;
        if (targetEnhanceData != null)
            targetEnhanceData = null;

        targetItemImg.enabled = false;
        needGoldText.text = 0.ToString();
    }
}
