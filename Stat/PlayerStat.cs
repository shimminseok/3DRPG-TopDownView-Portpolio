using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStat : BaseStat
{
    public Stat CriticalChance = new Stat(StatType.CriticalChance);
    public Stat MP = new Stat(StatType.MaxMP);
    public Stat HPRegen = new Stat(StatType.HPRegen);
    public Stat MPRegen = new Stat(StatType.MPRegen);
    public Stat Experience = new Stat(StatType.Experience);

    protected override void Start()
    {
        base.Start();
        Stats.Add(StatType.CriticalChance, CriticalChance);
        Stats.Add(StatType.MaxMP, MP);
        Stats.Add(StatType.HPRegen, HPRegen);
        Stats.Add(StatType.MPRegen, MPRegen);
        Stats.Add(StatType.Experience, Experience);

        foreach (Stat stat in Stats.Values)
        {
            stat.OnStatChanged += (float value) =>
            {
                UICharacterInfo.Instance.UpdateStatUI(stat);
            };
        }
    }
    public void InitializeFromJob(JobData _jobData)
    {
        Health.ModifyBaseValue(_jobData.Health);
        CurrentHP.ModifyBaseValue(_jobData.Health);
        MP.ModifyBaseValue(_jobData.MP);
        Attack.ModifyBaseValue(_jobData.AttackPower);
        Defense.ModifyBaseValue(_jobData.Defense);
        AttackSpd.ModifyBaseValue(_jobData.AttackSpd);
        CriticalChance.ModifyBaseValue(_jobData.CriticalChance);
        HPRegen.ModifyBaseValue(_jobData.HPRegen);
        MPRegen.ModifyBaseValue(_jobData.MPRegen);
        MoveSpd.ModifyBaseValue(1f);
    }

    public void ResetModify()
    {
        foreach (Stat stat in Stats.Values)
        {
            stat.ResetModifiers();
        }
    }
    public void RecoverHP(int _value)
    {
        //체력 회복이 MaxHP의 FinalValue를 넘지않게 하기위해
        CurrentHP.ModifyBaseValue(_value,0,Health.FinalValue);
    }
    public void RecoverMP(int _value)
    {
        CurrentMP.ModifyBaseValue(_value,0,MP.FinalValue);
    }
    public override Stat GetStat(StatType _type)
    {
        Stat baseStat = base.GetStat(_type);
        if(baseStat != null)
            return baseStat;

        return _type switch
        {
            StatType.CriticalChance => CriticalChance,
            StatType.MaxMP => MP,
            StatType.HPRegen => HPRegen,
            StatType.MPRegen => MPRegen,
            _ => throw new System.ArgumentOutOfRangeException(nameof(_type), "Invalid stat type!")
        };

    }
}
