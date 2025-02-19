using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStat : BaseStat
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    public void InitializeFromMonsterData(MonsterData _monsterData)
    {

        foreach(Stat stat in Stats.Values)
        {
            stat.ResetModifiers();
        }
        Health.ModifyBaseValue(_monsterData.Health);
        CurrentHP.ModifyBaseValue(_monsterData.Health);
        Attack.ModifyBaseValue(_monsterData.AttackPower);
        Defense.ModifyBaseValue(_monsterData.Defense);
        AttackSpd.ModifyBaseValue(_monsterData.AttackSpd);
    }

}
