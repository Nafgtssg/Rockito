using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;
    [Header("UI Stuff")]
    public Image book;
    public TextMeshProUGUI text;
    [Header("Inventory")]
    public List<InteractableData> inventory;
    [Header("Dialog System")]
    public TextMeshProUGUI charName;
    public TextMeshProUGUI dialog;
    void Awake()
    {
        if (manager != null && manager != this) Destroy(gameObject);
        else {
            manager = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) book.enabled = !book.enabled;
    }
}
