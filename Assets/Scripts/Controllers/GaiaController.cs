using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaiaController : MonoBehaviour
{
    public GaiaController gaia;
    [Header("Follow Settings")]
    public Transform player; // Assign the player's transform in inspector
    public float followDistance = 2f; // How close to stay to player
    public float smoothSpeed = 5f; // How smoothly the companion follows
    public float heightOffset = 0.5f; // For floating effect (like Navi)

    [Header("Orbit Settings")]
    public bool orbitPlayer = true; // Whether to circle around player
    public float orbitSpeed = 1f; // How fast to orbit
    public float orbitDistanceVariation = 0.5f; // How much orbit distance varies

    private Vector3 targetPosition;
    private float currentAngle;
    private float randomOrbitOffset;

    void Awake()
    {
        if (gaia != null && gaia != this) Destroy(gameObject);
        else
        {
            gaia = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        // Initialize random orbit offset for variation
        player = PlayerController.player.transform;
        randomOrbitOffset = Random.Range(0f, 360f);
    }

    void Update()
    {
        if (player == null) return;

        if (orbitPlayer)
        {
            // Calculate orbiting position
            currentAngle += orbitSpeed * Time.deltaTime;
            float orbitDistance = followDistance + Mathf.Sin(Time.time + randomOrbitOffset) * orbitDistanceVariation;
            
            // Calculate target position in a circle around player
            Vector3 orbitOffset = new Vector3(
                Mathf.Cos(currentAngle) * orbitDistance,
                heightOffset,
                Mathf.Sin(currentAngle) * orbitDistance
            );
            
            targetPosition = player.position + orbitOffset;
        }
        else
        {
            // Simple follow behind logic
            Vector3 directionToPlayer = (transform.position - player.position).normalized;
            targetPosition = player.position + directionToPlayer * followDistance;
            targetPosition.y += heightOffset;
        }

        // Smooth movement towards target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // Make the companion face the player
        transform.LookAt(player.position);
    }
}
