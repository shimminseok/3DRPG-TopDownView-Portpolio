using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

public class UIInventory : UIPanel
{
    public static UIInventory Instance;

    [Header("SlotPrefabs")]
    [SerializeField] InventorySlot SlotPrefab;
    [SerializeField] Transform slotRoot;


    [SerializeField] TextMeshProUGUI gold;
    public List<InventorySlot> InventorySlots = new List<InventorySlot>();
    public Dictionary<int, InventorySlot> itemDic = new Dictionary<int, InventorySlot>();



    public InventorySlot SelectedItem { get; private set; }

    

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
    private void Start()
    {
        InitializeSlots();

        InventoryManager.Instance.OnInventorySlotUpdate += UpdateInventorySlot;
        AccountManager.Instance.OnChangedGold += UpdateGoldUI;
    }
    void InitializeSlots()
    {
        for (int i = 0; i < InventoryManager.Instance.GetInventoryItem().Count; i++)
        {
            InventorySlot newItem = Instantiate(SlotPrefab, slotRoot);
            newItem.SetItemInfo(InventoryManager.Instance.GetInventoryItem()[i]);
            newItem.Index = i;
            InventorySlots.Add(newItem);
        }
    }
    void CreateInvenSlot(int _slotCount)
    {
        InventorySlot newItem = Instantiate(SlotPrefab, slotRoot);
        for (int i = 0; i < _slotCount; i++)
        {
            newItem.SetItemInfo(InventoryManager.Instance.GetInventoryItem()[i]);
            newItem.Index = i;
            InventorySlots.Add(newItem);
        }
    }
    public void SelecteItem(InventorySlot _item)
    {
        if (SelectedItem != null && SelectedItem != _item)
            SelectedItem.DeSelectedSlot();

        SelectedItem = _item;
    }

    public override void OnClickOpenButton()
    {
        base.OnClickOpenButton();
    }
    public override void OnClickCloseButton()
    {
        base.OnClickCloseButton();
        SelectedItem?.DeSelectedSlot();
        SelectedItem = null;
    }

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < InventoryManager.Instance.GetInventoryItem().Count; i++)
        {
            if (InventoryManager.Instance.GetInventoryItem()[i] == null)
            {
                continue;
            }

            InventorySlots[i].SetItemInfo(InventoryManager.Instance.GetInventoryItem()[i]);
        }
    }
    public void UpdateInventorySlot(int _index)
    {
        if (_index < 0 || _index >= InventorySlots.Count)
            return;

        SaveItemData itemData = InventoryManager.Instance.GetInventoryItem()[_index];

        InventorySlots[_index].SetItemInfo(itemData);
    }

    void UpdateGoldUI(int _gold)
    {
        gold.text = _gold.ToString("N0");
    }

}
