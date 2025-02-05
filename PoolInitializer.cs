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
            Debug.LogWarning($"���ҽ� ������ ���ҽ��� �����ϴ�. : {path}");
        }
        else
        {
            ObjectPoolManager.Instance.CreatePool(go, poolCount);
        }
    }
}
