using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class BuffFactory 
{

    public static Buff CreateBuff(BuffData _buffData, SkillEffect _skillEffect, GameObject _target)
    {
        if( _buffData == null  || _skillEffect == null)
        {
            Debug.LogError("BuffData �Ǵ� SkillEffect�� ��ȿ���� �ʽ��ϴ�.");
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

        Debug.Log($"[BuffFactory] {buff.Name} ������ ���� ���: {_target}, ���ӽð�: {buff.Duration}, ��: {buff.FlatValue}");
        return buff;
    }
}
