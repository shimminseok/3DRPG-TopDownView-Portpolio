using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NameTag : MonoBehaviour
{
    public Transform target;
    public TextMeshProUGUI nameTxt;
    public Vector3 offset;

    Camera mainCam;
    void Start()
    {
        mainCam = Camera.main;
    }
    private void LateUpdate()
    {
        if (target == null) return;

        // 이름표 위치 업데이트
        transform.position = target.position + offset;
                // 카메라를 향하도록 회전
        transform.LookAt(mainCam.transform);
        transform.Rotate(0, 180, 0); // 뒤집힘 방지
    }

    public void SetName(Transform _target, string _name)
    {
        target = _target;
        nameTxt.text = _name;
    }
}
