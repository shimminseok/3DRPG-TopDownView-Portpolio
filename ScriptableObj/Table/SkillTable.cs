using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillTable", menuName = "Table/SkillTable")]
public class SkillTable : ScriptableObject
{
    public List<SkillData> skillList = new List<SkillData>();

    Dictionary<int, SkillData> skillDataDic = new Dictionary<int, SkillData>();

    private void OnEnable()
    {
        skillDataDic.Clear();
        foreach (SkillData skill in skillList)
        {
            skillDataDic[skill.ID] = skill;
        }
    }
    public SkillData GetSkillDataByID(int _id)
    {
        return skillDataDic[_id];
    }

    public List<SkillData> GetSKillsByJobID(int _jobID)
    {
        return skillDataDic.Values.Where(x => x.RequiredChcaracterType == (ClassType)_jobID || x.RequiredChcaracterType == ClassType.All).ToList();
    }
}
