using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAniEventListener : AniEventListener
{
    [SerializeField] AttackAreaCollider attackAreaCollider;
    PlayerController controller;
    

    protected override void Awake()
    {
        base.Awake();
        controller = GetComponent<PlayerController>();

    }
    void Start()
    {
        aniEventDic = new Dictionary<string, Action>
        {
            {"AttackStart", EnableAttackCollider},
            {"Attack",Attack },
            {"AttackEnd",DisableAttackCollider },
            {"EndAnimation", EndAnimation},
        };
    }
    void Attack()
    {
        attackAreaCollider.PerformAttack();
        
    }
    void EndAnimation()
    {
        controller.ChangeCharacterState(CharacterState.Idle);
    }
    void EnableAttackCollider()
    {
        attackAreaCollider?.AttackStart();
    }
    void DisableAttackCollider()
    {
        attackAreaCollider?.AttackEnd();
    }
}
