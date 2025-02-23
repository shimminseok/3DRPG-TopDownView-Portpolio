using JetBrains.Annotations;
using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Common
{
}
#region[enum]
[Flags]
public enum SceneType
{
    Title,
    InGame
}
public enum StatType
{
    None,
    AttackPow,
    Defense,
    MaxHP,
    MaxMP,
    HPRegen,
    MPRegen,
    AttackSpd,
    MoveSpd,
    CriticalChance,
    Experience,
    Level,
    CurrentHP,
    CurrentMP
}
public enum DropSlotType
{
    Skill,
    Item,
}
public enum SkillTarget
{
    Self,
    Enemy,
    AreaEnemy,
    AreaParty,
    AllyParty

}
public enum SkillEffectType
{
    Damage,
    Buff,
    Debuff
}
public enum BuffCategory
{
    Buff,
    Debuff
}
public enum BuffType
{
    Stun,
    StatBuff,
    StatDeBuff,
}
public enum CharacterType
{
    NPC,
    Player,
    Monster
}
public enum ClassType
{
    All,
    Warrior,
    Assassin,
    Witch,
    Priestess
}
public enum MonsterType
{
    Common,
    Elite,
    Boss
}
public enum NPCType
{
    Rommer,
    Static,
    Combat
}
public enum QuestType
{
    Main,
    Side,
    Repeat,
}
public enum QuestStatus
{
    NotStarted,
    InProgress,
    Completed,
    Failed
}
public enum QuestCategory
{
    KillMonster,
    Collect,   // ����
    Delivery,  // ���
}
public enum QuestTargetType
{
    Monster,
    Item,
    NPC,
    Location
}
public enum ItemType
{
    Weapon,
    Armor,
    Helmet,
    Goroves,
    Shoes,
    Potion,
    Material
}
public enum SkillRangeType
{
    Rectangle,
    Sector,
    Circle,
    Point
}
[Flags]
public enum NPCFunction
{
    None = 0,
    Quest = 1 << 0,
    Shop = 1 << 1,
    Enhance = 1 << 2,
}
#endregion[enum]
#region[interface]
public interface IMoveable
{
    void Move(Vector3 _movement);
    void StopMovement();
}
public interface IAttacker
{
    float FinalDam { get; }
    void Attack(IDamageable _target);
}
public interface IDamageable
{
    Transform Transform { get; }
    void TakeDamage(int _damage, IAttacker _attacker = null);
    int CalculateDamage(int _dam);
    void Die();
}
public interface ISkillCaseter
{
    void UseSkill(SaveSkillData _data);

}
public interface ITargetable
{
    void SetTarget();
}
#region[NPC Interface]
public interface IInteractable
{
    void OnInteract();
    void OnExitInteract();
}
public interface IQuestGiver
{
    void GiveQuest();
    void GiveReward();
}
public interface INPCFunction
{
    NPCFunction FuncType { get; }
    void Initialize(NPCData _data);
    void Execute();
}
public interface IDisplayable
{
    void ShowHUD();
    void HideHUD();
}
#endregion[NPC Interface]
#endregion[Interface]

#region[Class]
[Serializable]
public class BuffData
{
    public int ID;
    public string Name;             //���� �̸�
    public BuffCategory Category; // ���� ����
    public BuffType BuffType;          // ���� Ÿ��
    public string Description;    // ����
    public Sprite BuffImage;

}
[Serializable]
public class SkillData
{
    public string Name;         //��ų �̸�
    public int ID;                  //��ųID
    public float CastingTime;   //ĳ���ýð�
    public float CoolTime;      //��ų ��Ÿ��
    public int RequiredMP;      //�䱸 MP
    public SkillRangeType RangeType;
    public short Width; //��ų ���� ����
    public short Length;// ��ų ���� ����
    public short Radius; // ���� / ��ä�� ������
    public short Angle; //��ä�� ����
    public string Description;           //��ų����
    public GameObject EffectPrefab;  //��ų����Ʈ
    public AnimationClip AnimationClip;
    public ClassType RequiredChcaracterType; //��� ���� ����
    public Sprite SkillImage;




    [Header("Skill Effect")]
    public List<SkillEffect> SkillEffects;
}
[Serializable]
public class QuestData
{
    //����Ʈ �⺻ ����
    [Header("����Ʈ �⺻ ����")]
    public int ID;
    public string Name;             //����Ʈ �̸�
    public string Description;    // ����
    public QuestType Type;
    public QuestCategory Cartegory;

