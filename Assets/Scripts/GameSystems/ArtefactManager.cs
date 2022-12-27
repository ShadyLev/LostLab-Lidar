using UnityEngine;
using TMPro;

public class ArtefactManager : MonoBehaviour
{
    [Tooltip("Text object to display artefact amount.")]
    [SerializeField] TextMeshProUGUI artefactCountText;

    [Tooltip("Parent object of all artefacts in scene.")]
    [SerializeField] private GameObject artefactParentHolder;

    private int m_maxArtefactAmount; // Max amount of artefacts in scene
    private int m_currentArtefactAmount; // Current amount of artefacts player has collected.
    public int CurrentArtefactAmount
    {
        get { return m_currentArtefactAmount; }
        set 
        {
            if (value > m_maxArtefactAmount)
                value = m_maxArtefactAmount;

            m_currentArtefactAmount = value; 
        }
    }

    [SerializeField] bool m_hasAllArtefacts = false; // Bool that shows if player has collected all artefacts in scene.
    public bool HasAllArtefacts
    {
        get { return m_hasAllArtefacts; }
    }

    private void Start()
    {
        // Set max artefact amount based on the child count of artefact parent holder.
        m_maxArtefactAmount = artefactParentHolder.transform.childCount;
    }

    private void Update()
    {
        CheckIfHasAllArtefacts();

        DisplayCurrentArtefactAmount();
    }

    /// <summary>
    /// Changes the hasAllArtefacts bool to true if player collected all artefacts.
    /// </summary>
    void CheckIfHasAllArtefacts()
    {
        if(CurrentArtefactAmount == m_maxArtefactAmount)
            m_hasAllArtefacts = true;
    }

    /// <summary>
    /// Changes the text to current amount of collected artefacts.
    /// </summary>
    void DisplayCurrentArtefactAmount()
    {
        artefactCountText.SetText(CurrentArtefactAmount.ToString() + " / " + m_maxArtefactAmount);
    }
}
