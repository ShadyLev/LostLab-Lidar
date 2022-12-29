using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Tooltip("Is game paused.")]
    [SerializeField] bool isPaused = false;


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
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                // Pause time
                Time.timeScale = 0;

                camMapController.enabled = true; // Enable map camera controller
                playerInput.enabled = false; // Disable player input
                mapCam.enabled = true; // Enable map camera

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

                // Hide and lock player cursor
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
