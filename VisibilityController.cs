using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisibilityController : MonoBehaviour
{
    
    [SerializeField] Camera miniMapCam;
    [SerializeField] GameObject obj;

    float checkTimer = 0;
    void Start()
    {
        miniMapCam = Camera.allCameras.FirstOrDefault(cam => cam.name == "MinimapCam");
    }
    void Update()
    {
        checkTimer += Time.deltaTime;
        if (obj != null && checkTimer >= 0.2f)
        {
            bool isVisible = IsInView();
            if(isVisible && !obj.activeSelf)
            {
                obj.SetActive(true);
            }
            else if(!isVisible && obj.activeSelf)
            {
                obj.SetActive(false);
            }

            checkTimer = 0;
        }
    }

    bool IsInView()
    {
        Vector3 viewPort = miniMapCam.WorldToViewportPoint(transform.position);
        return viewPort.x >= -0.1f && viewPort.x <= 1.1f &&
               viewPort.y >= -0.1f && viewPort.y <= 1.1f &&
               viewPort.z >= 0;
    }
}
