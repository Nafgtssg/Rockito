using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    public static CameraController controller;
    public Camera mainCamera;
    public Transform target;
    public bool followTarget = true;
    [Header("Position Settings")]
    public float distance = 5f;
    public float minDistance = 3f;
    public float maxDistance = 10f;
    public float height = 2f;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;
    
    [Header("Rotation Settings")]
    public float tilt = 45f; // Phi angle
    public float rotation = 0f; // Theta angle
    public float rotationSpeed = 100f;
    
    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float zoomSmoothness = 5f;
    
    void Awake()
    {
        if (controller != null && controller != this) Destroy(gameObject);
        else
        {
            controller = this;
            DontDestroyOnLoad(gameObject);
        }
        mainCamera = GetComponent<Camera>();
    }
    
    void Start()
    {
        target = PlayerController.player.transform;
        if (followTarget)
        {
            UpdateCameraPosition(1);
            transform.LookAt(target.position + offset);
        }
    }
    
    void LateUpdate()
    {
        if (target == null && followTarget) return;
        
        UpdateCameraPosition(smoothSpeed);
    }
    
    void UpdateCameraPosition(float smoothSpeed)
    {
        // Convert spherical coordinates to Cartesian
        float phi = tilt * Mathf.Deg2Rad;
        float theta = rotation * Mathf.Deg2Rad;
        
        // Calculate position based on spherical coordinates
        float x = distance * Mathf.Sin(phi) * Mathf.Sin(theta);
        float z = distance * Mathf.Sin(phi) * Mathf.Cos(theta);
        float y = distance * Mathf.Cos(phi);
        
        Vector3 desiredPosition = target.position + offset + new Vector3(x, y, z);
        
        // Smooth movement
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        if (followTarget && !(!GameManager.manager.inDialog || !GameManager.manager.inPopup ||
            !GameManager.manager.isPlaying || !GameManager.manager.isBookOpen || !GameManager.manager.isCamera || PlayerController.player.canMove)) transform.LookAt(target.position + offset);
    }
    public void SetCameraData(CameraModifier cameraModifier)
    {
        StopAllCoroutines();
        followTarget = cameraModifier.followTarget;
        mainCamera.orthographic = cameraModifier.orthographic;
        rotation = cameraModifier.addRotation;
        tilt = cameraModifier.addTilt;
        mainCamera.orthographicSize = cameraModifier.addSize;
        mainCamera.nearClipPlane = cameraModifier.addClipingPlane;
        offset = cameraModifier.addOffset;
        transform.position = cameraModifier.setPosition;
        transform.rotation = Quaternion.Euler(cameraModifier.setRotation);
        if (followTarget) StartCoroutine(StepsCameraUpdate());
    }
    public void ModifyCameraData(CameraModifier cameraModifier)
    {
        StopAllCoroutines();
        followTarget = cameraModifier.followTarget;
        mainCamera.orthographic = cameraModifier.orthographic;
        rotation += cameraModifier.addRotation;
        tilt += cameraModifier.addTilt;
        mainCamera.orthographicSize += cameraModifier.addSize;
        mainCamera.nearClipPlane += cameraModifier.addClipingPlane;
        offset += cameraModifier.addOffset;
        if (followTarget) StartCoroutine(StepsCameraUpdate());
    }
    public IEnumerator StepsCameraUpdate()
    {
        for (int i = 0; i < Mathf.Floor(1 / smoothSpeed); i++)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            transform.LookAt(target.position + offset);
        }
        yield return new WaitForSeconds(0f);
    }
}