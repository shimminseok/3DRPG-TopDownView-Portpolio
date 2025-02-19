using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnhancementTable", menuName = "Table/Enhancement")]
public class EnhancementTable : ScriptableObject
{
    public List<EnhanceData> EnhanceDataList = new List<EnhanceData>();

    Dictionary<(int, int), EnhanceData> EnhanceDataDic = new Dictionary<(int, int), EnhanceData>(); // Key : ItemLevel, ItemGrade


    private void OnEnable()
    {
        EnhanceDataDic.Clear();
        foreach (EnhanceData data in EnhanceDataList)
        {
            data.InitializeDictionary();
            EnhanceDataDic[(data.EnhancementStep, data.Grade)] = data;
        }
    }

    public EnhanceData GetEnhanceDataByLevelAndGrade(int _level, int _grade)
    {
        if (EnhanceDataDic.TryGetValue((_level, _grade), out EnhanceData enhanceData))
            return enhanceData;

        return null;
    }
}
