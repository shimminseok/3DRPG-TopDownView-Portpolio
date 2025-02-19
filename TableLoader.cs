using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableLoader : MonoBehaviour
{
    [SerializeField] NPCTable NPCTable;
    [SerializeField] BuffTable BuffTable;
    [SerializeField] QuestTable QuestTable;
    [SerializeField] SkillTable SkillTable;
    [SerializeField] MonsterTable MonsterTable;
    [SerializeField] ItemTable ItemTable;
    [SerializeField] JobTable JobTable;
    [SerializeField] EnhancementTable EnhancementTable;

    public static TableLoader Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadTables();
        }
    }

    public void LoadTables()
    {
        Debug.LogWarning("���̺��� �ε��մϴ�.");
    }

    public T GetTable<T>() where T : ScriptableObject
    {
        if (typeof(T) == typeof(NPCTable)) return NPCTable as T;
        if (typeof(T) == typeof(BuffTable)) return BuffTable as T;
        if (typeof(T) == typeof(QuestTable)) return QuestTable as T;
        if (typeof(T) == typeof(SkillTable)) return SkillTable as T;
        if (typeof(T) == typeof(MonsterTable)) return MonsterTable as T;
        if(typeof(T) == typeof(ItemTable)) return ItemTable as T;
        if(typeof(T) == typeof(JobTable)) return JobTable as T;
        if(typeof(T) == typeof(EnhancementTable)) return EnhancementTable as T;

        Debug.LogError("���̺��� �������� �ʽ��ϴ�.");
        return null;

    }
}

