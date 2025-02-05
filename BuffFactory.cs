using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//임시
// 스텟부스트 종류 : 공격력, 방어력, 이동속도, 공격속도 등을 추가해줘야함.
public static class BuffFactory 
{

    public static Buff CreateBuff(BuffData _buffData, SkillEffect _skillEffect, GameObject _target)
    {
        if( _buffData == null  || _skillEffect == null)
        {
            Debug.LogError("BuffData 또는 SkillEffect가 유효하지 않습니다.");
            return null;
        }

        Buff buff = ScriptableObject.CreateInstance<Buff>();
        buff.ID = _buffData.ID;
        buff.Name = _buffData.Name;
        buff.Description = _buffData.Description;
        buff.Category = _buffData.Category;
        buff.BuffType = _buffData.BuffType;
        buff.Target = _target;
        buff.StatType = _skillEffect.StatType;
        buff.Duration = _skillEffect.Duration;
        buff.FlatValue = _skillEffect.FlatValue;
        buff.PercentValue = _skillEffect.PercentValue;

        Debug.Log($"[BuffFactory] {buff.Name} 버프가 생성 대상: {_target}, 지속시간: {buff.Duration}, 값: {buff.FlatValue}");
        return buff;
    }
}
