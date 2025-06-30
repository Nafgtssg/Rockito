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
    public List<Pickup> inventory;
    [Header("Dialog System")]
    public AudioSource audioSource;
    public DialogNode currentDialog;
    public bool inDialog = false;
    public GameObject dialogBox;
    public TextMeshProUGUI charName;
    public TextMeshProUGUI dialogText;
    public float charactersPerSecond = 30f;
    public AudioClip charSound;
    private bool isTyping = false;
    private bool safeDialog = false;
    private Coroutine typingRoutine;
    void Awake()
    {
        if (manager != null && manager != this) Destroy(gameObject);
        else
        {
            manager = this;
            DontDestroyOnLoad(gameObject);
        }
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) book.enabled = !book.enabled;
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return)) && inDialog && safeDialog)
            PassDialog();
    }
    public void StartDialog(DialogNode dialog)
    {
        StartCoroutine(SafeDialog());
        // Stop any ongoing typing
        if (isTyping)
        {
            StopCoroutine(typingRoutine);
            isTyping = false;
        }
        inDialog = true;
        dialogBox.SetActive(true);
        charName.text = dialog.displayName;
        currentDialog = dialog;
        // Start typing effect
        typingRoutine = StartCoroutine(TypeText(currentDialog.dialogText));
    }
    public void PassDialog()
    {
        // If text is still typing, complete it immediately
        if (isTyping)
        {
            StopCoroutine(typingRoutine);
            dialogText.text = currentDialog.dialogText;
            isTyping = false;
            safeDialog = false;
            return;
        }
        if (currentDialog.nextNode == null)
        {
            inDialog = false;
            dialogBox.SetActive(false);
        }
        else StartDialog(currentDialog.nextNode);
    }
    IEnumerator SafeDialog()
    {
        yield return new WaitForSeconds(0.25f);
        safeDialog = true;
    }
    IEnumerator TypeText(string text) {
        isTyping = true;
        dialogText.text = "";
        float delay = 1f / charactersPerSecond;

        foreach (char letter in text.ToCharArray())
        {
            dialogText.text += letter;

            // Play sound for each character
            PlayTypeSound();

            // Wait, but check for input to speed up
            float timer = 0f;
            while (timer < delay)
            {
                if (Input.GetButtonDown("Submit")) // Using "Submit" (usually Enter/Return)
                {
                    // Skip to end if player presses the submit button
                    dialogText.text = text;
                    isTyping = false;
                    yield break;
                }
                timer += Time.deltaTime;
                yield return null;
            }
        }

        isTyping = false;
    }
    void PlayTypeSound() {
        if (currentDialog.sound != null) {
            // Small pitch variation for more natural sound
            audioSource.pitch = Random.Range(1f - currentDialog.pitchVariation, 1f + currentDialog.pitchVariation);
            audioSource.PlayOneShot(currentDialog.sound);
        }
    }
}
