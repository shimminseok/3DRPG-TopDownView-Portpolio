using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance { get; private set; }

    [SerializeField]
    Dictionary<ItemType, SaveItemData> equipmentItems = new Dictionary<ItemType, SaveItemData>();

    public event Action<ItemType> OnEquipmentChanged;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        //임시(초기화)
        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            if (IsEquipableType(type))
            {
                equipmentItems[type] = null;
            }
        }
    }

    public bool IsEquipableType(ItemType _type)
    {
        switch (_type)
        {
            case ItemType.Weapon:
            case ItemType.Armor:
            case ItemType.Helmet:
            case ItemType.Goroves:
            case ItemType.Shoes:
                return true;

            default:
                return false;
        }
    }



    public SaveItemData GetEquipmentItem(ItemType _type)
    {
        if (equipmentItems.ContainsKey(_type))
        {
            return equipmentItems[_type];
        }
        return null;
    }

    public void EquipItem(SaveItemData _data)
    {
        if (_data == null || !IsEquipableType(_data.ItemData.ItemType))
        {
            Debug.Log("장착이 불가능한 아이템입니다.");
            return;
        }
        ItemType slot = _data.ItemData.ItemType;

        if (equipmentItems.ContainsKey(slot))
        {
            UnEquipItem(slot);
            equipmentItems[slot] = _data;
            foreach (var stat in equipmentItems[slot].ItemData.ItemStats.Stats)
            {
                PlayerController.Instance.characterStat.Stats[stat.Key].ModifyEquipmentValue(stat.Value);
            }
            OnEquipmentChanged?.Invoke(slot);

        }
    }

    public void UnEquipItem(ItemType _type)
    {
        if (equipmentItems.ContainsKey(_type) && equipmentItems[_type] != null)
        {
            InventoryManager.Instance.AddItem(equipmentItems[_type]);
            foreach (var stat in equipmentItems[_type].ItemData.ItemStats.Stats)
            {
                PlayerController.Instance.characterStat.Stats[stat.Key].ModifyEquipmentValue(-stat.Value);
            }
            equipmentItems[_type] = null;
            OnEquipmentChanged?.Invoke(_type);
        }
    }


}
