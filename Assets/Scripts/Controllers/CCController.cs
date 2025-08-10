using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCController : MonoBehaviour
{
    public static CCController controller;
    public Camera cinematic;

    void Awake()
    {
        if (controller != null && controller != this) Destroy(gameObject);
        else
        {
            controller = this;
            DontDestroyOnLoad(gameObject);
        }
        cinematic = gameObject.GetComponent<Camera>();
        cinematic.enabled = false;
    }
}
