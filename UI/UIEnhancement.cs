using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnhancement : UIPanel
{
    public static UIEnhancement Instance;

    [SerializeField] Transform enhanceItemListSlotRoot;
    [SerializeField] EnhancementItemListSlot enhanceItemListSlotPrefab = new EnhancementItemListSlot();
    EnhancementItemListSlot slelectedEnhanceListSlot;

    [Header("TargetItemEnhanceInfo")]
    [SerializeField] Image targetItemImg;
    [SerializeField] List<EnhanceMaterialSlot> materialItems = new List<EnhanceMaterialSlot>();
    List<EnhancementItemListSlot> enhancementItemListSlots = new List<EnhancementItemListSlot>();


    SaveItemData targetItem;
    EnhanceData targetEnhanceData;
    protected override void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetEnhanceTargetItem(EnhancementItemListSlot _itemData)
    {
        if(slelectedEnhanceListSlot == null || slelectedEnhanceListSlot  != _itemData)
        {
            slelectedEnhanceListSlot = _itemData;
            targetItem = slelectedEnhanceListSlot.TargetItemData;
            targetEnhanceData = TableLoader.Instance.GetTable<EnhancementTable>().GetEnhanceDataByLevelAndGrade(targetItem.enhanceLevel, targetItem.ItemData.ItemGrade);
            targetItemImg.sprite = targetItem.ItemData.ItemImg;
        }


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
        for (int i = 0; i < Enum.GetValues(typeof(ItemType)).Length; i++)
        {
            if (i < (int)ItemType.Potion)
            {
                var targetItem = EquipmentManager.Instance.GetEquipmentItem((ItemType)i);
                if (targetItem == null)
                    continue;

                var slot = Instantiate(enhanceItemListSlotPrefab,enhanceItemListSlotRoot);
                slot.SetEnhanceListSlot(targetItem);
            }
        }
    }
    public override void OnClickCloseButton()
    {
        base.OnClickCloseButton();
        EnhancementManager.Instance.OnEnhancedItem -= UpdateEnhancementUI;
        EnhancementManager.Instance.OnEnhanceSuccess -= EnhanceSuccess;
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
            }
        }
    }
    public void EnhanceSuccess()
    {
        targetEnhanceData = TableLoader.Instance.GetTable<EnhancementTable>().GetEnhanceDataByLevelAndGrade(targetItem.enhanceLevel, targetItem.ItemData.ItemGrade);
        SetEnhanceTargetItem(slelectedEnhanceListSlot);
    }
}
