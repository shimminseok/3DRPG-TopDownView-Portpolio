using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public int MaxLine = 7;



    List<SaveItemData> inventory = new List<SaveItemData>();
    Dictionary<int, List<SaveItemData>> itemDictionary = new Dictionary<int, List<SaveItemData>>();
    public event Action<int> OnInventorySlotUpdate;
    public event Action<int, SaveItemData> OnHUDItemSlotUpdated;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        for (int i = 0; i < MaxLine * 7; i++)
        {
            inventory.Add(null);
        }
    }
    public void AddItem(SaveItemData _item)
    {
        // 스택형 아이템
        if (_item.ItemData.IsStackable)
        {
            SaveItemData findItem = inventory.Find(x => x != null && x.ItemID == _item.ItemID);
            if (findItem == null)
            {
                // To Do 인벤토리가 꽉찼는지 확인
                int index = inventory.FindIndex(x => x == null);
                if (index < 0)
                {
                    Debug.Log("인벤토리가 가득 찼습니다.");
                    return;
                }
                findItem = _item.DeepCopy();
                inventory[index] = findItem;
                OnInventorySlotUpdate?.Invoke(index);
            }
            else
            {
                int index = inventory.IndexOf(findItem);
                findItem.Quantity += _item.Quantity;
                OnInventorySlotUpdate?.Invoke(index);
            }

        }
        else //추가하는 아이템이 겹쳐지지않는 아이템
        {
            int index = inventory.FindAll(x => x == null).Count;
            if (index < _item.Quantity)
            {
                Debug.Log("인벤토리 공간이 부족하여 구매 할 수 없습니다.");
                return;
            }
            SaveItemData item = _item.DeepCopy();
            item.Quantity = 1;
            for (int i = 0; i < _item.Quantity; i++)
            {
                index = inventory.FindIndex(x => x == null);
                inventory[index] = item;
                OnInventorySlotUpdate?.Invoke(index);
            }
        }
    }
    /// <summary>
    /// 인벤토리를 확장하는 함수
    /// </summary>
    /// <param name="_addLine"></param>
    public void ExpandInventory(int _addLine)
    {
        for (int i = 0; i < _addLine * 7; i++)
        {
            inventory.Add(null);
        }
    }
    public void RemoveItem(int _index)
    {
        inventory[_index] = null;
        OnInventorySlotUpdate?.Invoke(_index);
    }
    public void SwichItem(int _from, int _to)
    {
        var temp = inventory[_from];
        inventory[_from] = inventory[_to];
        inventory[_to] = temp;

        OnInventorySlotUpdate?.Invoke(_from);
        OnInventorySlotUpdate?.Invoke(_to);

        //TO DO
        //아이템을 등록한 상태에서 인벤토리내에서 아이템을 이동 할 경우 HUD에서 추적 할 수 있도록 변경 
    }
    public void UseItem(int _index,SaveItemData _item)
    {
        if (_item == null)
            return;

        switch (_item.ItemData.ItemType)
        {
            case ItemType.Potion:
                foreach (var stat in _item.ItemData.ItemStats.Stats)
                {
                    if (stat.Key == StatType.HPRegen)
                    {
                        PlayerController.Instance.characterStat.RecoverHP((int)stat.Value);
                        Debug.Log($"{_item.ItemData.Name}을 사용하여 HP {stat.Value} 회복!");
                    }
                    else if (stat.Key == StatType.MPRegen)
                    {
                        PlayerController.Instance.characterStat.RecoverMP((int)stat.Value);
                        Debug.Log($"{_item.ItemData.Name}을 사용하여 MP {stat.Value} 회복!");
                    }
                }
                _item.Quantity--;
                OnInventorySlotUpdate?.Invoke(_index);
                if(_item.Quantity == 0)
                {
                    RemoveItem(_index);
                }
                OnHUDItemSlotUpdated?.Invoke(_index, _item);
                break;
        }
    }

    public List<SaveItemData> GetInventoryItem()
    {
        return inventory;
    }
    public SaveItemData GetInventoryItemAtSlot(int _index)
    {
        return (_index >= 0 && _index < inventory.Count) ? inventory[_index] : null;
    }
    public void AssignHUDSlot(int _index, SaveItemData _item)
    {
        if (_index < 0 || _index >= inventory.Count) return;

        inventory[_index] = _item;
        OnInventorySlotUpdate?.Invoke(_index);
    }
}
