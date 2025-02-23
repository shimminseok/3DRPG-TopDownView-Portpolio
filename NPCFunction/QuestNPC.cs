using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public class QuestNPC : MonoBehaviour, INPCFunction
{
    NPCController npcController;

    List<QuestData> cachedQuests;
    List<QuestData> npcQuestList;

    public NPCFunction FuncType => NPCFunction.Quest;

    void Awake()
    {
        npcController = GetComponent<NPCController>();
        if (npcController == null)
        {
            Debug.LogWarning("NPCController X");
            return;
        }

    }
    /// <summary>
    /// 퀘스트를 주는 함수
    /// </summary>
    public void Execute()
    {
        UIDescription.Instance.OpenQuestAcceptUI(npcController);

    }
    public void Initialize(NPCData _data)
    {
        RegisterQuest(_data);
    }
    void RegisterQuest(NPCData _data)
    {
        if (cachedQuests == null)
        {
            QuestTable questTable = TableLoader.Instance.GetTable<QuestTable>();
            cachedQuests = questTable.GetQuestsByStartNPCID(_data.NPCID);
        }
        npcQuestList = cachedQuests.Where(q => !QuestManager.Instance.finishedQuestData.Contains(q.ID)).ToList();
        Debug.LogWarning("퀘스트 등록 완료");
    }

    public List<QuestData> GetAvailQuestList()
    {
        List<QuestData> availQuestList = new List<QuestData>();

        foreach(var quest in npcQuestList)
        {
            if (PlayerController.Instance.characterStat.Level.FinalValue < quest.LevelRequirement)
                continue;

            if (QuestManager.Instance.finishedQuestData.Contains(quest.ID))
                continue;

            bool prerequisitesCompleted = true;
            foreach(var prevQuestID in quest.PrerequisiteQuest)
            {
                if(!QuestManager.Instance.finishedQuestData.Contains(prevQuestID))
                {
                    prerequisitesCompleted = false;
                    break;
                }
            }

            if(prerequisitesCompleted)
            {
                availQuestList.Add(quest);
            }
        }
        return availQuestList;
    }
    void UpdateQuestList(int _questID)
    {
        npcQuestList = cachedQuests.Where(x => !QuestManager.Instance.finishedQuestData.Contains(x.ID)).ToList();
    }
}
