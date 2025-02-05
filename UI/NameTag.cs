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

        // �̸�ǥ ��ġ ������Ʈ
        transform.position = target.position + offset;
                // ī�޶� ���ϵ��� ȸ��
        transform.LookAt(mainCam.transform);
        transform.Rotate(0, 180, 0); // ������ ����
    }

    public void SetName(Transform _target, string _name)
    {
        target = _target;
        nameTxt.text = _name;
    }
}
