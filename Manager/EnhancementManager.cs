using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class EnhancementManager : MonoBehaviour
{
    public static EnhancementManager Instance;


    public event Action<SaveItemData> OnEnhancedItem;
    public event Action OnEnhanceSuccess;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

    }
    void Start()
    {

    }

    public void TryEnhance(SaveItemData _targetItem)
    {
        int level = _targetItem.enhanceLevel;
        int grade = _targetItem.ItemData.ItemGrade;

        EnhanceData enhanceData = TableLoader.Instance.GetTable<EnhancementTable>().GetEnhanceDataByLevelAndGrade(level, grade);

        if (enhanceData == null)
            Debug.LogWarning($"강화 데이터가 존재하지 않습니다. {level},{grade}");

        //재화 확인
        if (!AccountManager.Instance.IsEnoughtGold(enhanceData.GoldCost))
            return;

        //재료 확인
        foreach (var mat in enhanceData.Requirements)
        {
            if (!CheckMaterialInInventory(mat))
            {
                Debug.LogWarning($"재료가 부족합니다. {mat.MaterialID}");
                return;
            }
        }
        foreach (var mat in enhanceData.Requirements)
        {
            SaveItemData itemData = InventoryManager.Instance.GetInventoryItemByItemID(mat.MaterialID);
            InventoryManager.Instance.UseItem(-1, itemData, mat.Quantity);
        }
        //강화 확률
        bool isSuccess = UnityEngine.Random.value <= enhanceData.SuccessRate;
        if (isSuccess)
        {
            //강화 성공
            EnhanceSuccess(_targetItem);
        }
        else
            Debug.LogWarning("강화 실패");

        //UIUpdate
        AccountManager.Instance.UseGold(enhanceData.GoldCost);
        OnEnhancedItem?.Invoke(_targetItem);
    }
    void EnhanceSuccess(SaveItemData _targetItem)
    {
        Debug.Log("강화 성공!");
        _targetItem.enhanceLevel++;
        OnEnhanceSuccess?.Invoke();


    }
    void EnhanceFailure()
    {

    }
    bool CheckMaterialInInventory(MaterialRequirement _requiredMat)
    {
        return InventoryManager.Instance.GetInventoryItem().Exists(x => x != null && x.ItemID == _requiredMat.MaterialID && x.Quantity >= _requiredMat.Quantity);
    }
}
