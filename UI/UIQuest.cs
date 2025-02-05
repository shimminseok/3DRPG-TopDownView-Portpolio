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
    List<SaveQuestData> progressQuestData = new List<SaveQuestData>();
    List<SaveQuestData> completedQuestData = new List<SaveQuestData>();


    private void Awake()
    {
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
    }
    public void CreateQuestSlot()
    {


        for (int i = 0; i < Enum.GetValues(typeof(QuestCategory)).Length; i++)
        {
            var activeQuest = QuestManager.Instance.GetActiveQuests((QuestCategory)i);
            if (activeQuest != null)
            {
                foreach (var questData in activeQuest)
                {
                    if (questData.Status == QuestStatus.InProgress)
                    {
                        if (!progressQuestData.Contains(questData))
                        {
                            progressQuestData.Add(questData);
                            QuestListSlot newSlot = Instantiate(slotPrefabs, slotRoot);
                            newSlot.SetQuestInfo(questData);
                        }
                    }
                    else if (questData.Status == QuestStatus.Completed)
                    {
                        completedQuestData.Add(questData);
                    }
                }
            }
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
    public void UpdateQuestUI()
    {

    }
    public override void OnClickOpenButton()
    {
        base.OnClickOpenButton();
        CreateQuestSlot();
        ShowQuestDetailInfo(SelectedQuestSlot);
        QuestManager.Instance.OnQuestCountChanged += UpdateQuestUI;


    }
    public override void OnClickCloseButton()
    {
        base.OnClickCloseButton();
        QuestManager.Instance.OnQuestCountChanged -= UpdateQuestUI;

    }
}
