using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target;
    public float height = 10f;
    public float distance = 5f;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position
        Debug.Log($"({transform.rotation.eulerAngles.x}, {transform.rotation.eulerAngles.y}, {transform.rotation.eulerAngles.z})");
        Vector3 desiredPosition = target.position;
        desiredPosition.x -= (distance + offset.x) * Mathf.Sin(transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
        desiredPosition.y += height + offset.y;
        desiredPosition.z -= (distance + offset.z) * Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad);

        // Smoothly move to desired position, jprdl
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}