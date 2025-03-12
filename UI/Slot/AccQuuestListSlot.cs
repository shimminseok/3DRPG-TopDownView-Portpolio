using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class AccQuuestListSlot : MonoBehaviour, ISelectableSlot
{
    public int npcID;

    [SerializeField] TextMeshProUGUI questName;
    [SerializeField] Image selectedImg;

    bool isSelected;
    QuestData data;

    public void SetAccQuestListSlot(QuestData _data)
    {
        data = _data;
        questName.text = $"<color={UIHelper.GetColorByQuestType(data.Type)}>[{UIHelper.GetTypeTextByQuestType(data.Type)}] {data.Name} </color>";
    }

    public void OnClickSlot()
    {
        if (isSelected)
            DeSelectedSlot();
        else
            SelectedSlot();

        selectedImg.enabled = isSelected;
    }

    public void SelectedSlot()
    {
        selectedImg.enabled = true;
        QuestManager.Instance.OnTargetAchieved(QuestTargetType.NPC, npcID);
        UIDescription.Instance.OnClickAcceptQuest(data);
    }

    public void DeSelectedSlot()
    {
        selectedImg.enabled = false;
    }
}
