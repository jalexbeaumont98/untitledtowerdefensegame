using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{

    [Header("References")]
    [SerializeField] Camera mainCamera;

    public float speedCalc = 1 / 24f;
    public float dragSpeed = 5.0f;         // Speed at which the camera will move
    public float zoomSpeed = 2.0f;         // Speed at which the camera will zoom
    public float minZoom = 20.0f;          // Minimum field of view for zooming in
    public float maxZoom = 60.0f;          // Maximum field of view for zooming out
    public Vector2 minCameraPos;           // Minimum X and Y coordinates the camera can move to
    public Vector2 maxCameraPos;           // Maximum X and Y coordinates the camera can move to


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        UpdateCameraBounds();
    }

    public void PanCamera(Vector3 newPos)
    {
        
        //newPos.x = Mathf.Clamp(newPos.x, minCameraPos.x, maxCameraPos.x);
        //newPos.y = Mathf.Clamp(newPos.y, minCameraPos.y, maxCameraPos.y);
        newPos.z = mainCamera.transform.position.z;

        mainCamera.transform.position = newPos;
    }

    
    public void HandleZooming(float scrollData)
    {
        
        mainCamera.fieldOfView -= scrollData * zoomSpeed;
        mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, minZoom, maxZoom);

        UpdateCameraBounds();

        dragSpeed = speedCalc * mainCamera.fieldOfView;
    }

    void UpdateCameraBounds()
    {
        float height = 2f * mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;

        minCameraPos.x = -width / 2;
        maxCameraPos.x = width / 2;
        minCameraPos.y = -height / 2;
        maxCameraPos.y = height / 2;
    }
}
