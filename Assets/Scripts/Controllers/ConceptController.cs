using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Image))]
public class ConceptController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string conceptID;
    public Image conceptImage;
    public TextMeshProUGUI conceptText;
    public string conceptDescription;
    private Transform originalParent;
    private Vector3 originalPosition;
    private CanvasGroup canvasGroup;
    public ConceptBoxController currentBox;
    private RectTransform rectTransform;
    public System.Action<string> OnPointerEnterEvent;
    public System.Action OnPointerExitEvent;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void Initialize(ConceptPair conceptData)
    {
        conceptID = conceptData.conceptID;
        conceptDescription = conceptData.description;
        conceptImage.sprite = conceptData.conceptImage;
        conceptText.text = conceptData.conceptText;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
        
        // Bring to front while dragging
        transform.SetAsLastSibling();
        
        // Remove from current box if assigned
        if (currentBox != null)
        {
            currentBox.RemoveConcept();
            currentBox = null;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Convert screen position to local position within parent
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform, 
            eventData.position, 
            eventData.pressEventCamera, 
            out Vector2 localPos);
            
        rectTransform.anchoredPosition = localPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        // If not dropped on a box, stay where it was dropped
        if (currentBox == null)
        {
            // Keep the current position but ensure it stays within bounds
            ClampToContainer();
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterEvent?.Invoke(conceptDescription);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitEvent?.Invoke();
    }
    private void ClampToContainer()
    {
        // Get the container's bounds
        RectTransform container = originalParent as RectTransform;
        Vector3[] containerCorners = new Vector3[4];
        container.GetWorldCorners(containerCorners);
        
        // Get the concept's bounds
        Vector3[] conceptCorners = new Vector3[4];
        rectTransform.GetWorldCorners(conceptCorners);
        
        // Calculate min/max positions
        float minX = containerCorners[0].x + rectTransform.rect.width/2;
        float maxX = containerCorners[2].x - rectTransform.rect.width/2;
        float minY = containerCorners[0].y + rectTransform.rect.height/2;
        float maxY = containerCorners[2].y - rectTransform.rect.height/2;
        
        // Clamp position
        Vector3 worldPos = rectTransform.position;
        worldPos.x = Mathf.Clamp(worldPos.x, minX, maxX);
        worldPos.y = Mathf.Clamp(worldPos.y, minY, maxY);
        rectTransform.position = worldPos;
    }
    public void AssignToBox(ConceptBoxController box)
    {
        currentBox = box;
        transform.SetParent(box.transform);
        rectTransform.anchoredPosition = Vector2.zero;
    }
}