using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;
    [Header("Data of the World")]
    [SerializeField] private List<InteractableRecord> interactableStates = new List<InteractableRecord>();
    [SerializeField] private List<DialogState> dialogStates = new List<DialogState>();
    public List<Pickup> inventory;
    public List<Pickup> keyItems;
    public List<Pickup> rock;
    private const string SAVE_FOLDER = "Saves";
    private const string saveName = "save";
    private const string SAVE_EXTENSION = ".gaia";
    [Header("UI Stuff")]
    public GameObject book;
    public Animator bookAnimator;
    public bool isBookOpen = false;
    public TextMeshProUGUI text;
    public int stateBook = 0;
    public int stateBookIncrement = 5;
    public GameObject gameHints;
    public GameObject bookHints;
    public GameObject inventoryButtons;
    public GameObject menuButtons;
    public GameObject craftingButtons;
    public GameObject questButtons;
    [Header("Sistema de Inventario")]
    public GameObject[] inventorySlot;
    public TextMeshProUGUI invName;
    public TextMeshProUGUI invDescription;
    public Image invImage;
    public Image recievedImage;
    public TextMeshProUGUI recievedText;
    public Animator recievedAnimator;
    [Header("Sistema de Misiones")]
    public GameObject[] questSlot;
    [Header("Sistema de Crafteo")]
    public GameObject[] materialSlot;
    public TextMeshProUGUI[] materialAmount;
    public GameObject[] craftingSlots;
    public List<Recipe> recipes = new List<Recipe>();
    public Pickup wrongRecipe;
    private Dictionary<Pickup, int> craftingMaterials = new Dictionary<Pickup, int>();
    private Pickup[] selectedMaterials = new Pickup[2]; // Track the two selected materials
    private int[] materialCounts = new int[2]; // Track counts for each material
    public TextMeshProUGUI crafName;
    [Header("Sistema de Diálogo")]
    public AudioSource audioSource;
    public DialogNode currentDialog;
    public Interactable gaiaInteractable;
    public GameObject dialogBox;
    public GameObject charName;
    public GameObject dialogHints;
    public TextMeshProUGUI dialogText;
    public Image[] dialogPortrait;
    public GameObject[] choiceButtons;
    public float charactersPerSecond = 30f;
    private bool isTyping = false;
    private bool isChoice = false;
    private bool safeDialog = false;
    private Coroutine typingRoutine;
    public bool inDialog = false;
    [Header("Sistema de Conceptos")]
    public GameObject conceptGame;
    public ConceptData gameData;
    public GameObject conceptPrefab;
    public GameObject boxPrefab;
    public Transform conceptsContainer;
    public Transform boxesContainer;
    public GameObject resultsPanel;
    public GameObject resultsButton;
    public bool isPlaying;
    [Header("Sistema de Popup")]
    public Popup currentPopup;
    public GameObject popup;
    public Animator popupAnimator;
    public GameObject popupTitle;
    public GameObject popupDescription;
    public Image popupImage;
    public RectTransform popupMaster;
    public bool inPopup;
    [Header("Sistema de Cambio de Escena")]
    public string loadedScene;
    public ChangeScene newScene;
    public Animator sceneChangeAnimator;
    public bool isCamera;
    [Header("Sistema de Menú Principal")]
    public GameObject mainMenu;
    [Header("Debug")]
    public Sprite saveIcon;
    public bool stopGameLoading;
    public TextMeshProUGUI stopGameLoadingText;
    void Awake()
    {
        if (manager != null && manager != this) Destroy(gameObject);
        else
        {
            manager = this;
        }
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "TitleScreen") LoadGame();
        else mainMenu.SetActive(true);
        ClearCraftingSlots();
    }

    void Update()
    {
        if (isBookOpen)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) bookAnimator.SetTrigger("turnLeft");
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) bookAnimator.SetTrigger("turnRight");
            if (Input.GetKeyDown(KeyCode.Escape)) bookAnimator.SetTrigger("book");
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !isCamera && !isChoice && !isPlaying && !isTyping && !inDialog && !inPopup) OpenBook();
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return)) PassAction();
            if (Input.GetKeyDown(KeyCode.Q) && gaiaInteractable != null && !isCamera && !isChoice && !isPlaying && !isTyping && !inDialog && !inPopup) gaiaInteractable.Interact(); 
        }
    }

    public bool RegisterInteractable(int internalId, GameObject data, Interactable interactable)
    {
        var record = interactableStates.Find(x => x.internalId == internalId);
        if (record == null)
        {
            record = new InteractableRecord()
            {
                internalId = internalId,
                interactable = interactable,
                position = data.transform.position,
                rotation = data.transform.rotation,
                scale = data.transform.localScale,
                available = true
            };
            interactableStates.Add(record);
        }
        return record.available;
    }

    public void LoadInteractableState(int internalId, InteractableController interactableController)
    {
        var record = interactableStates.Find(x => x.internalId == internalId);
        if (record == null)
        {
            RegisterInteractable(internalId, interactableController.gameObject, interactableController.interactable);
        }
        else
        {
            interactableController.interactable = record.interactable;
            interactableController.transform.position = record.position;
            interactableController.transform.rotation = record.rotation;
            interactableController.transform.localScale = record.scale;
            interactableController.gameObject.SetActive(record.available);
        }
    }
    public void SetInteractableState(InteractableRecord state)
    {
        var record = interactableStates.Find(x => x.internalId == state.internalId);
        interactableStates.Remove(record);
        interactableStates.Add(state);
        // Actualizar el objeto, no sólo su estado
        InteractableController[] controllers = (InteractableController[])GameObject.FindObjectsOfType<InteractableController>();
        InteractableController fix = controllers.ToList().Find(x => x.internalId == state.internalId);
        if (fix != null) {
            fix.interactable = state.interactable;
            fix.transform.position = state.position;
            fix.transform.rotation = state.rotation;
            fix.transform.localScale = state.scale;
            fix.gameObject.SetActive(state.available);
        }
    }
    public void ToggleInteractableState(int id)
    {
        var record = interactableStates.Find(x => x.internalId == id);
        interactableStates.Remove(record);
        record.available = !record.available;
        interactableStates.Add(record);
    }
    public void StartGame(Effect startGame)
    {
        DeleteGame();
        startGame.Execute();
        PlayerController.player.canMove = true;
        mainMenu.SetActive(false);
    }
    public void DeleteGame()
    {
        string savePath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER, saveName + SAVE_EXTENSION);

        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log($"Save deleted: {savePath}");
        }
        interactableStates.Clear();
        dialogStates.Clear();
        inventory.Clear();
        keyItems.Clear();
        rock.Clear();
        stateBook = 0;
    }
    public void SaveGame()
    {
        // Create save directory if it doesn't exist
        string saveDir = Path.Combine(Application.persistentDataPath, SAVE_FOLDER);
        if (!Directory.Exists(saveDir))
        {
            Directory.CreateDirectory(saveDir);
        }

        // Create save data
        GameSaveData saveData = new GameSaveData
        {
            interactableStates = interactableStates,
            dialogStates = dialogStates,
            inventory = inventory,
            keyItems = keyItems,
            rock = rock,
            playerPosition = PlayerController.player.transform.position,
            playerScale = PlayerController.player.transform.localScale,
            cameraRotation = CameraController.controller.rotation,
            cameraTilt = CameraController.controller.tilt,
            cameraNearClippingPlane = CameraController.controller.mainCamera.nearClipPlane,
            cameraSize = CameraController.controller.mainCamera.orthographicSize,
            cameraOrthographic = CameraController.controller.mainCamera.orthographic,
            cameraFollowTarget = CameraController.controller.followTarget,
            cameraOffset = CameraController.controller.offset,
            currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            debugStopGameLoading = stopGameLoading,
            gameTime = Time.time
        };

        // Convert to JSON
        string jsonData = JsonUtility.ToJson(saveData, true);

        // Save to file
        string savePath = Path.Combine(saveDir, saveName + SAVE_EXTENSION);
        File.WriteAllText(savePath, jsonData);

        recievedAnimator.SetTrigger("appear");
        recievedImage.sprite = saveIcon;
        recievedText.text = "Partida Guardada";
        Debug.Log($"Game saved to: {savePath}");
    }

    public void LoadGame()
    {
        mainMenu.SetActive(false);
        string savePath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER, saveName + SAVE_EXTENSION);

        if (!File.Exists(savePath)) return;

        // Read save file
        string jsonData = File.ReadAllText(savePath);
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(jsonData);
        stopGameLoading = saveData.debugStopGameLoading;
        UpdateGameLoading(stopGameLoading);

        if (stopGameLoading) return;

        loadedScene = saveData.currentScene;
        if (loadedScene != UnityEngine.SceneManagement.SceneManager.GetActiveScene().name) LoadGameSceneChange();
        else
        {
            ApplyLoadGame();
        }

        Debug.Log($"Game loaded from: {savePath}");
    }
    public void ApplyLoadGame()
    {
        string savePath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER, saveName + SAVE_EXTENSION);
        string jsonData = File.ReadAllText(savePath);
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(jsonData);

        interactableStates = saveData.interactableStates;
        dialogStates = saveData.dialogStates;
        inventory = saveData.inventory;
        keyItems = saveData.keyItems;
        rock = saveData.rock;
        PlayerController.player.transform.position = saveData.playerPosition;
        PlayerController.player.transform.localScale = saveData.playerScale;
        CameraController.controller.rotation = saveData.cameraRotation;
        CameraController.controller.tilt = saveData.cameraTilt;
        CameraController.controller.mainCamera.nearClipPlane = saveData.cameraNearClippingPlane;
        CameraController.controller.mainCamera.orthographicSize = saveData.cameraSize;
        CameraController.controller.mainCamera.orthographic = saveData.cameraOrthographic;
        CameraController.controller.followTarget = saveData.cameraFollowTarget;
        CameraController.controller.offset = saveData.cameraOffset;
        if (saveData.cameraFollowTarget) StartCoroutine(CameraController.controller.StepsCameraUpdate());

        PlayerController.player.canMove = true;
        loadedScene = "";
    }
    public void ToggleGameLoading()
    {
        stopGameLoading = !stopGameLoading;
        UpdateGameLoading(stopGameLoading);
    }
    void UpdateGameLoading(bool stopGameLoading)
    {
        if (stopGameLoading) stopGameLoadingText.text = "Activar Cargado";
        else stopGameLoadingText.text = "Desactivar Cargado";
    }
    /* UI STUFF */
    void OpenBook()
    {
        isBookOpen = true;
        book.SetActive(true);
        bookAnimator.SetTrigger("book");
        text.gameObject.SetActive(false);
        gameHints.SetActive(false);
        bookHints.SetActive(true);
        SetBookPage();
    }
    public void TurnLeft()
    {
        stateBook -= 1;
        if (stateBook < 0) stateBook += stateBookIncrement;
        SetBookPage();
    }
    public void TurnRight()
    {
        stateBook += 1;
        if (stateBook >= stateBookIncrement) stateBook -= stateBookIncrement;
        SetBookPage();
    }
    void SetBookPage()
    {
        TextMeshProUGUI[] data = bookHints.GetComponentsInChildren<TextMeshProUGUI>();
        switch (stateBook)
        {
            case 0:
                inventoryButtons.SetActive(true);
                menuButtons.SetActive(false);
                craftingButtons.SetActive(false);
                data[1].text = "Fabricación";
                data[0].text = "Objetos Llave";
                UpdateInventory();
                break;
            case 1:
                inventoryButtons.SetActive(true);
                menuButtons.SetActive(false);
                craftingButtons.SetActive(false);
                data[1].text = "Inventario";
                data[0].text = "Minerales";
                UpdateInventory();
                break;
            case 2:
                inventoryButtons.SetActive(true);
                menuButtons.SetActive(false);
                craftingButtons.SetActive(false);
                data[1].text = "Objetos Llave";
                data[0].text = "Menú";
                UpdateInventory();
                break;
            case 3:
                inventoryButtons.SetActive(false);
                menuButtons.SetActive(true);
                craftingButtons.SetActive(false);
                data[1].text = "Minerales";
                data[0].text = "Fabricación";
                break;
            case 4:
                inventoryButtons.SetActive(false);
                menuButtons.SetActive(false);
                craftingButtons.SetActive(true);
                data[1].text = "Menú";
                data[0].text = "Inventario";
                UpdateInventory();
                break;
            default:
                inventoryButtons.SetActive(false);
                menuButtons.SetActive(false);
                craftingButtons.SetActive(false);
                data[1].text = "";
                data[0].text = "";
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
                LoadInventory(inventory, inventorySlot);
                invName.text = "Inventario";
                break;
            case 1:
                LoadInventory(keyItems, inventorySlot);
                invName.text = "Objetos Llave";
                break;
            case 2:
                LoadInventory(rock, inventorySlot);
                invName.text = "Rocas y Minerales";
                break;
            case 4:
                LoadInventory(rock, materialSlot);
                crafName.text = "";
                break;
            default: break;
        }
    }
    void LoadInventory(List<Pickup> list, GameObject[] slots)
    {
        // Populate slots with items
        for (int i = 0; i < list.Count; i++)
        {
            if (i >= slots.Length) break;

            GameObject slot = slots[i];
            Pickup item = list[i];

            // Set slot active and assign icon
            slot.SetActive(true);
            slot.GetComponent<Image>().sprite = item.icon;

            // Add hover events
            Button slotButton = slot.GetComponent<Button>();
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(() => SelectItem(item));

            // Add hover effect
            EventTrigger trigger = slot.GetComponent<EventTrigger>();
            if (trigger == null) trigger = slot.AddComponent<EventTrigger>();

            // Clear existing triggers
            trigger.triggers.Clear();

            // Add pointer enter event
            var pointerEnter = new EventTrigger.Entry();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.RemoveAllListeners();
            pointerEnter.callback.AddListener((data) => { OnHoverItem(item); });
            trigger.triggers.Add(pointerEnter);

            // Add pointer exit event
            var pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.RemoveAllListeners();
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
        crafName.text = invName.text = item.displayName;
        invDescription.text = item.description;
        invImage.sprite = item.icon;
        invImage.color = new Color(1, 1, 1, 1);
    }
    void ClearItemDisplay()
    {
        invName.text = stateBook switch { 0 => "Inventario", 1 => "Objetos Llave", 2 => "Rocas y Minerales", _ => "_" };
        invDescription.text = "Posa el cursor sobre un objeto para ver sus detalles.";
        invImage.sprite = null;
        invImage.color = new Color(1, 1, 1, 0);
    }
    public void SelectItem(Pickup item)
    {
        for (int i = 0; i < 2; i++)
        {
            if (selectedMaterials[i] == null)
            {
                // Empty slot - add material
                selectedMaterials[i] = item;
                materialCounts[i] = 1;
                materialAmount[i].text = $"{materialCounts[i]}";
                UpdateCraftingUI();
                break;
            }
            else if (selectedMaterials[i] == item)
            {
                // Existing material - increment count
                materialCounts[i]++;
                materialAmount[i].text = $"{materialCounts[i]}";
                UpdateCraftingUI();
                break;
            }
        }
    }

    public void ClearCraftingSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < 2)
        {
            selectedMaterials[slotIndex] = null;
            materialCounts[slotIndex] = 0;
            UpdateCraftingUI();
        }
    }

    void UpdateCraftingUI()
    {
        // Update material slots display
        for (int i = 0; i < 2; i++)
        {
            if (selectedMaterials[i] != null)
            {
                craftingSlots[i].SetActive(true);
                craftingSlots[i].GetComponent<Image>().sprite = selectedMaterials[i].icon;
            }
            else
                craftingSlots[i].SetActive(false);
        }

        // Check for valid recipe
        CheckRecipes();
    }

    void CheckRecipes()
    {
        craftingSlots[2].SetActive(false); // Hide result slot initially

        if (selectedMaterials[0] == null || selectedMaterials[1] == null)
            return;

        bool matchesRecipe = false;
        foreach (Recipe recipe in recipes)
        {
            // Check if materials match recipe (order doesn't matter)
            if ((recipe.first.pickup == selectedMaterials[0] && recipe.second.pickup == selectedMaterials[1]) ||
                (recipe.first.pickup == selectedMaterials[1] && recipe.second.pickup == selectedMaterials[0]))
            {
                // Check if we have enough materials
                int count1 = selectedMaterials[0] == recipe.first.pickup ? materialCounts[0] : materialCounts[1];
                int count2 = selectedMaterials[1] == recipe.second.pickup ? materialCounts[1] : materialCounts[0];

                if (count1 == recipe.first.amount && count2 == recipe.second.amount)
                {
                    matchesRecipe = true;
                }
            }

            if (matchesRecipe)
            {
                // Show result
                craftingSlots[2].SetActive(true);
                craftingSlots[2].GetComponent<Image>().sprite = recipe.result.icon;

                // Add click event to craft the item
                Button resultButton = craftingSlots[2].GetComponent<Button>();
                resultButton.onClick.RemoveAllListeners();
                resultButton.onClick.AddListener(() => CraftItem(recipe.result));
                return;
            }
        }
        if (!matchesRecipe)
        {
            craftingSlots[2].SetActive(true);
            craftingSlots[2].GetComponent<Image>().sprite = wrongRecipe.icon;

            Button resultButton = craftingSlots[2].GetComponent<Button>();
            resultButton.onClick.RemoveAllListeners();
            resultButton.onClick.AddListener(() => CraftItem(wrongRecipe));
        }
    }

    public void CraftItem(Pickup result)
    {
        // Add result to inventory
        result.Interact();

        // Clear crafting slots
        ClearCraftingSlots();

        // Update UI
        LoadInventory(rock, materialSlot);
    }

    void ClearCraftingSlots()
    {
        for (int i = 0; i < 2; i++)
        {
            selectedMaterials[i] = null;
            materialCounts[i] = 0;
            craftingSlots[i].SetActive(false);
        }
        craftingSlots[2].SetActive(false);
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
        if (currentDialog.displayName == "") charName.SetActive(false);
        else
        {
            charName.SetActive(true);
            charName.GetComponentInChildren<TextMeshProUGUI>().text = currentDialog.displayName;
        }
        typingRoutine = StartCoroutine(TypeText(currentDialog.dialogText));
        dialogHints.GetComponentInChildren<TextMeshProUGUI>().text = "Saltar texto";

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
            dialogHints.GetComponentInChildren<TextMeshProUGUI>().text = "Siguiente diálogo";
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
        dialogHints.GetComponentInChildren<TextMeshProUGUI>().text = "Siguiente diálogo";
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

        dialogBox.SetActive(false);
        conceptGame.SetActive(true);
        resultsPanel.SetActive(true);
        resultsButton.SetActive(true);

        // Limpio los conceptos y cajas ya existentes
        foreach (Transform child in conceptsContainer) Destroy(child.gameObject);
        foreach (Transform child in boxesContainer) Destroy(child.gameObject);

        // Se inicializa el juego
        resultsPanel.GetComponentInChildren<TextMeshProUGUI>().text = "";
        CreateConceptsInColumns();
        CreateBoxesInColumns();
        if (gameData.randomizeConcepts) RandomizeConceptPositions();
    }
    private void CreateConceptsInColumns()
    {
        RectTransform containerRect = conceptsContainer as RectTransform;
        float containerHeight = containerRect.rect.height;
        float containerWidth = containerRect.rect.width;
        
        float conceptHeight = gameData.size.y;
        float verticalSpacing = 8f;
        float totalHeightPerConcept = conceptHeight + verticalSpacing;
        int maxConceptsPerColumn = Mathf.FloorToInt(containerHeight / totalHeightPerConcept);
        maxConceptsPerColumn = Mathf.Max(1, maxConceptsPerColumn);
        int columns = Mathf.CeilToInt((float)gameData.concepts.Length / maxConceptsPerColumn);
        columns = Mathf.Max(1, columns);
        float conceptWidth = gameData.size.x;
        float horizontalSpacing = 8f;
        float totalWidthPerColumn = conceptWidth + horizontalSpacing;
        float startX = -(columns - 1) * totalWidthPerColumn * 0.5f;
        
        for (int i = 0; i < gameData.concepts.Length; i++)
        {
            int column = i / maxConceptsPerColumn;
            int positionInColumn = i % maxConceptsPerColumn;
            
            // Calculate position
            float xPos = startX + column * totalWidthPerColumn;
            float yPos = (containerHeight * 0.5f) - (positionInColumn * totalHeightPerConcept) - (conceptHeight * 0.5f);

            // Instantiate concept
            GameObject conceptObj = Instantiate(conceptPrefab, conceptsContainer);
            RectTransform conceptRect = conceptObj.GetComponent<RectTransform>();
            conceptRect.anchoredPosition = new Vector2(xPos, yPos);
            conceptRect.sizeDelta = gameData.size;
     
            ConceptController draggable = conceptObj.GetComponent<ConceptController>();
            draggable.Initialize(gameData.concepts[i]);
            draggable.OnPointerEnterEvent += OnHoverConcept;
            draggable.OnPointerExitEvent += ClearConceptDisplay;
        }
    }
    void OnHoverConcept(string description) =>
    resultsPanel.GetComponentInChildren<TextMeshProUGUI>().text =
    description;
    void ClearConceptDisplay() =>
    resultsPanel.GetComponentInChildren<TextMeshProUGUI>().text =
    "";

    private void CreateBoxesInColumns()
    {
        if (gameData.boxesBackground != null)
            boxesContainer.GetComponent<Image>().sprite = gameData.boxesBackground;
        else boxesContainer.GetComponent<Image>().sprite = null;
        RectTransform containerRect = boxesContainer as RectTransform;
        float containerHeight = containerRect.rect.height;
        float containerWidth = containerRect.rect.width;
        
        float boxHeight = gameData.size.y + 8;
        float verticalSpacing = 16f;
        float totalHeightPerBox = boxHeight + verticalSpacing;
        int maxBoxPerColumn = Mathf.FloorToInt(containerHeight / totalHeightPerBox);
        maxBoxPerColumn = Mathf.Max(1, maxBoxPerColumn);
        int columns = Mathf.CeilToInt((float)gameData.concepts.Length / maxBoxPerColumn);
        columns = Mathf.Max(1, columns);
        float boxWidth = gameData.size.x + 8;
        float horizontalSpacing = 16f;
        float totalWidthPerColumn = boxWidth + horizontalSpacing;
        float startX = -(columns - 1) * totalWidthPerColumn * 0.5f;
        
        string[] shuffledBoxIDs = gameData.randomizeBoxes ? ShuffleArray(gameData.boxIDs) : gameData.boxIDs;
        for (int i = 0; i < gameData.concepts.Length; i++)
        {
            int column = i / maxBoxPerColumn;
            int positionInColumn = i % maxBoxPerColumn;
            
            // Calculate position
            float xPos = startX + column * totalWidthPerColumn;
            float yPos = (containerHeight * 0.5f) - (positionInColumn * totalHeightPerBox) - (boxHeight * 0.5f);
            
            // Instantiate concept
            GameObject boxObj = Instantiate(boxPrefab, boxesContainer);
            RectTransform boxRect = boxObj.GetComponent<RectTransform>();
            boxRect.anchoredPosition = new Vector2(xPos, yPos);
            boxRect.sizeDelta = gameData.size + new Vector2(8, 8);
     
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
        resultsButton.SetActive(false);
        resultsPanel.GetComponentInChildren<TextMeshProUGUI>().text = $"¡Has obtenido {correctMatches} correctas de {totalMatches}!";

        // Optional: Highlight correct/incorrect matches
        HighlightMatches();
        StartCoroutine(EndConceptGame(correctMatches == totalMatches));
    }
    IEnumerator EndConceptGame(bool perfect)
    {
        yield return new WaitForSeconds(4f);
        isPlaying = false;
        inDialog = false;
        resultsPanel.GetComponentInChildren<TextMeshProUGUI>().text = "";
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
        popupAnimator.SetTrigger("popup");
        yield return new WaitForSeconds(.1f);
        inPopup = false;
        popupTitle.SetActive(false);
        popupDescription.SetActive(false);
        if (currentPopup.onEnding != null) currentPopup.onEnding.Execute();
    }

    public void TriggerSceneChange(ChangeScene change)
    {
        if (isBookOpen)
        {
            isBookOpen = !isBookOpen;
            bookAnimator.SetTrigger("book");
        }
        isCamera = true;
        PlayerController.player.rb.useGravity = false;
        newScene = change;
        sceneChangeAnimator.gameObject.SetActive(true);
        sceneChangeAnimator.SetTrigger("change");
    }
    void LoadGameSceneChange()
    {
        isCamera = true;
        PlayerController.player.rb.useGravity = false;
        sceneChangeAnimator.gameObject.SetActive(true);
        sceneChangeAnimator.SetTrigger("change");
    }
    public void PassSceneChange()
    {
        if (loadedScene != "")
        {
            SceneManager.LoadScene(loadedScene);
            ApplyLoadGame();
        }
        else
        {
            PlayerController.player.transform.position = newScene.playerNewPos;
            PlayerController.player.transform.localScale = newScene.playerNewLocalScale;
            PlayerController.player.transform.rotation = Quaternion.Euler(newScene.playerNewRot);
            if (newScene.duringChange != null) newScene.duringChange.Execute();
            SceneManager.LoadScene(newScene.sceneName);
        }
        StartCoroutine(WaitUntilSceneChanges());
    }
    IEnumerator WaitUntilSceneChanges()
    {
        if (newScene != null)
        {
            for (; !SceneManager.GetSceneByName(newScene.sceneName).isLoaded;)
            {
                yield return new WaitForSeconds(0.1f);
                Debug.Log($"Still loading {!SceneManager.GetSceneByName(newScene.sceneName).isLoaded}");
            }
        }
        else
        {
            for (; SceneManager.GetSceneByName(loadedScene).isLoaded;)
            {
                yield return new WaitForSeconds(0.1f);
                Debug.Log($"Still loading {SceneManager.GetSceneByName(loadedScene).isLoaded}");
            }
        }
        yield return new WaitForSeconds(0.25f);
        sceneChangeAnimator.SetTrigger("proceed");
    }
    public void EndSceneChange()
    {
        text.text = "";
        isCamera = false;
        PlayerController.player.rb.useGravity = true;
        sceneChangeAnimator.gameObject.SetActive(false);
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (sceneName == "TitleScreen")
        {
            PlayerController.player.canMove = false;
            mainMenu.SetActive(true);
            gameHints.SetActive(false);
        }
        else if (sceneName == "DarkStart") gameHints.SetActive(false);
        else gameHints.SetActive(true);
    }
    public void TriggerCinematicMode(CinematicModifier modifier = null)
    {
        if (isBookOpen)
        {
            isBookOpen = !isBookOpen;
            bookAnimator.SetTrigger("book");
        }
        if (isCamera)
        {
            PlayerController.player.rb.useGravity = true;
            sceneChangeAnimator.SetTrigger("proceed");
        }
        else
        {
            isCamera = true;
            PlayerController.player.rb.useGravity = false;
            sceneChangeAnimator.gameObject.SetActive(true);
            sceneChangeAnimator.SetTrigger("cinema");
            sceneChangeAnimator.SetBool("finishCinematic", false);
        }
        if (modifier != null)
        {
            StartCoroutine(SetCinematicMode(modifier));
        }
    }
    IEnumerator SetCinematicMode(CinematicModifier modifier)
    {
        yield return new WaitForSeconds(1.4f);
        CCController.controller.transform.position = modifier.camPos;
        CCController.controller.transform.rotation = Quaternion.Euler(modifier.camRot);
        CCController.controller.cinematic.fieldOfView = modifier.camFOV;
    }
    public void StartCinematicMode()
    {
        CCController.controller.cinematic.enabled = true;
        CameraController.controller.mainCamera.enabled = false;
    }
    public void FinishCinematicMode()
    {
        sceneChangeAnimator.SetBool("finishCinematic", true);
        sceneChangeAnimator.SetTrigger("proceed");
    }
    public void EndCinematicMode()
    {
        CameraController.controller.mainCamera.enabled = true;
        CCController.controller.cinematic.enabled = false;
        PlayerController.player.rb.useGravity = true;
        PlayerController.player.canMove = true;
        isCamera = false;
    }
    public void TriggerEffectDelay(float delayInSeconds, Effect effect)
    {
        if (effect == null) return;
        StartCoroutine(EffectWithDelay(delayInSeconds, effect));
    }
    IEnumerator EffectWithDelay(float delayInSeconds, Effect effect)
    {
        yield return new WaitForSeconds(delayInSeconds);
        effect.Execute();
    }
    public bool CanMove()
    {
        return !(isBookOpen || isCamera || isChoice || isPlaying || isTyping || inDialog || inPopup);
    }
}

[System.Serializable]
public class DialogState
{
    public string id;
    public int dialogState;
}

[System.Serializable]
public class InteractableRecord
{
    public int internalId;
    public Interactable interactable;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public bool available;
}

[System.Serializable]
public class GameSaveData
{
    public List<InteractableRecord> interactableStates = new List<InteractableRecord>();
    public List<DialogState> dialogStates = new List<DialogState>();
    public List<Pickup> inventory = new List<Pickup>();
    public List<Pickup> rock = new List<Pickup>();
    public List<Pickup> keyItems = new List<Pickup>();
    public Vector3 playerPosition;
    public Vector3 playerScale;
    public float cameraRotation;
    public float cameraTilt;
    public float cameraNearClippingPlane;
    public float cameraSize;
    public bool cameraOrthographic;
    public bool cameraFollowTarget;
    public Vector3 cameraOffset;
    public string currentScene;
    public bool debugStopGameLoading;
    public float gameTime;
}