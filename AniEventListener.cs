using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class AniEventListener : MonoBehaviour
{
    protected Dictionary<string, Action> aniEventDic = new Dictionary<string, Action>();


    protected virtual void Awake()
    {
        
    }
    public void OnAnimationEvent(string eventName)
    {
        if (aniEventDic.TryGetValue(eventName, out var action) && action != null)
        {
            action.Invoke();
        }
        else
        {
            Debug.LogWarning($"애니메이션 이벤트 '{eventName}'가 정의되지 않았습니다.");
        }
    }


}
