using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public Animator animator;

    public AnimatorStateInfo currentAniStateInfo;
    [SerializeField] string movingParam = "isMove";
    [SerializeField] string attackParam = "Attack";
    [SerializeField] string stunParam = "isStun";
    [SerializeField] string dieParam = "Dead";


    protected virtual void Awake()
    {
        //animator = GetComponent<Animator>();
    }
    public virtual void PlayMoveAnimation(bool _isMoving)
    {
        animator.SetBool(movingParam, _isMoving);
    }
    public virtual void PlayAttackAnimation()
    {
        animator.SetTrigger(attackParam);
    }
    public void ResetTrigger(string _param)
    {
        animator.ResetTrigger(_param);
    }
    public virtual void PlayStunParam(bool _isStun)
    {
        animator.SetBool(stunParam, _isStun);
    }
    public virtual void PlayDieAnimation()
    {
        animator.SetTrigger(dieParam);
    }
    public void PlaySkillAnimation(SkillData _skillData)
    {
        if (_skillData == null)
        {
            Debug.LogWarning("SKill Data null");
            return;
        }
        if (_skillData.AnimationClip != null)
        {
            animator.Play(_skillData.AnimationClip.name);
        }
        else
            Debug.LogWarning($"애니메이션 정보가 없음 {_skillData.Name}");
    }
    public virtual void ResetAnimatorParameters()
    {
        animator.SetBool(movingParam, false);
        animator.SetBool(stunParam, false);
        animator.ResetTrigger(attackParam);
        animator.ResetTrigger(dieParam);
    }
    public virtual void ChangeCharacterState(CharacterState _state)
    {
        switch (_state)
        {
            case CharacterState.Idle:
                ResetAnimatorParameters();
                break;
            case CharacterState.Move:
                animator.SetBool(movingParam, true);
                break;
            case CharacterState.Attack:
                animator.SetTrigger(attackParam);
                break;
            case CharacterState.Stun:
                animator.SetBool(stunParam,true);
                break;
            case CharacterState.Dead:
                animator.SetTrigger(dieParam);
                break;
            case CharacterState.Hit:
                animator.SetTrigger("Hit");
                break;
        }
        currentAniStateInfo = animator.GetCurrentAnimatorStateInfo(0);
    }
}
