using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentShaderController : MonoBehaviour
{
    public Texture uniqueTexture;  // Inspector���� ���������� ����
    public Material depthMaterial;


    Material instanceMaterial;
    void Start()
    {
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain != null && brain.OutputCamera)
        {
            brain.OutputCamera.depthTextureMode = DepthTextureMode.Depth;
            Debug.Log("Depth Texture Ȱ��ȭ��: Cinemachine Camera");
        }

        Renderer rend = GetComponent<Renderer>();
        if (rend != null && depthMaterial != null)
        {
            //  ���� ��Ƽ������ �����Ͽ� ���� �ν��Ͻ� ����
            instanceMaterial = new Material(depthMaterial);

            //  ���� ������Ʈ�� �´� �ؽ�ó ����
            if (uniqueTexture != null)
            {
                instanceMaterial.SetTexture("_BaseMap", uniqueTexture);
            }

            // ���� ��Ƽ������ ������Ʈ�� ����
            rend.material = instanceMaterial;
        }
    }

    private void Update()
    {
        if (instanceMaterial == null) return;

        bool isHidingPlayer = IsWallBlockingPlayer();

        float transparency = isHidingPlayer ? 0.1f : 1.0f;
        instanceMaterial.SetFloat("_Transparency", transparency);
    }

    bool IsWallBlockingPlayer()
    {
        if (Camera.main == null || PlayerController.Instance == null) return false;

        // ī�޶󿡼� �÷��̾���� Ray�� ���� ���� ���θ��� �ִ��� Ȯ��
        Ray ray = new Ray(Camera.main.transform.position, (PlayerController.Instance.transform.position - Camera.main.transform.position).normalized);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Ray�� ���� ������ ���� �÷��̾ ������ ����!
            if (hit.transform == transform)
            {

                return true;
            }
        }

        return false;
    }

}
