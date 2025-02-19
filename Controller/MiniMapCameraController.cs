using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCameraController : MonoBehaviour
{
    [SerializeField] Transform PlayerTarans;
    float y;
    void Start()
    {
        y = transform.position.y;
    }

    private void LateUpdate()
    {
        Vector3 newPos = PlayerTarans.position;
        newPos.y = y;
        //transform.LookAt(PlayerController.Instance.transform);
        transform.position = newPos;
    }
}
