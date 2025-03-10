using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    Dictionary<QuestCategory, List<SaveQuestData>> activeQuests = new Dictionary<QuestCategory, List<SaveQuestData>>();


    public event Action<SaveQuestData> OnQuestCountChanged;
    public event Action<SaveQuestData> OnQuestAccepted;
    public event Action<SaveQuestData> OnQuestCompleted;
    public event Action<SaveQuestData> OnQuestAbandoned;
    public event Action<RewardData> OnQuestReward;

    //public List<int> finishedQuestData = new List<int>();
    //HashSet ��� ����, int
    public HashSet<int> finishedQuestData = new HashSet<int>();
    public Dictionary<QuestCategory, List<SaveQuestData>> ActiveQuests => activeQuests;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        activeQuests = GameManager.Instance.LoadGameData().ActiveQuests ?? new Dictionary<QuestCategory, List<SaveQuestData>>();
    }

    /// <summary>
    /// ����Ʈ�� �����ϴ� �Լ�
    /// </summary>
    /// <param name="_quest"></param>
    public void AcceptQuest(QuestData _quest)
    {
        if (!activeQuests.TryGetValue(_quest.Cartegory, out var quests))
        {
            quests = new List<SaveQuestData>();
            activeQuests[_quest.Cartegory] = quests;
        }
        SaveQuestData saveData = new SaveQuestData(_quest);
        saveData.Status = QuestStatus.InProgress;
        if (!quests.Exists(x => x.QuestID == _quest.ID))
        {
            quests.Add(saveData);
            activeQuests[_quest.Cartegory] = quests;
            OnQuestAccepted?.Invoke(saveData);
            
            Debug.Log($"����Ʈ ���� {_quest.Name}");
        }
    }
    /// <summary>
    /// ����Ʈ ����
    /// </summary>
    /// <param name="_quest"></param>
    public void AbandonQuest(SaveQuestData _quest)
    {
        if(activeQuests.TryGetValue(_quest.QuestTableData.Cartegory, out var quests))
        {
            SaveQuestData targetQuest = quests.Find(x => x.QuestID == _quest.QuestID);
            if(targetQuest != null)
            {
                quests.Remove(targetQuest);
                OnQuestAbandoned?.Invoke(targetQuest);
            }
            Debug.Log($"����Ʈ ���� {_quest.QuestTableData.Name}");

        }
    }
    /// <summary>
    /// ����Ʈ�� Count�� ������Ű�� �Լ�
    /// </summary>
    /// <param name="_target"></param>
    /// <param name="_targetID"></param>
    public void OnTargetAchieved(QuestTargetType _target, int _targetID)
    {
        foreach (var quests in activeQuests.Values)
        {
            foreach (var quest in quests)
            {
                QuestData tableData = quest.GetQuestData();
                bool prevConditionCompleted = true;

                foreach (var condition in quest.Conditions)
                {
                    if (!prevConditionCompleted) break;

                    if (tableData.Conditions[condition.ConditionIndex].TargetType == _target &&
                        tableData.Conditions[condition.ConditionIndex].TargetID == _targetID)
                    {
                        if (!condition.IsConditionCompleted(tableData))
                        {
                            condition.CurrentCount++;
                            OnQuestCountChanged?.Invoke(quest);
                        }
                    }
                    prevConditionCompleted = condition.IsConditionCompleted(tableData);
                }
            }
        }
    }
    /// <summary>
    /// ����Ʈ �ϷḦ üũ�ϴ� �Լ�
    /// </summary>
    public void CheckQuestCompletion(SaveQuestData _quest)
    {
        if (_quest.IsCompleted)
        {
            CompletedQuest(_quest);
        }
    }
    /// <summary>
    /// ����Ʈ �Ϸ� �Լ�
    /// </summary>
    /// <param name="_data"></param>
    void CompletedQuest(SaveQuestData _data)
    {
        QuestData tableData = _data.QuestTableData;
        _data.Status = QuestStatus.Completed;
        activeQuests[tableData.Cartegory].Remove(_data);
        finishedQuestData.Add(_data.QuestID);
        OnQuestCompleted?.Invoke(_data);
        OnQuestReward?.Invoke(_data.GetQuestData().Reward);
    }
    public List<SaveQuestData> GetActiveQuests(QuestCategory _category)
    {
        if (activeQuests.TryGetValue(_category, out var questList))
        {
            return questList;
        }
        Debug.LogWarning("��ϵ� ����Ʈ�� �����ϴ�.");
        return null;
    }
    public SaveQuestData GetActiveQuest(QuestCategory _category, int _id)
    {
        if (activeQuests.TryGetValue(_category, out var questList))
        {
            return questList.Find(x => x.QuestID == _id);
        }
        Debug.LogWarning("��ϵ� ����Ʈ�� �����ϴ�.");
        return null;
    }
    public bool CanCompleteQuestForNPC(int _npcID)
    {
        foreach(var questList in activeQuests.Values)
        {
            foreach(var quest in questList)
            {
                if(quest.QuestTableData.EndNPC == _npcID && quest.IsCompleted)
                {
                    return true;
                }
            }
        }

        return false;
    }
    public SaveQuestData GetCompleteQuestByNPCID(int _npcID)
    {
        foreach (List<SaveQuestData> quests in activeQuests.Values)
        {
            foreach(SaveQuestData quest in quests)
            {
                if(quest.QuestTableData.EndNPC == _npcID)
                {
                    return quest;
                }
            }
        }
        return null;
    }
}
