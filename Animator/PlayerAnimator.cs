using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : CharacterAnimator
{
    void Start()
    {
        
    }
    public override void PlayAttackAnimation()
    {
        base.PlayAttackAnimation();
    }

    public void PlayDodgeAnimation()
    {
        animator.SetTrigger("Dodge");
    }
    public void PlayDefandingAttack()
    {
        animator.SetTrigger("DefandAttack");
    }
    public void PlayDefandAnimation(bool _isBlocking)
    {
        animator.SetBool("isBlocking", _isBlocking);
    }

    public override void ResetAnimatorParameters()
    {
        animator.ResetTrigger("Dodge");
        animator.ResetTrigger("DefandAttack");
        animator.SetBool("isBlocking", false);
    }
    public override void ChangeCharacterState(CharacterState _state)
    {
        switch(_state)
        {
            case CharacterState.Dodge:
                PlayDodgeAnimation();
                break;
            case CharacterState.Defend:
                PlayDefandAnimation(true);
                break;
            case CharacterState.Skill:
                break;
        }
        base.ChangeCharacterState(_state);
    }
}
