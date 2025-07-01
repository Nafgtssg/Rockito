using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;
    [Header("UI Stuff")]
    public GameObject book;
    private bool isBookOpen = false;
    public TextMeshProUGUI text;
    [Header("Inventory")]
    public List<Pickup> inventory;
    public GameObject[] inventorySlot;
    public TextMeshProUGUI invName;
    public TextMeshProUGUI invDescription;
    public Image invImage;
    [Header("Dialog System")]
    public AudioSource audioSource;
    public DialogNode currentDialog;
    public bool inDialog = false;
    public GameObject dialogBox;
    public TextMeshProUGUI charName;
    public TextMeshProUGUI dialogText;
    public GameObject[] choiceButtons;
    public float charactersPerSecond = 30f;
    private bool isTyping = false;
    private bool isChoice = false;
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isBookOpen = !isBookOpen;
            book.SetActive(isBookOpen);
            if (isBookOpen) UpdateInventory();
        }
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return)) && inDialog && safeDialog && !isChoice)
                PassDialog();
    }
    void UpdateInventory() {
        // Clear all slots first
        foreach (GameObject slot in inventorySlot)
        {
            slot.GetComponent<Image>().sprite = null;
            slot.GetComponent<Button>().onClick.RemoveAllListeners();
            slot.SetActive(false);
        }

        // Populate slots with items
        for (int i = 0; i < inventory.Count; i++)
        {
            if (i >= inventorySlot.Length) break;

            GameObject slot = inventorySlot[i];
            Pickup item = inventory[i];

            // Set slot active and assign icon
            slot.SetActive(true);
            slot.GetComponent<Image>().sprite = item.icon;

            // Add hover events
            Button slotButton = slot.GetComponent<Button>();
            slotButton.onClick.AddListener(() => SelectItem(item));

            // Add hover effect
            EventTrigger trigger = slot.GetComponent<EventTrigger>();
            if (trigger == null) trigger = slot.AddComponent<EventTrigger>();

            // Clear existing triggers
            trigger.triggers.Clear();

            // Add pointer enter event
            var pointerEnter = new EventTrigger.Entry();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((data) => { OnHoverItem(item); });
            trigger.triggers.Add(pointerEnter);

            // Add pointer exit event
            var pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => { ClearItemDisplay(); });
            trigger.triggers.Add(pointerExit);
        }

        // Clear details if inventory is empty
        if (inventory.Count == 0)
        {
            ClearItemDisplay();
        }
    }
    void OnHoverItem(Pickup item)
    {
        invName.text = item.displayName;
        invDescription.text = item.description;
        invImage.sprite = item.icon;
    }
    void ClearItemDisplay()
    {
        invName.text = "";
        invDescription.text = "Posa el cursor sobre un objeto para ver sus detalles.";
        invImage.sprite = null;
    }
    public void SelectItem(Pickup item)
    {
        // Handle item selection/use here
        Debug.Log("Selected: " + item.displayName);
    }
    /* DIALOGO */
    public void StartDialog(DialogNode dialog)
    {
        currentDialog = dialog;
        StartCoroutine(SafeDialog());
        if (isTyping)
        {
            StopCoroutine(typingRoutine);
            isTyping = false;
        }
        inDialog = true;
        dialogBox.SetActive(true);
        charName.text = currentDialog.displayName;
        typingRoutine = StartCoroutine(TypeText(currentDialog.dialogText));
        if (currentDialog.hasChoices && currentDialog.choices.Length > 0)
        {
            isChoice = true;
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (i < currentDialog.choices.Length)
                {
                    choiceButtons[i].SetActive(true);
                    choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentDialog.choices[i].choiceText;

                    Button button = choiceButtons[i].GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    int currentIndex = i;
                    button.onClick.AddListener(() => MakeChoice(currentIndex));
                }
                else
                    choiceButtons[i].SetActive(false);
            }
        }
        else
            foreach (var button in choiceButtons)
                button.SetActive(false);
    }
    public void PassDialog() {
        // If text is still typing, complete it immediately
        if (isTyping) {
            StopCoroutine(typingRoutine);
            dialogText.text = currentDialog.dialogText;
            isTyping = false;
            return;
        }
        if (currentDialog.nextNode == null) {
            inDialog = false;
            safeDialog = false;
            isChoice = false;
            dialogBox.SetActive(false);
        }
        else StartDialog(currentDialog.nextNode);
    }
    public void MakeChoice(int choiceIndex) {
        if (currentDialog.hasChoices && choiceIndex < currentDialog.choices.Length) {
            isChoice = false;
            StartDialog(currentDialog.choices[choiceIndex].nextNode);
        }
    }
    IEnumerator SafeDialog() {
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

            // Play sound for ALMOST each character
            if (!(letter == ' ' || letter == '!' || letter == '?' || letter == '¡' || letter == '¿')) PlayTypeSound();

            float timer = 0f;
            while (timer < delay) {
                if (Input.GetButtonDown("Submit")) {
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
