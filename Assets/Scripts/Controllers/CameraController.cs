using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target;
    public float height = 10f;
    public float distance = 5f;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    [Range(0.0f, 360.0f)] public float rotation = 0;
    void LateUpdate() {
        if (target == null) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            rotation -= 45;
        if (Input.GetKeyDown(KeyCode.RightArrow))
            rotation += 45;

        // Calculate desired position
        transform.eulerAngles = new Vector3(45, rotation, 0);
        Vector3 desiredPosition = target.position;
        desiredPosition.x -= (distance + offset.x) * Mathf.Sin(rotation * Mathf.Deg2Rad);
        desiredPosition.y += height + offset.y;
        desiredPosition.z -= (distance + offset.z) * Mathf.Cos(rotation * Mathf.Deg2Rad);

        // Smoothly move to desired position, jprdl
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
