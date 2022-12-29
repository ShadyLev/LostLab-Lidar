using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    [SerializeField] LevelLoader levelLoader;
    [SerializeField] int sceneToLoadIndex;

    [SerializeField] private BoxCollider boxCollider;

    private void OnTriggerEnter(Collider other)
    {
        KillPlayer();
    }

    void KillPlayer()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        levelLoader.LoadLevel(sceneToLoadIndex);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(transform.position, boxCollider.size);
    }
}
