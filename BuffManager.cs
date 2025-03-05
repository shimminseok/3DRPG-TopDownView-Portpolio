using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;


public class BuffManager : MonoBehaviour
{
    Dictionary<BuffType, List<Buff>> activeBuffDic = new Dictionary<BuffType, List<Buff>>();


    public event Action<Buff> OnBuffAdded;
    public event Action<Buff> OnBuffRemoved;

    public Coroutine buffTimers;
    /// <summary>
    /// 버프를 추가하는 함수
    /// </summary>
    /// <param name="buffId"></param>
    public void AddBuff(Buff _buff)
    {
        if(_buff == null)
        {
            Debug.LogError("추가하려는 Buff가 null입니다.");
            return;
        }
        if(!activeBuffDic.TryGetValue(_buff.BuffType, out var buffList))
        {
            buffList = new List<Buff>();
            activeBuffDic[_buff.BuffType] = buffList;
        }

        buffList.Add(_buff);
        _buff.ApplyBuff();
        OnBuffAdded?.Invoke(_buff);
        if(buffTimers == null)
            buffTimers = StartCoroutine(UpdateBuffTimers());

    }
    public void RemoveBuff(Buff _buff)
    {
        if (activeBuffDic.TryGetValue(_buff.BuffType, out var buffList))
        {
            _buff.RemoveBuff();
            buffList.Remove(_buff);

            if (buffList.Count == 0)
                activeBuffDic.Remove(_buff.BuffType);

            OnBuffRemoved?.Invoke(_buff);
        }
    }
    public void RemoveAllBuff()
    {
        foreach (var buffList in activeBuffDic.Values)
        {
            foreach (var buff in buffList)
            {
                RemoveBuff(buff);
            }
        }

        activeBuffDic.Clear();
    }
    IEnumerator UpdateBuffTimers()
    {
        while (true)
        {
            if(activeBuffDic.Count == 0)
            {
                yield return null;
                continue;
            }
            yield return new WaitForSeconds(0.1f);
            foreach (var buffList in activeBuffDic.Values.ToList())
            {
                for (var i = 0; i < buffList.Count; i++)
                {
                    buffList[i].UpdateDuration(0.1f);
                    if (buffList[i].Duration <= 0)
                    {
                        RemoveBuff(buffList[i]);
                    }
                }
            }
        }
    }
    public Buff GetActiveBuffByStatType(BuffType _type, StatType _stat)
    {
        if (!activeBuffDic.TryGetValue(_type, out var buffList))
        {
            return null;
        }
        return buffList.Find(x => x.StatType == _stat);
    }
}
