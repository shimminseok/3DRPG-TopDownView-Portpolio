using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class QuestListSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI questTypeTxt;
    [SerializeField] TextMeshProUGUI questName;
    [SerializeField] Image selectedImg;

    SaveQuestData data;

    public SaveQuestData Data => data;
    public void SetQuestInfo(SaveQuestData _data)
    {
        data = _data;
        QuestData questData = data.GetQuestData();
        if (data != null)
        {
            string hexColor = UIHelper.GetColorByQuestType(questData.Type);
            questTypeTxt.text = $"<color={hexColor}>{UIHelper.GetTypeTextByQuestType(questData.Type)}</color>";
            questName.text = $"<color={hexColor}>{questData.Name}</color>";
        }
    }
    public void UpdateQuestInfo()
    {
        if (data == null)
            return;


    }

    public void SelectedSlot()
    {
        UIQuest.Instance.SelectedQuest(this);
        selectedImg.enabled = true;
    }
    public void DeSelectedSlot()
    {
        selectedImg.enabled = false;
    }
}
