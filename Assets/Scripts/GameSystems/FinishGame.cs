using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class FinishGame : MonoBehaviour
{
    ArtefactManager artefactManager; // Reference to artefact manager
    [SerializeField] GameObject cannotExitText;
    [SerializeField] LevelLoader levelLoader;
    [SerializeField] int sceneIDToLoad;

    private void Start()
    {
        artefactManager = GameObject.FindGameObjectWithTag("Player").GetComponent<ArtefactManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (artefactManager.HasAllArtefacts)
        {
            LoadEndScene();
        }
        else
        {
            StartCoroutine(ShowCannotExitText());
        }
    }

    void LoadEndScene()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        levelLoader.LoadLevel(sceneIDToLoad);
    }

    IEnumerator ShowCannotExitText()
    {
        cannotExitText.SetActive(true);

        yield return new WaitForSeconds(1f);

        cannotExitText.SetActive(false);
    }
}
