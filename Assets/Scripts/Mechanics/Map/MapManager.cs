using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("GameObject references")]
    [Tooltip("Canvas with controls.")]
    [SerializeField] GameObject controlsCanvas;
    [SerializeField] TogglePauseMenu pauseMenu;

    [Header("HDRP Volumes")]
    [SerializeField] GameObject normalVolume;
    [SerializeField] GameObject menuVolume;

    [Header("Map values.")]
    [Tooltip("Key to open the map.")]
    [SerializeField] KeyCode openKey = KeyCode.Tab;
    [Tooltip("Is game paused.")]
    [SerializeField] bool isPaused = false;
    public bool IsMapOpened { get { return isPaused; } }


    //----HIDDEN VALUES----
    private MapCameraController camMapController; // Map camera controller script
    private Camera mapCam; // Map camera
    private PlayerInput playerInput; // Player input script


    // Start is called before the first frame update
    void Start()
    {
        // Assign references 
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        mapCam = GetComponentInChildren<Camera>();
        camMapController = GetComponent<MapCameraController>();

        // Disable map camera controller and map camera
        camMapController.enabled = false;
        mapCam.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // If pause menu is opened do not progress.
        if (pauseMenu.IsPauseMenuOpened)
            return;

        if (Input.GetKeyDown(openKey))
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                // Pause time
                Time.timeScale = 0;

                camMapController.enabled = true; // Enable map camera controller
                playerInput.enabled = false; // Disable player input
                mapCam.enabled = true; // Enable map camera
                controlsCanvas.SetActive(true); // Enable controls canvas

                // HDRP Volume swap
                normalVolume.SetActive(false); // Disable normal volume
                menuVolume.SetActive(true); // Enable menu volume

                // Show and unlock player cursor
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                // Unpause time
                Time.timeScale = 1;

                camMapController.enabled = false; // Disable map camera controller
                playerInput.enabled = true; // Enable player input
                mapCam.enabled = false; // Disable map camera
                controlsCanvas.SetActive(false); // Disable controls canvas

                // HDRP Volume swap
                normalVolume.SetActive(true); // Disable normal volume
                menuVolume.SetActive(false); // Enable normal volume

                // Hide and lock player cursor
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
