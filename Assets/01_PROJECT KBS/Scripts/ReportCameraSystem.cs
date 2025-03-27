using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cinemachine;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class ReportCameraSystem : MonoBehaviour
{
    public static ReportCameraSystem Instance {get; private set;}
    public Vector3 AimingPoint => cameraAimingPoint;
    [field: SerializeField] public Camera MainCamera {get; private set;}

    [field: SerializeField] public Cinemachine.CinemachineVirtualCamera TpsCamera {get; private set;}

    [field: SerializeField] public LayerMask LayerMask {get; private set;}

    private Vector3 cameraAimingPoint;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        Ray ray = MainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000f, LayerMask , QueryTriggerInteraction.Ignore))
        {
            cameraAimingPoint = hitInfo.point;
            //추가
            if (hitInfo.collider.CompareTag("Capsule"))
            {
                Transform capsule = hitInfo.collider.transform;
                capsule.localScale *= 3f;
            }
        }
        else
        {
            cameraAimingPoint = ray.GetPoint(1000f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(MainCamera.transform.position, cameraAimingPoint);      
    }
}
