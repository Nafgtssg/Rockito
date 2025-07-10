using UnityEngine;
using UnityEngine.EventSystems;

public class ConceptBoxController : MonoBehaviour, IDropHandler
{
    public string boxID;
    public ConceptController currentConcept;
    
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            ConceptController concept = eventData.pointerDrag.GetComponent<ConceptController>();
            if (concept != null)
            {
                // If this box already has a concept, swap them
                if (currentConcept != null)
                {
                    currentConcept.AssignToBox(concept.currentBox);
                }
                
                concept.AssignToBox(this);
                currentConcept = concept;
            }
        }
    }

    public void RemoveConcept()
    {
        currentConcept = null;
    }
}