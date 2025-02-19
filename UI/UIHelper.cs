using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public static class UIHelper
{
    public static void UpdateQuestCondition(TextMeshProUGUI conditionText, QuestConditionProgress progress, QuestData questData)
    {
        MonsterTable monsterTable = TableLoader.Instance.GetTable<MonsterTable>();
        ItemTable itemTable = TableLoader.Instance.GetTable<ItemTable>();
        NPCTable npcTable = TableLoader.Instance.GetTable<NPCTable>();

        QuestCondition condition = questData.Conditions[progress.ConditionIndex];
        string targetName = condition.TargetType switch
        {
            QuestTargetType.Monster => monsterTable.GetMonsterDataByID(condition.TargetID).MonsterName,
            QuestTargetType.Item => itemTable.GetItemDataByID(condition.TargetID).Name,
            QuestTargetType.NPC => npcTable.GetNPCDataByID(condition.TargetID).Name,
            //QuestTargetType.Location => monsterTable.GetMonsterDataByID(condition.TargetID).MonsterName,
            _ => "Unknown"
        };

        string text = string.Format(condition.QuestConditionTxt, targetName, condition.RequiredCount);
        conditionText.text = progress != null
            ? string.Format("{0} ({1}/{2})", text, progress.CurrentCount, condition.RequiredCount)
            : text;

    }
    public static void UpdateQuestConditions
        (
        List<TextMeshProUGUI> conditionTexts,
        List<QuestCondition> conditions,
        List<QuestConditionProgress> progress,
        QuestData questData,
        bool _showNextCondition,
        TextMeshProUGUI descriptionText = null
        )
    {
        MonsterTable monsterTable = TableLoader.Instance.GetTable<MonsterTable>();
        ItemTable itemTable = TableLoader.Instance.GetTable<ItemTable>();
        NPCTable npcTable = TableLoader.Instance.GetTable<NPCTable>();

        bool prevConditionCompleted = true;

        for (int i = 0; i < conditionTexts.Count; i++)
        {
            if (i < conditions.Count)
            {
                QuestCondition condition = conditions[i];
                string targetName = condition.TargetType switch
                {
                    QuestTargetType.Monster => monsterTable.GetMonsterDataByID(condition.TargetID).MonsterName,
                    QuestTargetType.Item => itemTable.GetItemDataByID(condition.TargetID).Name,
                    QuestTargetType.NPC => npcTable.GetNPCDataByID(condition.TargetID).Name,
                    //QuestTargetType.Location => monsterTable.GetMonsterDataByID(condition.TargetID).MonsterName,
                    _ => "Unknown"
                };

                if (i == 0 || prevConditionCompleted || _showNextCondition)
                {
                    string conditionText = string.Format(condition.QuestConditionTxt, targetName, condition.RequiredCount);
                    conditionTexts[i].text = progress != null
                        ? string.Format("{0} ({1}/{2})", conditionText, progress[i].CurrentCount, condition.RequiredCount)
                        : conditionText;

                    if (progress != null && progress[i].IsConditionCompleted(questData))
                    {
                        conditionTexts[i].fontStyle = FontStyles.Strikethrough;
                        conditionTexts[i].color = Color.green;
                        prevConditionCompleted = true;
                    }
                    else
                    {
                        conditionTexts[i].fontStyle = FontStyles.Normal;
                        conditionTexts[i].color = Color.white;
                        prevConditionCompleted = false;
                    }
                }
                else
                {
                    conditionTexts[i].text = "";
                }
            }
            else
            {
                conditionTexts[i].text = "";
            }
        }

        if (descriptionText != null)
        {
            descriptionText.text = questData.Description;
        }
    }

    public static string GetColorByQuestType(QuestType _type)
    {
        string hexColor = string.Empty;

        switch (_type)
        {
            case QuestType.Main:
                hexColor = "#BA7F54";
                break;
            case QuestType.Side:
                hexColor = "#576DFF";
                break;
            case QuestType.Repeat:
                hexColor = "#576D79";
                break;
        }
        return hexColor;
    }
    public static string GetTypeTextByQuestType(QuestType _type)
    {
        string questType = string.Empty;
        switch (_type)
        {
            case QuestType.Main:
                questType = "메인";
                break;
            case QuestType.Side:
                questType = "서브";
                break;
            case QuestType.Repeat:
                questType = "반복";
                break;
        }

        return questType;
    }


}
