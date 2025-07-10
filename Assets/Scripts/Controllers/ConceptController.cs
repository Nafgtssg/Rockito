using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Image))]
public class ConceptController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string conceptID;
    public Image conceptImage;
    public TextMeshProUGUI conceptText;
    
    public Transform originalParent;
    public CanvasGroup canvasGroup;
    public ConceptBoxController currentBox;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void Initialize(ConceptPair conceptData)
    {
        conceptID = conceptData.conceptID;
        conceptImage.sprite = conceptData.conceptImage;
        conceptText.text = conceptData.conceptText;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
        
        // Remove from current box if assigned
        if (currentBox != null)
        {
            currentBox.RemoveConcept();
            currentBox = null;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        
        // If not dropped on a box, return to original position
        if (currentBox == null)
        {
            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
        }
    }

    public void AssignToBox(ConceptBoxController box)
    {
        currentBox = box;
        transform.SetParent(box.transform);
        transform.localPosition = Vector3.zero;
    }
}