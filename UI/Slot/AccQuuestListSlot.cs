using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class AccQuuestListSlot : MonoBehaviour
{
    
    [SerializeField] TextMeshProUGUI questName;
    [SerializeField] Image selectedImg;

    QuestData data;
    public void SetAccQuestListSlot(QuestData _data)
    {
        data = _data;
        questName.text = $"<color={UIHelper.GetColorByQuestType(data.Type)}>[{UIHelper.GetTypeTextByQuestType(data.Type)}] {data.Name} </color>";
    }

    public void OnClickSlot()
    {
        selectedImg.enabled = true;
        UIDescription.Instance.OnClickAcceptQuest(data);
    }
}
