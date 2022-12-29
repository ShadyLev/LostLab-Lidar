using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public bool isPaused = false;
    private MapCameraController camMapController;
    private Camera mapCam;
    [SerializeField] private PlayerInput playerInput;


    // Start is called before the first frame update
    void Start()
    {
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();

        camMapController = GetComponent<MapCameraController>();
        mapCam = GetComponentInChildren<Camera>();

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
                Time.timeScale = 0;
                camMapController.enabled = true;

                playerInput.enabled = false;

                mapCam.enabled = true;

                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                Time.timeScale = 1;
                camMapController.enabled = false;

                playerInput.enabled = true;

                mapCam.enabled = false;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
