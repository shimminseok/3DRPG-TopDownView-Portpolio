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

    public QuestListSlot SelectedQuestSlot { get; private set; }



    [Header("QuestInfo")]
    [SerializeField] GameObject questInfoBG;
    [SerializeField] TextMeshProUGUI questInfo_QuestName;
    [SerializeField] TextMeshProUGUI questInfo_QuestConditions;
    [SerializeField] List<TextMeshProUGUI> questInfo_QuestDetailConditions = new List<TextMeshProUGUI>();
    [SerializeField] TextMeshProUGUI questInfo_QuestDescript;
    [SerializeField] List<RewardSlot> questInfo_Rewards = new List<RewardSlot>();


    Dictionary<QuestStatus, List<SaveQuestData>> questStatusDic = new Dictionary<QuestStatus, List<SaveQuestData>>();
    List<QuestListSlot> progressQuestSlots = new List<QuestListSlot>();
    SaveQuestData[] questData = new SaveQuestData[10];
    List<SaveQuestData> progressQuestData = new List<SaveQuestData>();
    List<SaveQuestData> completedQuestData = new List<SaveQuestData>();

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
    void Start()
    {
        QuestManager.Instance.OnQuestAccepted += AcceptQuest;
        QuestManager.Instance.OnQuestAbandoned += AbandonQuest;
        QuestManager.Instance.OnQuestCompleted += CompletedQuest;

    }
    /// <summary>
    /// 퀘스트 수락 함수
    /// </summary>
    /// <param name="_data">퀘스트 데이터</param>
    public void AcceptQuest(SaveQuestData _data)
    {
        if(_data.Status == QuestStatus.InProgress && !progressQuestData.Exists(x => x.QuestID == _data.QuestID))
        {
            progressQuestData.Add(_data);
            QuestListSlot newSlot = Instantiate(slotPrefabs, slotRoot);
            newSlot.SetQuestInfo(_data);
            progressQuestSlots.Add(newSlot);
        }
    }
    /// <summary>
    /// 퀘스트 포기 함수
    /// </summary>
    /// <param name="_data"></param>
    public void AbandonQuest(SaveQuestData _data)
    {
        SaveQuestData targetQuest = progressQuestData.Find(x => x.QuestID == _data.QuestID);
        if(targetQuest != null)
        {
            progressQuestData.Remove(targetQuest);
            Destroy(progressQuestSlots.Find(x => x.Data.QuestID == targetQuest.QuestID).gameObject);
            progressQuestSlots.RemoveAll(x => x.Data.QuestID == _data.QuestID);
            ShowQuestDetailInfo(null);
            SelectedQuestSlot = null;
        }
    }
    public void CompletedQuest(SaveQuestData _data)
    {
        SaveQuestData targetQuest = progressQuestData.Find(x => x.QuestID == _data.QuestID);
        if(targetQuest != null)
        {
            progressQuestData.Remove(targetQuest);
            Destroy(progressQuestSlots.Find(x => x.Data.QuestID == targetQuest.QuestID).gameObject);
            completedQuestData.Add(targetQuest);
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

            int slotIndex = 0;
            if (data.Reward.EXPReward > 0 && slotIndex < questInfo_Rewards.Count)
            {
                questInfo_Rewards[slotIndex].gameObject.SetActive(true);
                questInfo_Rewards[slotIndex].SetRewardExp(data.Reward.EXPReward);
                slotIndex++;
            }
            if (data.Reward.GoldReward > 0 && slotIndex < questInfo_Rewards.Count)
            {
                questInfo_Rewards[slotIndex].gameObject.SetActive(true);
                questInfo_Rewards[slotIndex].SetRewardGold(data.Reward.GoldReward);
                slotIndex++;
            }
            for (int i = 0; i < data.Reward.ItemRewards.Count; i++)
            {
                if (slotIndex < questInfo_Rewards.Count)
                {
                    questInfo_Rewards[slotIndex].gameObject.SetActive(true);
                    questInfo_Rewards[slotIndex].SetRewardItem(data.Reward.ItemRewards[i]);
                    slotIndex++;
                }
                else
                {
                    Debug.LogWarning("보상 슬롯이 부족합니다!");
                    break;
                }
            }
            for (int i = slotIndex; i < questInfo_Rewards.Count; i++)
            {
                questInfo_Rewards[i].gameObject.SetActive(false);
            }
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

    public void OnClickCompletedQuestTab()
    {
        Debug.Log($"{completedQuestData.Count}");
    }

    public void OnClickAbandonQuestBtn()
    {
        QuestManager.Instance.AbandonQuest(SelectedQuestSlot.Data);

    }
}
