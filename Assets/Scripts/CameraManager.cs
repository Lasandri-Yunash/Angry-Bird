using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _idleCam;
    [SerializeField] private CinemachineVirtualCamera _followCam;


    private void Awake()
    {
        SwitchToIdleCam();
    }

   public void SwitchToIdleCam()
    {
        _idleCam.enabled = true;
        _followCam.enabled = false;
    }

    public void SwitchFollowCam(Transform followTransform)
    {
        _followCam.Follow = followTransform;

        _followCam.enabled = true;
        _idleCam.enabled = false ;
    }
}
