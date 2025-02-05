using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIQuestAccept : UIPanel
{
    [SerializeField] AccQuuestListSlot accQuestListPrefab;
    [SerializeField] Transform accQuestObjRoot;
    [SerializeField] TextMeshProUGUI questNameTxt;
    [SerializeField] TextMeshProUGUI questConditionTxt;
    [SerializeField] List<TextMeshProUGUI> questDetailConditionTxtList;
    [SerializeField] TextMeshProUGUI descript;
    [SerializeField] List<InventorySlot> rewardItems;

    QuestData selectedQuestData;
    public void ShowQuestAcceptUI(QuestData _questData)
    {
        selectedQuestData = _questData;
        questNameTxt.text = _questData.Name;
        questConditionTxt.text = "";
        UIHelper.UpdateQuestConditions(questDetailConditionTxtList, _questData.Conditions, null, _questData, true, descript);

        for (int i = 0; i < rewardItems.Count; i++)
        {
            rewardItems[i].gameObject.SetActive(i < _questData.Rewards.Count);
            if (i < _questData.Rewards.Count)
            {
                for (int j = 0; j < _questData.Rewards[i].ItemRewards.Count; j++)
                {
                    rewardItems[i].SetItemInfo(_questData.Rewards[i].ItemRewards[j]);
                }
            }
            else
            {

            }
        }
        OnClickOpenButton();
    }
    public void OnClickAvailQuest(NPCController _controller)
    {
        foreach (Transform child in accQuestObjRoot)
        {
            child.gameObject.SetActive(false);
        }
        QuestNPC questNPC = _controller.GetComponent<QuestNPC>();
        if (questNPC == null)
            return;

        //Äù½ºÆ® ¸ñ·ÏÀ» °¡Á®¿È
        var questList = questNPC.GetAvailQuestList();

        for (int i = 0; i < questList.Count; i++)
        {
            AccQuuestListSlot slot;
            if (i >= accQuestObjRoot.childCount)
            {
                slot = Instantiate(accQuestListPrefab, accQuestObjRoot);
            }
            else
            {
                GameObject go = accQuestObjRoot.GetChild(i).gameObject;
                go.SetActive(true);
                slot = go.GetComponent<AccQuuestListSlot>();
            }

            slot.SetAccQuestListSlot(questList[i]);
        }
    }
    public void OnClickAcceptQuestButton()
    {
        QuestManager.Instance.AcceptQuest(selectedQuestData);
        UIManager.Instance.AllClosePanel();
    }
    public override void OnClickOpenButton()
    {
        base.OnClickOpenButton();
    }
    public override void OnClickCloseButton()
    {
        base.OnClickCloseButton();
    }
}
