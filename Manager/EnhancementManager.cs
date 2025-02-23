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
            Debug.LogWarning($"��ȭ �����Ͱ� �������� �ʽ��ϴ�. {level},{grade}");

        //��ȭ Ȯ��
        if (!AccountManager.Instance.IsEnoughtGold(enhanceData.GoldCost))
            return;

        //��� Ȯ��
        foreach (var mat in enhanceData.Requirements)
        {
            if (!CheckMaterialInInventory(mat))
            {
                Debug.LogWarning($"��ᰡ �����մϴ�. {mat.MaterialID}");
                return;
            }
        }
        foreach (var mat in enhanceData.Requirements)
        {
            SaveItemData itemData = InventoryManager.Instance.GetInventoryItemByItemID(mat.MaterialID);
            InventoryManager.Instance.UseItem(-1, itemData, mat.Quantity);
        }
        //��ȭ Ȯ��
        bool isSuccess = UnityEngine.Random.value <= enhanceData.SuccessRate;
        if (isSuccess)
        {
            //��ȭ ����
            EnhanceSuccess(_targetItem);
        }
        else
            Debug.LogWarning("��ȭ ����");

        //UIUpdate
        AccountManager.Instance.UseGold(enhanceData.GoldCost);
        OnEnhancedItem?.Invoke(_targetItem);
    }
    void EnhanceSuccess(SaveItemData _targetItem)
    {
        Debug.Log("��ȭ ����!");
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
