using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffTable",menuName = "Table/BuffTable")]
public class BuffTable : ScriptableObject
{
    [SerializeField] List<BuffData> buffList = new List<BuffData>();

    Dictionary<int, BuffData> buffDataDic = new Dictionary<int, BuffData>();

    void OnEnable()
    {
        buffDataDic.Clear();
        foreach (var item in buffList)
        {
            buffDataDic[item.ID] = item;
        }
    }
    public BuffData GetBuffDataByID(int _id)
    {
        return buffDataDic[_id];
    }
}
