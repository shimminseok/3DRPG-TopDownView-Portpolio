using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum PoolType
{
    Monster,
    Effect,
    Object,
    NameTag
}
public class PoolInitializer : MonoBehaviour
{
    [SerializeField] PoolType poolType;
    [SerializeField] int poolCount;
    void Start()
    {

        string path = $"{poolType}/{gameObject.name}";
        GameObject go = Resources.Load<GameObject>(path);
        if (go == null)
        {
            Debug.LogWarning($"리소스 폴더에 리소스가 없습니다. : {path}");
        }
        else
        {
            ObjectPoolManager.Instance.CreatePool(go, poolCount);
        }
    }
}
