using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStat : MonoBehaviour
{
    public Stat Health = new Stat(StatType.MaxHP);
    public Stat Attack = new Stat(StatType.AttackPow);
    public Stat Defense = new Stat(StatType.Defense);
    public Stat MoveSpd = new Stat(StatType.MoveSpd);
    public Stat AttackSpd = new Stat(StatType.AttackSpd);
    public Stat CurrentHP = new Stat(StatType.CurrentHP);
    public Stat CurrentMP = new Stat(StatType.CurrentMP);
    public Stat Level = new Stat(StatType.Level);

    public Dictionary<StatType, Stat> Stats = new Dictionary<StatType, Stat>();
    protected virtual void Start()
    {
        Stats.Add(StatType.MaxHP, Health);
        Stats.Add(StatType.AttackPow, Attack);
        Stats.Add(StatType.Defense, Defense);
        Stats.Add(StatType.MoveSpd, MoveSpd);
        Stats.Add(StatType.AttackSpd, AttackSpd);
        Stats.Add(StatType.CurrentHP, CurrentHP);
        Stats.Add(StatType.CurrentMP, CurrentMP);
        Stats.Add(StatType.Level, Level);

    }

    public void ApplyBuff(StatType _type, float _flat, float _percent)
    {
        Stat targetStat = GetStat(_type);
        targetStat.ModifyBuffValue(_flat, _percent);

    }
    public void RemoveBuff(StatType _type, float _flat, float _percent)
    {
        Stat targetStat = GetStat(_type);
        targetStat.ModifyBuffValue(-_flat, -_percent);
    }
    public virtual Stat GetStat(StatType _type)
    {
        return _type switch
        {
            StatType.AttackPow => Attack,
            StatType.Defense => Defense,
            StatType.MaxHP => Health,
            StatType.AttackSpd => AttackSpd,
            StatType.MoveSpd => MoveSpd,
            _ => null
        };
    }
}
