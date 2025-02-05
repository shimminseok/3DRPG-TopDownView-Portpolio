using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    [SerializeField] CharacterAnimator animator;
    public bool IsStun { get; private set; }

    float statBoostValue = 0;
    public void SetStun(bool _isStunned)
    {
        IsStun = _isStunned;
        if (IsStun)
            animator.ChangeCharacterState(CharacterState.Stun);
        else
            animator.ResetAnimatorParameters();
        Debug.LogWarning(IsStun ? "스턴" : "스턴 해제");
    }

    public void ApplyStatBoost(StatType _type, float _value)
    {
        statBoostValue += _value;
        string signSymbol = _value > 0 ? "+" : "-";
        Debug.LogWarning($"{_type} : {signSymbol}{_value}");
    }

    public void RemoveStatBoost(StatType _type, float _value)
    {
        statBoostValue -= _value;
        string signSymbol = _value > 0 ? "-" : "+";

        Debug.LogWarning($"{_type}  : {signSymbol}{_value}");
    }

    public void ResetStatus()
    {
        IsStun = false;
        statBoostValue = 0;
        Debug.Log("모든 버프 / 디버프가 초기화 되었습니다.");
    }
}
