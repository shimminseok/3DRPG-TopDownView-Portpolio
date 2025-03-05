using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AccountManager : MonoBehaviour
{
    public static AccountManager Instance { get; private set; }

    public int Gold {  get; private set; }


    public event Action<int> OnChangedGold;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    void Start()
    {
        Gold = GameManager.Instance.LoadGameData().Gold;
        QuestManager.Instance.OnQuestReward += ApplyGoldRewad;
    }

    public void AddGold(int _amount)
    {
        Gold += _amount;
        Debug.Log($"°ñµå È¹µæ : {_amount}, ÃÑ °ñµå : {Gold}");
        OnChangedGold?.Invoke(Gold);
    }
    public void UseGold(int _amount)
    {
        if(!IsEnoughtGold(_amount))
        {
            Debug.Log("¼ÒÀ¯ °ñµå°¡ ºÎÁ·ÇÕ´Ï´Ù");
            return;
        }
        Gold -= _amount;
        OnChangedGold?.Invoke(Gold);
    }
    public bool IsEnoughtGold(int _amount)
    {
        return Gold - _amount >= 0;
    }
    public void ApplyGoldRewad(RewardData _reward)
    {
        AddGold(_reward.GoldReward);
    }
}
