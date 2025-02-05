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
        // ������ ������
        if (_item.ItemData.IsStackable)
        {
            SaveItemData findItem = inventory.Find(x => x != null && x.ItemID == _item.ItemID);
            if (findItem == null)
            {
                // To Do �κ��丮�� ��á���� Ȯ��
                int index = inventory.FindIndex(x => x == null);
                if (index < 0)
                {
                    Debug.Log("�κ��丮�� ���� á���ϴ�.");
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
        else //�߰��ϴ� �������� ���������ʴ� ������
        {
            int index = inventory.FindAll(x => x == null).Count;
            if (index < _item.Quantity)
            {
                Debug.Log("�κ��丮 ������ �����Ͽ� ���� �� �� �����ϴ�.");
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
    /// �κ��丮�� Ȯ���ϴ� �Լ�
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
        //�������� ����� ���¿��� �κ��丮������ �������� �̵� �� ��� HUD���� ���� �� �� �ֵ��� ���� 
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
                        Debug.Log($"{_item.ItemData.Name}�� ����Ͽ� HP {stat.Value} ȸ��!");
                    }
                    else if (stat.Key == StatType.MPRegen)
                    {
                        PlayerController.Instance.characterStat.RecoverMP((int)stat.Value);
                        Debug.Log($"{_item.ItemData.Name}�� ����Ͽ� MP {stat.Value} ȸ��!");
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
