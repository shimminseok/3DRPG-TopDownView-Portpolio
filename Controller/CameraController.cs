using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField] Vector3 cameraOffset;
    [SerializeField] float mouseSensitivity = 100f;
    [SerializeField] float distance;
    [SerializeField] float zoomSpeed;
    [SerializeField] float maxDistance;
    [SerializeField] float minDistance;
    [SerializeField] Transform playerBody;

    [SerializeField] CinemachineBrain brainCam;
    [SerializeField] CinemachineVirtualCamera mainCam;
    [SerializeField] CinemachineVirtualCamera dialogueCam;


    CinemachineFramingTransposer transposer;

    LayerMask obstacleLayer;

    float xRotation = 0f;
    float yRotation = 0f;


    float cinemachineTargetYaw;
    float cinemachineTargetPitch;


    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        transposer = mainCam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;

        xRotation = transform.eulerAngles.x;
        yRotation = transform.eulerAngles.y;

        obstacleLayer = 1 << LayerMask.NameToLayer("Obstacle");
    }
    void LateUpdate()
    {
        // ���콺 ������ ��ư�� ������ ���� ȸ�� ó��
        //HandleRotation();
        //HandleZoom();
        //if (Input.GetMouseButton(1))
        //{
        //    float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        //    float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //    // �÷��̾��� Y�� ȸ�� (�¿� ȸ��)
        //    yRotation += mouseX;

        //    // ī�޶��� X�� ȸ�� (���� ȸ��)
        //    xRotation -= mouseY;
        //    xRotation = Mathf.Clamp(xRotation, -90f, 90f); // ȸ�� ������ �����Ͽ� ī�޶� �������� �ʵ��� ��
        //    Debug.Log(xRotation);
        //    virtualCam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        //}
        HandleZoom();
        //mainCam.transform.localPosition = mainCam.transform.rotation * new Vector3(0, 0, -distance) + playerBody.position;
    }
    void HandleRotation()
    {
        if (!Input.GetMouseButton(1))
            return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if(Mathf.Abs(mouseX) > Mathf.Epsilon || Mathf.Abs(mouseY) > Mathf.Epsilon)
        {
            mouseX *= mouseSensitivity * Time.deltaTime;
            mouseY *= mouseSensitivity * Time.deltaTime;

            yRotation += mouseX;
            xRotation += mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            mainCam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }
    void HandleZoom()
    {
        float zoomAmount = Input.GetAxis("Mouse ScrollWheel");
        if(Mathf.Abs(zoomAmount) > Mathf.Epsilon)
        {
            distance -= zoomAmount * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance,maxDistance);
            transposer.m_CameraDistance = distance;
        }
    }

    public void ChangeDialogueCamera(bool _isChange)
    {
        brainCam.m_DefaultBlend.m_Time = _isChange ? 0.25f : 0.5f;
        dialogueCam.Priority = _isChange ? 11 : 0;
    }

}
