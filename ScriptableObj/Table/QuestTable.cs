using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "QuestTable", menuName = "Table/QuestTable")]
public class QuestTable : ScriptableObject
{
    [SerializeField] List<QuestData> questData = new List<QuestData>();
    public Dictionary<int, QuestData> questDataDic = new Dictionary<int, QuestData>();

    private void OnEnable()
    {
        InitDictionary();
    }
    void InitDictionary()
    {
        questDataDic.Clear();
        foreach(QuestData data in questData)
        {
            questDataDic[data.ID] = data;
        }
    }
    public QuestData GetQuestDataByID(int _id)
    {
        return questDataDic[_id];
    }
    public List<QuestData> GetQuestsByStartNPCID(int _npcID)
    {
        return questDataDic.Values.Where(x => x.StartNPC == _npcID).ToList();
    }
}
