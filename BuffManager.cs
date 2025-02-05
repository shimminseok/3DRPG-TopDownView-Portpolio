using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuffManager : MonoBehaviour
{
    Dictionary<BuffType, List<Buff>> activeBuffDic = new Dictionary<BuffType, List<Buff>>();
    Dictionary<int, Coroutine> activeCoroutine = new Dictionary<int, Coroutine>();


    public event Action<Buff> OnBuffAdded;
    public event Action<Buff> OnBuffRemoved;

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
        Coroutine activeCo = StartCoroutine(RemoveEffectAfterDuration(_buff));

        activeCoroutine[_buff.ID] = activeCo;
    }
    public void RemoveBuff(Buff _buff)
    {
        if (activeBuffDic.TryGetValue(_buff.BuffType, out var buffList))
        {
            _buff.RemoveBuff();
            buffList.Remove(_buff);

            if (buffList.Count == 0)
                activeBuffDic.Remove(_buff.BuffType);

            activeCoroutine.Remove(_buff.ID);

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

        foreach (var co in activeCoroutine.Values)
        {
            StopCoroutine(co);
        }

        activeBuffDic.Clear();
        activeCoroutine.Clear();
    }
    public IEnumerator RemoveEffectAfterDuration(Buff _buff)
    {
        while(_buff.Duration > 0)
        {
            _buff.UpdateDuration(0.1f);
            yield return new WaitForSeconds(0.1f);
        }
        RemoveBuff(_buff);
    }

    public Buff GetActiveBuff(BuffType _type, int _id)
    {
        if(!activeBuffDic.TryGetValue(_type, out var buffList))
        {
            return null; 
        }
        return buffList.Find(x => x.ID == _id);
    }
}