    [Header("����Ʈ ��ǥ ")]
    public List<QuestCondition> Conditions; //����Ʈ ���� ���

    [Header("����Ʈ ���� ����")]
    public bool IsRepeatable; //�ݺ� ���� ����
    public int TargetCount;
    public int StartNPC; //���� NPC
    public int EndNPC; // �Ϸ� NPC
    public QuestData NextQuest;

    [Header("����")]
    public RewardData Reward;

    [Header("����Ʈ ���� ����")]
    public float TimeLimit; //�ð� ����
    public int LevelRequirement; //���� ����
    public List<int> PrerequisiteQuest; //���� ����Ʈ

    //����Ʈ ���
    [Header("����Ʈ ���")]
    public List<string> PreQuestDialogues;
    public List<string> PostQuestDialogues;
}
[Serializable]
public class QuestCondition
{
    public QuestTargetType TargetType;
    [Header("����")]
    public string QuestConditionTxt;
    [Header("Ÿ�� ID")]
    public int TargetID;
    [Header("��ǥ ī��Ʈ")]
    public int RequiredCount;

}
[Serializable]
public class RewardData
{
    public int EXPReward;
    public int GoldReward;
    public List<SaveItemData> ItemRewards = new List<SaveItemData>();
}
[Serializable]
public class NPCData
{
    public int NPCID;
    public string Name;
    public List<int> SaleItemIDs;
    public List<string> DefaultDialogues;

    [Header("Function")]
    public NPCFunction NPCFunctions;

    public bool HasFunction(NPCFunction _func)
    {
        return (NPCFunctions & _func) == _func;
    }
}
[Serializable]
public class JobData
{
    public int JobID;
    public string JobName;
    public UnityEditor.Animations.AnimatorController Animator;
    public GameObject JobPrefabs;
    public Avatar JobAvatar;
    [Header("BaseStat")]
    public int Health;
    public int MP;
    public int Defense;
    public int AttackPower;
    public float AttackSpd;
    public float CriticalChance;
    public int HPRegen;
    public int MPRegen;
    [Header("AttackRangeData")]
    public AttackRangeData AttackRangeData;
}
[Serializable]
public class AttackRangeData
{
    public float Range;     //���� ����
    public float Angle;     //��ä�� ����
    public Vector3 Offset;
    public GameObject EffectPrefab;
}
[Serializable]
public class MonsterData
{
    public int MonsterID;
    public string MonsterName;
    public MonsterType MonsterType;
    public GameObject MonsterPrefabs;
    public Avatar MonsterAvatar;
    public bool IsAggressive;
    [Header("BaseStat")]
    public int Level;
    public int Health;
    public int Defense;
    public int AttackPower;
    public float AttackSpd;
    [Header("AttackRangeData")]
    public AttackRangeData AttackRangeData;
}
[Serializable]
public class ItemData
{
    public string Name;
    public int ItemID;
    public Sprite ItemImg;
    public ItemType ItemType;
    public int ItemGrade;
    public List<ClassType> UsableClass;
    public ItemStats ItemStats;
    public string ItemDescript;

    public bool IsSellable;
    public bool IsStackable;
    public bool IsUseable;
    public int Price;


    [Header("���� ����")]
    public float Cooldown;        // ���� ���ð�
    public bool IsInstantUse;     // ��� ��� ���� (ex: Ŭ�� �� �ٷ� ���)
}

[Serializable]
public class ItemStats
{
    [SerializeField] List<StatEntry> StatEntries;
    public Dictionary<StatType, float> Stats;


    public void Initialize()
    {
        Stats = new Dictionary<StatType, float>();

        foreach (var stat in StatEntries)
        {
            if (!Stats.ContainsKey(stat.StatType))
            {
                Stats[stat.StatType] = stat.Value;
            }
        }
    }
    public float GetStat(StatType _type)
    {
        return Stats.ContainsKey(_type) ? Stats[_type] : 0;
    }
    public void SetStat(StatType _type, float _value)
    {
        Stats[_type] = _value;
    }
}
[Serializable]
public class StatEntry
{
    public StatType StatType;
    public float Value;
}
[Serializable]
public class SkillEffect
{
    public SkillEffectType EffectType;
    public int BuffID;
    public SkillTarget TargetType;
    public StatType StatType;
    public float Duration;
    public float FlatValue;
    public float PercentValue;

