using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    [SerializeField] CinemachineVirtualCamera mainCam, flaskCam_0, flaskCam_1;

    void Awake() => Instance = this;


    public void ChangeCamera(int type)
    {
        switch (type)
        {
            case 0:
                mainCam.Priority = 10;
                flaskCam_0.Priority = 0;
                flaskCam_1.Priority = 0;
                break;
            case 1:
                flaskCam_0.Priority = 10;
                mainCam.Priority = 0;
                flaskCam_1.Priority = 0;
                break;
            case 2:
                flaskCam_1.Priority = 10;
                mainCam.Priority = 0;
                flaskCam_0.Priority = 0;
                break;
        }
    }
}
