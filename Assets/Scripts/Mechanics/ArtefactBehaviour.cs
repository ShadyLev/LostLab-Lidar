using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtefactBehaviour : MonoBehaviour
{
    // Model to get mesh from
    [SerializeField] GameObject artefactModel;

    //---HIDDEN---
    TheBlackout blackout; // Reference to the blackout script
    MeshFilter meshFilter; // Ref to own mesh filter
    MeshCollider meshCol; // Ref to own mesh collider

    // Start is called before the first frame update
    void Start()
    {
        // Set variables
        blackout = GetComponent<TheBlackout>();
        meshFilter = GetComponent<MeshFilter>();
        meshCol = GetComponent<MeshCollider>();

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
            blackout.FullBlackout();
        }
    }
}
