using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class WeaponZoom : MonoBehaviour
{
    [SerializeField] RigidbodyFirstPersonController fpsController;
    [SerializeField] float standardFov = 60.0f;
    [SerializeField] float zoomedFov = 20.0f;
    [SerializeField] float standardSensitivity = 2.0f;
    [SerializeField] float zoomedSensitivity = 0.25f;

    Camera camera;
    bool isZoomed;

    void Start()
    {
        camera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (isZoomed == true)
            {
                ZoomOut();
            }
            else if (isZoomed == false)
            {
                ZoomIn();
            }
        }
    }

    private void ZoomOut()
    {
        isZoomed = false;
        camera.fieldOfView = standardFov;
        fpsController.mouseLook.XSensitivity = standardSensitivity;
        fpsController.mouseLook.YSensitivity = standardSensitivity;
    }

    private void ZoomIn()
    {
        isZoomed = true;
        camera.fieldOfView = zoomedFov;
        fpsController.mouseLook.XSensitivity = zoomedSensitivity;
        fpsController.mouseLook.YSensitivity = zoomedSensitivity;
    }

    private void OnDisable()
    {
        ZoomOut();
    }
}
