using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIQuestAccept : UIPanel
{
    public static UIQuestAccept Instance;

    [SerializeField] AccQuuestListSlot accQuestListPrefab;
    [SerializeField] Transform accQuestObjRoot;
    [SerializeField] TextMeshProUGUI questNameTxt;
    [SerializeField] TextMeshProUGUI questConditionTxt;
    [SerializeField] List<TextMeshProUGUI> questDetailConditionTxtList;
    [SerializeField] TextMeshProUGUI descript;
    [SerializeField] List<RewardSlot> rewardItems;
    [SerializeField] List<GameObject> actionBtns;


    List<AccQuuestListSlot> registeredQuests = new List<AccQuuestListSlot>();

    QuestData selectedQuestData;
    SaveQuestData selectedSaveData;

    protected override void Awake()
    {
        base.Awake();
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else
            Destroy(gameObject);

    }
    public void ShowQuestAcceptUI(QuestData _questData)
    {
        selectedQuestData = _questData;
        questNameTxt.text = _questData.Name;
        questConditionTxt.text = "";
        //만약 _questData가 이미 수락한 퀘스트라면?
        SaveQuestData acceptSaveData = QuestManager.Instance.GetActiveQuest(_questData.Cartegory, _questData.ID);

        if (acceptSaveData != null)
        {
            UIHelper.UpdateQuestConditions(questDetailConditionTxtList, _questData.Conditions, acceptSaveData.Conditions, _questData, true, descript);
        }
        else
        {
            UIHelper.UpdateQuestConditions(questDetailConditionTxtList, _questData.Conditions, null, _questData, true, descript);
        }
        selectedSaveData = acceptSaveData;


        int slotIndex = 0;
        if (_questData.Reward.EXPReward > 0 && slotIndex < rewardItems.Count)
        {
            rewardItems[slotIndex].gameObject.SetActive(true);
            rewardItems[slotIndex].SetRewardExp(_questData.Reward.EXPReward);
            slotIndex++;
        }
        if (_questData.Reward.GoldReward > 0 && slotIndex < rewardItems.Count)
        {
            rewardItems[slotIndex].gameObject.SetActive(true);
            rewardItems[slotIndex].SetRewardGold(_questData.Reward.GoldReward);
            slotIndex++;
        }
        for (int i = 0; i < _questData.Reward.ItemRewards.Count; i++)
        {
            if (slotIndex < rewardItems.Count)
            {
                rewardItems[slotIndex].gameObject.SetActive(true);
                rewardItems[slotIndex].SetRewardItem(_questData.Reward.ItemRewards[i]);
                slotIndex++;
            }
            else
            {
                Debug.LogWarning("보상 슬롯이 부족합니다!");
                break;
            }
        }

        for (int i = slotIndex; i < rewardItems.Count; i++)
        {
            rewardItems[i].gameObject.SetActive(false);
        }


        Open();
    }
    public void OnClickAvailQuest(NPCController _controller)
    {
        foreach (Transform child in accQuestObjRoot)
        {
            child.gameObject.SetActive(false);
        }
       
        QuestNPC questNPC = _controller.npcFunction.GetFunction(NPCFunction.Quest) as QuestNPC;
        if (questNPC == null)
            return;

        //퀘스트 목록을 가져옴
        var questList = questNPC.GetAvailQuestList();

        SaveQuestData questData = QuestManager.Instance.GetCompleteQuestByNPCID(_controller.NPC_ID);
        if (questData != null && questList.All(x => x.ID != questData.QuestID))
        {
            QuestData endNpcQuest = questData.QuestTableData;
            questList.Add(endNpcQuest);
        }
        for (int i = 0; i < questList.Count; i++)
        {
            AccQuuestListSlot slot;
            if (i >= accQuestObjRoot.childCount)
            {
                slot = Instantiate(accQuestListPrefab, accQuestObjRoot);
                registeredQuests.Add(slot);
            }
            else
            {
                GameObject go = accQuestObjRoot.GetChild(i).gameObject;
                go.SetActive(true);
                slot = go.GetComponent<AccQuuestListSlot>();
            }

            slot.npcID = _controller.NPC_ID;
            slot.SetAccQuestListSlot(questList[i]);
        }
    }
    /// <summary>
    /// 퀘스트 수락 버튼 이벤트
    /// </summary>
    public void OnClickAcceptQuestButton()
    {
        QuestManager.Instance.AcceptQuest(selectedQuestData);
        UIManager.Instance.AllClosePanel();
    }
    /// <summary>
    /// 퀘스트 완료 버튼 이벤트
    /// </summary>
    public void OnClickQuestClearButton()
    {
        if (selectedSaveData != null)
        {
            if (selectedSaveData.IsCompleted)
            {
                QuestManager.Instance.CheckQuestCompletion(selectedSaveData);
                UIManager.Instance.AllClosePanel();
            }
        }
    }
    /// <summary>
    /// 퀘스트 포기 버튼 이벤트
    /// </summary>
    public void OnClickAbandonQuestButton()
    {
        if (selectedSaveData != null)
        {
            if (!selectedSaveData.IsCompleted)
            {
                QuestManager.Instance.AbandonQuest(selectedSaveData);
                UIManager.Instance.AllClosePanel();
            }
        }
    }
    public void HideAcceptQuestSlot()
    {
        registeredQuests.ForEach(x => x.gameObject.SetActive(false));
    }
    public override void Open()
    {
        base.Open();
        actionBtns[0].SetActive(selectedSaveData == null || selectedSaveData.Status == QuestStatus.NotStarted);
        actionBtns[1].SetActive(selectedSaveData != null && !selectedSaveData.IsCompleted);
        actionBtns[2].SetActive(selectedSaveData != null && selectedSaveData.IsCompleted);
    }
    public override void Close()
    {
        base.Close();
    }
}
