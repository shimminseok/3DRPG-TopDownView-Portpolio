using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
public class UIQuest : UIPanel
{
    public static UIQuest Instance;

    [SerializeField] ScrollView scrollView;
    [SerializeField] Transform slotRoot;
    [SerializeField] QuestListSlot slotPrefabs;

    public List<QuestListSlot> questSlots = new List<QuestListSlot>();
    public QuestListSlot SelectedQuestSlot { get; private set; }



    [Header("QuestInfo")]
    [SerializeField] GameObject questInfoBG;
    [SerializeField] TextMeshProUGUI questInfo_QuestName;
    [SerializeField] TextMeshProUGUI questInfo_QuestConditions;
    [SerializeField] List<TextMeshProUGUI> questInfo_QuestDetailConditions = new List<TextMeshProUGUI>();
    [SerializeField] TextMeshProUGUI questInfo_QuestDescript;
    [SerializeField] List<InventorySlot> questInfo_Rewards = new List<InventorySlot>();

    int childrenCount = 0;

    Dictionary<QuestStatus, List<SaveQuestData>> questStatusDic = new Dictionary<QuestStatus, List<SaveQuestData>>();
    List<QuestListSlot> progressQuestSlots = new List<QuestListSlot>();
    List<SaveQuestData> progressQuestData = new List<SaveQuestData>();
    List<SaveQuestData> completedQuestData = new List<SaveQuestData>();

    protected override void Awake()
    {
        base.Awake();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root);
        }
        else
            Destroy(gameObject);
    }
    void Start()
    {
        QuestManager.Instance.OnQuestAccepted += AcceptQuest;
        QuestManager.Instance.OnQuestAbandoned += AbandonQuest;
    }
    public void AcceptQuest(SaveQuestData _data)
    {
        if(_data.Status == QuestStatus.InProgress && !progressQuestData.Exists(x => x.QuestID == _data.QuestID))
        {
            progressQuestData.Add(_data);
            QuestListSlot newSlot = Instantiate(slotPrefabs, slotRoot);
            newSlot.SetQuestInfo(_data);
        }
        else if(_data.Status == QuestStatus.Completed)
        {
            completedQuestData.Add(_data);
        }
    }
    public void AbandonQuest(SaveQuestData _data)
    {
        SaveQuestData targetQuest = progressQuestData.Find(x => x.QuestID == _data.QuestID);
        if(targetQuest != null)
        {
            progressQuestData.Remove(targetQuest);
            progressQuestSlots.RemoveAll(x => x.Data.QuestID == _data.QuestID);
        }
    }
    public void SelectedQuest(QuestListSlot _selected)
    {
        if (SelectedQuestSlot != null && SelectedQuestSlot != _selected)
            SelectedQuestSlot.DeSelectedSlot();
        SelectedQuestSlot = _selected;

        ShowQuestDetailInfo(SelectedQuestSlot);
    }
    public void ShowQuestDetailInfo(QuestListSlot _showSlot = null)
    {
        questInfoBG.SetActive(_showSlot != null);
        if (_showSlot != null)
        {
            QuestData data = SelectedQuestSlot.Data.GetQuestData();
            questInfo_QuestName.text = data.Name;
            UIHelper.UpdateQuestConditions(questInfo_QuestDetailConditions, data.Conditions, _showSlot.Data.Conditions, data,false, questInfo_QuestDescript);
        }
    }
    public override void OnClickOpenButton()
    {
        base.OnClickOpenButton();
        ShowQuestDetailInfo(SelectedQuestSlot);
    }
    public override void OnClickCloseButton()
    {
        base.OnClickCloseButton();

    }
}
