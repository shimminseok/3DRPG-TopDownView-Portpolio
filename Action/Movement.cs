using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Movement : MonoBehaviour
{
    public enum ViewType
    {
        ThirdPerson,
        TopDown,
    }

    public ViewType viewType;
    public float moveSpeed = 5;

    Camera mainCamera;
    
    
    void Start()
    {
        mainCamera = Camera.main;
        switch(viewType)
        {
            case ViewType.ThirdPerson:
                break;
            case ViewType.TopDown:
                break;
        }
    }

    public Vector3 ThirdPersonMovement()
    {
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;


        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveDir = (cameraForward * input.z) + (cameraRight * input.x).normalized;
        moveDir.y += -9.81f;
        Vector3 movement = moveDir * moveSpeed * Time.deltaTime;

        return movement;
    }
}