    public Sprite BuffOverrideIcon; //Ŀ���� ���� Icon;
}
[Serializable]
public class MaterialRequirement
{
    public int MaterialID;
    public int Quantity;
}
[Serializable]
public class EnhanceData
{
    public string Name;
    public int EnhancementStep;
    public int Grade;
    public List<MaterialRequirement> Requirements;
    public int GoldCost;
    public float SuccessRate;

    Dictionary<int, int> MaterialDic;

    public void InitializeDictionary()
    {
        MaterialDic = new Dictionary<int, int>();
        foreach(var mat in Requirements)
        {
            MaterialDic[mat.MaterialID] = mat.Quantity;
        }
    }
    public int GetRequiredMaterialCount(int _matID)
    {
        if(MaterialDic.TryGetValue(_matID, out var count))
            return count;

        return 0;
    }
}
public class Buff : ScriptableObject
{
    public int ID;                     // ���� ID
    public string Name;                // ���� �̸�
    public string Description;         // ���� ����
    public BuffCategory Category;    // Buff / Debuff
    public BuffType BuffType;          // Buff �� Ÿ��
    public StatType StatType;
    public GameObject Target;          // ���� ���
    public float Duration;             // ���� �ð�
    public float FlatValue;                // ȿ�� ��
    public float PercentValue;                // ȿ�� ��


    public event Action<float> OnBuffUpdated;
    public BuffData BuffTableData;
    public void UpdateDuration(float _deltaTime)
    {
        OnBuffUpdated?.Invoke(Duration);
        Duration -= _deltaTime;
    }
    public void ApplyBuff()
    {
        if (BuffTableData == null)
        {
            BuffTableData = TableLoader.Instance.GetTable<BuffTable>().GetBuffDataByID(ID);
        }
        var statComponent = Target.GetComponent<BaseStat>();
        if (statComponent == null)
        {
            Debug.LogWarning($"[Buff] {Name} ���� ����: Target�� CharacterStat�� �����ϴ�.");
            return;
        }
        Debug.Log($"[Buff] {Name}��(��) {Target}�� ����Ǿ����ϴ�. (���ӽð�: {Duration}, ��: {FlatValue}) �� :{PercentValue}");
        switch (BuffType)
        {
            case BuffType.StatBuff:
                statComponent.ApplyBuff(StatType, FlatValue, PercentValue);
                break;
            case BuffType.StatDeBuff:
                statComponent.ApplyBuff(StatType, -FlatValue,-PercentValue);
                break;
        }
    }
    public void RemoveBuff()
    {
        if (BuffTableData == null)
        {
            BuffTableData = TableLoader.Instance.GetTable<BuffTable>().GetBuffDataByID(ID);
        }
        var statComponent = Target.GetComponent<BaseStat>();
        if (statComponent == null)
        {
            Debug.LogWarning($"[Buff] {Name} ���� ����: Target�� CharacterStat�� �����ϴ�.");
            return;
        }
        Debug.Log($"[Buff] {Name}��(��) {Target}���� ���ŵǾ����ϴ�.");
        switch (BuffType)
        {
            case BuffType.StatBuff:
                statComponent.RemoveBuff(StatType, FlatValue,PercentValue);
                break;
            case BuffType.StatDeBuff:
                statComponent.RemoveBuff(StatType, -FlatValue, -PercentValue);
                break;
        }

    }
}
public class Stat
{
    public StatType Type;
    public float BaseValue { get; private set; }
    public float BuffValue { get; private set; }
    public float EquipmentValue { get; private set; }
    public float PercentValue { get; private set; }
    public float FinalValue => (BaseValue + BuffValue + EquipmentValue) * (1 + PercentValue);
    public event Action<float> OnStatChanged;


