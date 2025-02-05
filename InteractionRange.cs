using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CapsuleCollider))]
public class InteractionRange : MonoBehaviour
{

    IInteractable interactable;
    void Awake()
    {
        interactable = GetComponentInParent<IInteractable>();
    }
    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
