using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimator : CharacterAnimator
{
    void Start()
    {

    }

    void Update()
    {
        
    }

    public override void ChangeCharacterState(CharacterState _state)
    {
        base.ResetAnimatorParameters();
        base.ChangeCharacterState(_state);

    }
    public override void ResetAnimatorParameters()
    {

    }
}
