using UnityEngine;

public class ArtefactBehaviour : MonoBehaviour
{
    //---SHOWN---
    [Tooltip("Model to get mesh from.")]
    [SerializeField] GameObject artefactModel;

    [Tooltip("Sound to play when the artefact is picked up.")]
    [SerializeField] AudioClip m_pickUpSound;

    //---HIDDEN---
    TheBlackout blackout; // Reference to the blackout script
    MeshFilter meshFilter; // Ref to own mesh filter
    MeshCollider meshCol; // Ref to own mesh collider
    ArtefactManager artefactManager; // Ref to artefact manager
    AudioSource audioSource; // Ref to the audio source component

    // Start is called before the first frame update
    void Start()
    {
        // Set variables
        blackout = GetComponent<TheBlackout>();
        meshFilter = GetComponent<MeshFilter>();
        meshCol = GetComponent<MeshCollider>();
        artefactManager = GameObject.FindGameObjectWithTag("Player").GetComponent<ArtefactManager>();
        audioSource = GetComponent<AudioSource>();

        // Set mesh filter and collider to artefactModel prefab
        Mesh tmpMesh = artefactModel.GetComponent<MeshFilter>().sharedMesh;
        meshFilter.sharedMesh = tmpMesh;
        meshCol.sharedMesh = tmpMesh;
    }

    private void OnTriggerEnter(Collider other)
    {
        // If player enters collider -> remove all points.
        if (other.CompareTag("Player"))
        {
            // Play sound
            PlayPickupSound();

            // Destroy all points
            blackout.FullBlackout();

            // Increment artefact amount
            artefactManager.CurrentArtefactAmount++;

            // Destroy self after delay
            Invoke("DestroySelf", 0.2f);
        }
    }

    /// <summary>
    /// Plays a selected pickup sound.
    /// </summary>
    void PlayPickupSound()
    {
        audioSource.PlayOneShot(m_pickUpSound, AudioManager.Instance.volumeSFX);
    }

    /// <summary>
    /// Destroys self.
    /// </summary>
    void DestroySelf()
    {
        Destroy(this.gameObject); 
    }
}
