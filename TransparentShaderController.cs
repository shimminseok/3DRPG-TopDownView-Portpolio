using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentShaderController : MonoBehaviour
{
    public Texture uniqueTexture;  // Inspector에서 개별적으로 설정
    public Material depthMaterial;


    Material instanceMaterial;
    void Start()
    {
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain != null && brain.OutputCamera)
        {
            brain.OutputCamera.depthTextureMode = DepthTextureMode.Depth;
            Debug.Log("Depth Texture 활성화됨: Cinemachine Camera");
        }

        Renderer rend = GetComponent<Renderer>();
        if (rend != null && depthMaterial != null)
        {
            //  기존 머티리얼을 복사하여 개별 인스턴스 생성
            instanceMaterial = new Material(depthMaterial);

            //  개별 오브젝트에 맞는 텍스처 적용
            if (uniqueTexture != null)
            {
                instanceMaterial.SetTexture("_BaseMap", uniqueTexture);
            }

            // 개별 머티리얼을 오브젝트에 적용
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

        // 카메라에서 플레이어까지 Ray를 쏴서 벽이 가로막고 있는지 확인
        Ray ray = new Ray(Camera.main.transform.position, (PlayerController.Instance.transform.position - Camera.main.transform.position).normalized);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Ray가 벽에 맞으면 벽이 플레이어를 가리는 상태!
            if (hit.transform == transform)
            {

                return true;
            }
        }

        return false;
    }

}
