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
    public Animator bookAnimator;
    public bool isBookOpen = false;
    public TextMeshProUGUI text;
    public int stateBook = 0;
    public GameObject bookHints;
    public GameObject inventoryButtons;
    public GameObject menuButtons;
    [Header("Inventory")]
    public List<Pickup> inventory;
    public List<Pickup> keyItems;
    public List<Pickup> rock;
    public GameObject[] inventorySlot;
    public TextMeshProUGUI invName;
    public TextMeshProUGUI invDescription;
    public Image invImage;
    [Header("Dialog System")]
    public AudioSource audioSource;
    [SerializeField] private List<DialogState> dialogStates = new List<DialogState>();
    public DialogNode currentDialog;
    public bool inDialog = false;
    public GameObject dialogBox;
    public GameObject charName;
    public TextMeshProUGUI dialogText;
    public Image[] dialogPortrait;
    public GameObject[] choiceButtons;
    public float charactersPerSecond = 30f;
    private bool isTyping = false;
    private bool isChoice = false;
    private bool safeDialog = false;
    private Coroutine typingRoutine;
    [Header("Concept Game System")]
    public GameObject conceptGame;
    public ConceptData gameData;
    public GameObject conceptPrefab;
    public GameObject boxPrefab;
    public Transform conceptsContainer;
    public Transform boxesContainer;
    public GameObject resultsPanel;
    public TextMeshProUGUI resultsText;
    public bool isPlaying;
    [Header("Popup System")]
    public Popup currentPopup;
    public GameObject popup;
    public Animator popupAnimator;
    public GameObject popupTitle;
    public GameObject popupDescription;
    public Image popupImage;
    public RectTransform popupMaster;
    public bool inPopup;
    void Awake()
    {
        if (manager != null && manager != this) Destroy(gameObject);
        else manager = this;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (isBookOpen)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) bookAnimator.SetTrigger("turnLeft");
            if (Input.GetKeyDown(KeyCode.RightArrow)) bookAnimator.SetTrigger("turnRight");
            if (Input.GetKeyDown(KeyCode.Escape)) bookAnimator.SetTrigger("book");
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape)) OpenBook();
        }
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
            PassAction();
    }
    void OpenBook()
    {
        isBookOpen = true;
        book.SetActive(true);
        bookAnimator.SetTrigger("book");
        bookHints.SetActive(true);
        SetBookPage();
    }
    public void TurnLeft()
    {
        stateBook -= 1;
        if (stateBook < 0) stateBook += 4;
        SetBookPage();
    }
    public void TurnRight()
    {
        stateBook += 1;
        if (stateBook >= 4) stateBook -= 4;
        SetBookPage();
    }
    void SetBookPage()
    {
        switch (stateBook)
        {
            case 0:
            case 1:
            case 2:
                inventoryButtons.SetActive(true);
                menuButtons.SetActive(false);
                UpdateInventory();
                break;
            case 3:
                inventoryButtons.SetActive(false);
                menuButtons.SetActive(true);
                break;
            default:
                inventoryButtons.SetActive(false);
                menuButtons.SetActive(false);
                break;
        }
    }
    void PassAction()
    {
        if (inPopup) StartCoroutine(PassPopup());
        else if (inDialog && safeDialog && !isChoice) PassDialog();
    }
    void UpdateInventory()
    {
        // Clear all slots first
        foreach (GameObject slot in inventorySlot)
        {
            slot.GetComponent<Image>().sprite = null;
            slot.GetComponent<Button>().onClick.RemoveAllListeners();
            slot.SetActive(false);
        }
        switch (stateBook)
        {
            case 0:
                LoadInventory(inventory);
                invName.text = "Inventario";
                break;
            case 1:
                LoadInventory(keyItems);
                invName.text = "Objetos Llave";
                break;
            case 2:
                LoadInventory(rock);
                invName.text = "Rocas y Minerales";
                break;
            default: break;
        }
    }
    void LoadInventory(List<Pickup> list)
    {
        // Populate slots with items
        for (int i = 0; i < list.Count; i++)
        {
            if (i >= inventorySlot.Length) break;

            GameObject slot = inventorySlot[i];
            Pickup item = list[i];

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
        if (list.Count == 0)
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
        invName.text = stateBook switch { 0 => "Inventario", 1 => "Objetos Llave", 2 => "Rocas y Minerales", _ => "_"};
        invDescription.text = "Posa el cursor sobre un objeto para ver sus detalles.";
        invImage.sprite = null;
    }
    public void SelectItem(Pickup item)
    {
        // Handle item selection/use here
        Debug.Log("Selected: " + item.displayName);
    }

    /*************
       DIALOGO 
    *************/
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
        charName.GetComponentInChildren<TextMeshProUGUI>().text = currentDialog.displayName;
        typingRoutine = StartCoroutine(TypeText(currentDialog.dialogText));

        // Portraits come here
        if (currentDialog.leftSpeaker != null)
        {
            dialogPortrait[0].sprite = currentDialog.leftSpeaker;
            dialogPortrait[0].gameObject.SetActive(true);
        }
        else dialogPortrait[0].gameObject.SetActive(false);
        if (currentDialog.rightSpeaker != null)
        {
            dialogPortrait[1].sprite = currentDialog.rightSpeaker;
            dialogPortrait[1].gameObject.SetActive(true);
        }
        else dialogPortrait[1].gameObject.SetActive(false);

        // Effects come here
        if (currentDialog.effect != null) currentDialog.effect.Execute();

        // Choices come here
        if (currentDialog.choices.Length > 0)
        {
            isChoice = true;
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (i < currentDialog.choices.Length)
                {
                    bool isChoiceValid = currentDialog.choices[i].validator == null || currentDialog.choices[i].validator.Validate();

                    choiceButtons[i].SetActive(true);
                    choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentDialog.choices[i].choiceText;

                    Button button = choiceButtons[i].GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    int currentIndex = i;
                    if (isChoiceValid)
                        button.onClick.AddListener(() => MakeChoice(currentIndex));
                    choiceButtons[i].GetComponent<Image>().color = isChoiceValid ? Color.white : new Color(0.5f, 0.5f, 0.5f);
                }
                else choiceButtons[i].SetActive(false);
            }
        }
        else
            foreach (var button in choiceButtons)
                button.SetActive(false);
    }

    public void PassDialog()
    {
        // If text is still typing, complete it immediately
        if (isTyping)
        {
            StopCoroutine(typingRoutine);
            dialogText.text = currentDialog.dialogText;
            isTyping = false;
            return;
        }
        if (!isPlaying)
        {
            if (currentDialog.nextNode == null)
            {
                dialogBox.SetActive(false);
                dialogPortrait[0].gameObject.SetActive(false);
                dialogPortrait[1].gameObject.SetActive(false);
                StartCoroutine(SafeDialogEnd());
            }
            else StartDialog(currentDialog.nextNode);
        }
    }
    IEnumerator SafeDialogEnd()
    {
        yield return new WaitForSeconds(.1f);
        inDialog = false;
        safeDialog = false;
        isChoice = false;
        if (currentDialog.onEnding != null) currentDialog.onEnding.Execute();
    }
    public void MakeChoice(int choiceIndex)
    {
        if (choiceIndex < currentDialog.choices.Length)
        {
            isChoice = false;
            StartDialog(currentDialog.choices[choiceIndex]);
        }
    }
    IEnumerator SafeDialog()
    {
        yield return new WaitForSeconds(0.25f);
        safeDialog = true;
    }
    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogText.text = "";
        float delay = 1f / charactersPerSecond;

        foreach (char letter in text.ToCharArray())
        {
            dialogText.text += letter;

            // Play sound for ALMOST each character
            if (!(letter == ' ' || letter == '!' || letter == '?' || letter == '¡' || letter == '¿')) PlayTypeSound();

            float timer = 0f;
            while (timer < delay)
            {
                if (Input.GetButtonDown("Submit"))
                {
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
    void PlayTypeSound()
    {
        if (currentDialog.sound != null)
        {
            // Small pitch variation for more natural sound
            audioSource.pitch = Random.Range(1f - currentDialog.pitchVariation, 1f + currentDialog.pitchVariation);
            audioSource.PlayOneShot(currentDialog.sound);
        }
    }
    public int GetDialogState(string id) // Intento obtener el estado de un NPC dada su Id, en caso de no existir, retorna el primer estado y guarda al NPC
    {
        DialogState state = dialogStates.Find(s => s.id == id);
        if (state == null)
        {
            state = new DialogState { id = id, dialogState = 0 };
            dialogStates.Add(state);
        }
        return state.dialogState;
    }
    public void SetDialogState(string id, int newState) //Establezco un estado a los diálogos de un NPC dada su Id
    {
        var state = dialogStates.Find(s => s.id == id);
        if (state == null)
        {
            state = new DialogState { id = id, dialogState = newState };
            dialogStates.Add(state);
        }
        else
            state.dialogState = newState;
    }
    /***************
      CONCEPT GAME
    ***************/
    public void StartConceptGame(ConceptData data)
    {
        gameData = data;
        isPlaying = true;
        inDialog = false;
        StartCoroutine(SetupConceptGame());
    }
    IEnumerator SetupConceptGame()
    {
        yield return new WaitForSeconds(2f);
        dialogBox.SetActive(false);
        conceptGame.SetActive(true);

        // Limpio los conceptos y cajas ya existentes
        foreach (Transform child in conceptsContainer) Destroy(child.gameObject);
        foreach (Transform child in boxesContainer) Destroy(child.gameObject);

        // Se inicializa el juego
        CreateConceptsInColumns();
        CreateBoxesInColumns();
        RandomizeConceptPositions();
    }
    private void CreateConceptsInColumns() // Se crean los conceptos en columnas bien organizadas
    {
        int conceptsPerColumn = Mathf.CeilToInt((float)gameData.concepts.Length / gameData.columns);

        for (int i = 0; i < gameData.concepts.Length; i++)
        {
            // Determine which column (0 or 1)
            int column = i / conceptsPerColumn;
            int positionInColumn = i % conceptsPerColumn;
            float containerHeight = ((RectTransform)conceptsContainer).rect.height;
            float verticalSpacing = containerHeight / (conceptsPerColumn + 1);

            // Calculate position
            Vector2 position = new Vector2(
            column * gameData.horizontalOffset,
            -positionInColumn * verticalSpacing + containerHeight / 4
        );

            // Instantiate concept
            GameObject conceptObj = Instantiate(conceptPrefab, conceptsContainer);
            conceptObj.GetComponent<RectTransform>().anchoredPosition = position;

            ConceptController draggable = conceptObj.GetComponent<ConceptController>();
            draggable.Initialize(gameData.concepts[i]);
        }
    }

    private void CreateBoxesInColumns() // Se crean las cajas en columnas bien organizadas
    {
        int boxesPerColumn = Mathf.CeilToInt(gameData.boxIDs.Length / gameData.columns);

        // Shuffle box IDs for randomization
        string[] shuffledBoxIDs = ShuffleArray(gameData.boxIDs);

        for (int i = 0; i < shuffledBoxIDs.Length; i++)
        {
            // Determine which column (0 or 1)
            int column = i / boxesPerColumn;
            int positionInColumn = i % boxesPerColumn;
            float containerHeight = ((RectTransform)conceptsContainer).rect.height;
            float verticalSpacing = containerHeight / (boxesPerColumn + 1);

            // Calculate position
            Vector2 position = new Vector2(
                column * gameData.horizontalOffset,
                -positionInColumn * verticalSpacing + containerHeight / 4
            );

            // Instantiate box
            GameObject boxObj = Instantiate(boxPrefab, boxesContainer);
            boxObj.GetComponent<RectTransform>().anchoredPosition = position;

            ConceptBoxController box = boxObj.GetComponent<ConceptBoxController>();
            box.boxID = shuffledBoxIDs[i];
            box.GetComponentInChildren<TextMeshProUGUI>().text = shuffledBoxIDs[i];
        }
    }

    private void RandomizeConceptPositions() // Aleatorizar las posiciones de los conceptos
    {
        // Get all concept objects
        ConceptController[] concepts = conceptsContainer.GetComponentsInChildren<ConceptController>();

        // Fisher-Yates shuffle algorithm
        for (int i = 0; i < concepts.Length; i++)
        {
            int randomIndex = Random.Range(i, concepts.Length);
            if (i != randomIndex)
            {
                // Swap positions
                Vector3 tempPos = concepts[i].transform.localPosition;
                concepts[i].transform.localPosition = concepts[randomIndex].transform.localPosition;
                concepts[randomIndex].transform.localPosition = tempPos;
            }
        }
    }

    // Helper method to shuffle arrays
    private T[] ShuffleArray<T>(T[] array)
    {
        T[] newArray = (T[])array.Clone();
        for (int i = 0; i < newArray.Length; i++)
        {
            int randomIndex = Random.Range(i, newArray.Length);
            T temp = newArray[i];
            newArray[i] = newArray[randomIndex];
            newArray[randomIndex] = temp;
        }
        return newArray;
    }

    public void CheckResults()
    {
        int correctMatches = 0;
        int totalMatches = gameData.concepts.Length;

        foreach (var concept in gameData.concepts)
        {
            // Find all boxes that might have this concept
            ConceptBoxController[] boxes = FindObjectsOfType<ConceptBoxController>();
            foreach (var box in boxes)
            {
                if (box.currentConcept != null &&
                    box.currentConcept.conceptID == concept.conceptID &&
                    box.boxID == concept.correctBoxID)
                {
                    correctMatches++;
                    break;
                }
            }
        }

        // Show results
        resultsPanel.SetActive(true);
        resultsText.text = $"You got {correctMatches} out of {totalMatches} correct!";

        // Optional: Highlight correct/incorrect matches
        HighlightMatches();
        StartCoroutine(EndConceptGame(correctMatches == totalMatches));
    }
    IEnumerator EndConceptGame(bool perfect)
    {
        yield return new WaitForSeconds(4f);
        isPlaying = false;
        conceptGame.SetActive(false);
        if (gameData.onCorrect != null && perfect) gameData.onCorrect.Execute();
        else if (gameData.onEnding != null) gameData.onEnding.Execute();
    }

    private void HighlightMatches()
    {
        ConceptBoxController[] boxes = FindObjectsOfType<ConceptBoxController>();

        foreach (var box in boxes)
        {
            if (box.currentConcept != null)
            {
                bool isCorrect = false;

                foreach (var concept in gameData.concepts)
                {
                    if (concept.conceptID == box.currentConcept.conceptID &&
                        concept.correctBoxID == box.boxID)
                    {
                        isCorrect = true;
                        break;
                    }
                }

                // Change color based on correctness
                Image boxImage = box.GetComponent<Image>();
                boxImage.color = isCorrect ? Color.green : Color.red;
            }
        }
    }
    public void TriggerPopup(Popup data)
    {
        inPopup = true;
        popup.SetActive(true);
        if (data.title == "") popupTitle.SetActive(false);
        else
        {
            popupTitle.SetActive(true);
            popupTitle.GetComponentInChildren<TextMeshProUGUI>().text = data.title;
        }

        if (data.description == "") popupDescription.SetActive(false);
        else
        {
            popupDescription.SetActive(true);
            popupDescription.GetComponentInChildren<TextMeshProUGUI>().text = data.description;
        }

        popupImage.sprite = data.sprite;
        popupMaster.sizeDelta = data.size;

        popupAnimator.SetInteger("type", (int)data.type);
        popupAnimator.SetTrigger("popup");
        currentPopup = data;
    }
    IEnumerator PassPopup()
    {
        yield return new WaitForSeconds(.1f);
        popup.SetActive(false);
        inPopup = false;
        if (currentPopup.onEnding != null) currentPopup.onEnding.Execute();
    }
}

[System.Serializable]
public class DialogState
{
    public string id;
    public int dialogState;
}