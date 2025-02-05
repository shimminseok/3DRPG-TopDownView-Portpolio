using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterTable", menuName = "Table/MonsterTable")]
public class MonsterTable : ScriptableObject
{
    [SerializeField]List<MonsterData> data;

    Dictionary<int,MonsterData> monsterDataDic = new Dictionary<int,MonsterData>();
    private void OnEnable()
    {
        monsterDataDic.Clear();
        foreach (MonsterData monsterData in data)
        {
            monsterDataDic[monsterData.MonsterID] = monsterData;
        }
    }
    public MonsterData GetMonsterDataByID(int _id)
    {
        return monsterDataDic[_id];  
    }
}
