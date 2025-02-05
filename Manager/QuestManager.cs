using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using System.Linq;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    Dictionary<QuestCategory, List<SaveQuestData>> activeQuests = new Dictionary<QuestCategory, List<SaveQuestData>>();

    Dictionary<(QuestTargetType, int), List<QuestConditionProgress>> questConditionMap = new Dictionary<(QuestTargetType, int), List<QuestConditionProgress>>();

    public event Action<List<SaveQuestData>> OnQuestListUpdated;
    public event Action OnQuestCountChanged;
    public event Action<SaveQuestData> OnQuestCompleted;
    public event Action<SaveQuestData> OnQuestAccepted;


    //public List<int> finishedQuestData = new List<int>();
    //HashSet ��� ����, int
    public HashSet<int> finishedQuestData = new HashSet<int>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            BuildQuestConditionMap();
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
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
            UpdateQuestConditionMap(saveData);
            activeQuests[_quest.Cartegory] = quests;
            //�����ϸ� UI�� ������Ʈ ���������.
            OnQuestAccepted?.Invoke(saveData);
            Debug.Log($"����Ʈ ���� {_quest.Name}");
        }
    }
    public void OnTargetAchieved(QuestTargetType _targetType, int _id)
    {
        var key = (_targetType, _id);

        if (questConditionMap.TryGetValue(key, out List<QuestConditionProgress> conditions))
        {
            QuestData questData = TableLoader.Instance.GetTable<QuestTable>().GetQuestDataByID(_id);
            foreach (var condition in conditions)
            {
                if (!condition.IsCompleted)
                {
                    condition.CurrentCount++;
                    OnQuestCountChanged?.Invoke();
                    Debug.Log($"����Ʈ ��ǥ �޼�: {questData.Name}, Count: {condition.CurrentCount}/{questData.Conditions[condition.ConditionIndex].RequiredCount}");
                }
            }
        }
    }
    private void BuildQuestConditionMap()
    {
        questConditionMap.Clear();
        foreach (var category in activeQuests)
        {
            foreach (var quest in category.Value)
            {
                QuestData data = quest.GetQuestData();
                UpdateQuestConditionMap(quest);
            }
        }
    }
    private void UpdateQuestConditionMap(SaveQuestData _quest)
    {
        QuestData data = _quest.GetQuestData();
        foreach(var condition in _quest.Conditions)
        {
            QuestCondition dataCondition = data.Conditions[condition.ConditionIndex];
            var key = (dataCondition.TargetType,dataCondition.TargetID);
            if(!questConditionMap.ContainsKey(key))
            {
                questConditionMap[key] = new List<QuestConditionProgress>();
            }
            questConditionMap[key].Add(condition);
        }
    }
    /// <summary>
    /// ����Ʈ �ϷḦ üũ�ϴ� �Լ�
    /// </summary>
    public void CheckQuestCompletion()
    {
        foreach (var category in activeQuests)
        {
            foreach (var quest in category.Value)
            {
                if (quest.IsCompleted)
                {
                    if (category.Value.Count == 0)
                        activeQuests.Remove(category.Key);

                    quest.Status = QuestStatus.Completed;
                    finishedQuestData.Add(quest.QuestID);
                    OnQuestCompleted?.Invoke(quest);
                }
            }
        }
    }
    public List<SaveQuestData> GetActiveQuests(QuestCategory _category)
    {
        if(activeQuests.TryGetValue(_category, out var questList))
        {
            return questList;
        }
        Debug.LogWarning("��ϵ� ����Ʈ�� �����ϴ�.");
        return null;
    }
}
