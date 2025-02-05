using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCTable", menuName = "Table/NPCTable")]

public class NPCTable : ScriptableObject
{
    [SerializeField] List<NPCData> npcData;

    Dictionary<int,NPCData> npcDataDic = new Dictionary<int,NPCData>();
    private void OnEnable()
    {
        npcDataDic.Clear();
        foreach (NPCData npc in npcData)
        {
            npcDataDic[npc.NPCID] = npc;
        }
    }
    public NPCData GetNPCDataByID(int _id)
    {
        return npcDataDic[_id];
    }
}
