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
    public Stat CurrentMP = new Stat(StatType.CurrentMP);
    public Stat HPRegen = new Stat(StatType.HPRegen);
    public Stat MPRegen = new Stat(StatType.MPRegen);
    public Stat Experience = new Stat(StatType.Experience);


    public event Action<float, int> OnGainExp;
    public event Action<int> OnLevelUp;
    protected override void Start()
    {
        base.Start();
        Stats.Add(StatType.CriticalChance, CriticalChance);
        Stats.Add(StatType.MaxMP, MP);
        Stats.Add(StatType.CurrentMP, CurrentMP);
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

        QuestManager.Instance.OnQuestReward += ApplyExpReward;
    }
    public void InitializeFromJob(JobData _jobData)
    {
        Health.ModifyBaseValue(_jobData.Health);
        CurrentHP.ModifyBaseValue(_jobData.Health);
        MP.ModifyBaseValue(_jobData.MP);
        CurrentMP.ModifyBaseValue(_jobData.MP);
        Attack.ModifyBaseValue(_jobData.AttackPower);
        Defense.ModifyBaseValue(_jobData.Defense);
        AttackSpd.ModifyBaseValue(_jobData.AttackSpd);
        CriticalChance.ModifyBaseValue(_jobData.CriticalChance);
        HPRegen.ModifyBaseValue(_jobData.HPRegen);
        MPRegen.ModifyBaseValue(_jobData.MPRegen);
        MoveSpd.ModifyBaseValue(1f);

        //임시
        Level.ModifyBaseValue(1);
    }

    public void ResetModify()
    {
        foreach (Stat stat in Stats.Values)
        {
            stat.ResetModifiers();
        }
    }

    public void RecoverMP(int _value)
    {
        CurrentMP.ModifyBaseValue(_value,0,MP.FinalValue);
    }
    public void GainExp(int _amount)
    {
        Experience.ModifyBaseValue(_amount);
        CheckLevelUp();
    }
    void ApplyExpReward(RewardData _reward)
    {
        GainExp(_reward.EXPReward);
    }
    void CheckLevelUp()
    {
        bool isLevelUp = false;
        while (Experience.FinalValue >= CalculateNextLevelEXP())
        {
            float remainingEXP = Experience.FinalValue - CalculateNextLevelEXP();
            LevelUp(remainingEXP);
            isLevelUp = true;
        }
        OnGainExp?.Invoke(Experience.FinalValue, CalculateNextLevelEXP());
        if(isLevelUp)
            OnLevelUp?.Invoke((int)Level.FinalValue);
    }
    void LevelUp(float _remainExp)
    {
        Level.ModifyBaseValue(1, 1); // 레벨 증가
        Experience.ResetModifiers(); // 기존 경험치 초기화
        Experience.ModifyBaseValue(_remainExp); // 초과 경험치 반영
        Debug.Log($"[PlayerStat] 레벨업! 현재 레벨: {Level.FinalValue}");
    }

    int CalculateNextLevelEXP()
    {
        float baseEXP = 100f;
        float growthFactor = 1.1f;

        return Mathf.RoundToInt(baseEXP * Mathf.Pow(Level.FinalValue, 2) * growthFactor);
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
