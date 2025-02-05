using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JobTable", menuName = "Table/JobTable")]
public class JobTable : ScriptableObject
{
    public List<JobData> data = new List<JobData>();

    public JobData GetJobDataByID(int _id)
    {
        return data.Find(x => x.JobID == _id);
    }
}
