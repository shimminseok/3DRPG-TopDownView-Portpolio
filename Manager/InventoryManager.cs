using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public int MaxLine = 7;

    public Dictionary<KeyCode, SaveItemData> ResisterdItems = new Dictionary<KeyCode, SaveItemData>();
    List<SaveItemData> inventory = new List<SaveItemData>();
    public event Action<int> OnInventorySlotUpdate;

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

        var gameData = GameManager.Instance.LoadGameData();
        inventory = (gameData.Inventory != null && gameData.Inventory.Count > 0)
                    ? gameData.Inventory
                    : Enumerable.Repeat<SaveItemData>(null, MaxLine * 7).ToList();
    }
    void Start()
    {
        QuestManager.Instance.OnQuestReward += AddRewardItem;
    }
    public void AddItem(SaveItemData _item)
    {
        if (_item.GetItemData() == null)
            return;
        // ������ ������
        if (_item.GetItemData().IsStackable)
        {
            SaveItemData findItem = inventory.Find(x => x != null && x.ItemID == _item.ItemID);
            if (findItem == null)
            {
                // To Do �κ��丮�� ��á���� Ȯ��
                int index = inventory.IndexOf(null);
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
            int enptySlotCnt = inventory.FindAll(x => x == null).Count;
            if (enptySlotCnt < _item.Quantity)
            {
                Debug.Log("�κ��丮 ������ �����Ͽ� ���� �� �� �����ϴ�.");
                return;
            }
            SaveItemData item = _item.DeepCopy();
            item.Quantity = 1;
            for (int i = 0; i < _item.Quantity; i++)
            {
                int index = inventory.FindIndex(x => x == null);
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
    public void UseItem(int _index, SaveItemData _item, int _useItemQty = 1)
    {
        if (_item == null)
            return;

        if (_index == -1)
            _index = inventory.FindIndex(x => x != null && x.ItemID == _item.ItemID);

        switch (_item.GetItemData().ItemType)
        {
            case ItemType.Potion:
                {
                    foreach (var stat in _item.GetItemData().ItemStats.Stats)
                    {
                        if (stat.Key == StatType.HPRegen)
                        {
                            PlayerController.Instance.characterStat.RecoverHP((int)stat.Value);
                            //ȸ�� ����Ʈ �߰�
                            Debug.Log($"{_item.GetItemData().Name}�� ����Ͽ� HP {stat.Value} ȸ��!");
                        }
                        else if (stat.Key == StatType.HPRegen)
                        {
                            PlayerController.Instance.characterStat.RecoverMP((int)stat.Value);
                            Debug.Log($"{_item.GetItemData().Name}�� ����Ͽ� MP {stat.Value} ȸ��!");
                        }
                    }

                    break;
                }
            case ItemType.Material:
                {                    
                    break;
                }
        }
        _item.Quantity -= _useItemQty;
        if (_item.Quantity == 0)
        {
            RemoveItem(_index);
            return;
        }
        OnInventorySlotUpdate?.Invoke(_index);

    }
    void AddRewardItem(RewardData _reward)
    {
        foreach (var reward in _reward.ItemRewards)
        {
            AddItem(reward);
        }
    }
    public List<SaveItemData> GetInventoryItem()
    {
        return inventory;
    }
    public SaveItemData GetInventoryItemByItemID(int _id)
    {

        return inventory.Find(x => x != null && x.ItemID == _id);
    }
    public SaveItemData GetInventoryItemAtSlot(int _index)
    {
        return (_index >= 0 && _index < inventory.Count) ? inventory[_index] : null;
    }
}
