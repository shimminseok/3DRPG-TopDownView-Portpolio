using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerBase : MonoBehaviour
{
    public Transform nameTagRoot;
    public CharacterType characterType;
    public CharacterState currentState;
    public string characterName;

    protected CharacterController characterController;

    protected virtual void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }



    protected virtual void Init()
    {
        //characterController.enabled = true;
        currentState = CharacterState.Idle;
        //ObjectPoolManager.Instance.GetObject
    }

}
