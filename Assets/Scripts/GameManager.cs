using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;
    [Header("UI Stuff")]
    public TextMeshProUGUI text;
    void Awake()
    {
        if (manager != null && manager != this) Destroy(gameObject);
        else {
            manager = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update() {
        
    }
}
