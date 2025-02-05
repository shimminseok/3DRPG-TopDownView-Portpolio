using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public class QuestNPC : MonoBehaviour, INPCFunction
{
    NPCController npcController;
    NPCData npcData;

    List<QuestData> cachedQuests;
    List<QuestData> npcQuestList;


    List<QuestData> availQuests;



    void Awake()
    {
        npcController = GetComponent<NPCController>();
        if (npcController == null)
        {
            Debug.LogWarning("NPCController X");
            return;
        }
        npcData = npcController.NPCData;
    }
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// 퀘스트를 주는 함수
    /// </summary>
    public void Execute()
    {
        Debug.Log("Give Quest");
        for (int i = 0; i < npcQuestList.Count; i++)
        {
            SaveQuestData data = new SaveQuestData(npcQuestList[i]);
            QuestManager.Instance.AcceptQuest(npcQuestList[i]);
        }

    }
    public void Initialize(NPCData _data)
    {
        npcData = _data;
        RegisterQuest();
    }
    void RegisterQuest()
    {
        if (cachedQuests == null)
        {
            QuestTable questTable = TableLoader.Instance.GetTable<QuestTable>();
            cachedQuests = questTable.GetQuestsByStartNPCID(npcData.NPCID);
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
                availQuestList.Add(quest);
        }


        return availQuestList;
    }
}
