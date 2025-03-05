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
        // 스택형 아이템
        if (_item.GetItemData().IsStackable)
        {
            SaveItemData findItem = inventory.Find(x => x != null && x.ItemID == _item.ItemID);
            if (findItem == null)
            {
                // To Do 인벤토리가 꽉찼는지 확인
                int index = inventory.IndexOf(null);
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
            int enptySlotCnt = inventory.FindAll(x => x == null).Count;
            if (enptySlotCnt < _item.Quantity)
            {
                Debug.Log("인벤토리 공간이 부족하여 구매 할 수 없습니다.");
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
                            //회복 이펙트 추가
                            Debug.Log($"{_item.GetItemData().Name}을 사용하여 HP {stat.Value} 회복!");
                        }
                        else if (stat.Key == StatType.HPRegen)
                        {
                            PlayerController.Instance.characterStat.RecoverMP((int)stat.Value);
                            Debug.Log($"{_item.GetItemData().Name}을 사용하여 MP {stat.Value} 회복!");
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
