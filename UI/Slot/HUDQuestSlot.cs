using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDQuestSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI questName;
    [SerializeField] TextMeshProUGUI questCurrentCondition;

    SaveQuestData saveQuestData;

    public SaveQuestData QuestData => saveQuestData;
    void Start()
    {

    }

    public void ActiveHUDQuestSlot(SaveQuestData _quest)
    {
        questName.text = _quest.QuestTableData.Name;
        bool isPrevQuestCompleted = true;
        foreach (var condition in _quest.Conditions)
        {
            isPrevQuestCompleted = condition.IsConditionCompleted(_quest.QuestTableData);
            if (!isPrevQuestCompleted)
            {
                UIHelper.UpdateQuestCondition(questCurrentCondition, condition, _quest.QuestTableData);
                break;
            }
        }
        saveQuestData = _quest;
    }
    public void UpdateHUDQuestSlot(SaveQuestData _quest)
    {
        bool isPrevQuestCompleted = true;
        foreach (var condition in _quest.Conditions)
        {
            isPrevQuestCompleted = condition.IsConditionCompleted(_quest.QuestTableData);
            if (!isPrevQuestCompleted)
            {
                UIHelper.UpdateQuestCondition(questCurrentCondition, condition, _quest.QuestTableData);
                break;
            }
        }
    }
}
