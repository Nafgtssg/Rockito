using UnityEngine;

[CreateAssetMenu(fileName = "New Concept Data", menuName = "Geodisea/Minigames/Concept for Matching")]
public class ConceptData : ScriptableObject
{
    public ConceptPair[] concepts;
    public string[] boxIDs; // IDs of all available boxes
}

[System.Serializable]
public class ConceptPair
{
    public string conceptID;
    public Sprite conceptImage;
    public string conceptText;
    public string correctBoxID;
}