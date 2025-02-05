using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionArea : MonoBehaviour
{

    CapsuleCollider detectionAreaCollider;

    private void Awake()
    {
        detectionAreaCollider = GetComponent<CapsuleCollider>();
    }


    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
        }
    }
}