    public bool IsChangeStat = false;
    public Stat(StatType _type)
    {
        Type = _type;
    }
    public void ResetModifiers()
    {
        BaseValue = 0;
        BuffValue = 0;
        EquipmentValue = 0;
        IsChangeStat = true;
    }
    /// <summary>
    /// BaseValue�� �����ϴ� �޼���
    /// </summary>
    /// <param name="_value">��ȭ��(��� : ����, ���� : ����)</param>
    public void ModifyBaseValue(float _value, float _min = 0, float _max = float.MaxValue)
    {
        IsChangeStat = true;
        BaseValue = Mathf.Clamp(BaseValue + _value, _min, _max);
        OnStatChanged?.Invoke( FinalValue);

        Debug.Log($"{Type} Update {FinalValue}");
    }
    /// <summary>
    /// BuffValue�� �����ϴ� �޼���
    /// </summary>
    /// <param name="_value">��ȭ��(��� : ����, ���� : ����)</param>
    public void ModifyBuffValue(float _flat, float _percent)
    {
        IsChangeStat = true;
        BuffValue += _flat;
        PercentValue += _percent;
        OnStatChanged?.Invoke( FinalValue);
        Debug.Log($"{Type} Update {FinalValue}");

    }
    /// <summary>
    /// EquipmentValue�� �����ϴ� �޼���
    /// </summary>
    /// <param name="_value">��ȭ��(��� : ����, ���� : ����)</param>
    public void ModifyEquipmentValue(float _value)
    {
        IsChangeStat = true;
        EquipmentValue += _value;
        OnStatChanged?.Invoke( FinalValue);
        Debug.Log($"{Type} Update {FinalValue}");
    }
    public void ModifyAllValue(float _value, float _percent = 0)
    {
        float remainingDam = _value;
        if(BuffValue > 0)
        {
            float damToBuff = MathF.Min(remainingDam, BuffValue);
            ModifyBuffValue(damToBuff, _percent);
            remainingDam -= damToBuff;
        }
        if(remainingDam > 0)
        {
            float damToEquip = Mathf.Min(remainingDam, EquipmentValue);
            ModifyEquipmentValue(damToEquip);
            remainingDam -= damToEquip;
        }
        if(remainingDam > 0)
        {
            ModifyBaseValue(-remainingDam,0,FinalValue);
        }
    }
}

#region[SaveData]
[Serializable]
public class SaveItemData
{
    public int ItemID = 0;
    public int Quantity = 0;
    public int enhanceLevel = 0;

    ItemData itemData = null;
    public ItemData ItemData
    {
        get
        {
            if(itemData == null || itemData.ItemID == 0)
            {
                itemData = TableLoader.Instance.GetTable<ItemTable>().GetItemDataByID(ItemID);
            }
            return itemData;
        }
    }
    public SaveItemData DeepCopy()
    {
        SaveItemData deepCopy = new SaveItemData();
        deepCopy.ItemID = ItemID;
        deepCopy.Quantity = Quantity;
        deepCopy.enhanceLevel = enhanceLevel;
        deepCopy.itemData = itemData;

        return deepCopy;
    }
}
[Serializable]
public class SaveQuestData
{
    public int QuestID;
    public int CurrentCount;
    public float Progress;
    public QuestStatus Status = QuestStatus.NotStarted;
    public List<QuestConditionProgress> Conditions = new List<QuestConditionProgress>();

    public QuestData QuestTableData;
    public bool IsCompleted => Conditions.TrueForAll(x => x.IsConditionCompleted(GetQuestData()));
    public SaveQuestData(QuestData _data)
    {
        QuestID = _data.ID;
        QuestTableData = _data;
        for (int i = 0; i < _data.Conditions.Count; i++)
        {
            QuestConditionProgress progress = new QuestConditionProgress();
            progress.ConditionIndex = i;
            progress.CurrentCount = 0;
            Conditions.Add(progress);
        }
    }
    public QuestData GetQuestData()
    {
        if(QuestTableData == null )
        {
            QuestTableData = TableLoader.Instance.GetTable<QuestTable>().GetQuestDataByID(QuestID);
        }
        return QuestTableData;
    }
}
[Serializable]
public class QuestConditionProgress
{
    public int ConditionIndex;
    public int CurrentCount;

    public bool IsCompleted;


    public bool IsConditionCompleted(QuestData _data)
    {
        IsCompleted = CurrentCount >= _data.Conditions[ConditionIndex].RequiredCount;
        return IsCompleted;
    }
}
public class SaveSkillData
{
    public int SkillID;
    public int SkillLevel = 1;
    public KeyCode HotKey;
    SkillData TableData;
    public SkillData GetSkillData()
    {
        if(TableData == null)
            TableData = TableLoader.Instance.GetTable<SkillTable>().GetSkillDataByID(SkillID);
        return TableData;
    }
}

[Serializable] public class GameSaveData
{
    public int Gold;
    public List<SaveItemData> Inventory;
    public List<SaveQuestData> ActiveQuests;
}
#endregion[SaveData]
#endregion[Class]
