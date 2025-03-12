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
        OnChangedGold?.Invoke(Gold);

        UIHUD.Instance.OnGetGoldDisplayed?.Invoke(_amount);
    }
    public void UseGold(int _amount)
    {
        if(!IsEnoughtGold(_amount))
        {
            return;
        }
        Gold -= _amount;
        OnChangedGold?.Invoke(Gold);
    }
    public bool IsEnoughtGold(int _amount)
    {
        bool hasEnough = Gold >= _amount;
        if (!hasEnough) UIHUD.Instance.OnAletMessage("골드가 부족합니다.");
        return hasEnough;
    }
    public void ApplyGoldRewad(RewardData _reward)
    {
        AddGold(_reward.GoldReward);
    }
}
